using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using rupload.Services;

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
                        string blobUrl = await currentBlobService.UploadBlob("ruploads", currentCommandLineArgsService.GetFirstCommand());
                        currentClipboardService.SetClipboard(blobUrl);
                        currentDeviceServices.ShutDownApp();
                    }));
            }
        }
    }
}