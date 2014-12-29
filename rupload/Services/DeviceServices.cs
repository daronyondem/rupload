
namespace rupload.Services
{
    public class DeviceServices : rupload.Services.IDeviceServices
    {
        public void ShutDownApp()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
