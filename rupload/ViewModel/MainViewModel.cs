using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using rupload.Helpers;
using rupload.Services;
using rupload.Services.Azure;
using rupload.Services.Model;
using System;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace rupload.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        const string containerName = "ruploads";

        IBlobService currentBlobService;
        ICommandLineArgsService currentCommandLineArgsService;
        IClipboardService currentClipboardService;
        IDeviceServices currentDeviceServices;
        INotificationService currentNotificationService;

        public MainViewModel(IBlobService blobService, ICommandLineArgsService commandLineArgsService, IClipboardService clipboardService, IDeviceServices deviceServices, INotificationService notificationService)
        {
            currentBlobService = blobService;
            currentCommandLineArgsService = commandLineArgsService;
            currentClipboardService = clipboardService;
            currentDeviceServices = deviceServices;
            currentNotificationService = notificationService;
        }

        RelayCommand _UploadCommand;
        public RelayCommand UploadCommand => _UploadCommand ?? (_UploadCommand = new RelayCommand(async () =>
        {
            string filePath = currentCommandLineArgsService.GetFirstCommand();
            string blobUrl = await currentBlobService.PreBuildUrl(containerName, filePath);
            currentClipboardService.SetUriToClipboard(blobUrl);
            blobUrl = await TryShortenUrlWithOuoPress(blobUrl);
            var progressIndicator = new Progress<UploadProgressUpdate>((UploadProgressUpdate progress) =>
            {
                Progress = progress.Percentage;
                UploadSpeed = progress.Description;
            });
            progressIndicator.ProgressChanged += ProgressIndicator_ProgressChanged;
            blobUrl = await currentBlobService.UploadBlob(containerName, filePath, progressIndicator);
            await currentNotificationService.CreateToast(ToastTemplateType.ToastImageAndText02, filePath, "Upload Complete");
            currentDeviceServices.ShutDownApp();
        }));

        void ProgressIndicator_ProgressChanged(object sender, UploadProgressUpdate e)
        {
            App.trayIcon.ToolTipText = e.Percentage.ToString();
        }

        async Task<string> TryShortenUrlWithOuoPress(string blobUrl)
        {
            try
            {
                string shortUrl = (await Request.ShortenUrlWithOuoPress(new OuoPressRequest() { url = blobUrl })).slug;
                currentClipboardService.SetUriToClipboard(shortUrl);
                return shortUrl;
            }
            catch (Exception)
            {
                return blobUrl;
            }
        }

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