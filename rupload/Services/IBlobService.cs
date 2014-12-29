﻿using System;
using System.Threading.Tasks;

namespace rupload.Services
{
    public interface IBlobService
    {
        Task<bool> CreateContainer(string containerName);
        Task<string> UploadBlob(string containerName, string path, IProgress<BlobUploadProgressUpdate> progress);
    }
}
