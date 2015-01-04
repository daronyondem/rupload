using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rupload.Services
{
    public class BlobServiceConfig : rupload.Services.IBlobServiceConfig
    {
        const string developmentStorageConnectionString = "UseDevelopmentStorage=true;";
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public bool UseDevelopmentStorage { get; set; }
        public DefaultEndpointsProtocol EndpointsProtocol { get; set; }

        public string GetConnectionString()
        {
            string conn = null;
            if(UseDevelopmentStorage)
            {
                conn = developmentStorageConnectionString;
            }
            else
            {
                conn = string.Format("DefaultEndpointsProtocol={0};AccountName={1};AccountKey={2};",
                    this.EndpointsProtocol.ToString(), this.AccountName, this.AccountKey);
            }
            return conn;
        }
    }
}
