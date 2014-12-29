//Helper file taken from http://blogs.msdn.com/b/kwill/archive/2013/03/06/asynchronous-parallel-block-blob-transfers-with-progress-change-notification-2-0.aspx

using System;
using System.ComponentModel;

using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Linq;

namespace rupload.Helpers
{

    // Class to allow for easy async upload and download functions with progress change notifications
    // Requires references to Microsoft.WindowsAzure.Storage.dll (Storage client 2.0) and Microsoft.WindowsAzure.StorageClient.dll (Storage client 1.7).
    // See comments on UploadBlobAsync and DownloadBlobAsync functions for information on removing the 1.7 client library dependency
    class BlobTransfer
    {
        // Public async events
        public event AsyncCompletedEventHandler TransferCompleted;
        public event EventHandler<BlobTransferProgressChangedEventArgs> TransferProgressChanged;

        // Public BlobTransfer properties
        public TransferTypeEnum TransferType;

        // Private variables
        private ICancellableAsyncResult asyncresult;
        private bool Working = false;
        private object WorkingLock = new object();
        private AsyncOperation asyncOp;

        // Used to calculate download speeds
        private Queue<long> timeQueue = new Queue<long>(200);
        private Queue<long> bytesQueue = new Queue<long>(200);
        private DateTime updateTime = System.DateTime.Now;

        // Private BlobTransfer properties
        private string m_FileName;
        private ICloudBlob m_Blob;

