using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using rupload.Helpers;
using rupload.Services;
using rupload.Services.Azure;
using rupload.Services.Model;
using System;

namespace rupload.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        const string containerName = "ruploads";

        IBlobService currentBlobService;
        ICommandLineArgsService currentCommandLineArgsService;
        IClipboardService currentClipboardService;
        IDeviceServices currentDeviceServices;

        public MainViewModel(IBlobService blobService, ICommandLineArgsService commandLineArgsService, IClipboardService clipboardService, IDeviceServices deviceServices)
        {
            currentBlobService = blobService;
            currentCommandLineArgsService = commandLineArgsService;
            currentClipboardService = clipboardService;
            currentDeviceServices = deviceServices;
        }

        RelayCommand _UploadCommand;
        public RelayCommand UploadCommand => _UploadCommand ?? (_UploadCommand = new RelayCommand(async () =>
        {
            string filePath = currentCommandLineArgsService.GetFirstCommand();
            string blobUrl = await currentBlobService.PreBuildUrl(containerName, filePath);
            currentClipboardService.SetUriToClipboard(blobUrl);
            try
            {
                string shortUrl = (await Request.ShortenUrlWithOuoPress(new OuoPressRequest() { url = blobUrl })).slug;
                currentClipboardService.SetUriToClipboard(shortUrl);
            }
            catch (Exception)
            {
            }
            var progressIndicator = new Progress<UploadProgressUpdate>((UploadProgressUpdate progress) =>
            {
                Progress = progress.Percentage;
                UploadSpeed = progress.Description;
            });
            blobUrl = await currentBlobService.UploadBlob(containerName, filePath, progressIndicator);
            currentDeviceServices.ShutDownApp();
        }));

        double _Progress = default(double);
        public double Progress
        {
            get
            {
                return _Progress;
            }
            set
            {
                if (_Progress == value) return;
                _Progress = value;
                RaisePropertyChanged(nameof(Progress));
            }
        }

        string _UploadSpeed = string.Empty;
        public string UploadSpeed
        {
            get
            {
                return _UploadSpeed;
            }
            set
            {
                if (_UploadSpeed == value) return;
                _UploadSpeed = value;
                RaisePropertyChanged(nameof(UploadSpeed));
            }
        }
    }
}