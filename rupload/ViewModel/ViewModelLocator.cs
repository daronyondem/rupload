using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using rupload.Services;

namespace rupload.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IBlobService>(() => { return new BlobService(); });
                SimpleIoc.Default.Register<ICommandLineArgsService, CommandLineArgsService>();
                SimpleIoc.Default.Register<IClipboardService, ClipboardService>();
            }
            else
            {
                SimpleIoc.Default.Register<IBlobService>(() => { return new BlobService(); });
                SimpleIoc.Default.Register<ICommandLineArgsService, CommandLineArgsService>();
                SimpleIoc.Default.Register<IClipboardService, ClipboardService>();
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