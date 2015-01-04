using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using rupload.Services;
using rupload.Services.Azure;
using rupload.Services.Model;
using System;

namespace rupload.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IBlobService currentBlobService;
        private ICommandLineArgsService currentCommandLineArgsService;
        private IClipboardService currentClipboardService;
        private IDeviceServices currentDeviceServices;
        public MainViewModel(IBlobService blobService, ICommandLineArgsService commandLineArgsService, IClipboardService clipboardService, IDeviceServices deviceServices)
        {
            currentBlobService = blobService;
            currentCommandLineArgsService = commandLineArgsService;
            currentClipboardService = clipboardService;
            currentDeviceServices = deviceServices;
        }

        private RelayCommand _uploadCommand;

        public RelayCommand UploadCommand
        {
            get
            {
                return _uploadCommand
                    ?? (_uploadCommand = new RelayCommand(
                    async () =>
                    {
                        var progressIndicator = new Progress<UploadProgressUpdate>((UploadProgressUpdate progress) => 
                        {
                            this.Progress = progress.Percentage;
                            this.UploadSpeed = progress.Description;
                        });
                        string blobUrl = await currentBlobService.UploadBlob("ruploads", currentCommandLineArgsService.GetFirstCommand(), progressIndicator);
                        currentClipboardService.SetClipboard(blobUrl);
                        currentDeviceServices.ShutDownApp();
                    }));
            }
        }

        public const string ProgressPropertyName = "Progress";

        private double _progress = 0;
        public double Progress
        {
            get
            {
                return _progress;
            }

            set
            {
                if (_progress == value)
                {
                    return;
                }

                _progress = value;
                RaisePropertyChanged(ProgressPropertyName);
            }
        }

        public const string UploadSpeedPropertyName = "UploadSpeed";

        private string _UploadSpeed = "";

        public string UploadSpeed
        {
            get
            {
                return _UploadSpeed;
            }

            set
            {
                if (_UploadSpeed == value)
                {
                    return;
                }

                _UploadSpeed = value;
                RaisePropertyChanged(UploadSpeedPropertyName);
            }
        }
    }
}