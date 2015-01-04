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
                SimpleIoc.Default.Register<IBlobServiceConfig, BlobServiceConfig>();
                SimpleIoc.Default.Register<IJsonConfigService<IBlobServiceConfig>, JsonConfigService<IBlobServiceConfig>>();              
                SimpleIoc.Default.Register<IBlobService, BlobService>();
                SimpleIoc.Default.Register<ICommandLineArgsService, CommandLineArgsService>();
                SimpleIoc.Default.Register<IClipboardService, ClipboardService>();
                SimpleIoc.Default.Register<IDeviceServices, DeviceServices>();
            }
            else
            {
                SimpleIoc.Default.Register<IBlobServiceConfig, BlobServiceConfig>();
                SimpleIoc.Default.Register<IJsonConfigService<IBlobServiceConfig>, JsonConfigService<IBlobServiceConfig>>();
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
            // TODO Clear the ViewModels
        }
    }
}