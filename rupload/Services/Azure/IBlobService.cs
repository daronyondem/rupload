using rupload.Helpers;
using rupload.Services.Model;
using System;
using System.Threading.Tasks;

namespace rupload.Services.Azure
{
    public interface IBlobService : IAsyncInitialization
    {
        Task<bool> CreateContainer(string containerName);
        Task<string> UploadBlob(string containerName, string path, IProgress<UploadProgressUpdate> progress);
    }
}
