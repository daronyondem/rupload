using System;
namespace rupload.Services.Azure
{
    public interface IBlobServiceConfig
    {
        string AccountKey { get; set; }
        string AccountName { get; set; }
        DefaultEndpointsProtocol EndpointsProtocol { get; set; }
        bool UseDevelopmentStorage { get; set; }
        string GetConnectionString();
    }
    public enum DefaultEndpointsProtocol
    {
        http,
        https
    }
}
