using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<string> UploadBlob(string containerName, string path)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlockBlobReference(System.IO.Path.GetFileName(path));
            await blob.UploadFromFileAsync(path, System.IO.FileMode.Open);

            return blob.Uri.ToString();
        }
    }
}