        public void UploadBlobAsync(ICloudBlob blob, string LocalFile)
        {
            // The class currently stores state in class level variables so calling UploadBlobAsync or DownloadBlobAsync a second time will cause problems.
            // A better long term solution would be to better encapsulate the state, but the current solution works for the needs of my primary client.
            // Throw an exception if UploadBlobAsync or DownloadBlobAsync has already been called.
            lock (WorkingLock)
            {
                if (!Working)
                    Working = true;
                else
                    throw new Exception("BlobTransfer already initiated. Create new BlobTransfer object to initiate a new file transfer.");
            }

            // Attempt to open the file first so that we throw an exception before getting into the async work
            using (FileStream fstemp = new FileStream(LocalFile, FileMode.Open, FileAccess.Read)) { }

            // Create an async op in order to raise the events back to the client on the correct thread.
            asyncOp = AsyncOperationManager.CreateOperation(blob);

            TransferType = TransferTypeEnum.Upload;
            m_Blob = blob;
            m_FileName = LocalFile;

            var file = new FileInfo(m_FileName);
            long fileSize = file.Length;

            FileStream fs = new FileStream(m_FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            ProgressStream pstream = new ProgressStream(fs);
            pstream.ProgressChanged += pstream_ProgressChanged;
            pstream.SetLength(fileSize);
            m_Blob.ServiceClient.DefaultRequestOptions.ParallelOperationThreadCount = 10;
            m_Blob.StreamWriteSizeInBytes = GetBlockSize(fileSize);
            asyncresult = m_Blob.BeginUploadFromStream(pstream, BlobTransferCompletedCallback, new BlobTransferAsyncState(m_Blob, pstream));
        }

        // Blob Upload Code
        // 200 GB max blob size
        // 50,000 max blocks
        // 4 MB max block size
        // Try to get close to 100k block size in order to offer good progress update response.
        private int GetBlockSize(long fileSize)
        {
            const long KB = 1024;
            const long MB = 1024 * KB;
            const long GB = 1024 * MB;
            const long MAXBLOCKS = 50000;
            const long MAXBLOBSIZE = 200 * GB;
            const long MAXBLOCKSIZE = 4 * MB;

            long blocksize = 100 * KB;
            //long blocksize = 4 * MB;
            long blockCount;
            blockCount = ((int)Math.Floor((double)(fileSize / blocksize))) + 1;
            while (blockCount > MAXBLOCKS - 1)
            {
                blocksize += 100 * KB;
                blockCount = ((int)Math.Floor((double)(fileSize / blocksize))) + 1;
            }

            if (blocksize > MAXBLOCKSIZE)
            {
                throw new ArgumentException("Blob too big to upload.");
            }

            return (int)blocksize;
        }
 

        public void DownloadBlobAsync(ICloudBlob blob, string LocalFile)
        {
            // The class currently stores state in class level variables so calling UploadBlobAsync or DownloadBlobAsync a second time will cause problems.
            // A better long term solution would be to better encapsulate the state, but the current solution works for the needs of my primary client.
            // Throw an exception if UploadBlobAsync or DownloadBlobAsync has already been called.
            lock (WorkingLock)
            {
                if (!Working)
                    Working = true;
                else
                    throw new Exception("BlobTransfer already initiated. Create new BlobTransfer object to initiate a new file transfer.");
            }

            // Create an async op in order to raise the events back to the client on the correct thread.
            asyncOp = AsyncOperationManager.CreateOperation(blob);

            TransferType = TransferTypeEnum.Download;
            m_Blob = blob;
            m_FileName = LocalFile;

            m_Blob.FetchAttributes();

            FileStream fs = new FileStream(m_FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            ProgressStream pstream = new ProgressStream(fs);
            pstream.ProgressChanged += pstream_ProgressChanged;
            pstream.SetLength(m_Blob.Properties.Length);
            m_Blob.ServiceClient.DefaultRequestOptions.ParallelOperationThreadCount = 10;
            asyncresult = m_Blob.BeginDownloadToStream(pstream, BlobTransferCompletedCallback, new BlobTransferAsyncState(m_Blob, pstream));
        }

        private void pstream_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BlobTransferProgressChangedEventArgs eArgs = null;
            int progress = (int)((double)e.BytesRead / e.TotalLength * 100);

            // raise the progress changed event on the asyncop thread
            eArgs = new BlobTransferProgressChangedEventArgs(e.BytesRead, e.TotalLength, progress, CalculateSpeed(e.BytesRead), null);
            asyncOp.Post(delegate(object e2) { OnTaskProgressChanged((BlobTransferProgressChangedEventArgs)e2); }, eArgs);
        }

        private void BlobTransferCompletedCallback(IAsyncResult result)
        {
            BlobTransferAsyncState state = (BlobTransferAsyncState)result.AsyncState;
            ICloudBlob blob = state.Blob;
            ProgressStream stream = (ProgressStream)state.Stream;

            try
            {
                stream.Close();

                // End the operation.
                if (TransferType == TransferTypeEnum.Download)
                    blob.EndDownloadToStream(result);
                else if (TransferType == TransferTypeEnum.Upload)
                    blob.EndUploadFromStream(result);

                // Operation completed normally, raise the completed event
                AsyncCompletedEventArgs completedArgs = new AsyncCompletedEventArgs(null, false, null);
                asyncOp.PostOperationCompleted(delegate(object e) { OnTaskCompleted((AsyncCompletedEventArgs)e); }, completedArgs);
            }
            catch (StorageException ex)
            {
                if (!state.Cancelled)
                {
                    throw (ex);
                }

                // Operation was cancelled, raise the event with the cancelled flag = true
                AsyncCompletedEventArgs completedArgs = new AsyncCompletedEventArgs(null, true, null);
                asyncOp.PostOperationCompleted(delegate(object e) { OnTaskCompleted((AsyncCompletedEventArgs)e); }, completedArgs);
            }
        }

        // Cancel the async download
        public void CancelAsync()
        {
            ((BlobTransferAsyncState)asyncresult.AsyncState).Cancelled = true;
            asyncresult.Cancel();
        }

        // Helper function to only raise the event if the client has subscribed to it.
        protected virtual void OnTaskCompleted(AsyncCompletedEventArgs e)
        {
            if (TransferCompleted != null)
                TransferCompleted(this, e);
        }

        // Helper function to only raise the event if the client has subscribed to it.
        protected virtual void OnTaskProgressChanged(BlobTransferProgressChangedEventArgs e)
        {
            if (TransferProgressChanged != null)
                TransferProgressChanged(this, e);
        }

        // Keep the last 200 progress change notifications and use them to calculate the average speed over that duration. 
        private double CalculateSpeed(long BytesSent)
        {
            double speed = 0;

            if (timeQueue.Count >= 200)
            {
                timeQueue.Dequeue();
                bytesQueue.Dequeue();
            }

            timeQueue.Enqueue(System.DateTime.Now.Ticks);
            bytesQueue.Enqueue(BytesSent);

            if (timeQueue.Count > 2)
            {
                updateTime = System.DateTime.Now;
                speed = (bytesQueue.Max() - bytesQueue.Min()) / TimeSpan.FromTicks(timeQueue.Max() - timeQueue.Min()).TotalSeconds;
            }

            return speed;
        }

        // A modified version of the ProgressStream from http://blogs.msdn.com/b/paolos/archive/2010/05/25/large-message-transfer-with-wcf-adapters-part-1.aspx
        // This class allows progress changed events to be raised from the blob upload/download.
        private class ProgressStream : Stream
        {
            #region Private Fields
            private Stream stream;
            private long bytesTransferred;
            private long totalLength;
            #endregion

            #region Public Handler
            public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
            #endregion

            #region Public Constructor
            public ProgressStream(Stream file)
            {
                this.stream = file;
                this.totalLength = file.Length;
                this.bytesTransferred = 0;
            }
            #endregion

            #region Public Properties
            public override bool CanRead
            {
                get
                {
                    return this.stream.CanRead;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    return this.stream.CanSeek;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    return this.stream.CanWrite;
                }
            }

            public override void Flush()
            {
                this.stream.Flush();
            }

            public override void Close()
            {
                this.stream.Close();
            }

            public override long Length
            {
                get
                {
                    return this.stream.Length;
                }
            }

            public override long Position
            {
                get
                {
                    return this.stream.Position;
                }
                set
                {
                    this.stream.Position = value;
                }
            }
            #endregion

            #region Public Methods
            public override int Read(byte[] buffer, int offset, int count)
            {
                int result = stream.Read(buffer, offset, count);
                bytesTransferred += result;
                if (ProgressChanged != null)
                {
                    try
                    {
                        OnProgressChanged(new ProgressChangedEventArgs(bytesTransferred, totalLength));
                        //ProgressChanged(this, new ProgressChangedEventArgs(bytesTransferred, totalLength));
                    }
                    catch (Exception)
                    {
                        ProgressChanged = null;
                    }
                }
                return result;
            }

            protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
            {
                if (ProgressChanged != null)
                    ProgressChanged(this, e);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.stream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                totalLength = value;
                //this.stream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                this.stream.Write(buffer, offset, count);
                bytesTransferred += count;
                {
                    try
                    {
                        OnProgressChanged(new ProgressChangedEventArgs(bytesTransferred, totalLength));
                        //ProgressChanged(this, new ProgressChangedEventArgs(bytesTransferred, totalLength));
                    }
                    catch (Exception)
                    {
                        ProgressChanged = null;
                    }
                }
            }

            protected override void Dispose(bool disposing)
            {
                stream.Dispose();
                base.Dispose(disposing);
            }

            #endregion
        }

        private class BlobTransferAsyncState
        {
            public ICloudBlob Blob;
            public Stream Stream;
            public DateTime Started;
            public bool Cancelled;

            public BlobTransferAsyncState(ICloudBlob blob, Stream stream)
                : this(blob, stream, DateTime.Now)
            { }

            public BlobTransferAsyncState(ICloudBlob blob, Stream stream, DateTime started)
            {
                Blob = blob;
                Stream = stream;
                Started = started;
                Cancelled = false;
            }
        }

        private class ProgressChangedEventArgs : EventArgs
        {
            #region Private Fields
            private long bytesRead;
            private long totalLength;
            #endregion

            #region Public Constructor
            public ProgressChangedEventArgs(long bytesRead, long totalLength)
            {
                this.bytesRead = bytesRead;
                this.totalLength = totalLength;
            }
            #endregion

            #region Public properties

            public long BytesRead
            {
                get
                {
                    return this.bytesRead;
                }
                set
                {
                    this.bytesRead = value;
                }
            }

            public long TotalLength
            {
                get
                {
                    return this.totalLength;
                }
                set
                {
                    this.totalLength = value;
                }
            }
            #endregion
        }

        public enum TransferTypeEnum
        {
            Download,
            Upload
        }

        public class BlobTransferProgressChangedEventArgs : System.ComponentModel.ProgressChangedEventArgs
        {
            private long m_BytesSent = 0;
            private long m_TotalBytesToSend = 0;
            private double m_Speed = 0;

            public long BytesSent
            {
                get { return m_BytesSent; }
            }

            public long TotalBytesToSend
            {
                get { return m_TotalBytesToSend; }
            }

            public double Speed
            {
                get { return m_Speed; }
            }

            public TimeSpan TimeRemaining
            {
                get
                {
                    TimeSpan time = new TimeSpan(0, 0, (int)((TotalBytesToSend - m_BytesSent) / (m_Speed == 0 ? 1 : m_Speed)));
                    return time;
                }
            }

            public BlobTransferProgressChangedEventArgs(long BytesSent, long TotalBytesToSend, int progressPercentage, double Speed, object userState)
                : base(progressPercentage, userState)
            {
                m_BytesSent = BytesSent;
                m_TotalBytesToSend = TotalBytesToSend;
                m_Speed = Speed;
            }
        }
    }
}