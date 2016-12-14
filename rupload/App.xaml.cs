using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Net.Http;
using System.Windows;

namespace rupload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static HttpClient httpClient = new HttpClient();
        public static TaskbarIcon trayIcon = default(TaskbarIcon);
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ViewModel.ViewModelLocator.Cleanup();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            trayIcon = FindResource("TrayIcon") as TaskbarIcon;
        }
    }
}
