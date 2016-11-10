using rupload.Helpers;
using rupload.Services.Model;
using System;
using System.Threading.Tasks;

namespace rupload.Services
{
    public class UrlShortenService : IUrlShortenService
    {
        public async Task<string> TryOuoPress(string url)
        {
            try
            {
                var req = new OuoPressRequest()
                {
                    url = url
                };
                var res = await Request.ShortenUrlWithOuoPress(req);
                string shortUrl = res.slug;
                return shortUrl;
            }
            catch (Exception)
            {
                return url;
            }
        }
    }
}
