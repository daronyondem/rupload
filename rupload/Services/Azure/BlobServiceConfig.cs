
namespace rupload.Services.Azure
{
    public class BlobServiceConfig
    {
        const string developmentStorageConnectionString = "UseDevelopmentStorage=true;";
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public bool UseDevelopmentStorage { get; set; }
        public DefaultEndpointsProtocol EndpointsProtocol { get; set; }

        public virtual string GetConnectionString()
        {
            string conn = null;
            if (UseDevelopmentStorage)
            {
                conn = developmentStorageConnectionString;
            }
            else
            {
                conn = $"DefaultEndpointsProtocol={EndpointsProtocol.ToString()};AccountName={AccountName};AccountKey={AccountKey};";
            }
            return conn;
        }
        public enum DefaultEndpointsProtocol
        {
            http,
            https
        }
    }
}
