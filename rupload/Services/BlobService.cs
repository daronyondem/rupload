using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using rupload.Helpers;
using System;
using System.Threading.Tasks;

namespace rupload.Services
{
    public class BlobService : IBlobService
    {
        CloudBlobClient blobClient;

        public BlobService(string appSettingsKeyName = "StorageConnection")
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings[appSettingsKeyName].ToString());
            blobClient = account.CreateCloudBlobClient();
        }

        public async Task<bool> CreateContainer(string containerName)
        {
            bool result = true;
            try
            {
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                await container.CreateIfNotExistsAsync();

                BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                await container.SetPermissionsAsync(containerPermissions);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;           
        }

        public async Task<string> UploadBlob(string containerName, string path, IProgress<BlobUploadProgressUpdate> progress)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlockBlobReference(System.IO.Path.GetFileName(path));
            
            var tcs = new TaskCompletionSource<string>();

            BlobTransfer transferUpload = new BlobTransfer();
            transferUpload.TransferProgressChanged += (object sender, BlobTransfer.BlobTransferProgressChangedEventArgs e) =>
            {
                progress.Report(new BlobUploadProgressUpdate()
                {
                    Percentage=  e.ProgressPercentage, 
                    Description = (e.Speed / 1024).ToString("N2") + "KB/s"
            });
            };
            transferUpload.TransferCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) =>
            {
                tcs.TrySetResult("");
            };
            transferUpload.UploadBlobAsync(blob, path);
                       
            await tcs.Task;

            blob.Properties.ContentType = System.Web.MimeMapping.GetMimeMapping(System.IO.Path.GetFileName(path));
            await blob.SetPropertiesAsync();

            return blob.Uri.ToString();
        }        
    }
}
