using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using rupload.Helpers;
using rupload.Services.Model;
using System;
using System.Threading.Tasks;

namespace rupload.Services.Azure
{
    public class BlobService : IBlobService
    {
        CloudBlobClient blobClient;
        BlobServiceConfig blobServiceConfig;
        IJsonConfigService<BlobServiceConfig> jsonConfigService;

        public BlobService(BlobServiceConfig blobServiceConfig, IJsonConfigService<BlobServiceConfig> jsonConfigService)
        {
            this.blobServiceConfig = blobServiceConfig;
            this.jsonConfigService = jsonConfigService;
            Initialization = InitializeAsync();
        }

        public async Task<bool> CreateContainer(string containerName)
        {
            await Initialization;
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

        public async Task<string> UploadBlob(string containerName, string path, IProgress<UploadProgressUpdate> progress)
        {
            await Initialization;
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlockBlobReference(System.IO.Path.GetFileName(path));
            
            var tcs = new TaskCompletionSource<string>();

            BlobTransfer transferUpload = new BlobTransfer();
            transferUpload.TransferProgressChanged += (object sender, BlobTransfer.BlobTransferProgressChangedEventArgs e) =>
            {
                progress.Report(new UploadProgressUpdate()
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

        public Task Initialization { get; private set; }
        private async Task InitializeAsync()
        {
            this.blobServiceConfig = await jsonConfigService.GetSettings();
            if (this.blobServiceConfig == null)
            {
                this.blobServiceConfig = new BlobServiceConfig() { UseDevelopmentStorage = true };
                await jsonConfigService.SaveSetting(this.blobServiceConfig);
            }
            CloudStorageAccount account = CloudStorageAccount.Parse(this.blobServiceConfig.GetConnectionString());
            blobClient = account.CreateCloudBlobClient();
        }
    }
}
