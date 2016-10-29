using System;
using System.Windows;

namespace rupload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ViewModel.ViewModelLocator.Cleanup();
        }
    }
}
