using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using rupload.Services;
using rupload.Services.Azure;

namespace rupload.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<BlobServiceConfig, BlobServiceConfig>();
                SimpleIoc.Default.Register<IJsonConfigService<BlobServiceConfig>, JsonConfigService<BlobServiceConfig>>();
                SimpleIoc.Default.Register<IBlobService, BlobService>();
                SimpleIoc.Default.Register<ICommandLineArgsService, CommandLineArgsService>();
                SimpleIoc.Default.Register<IClipboardService, ClipboardService>();
                SimpleIoc.Default.Register<IDeviceServices, DeviceServices>();
            }
            else
            {
                SimpleIoc.Default.Register<BlobServiceConfig, BlobServiceConfig>();
                SimpleIoc.Default.Register<IJsonConfigService<BlobServiceConfig>, JsonConfigService<BlobServiceConfig>>();
                SimpleIoc.Default.Register<IBlobService, BlobService>();
                SimpleIoc.Default.Register<ICommandLineArgsService, CommandLineArgsService>();
                SimpleIoc.Default.Register<IClipboardService, ClipboardService>();
                SimpleIoc.Default.Register<IDeviceServices, DeviceServices>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public static void Cleanup()
        {
            // Clear the ViewModels
            SimpleIoc.Default.Unregister<MainViewModel>();
            SimpleIoc.Default.Unregister<BlobServiceConfig>();
            SimpleIoc.Default.Unregister<IJsonConfigService<BlobServiceConfig>>();
            SimpleIoc.Default.Unregister<IBlobService>();
            SimpleIoc.Default.Unregister<ICommandLineArgsService>();
            SimpleIoc.Default.Unregister<IClipboardService>();
            SimpleIoc.Default.Unregister<IDeviceServices>();
        }
    }
}