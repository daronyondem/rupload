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
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ViewModel.ViewModelLocator.Cleanup();
        }
    }
}
