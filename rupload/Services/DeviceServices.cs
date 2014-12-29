using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
