using System.Threading.Tasks;

namespace rupload.Services
{
    public interface IUrlShortenService
    {
        Task<string> TryOuoPress(string blobUrl);
    }
}
