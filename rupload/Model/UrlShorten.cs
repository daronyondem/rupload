
namespace rupload.Services.Model
{
    // Whole URL shorten mechanism works only with http://ouo.press/
    public class UrlShorten
    {
        public bool error { get; set; }

        string _slug;
        public string slug
        {
            get
            {
                return "http://ouo.press/" + _slug;
            }
            set
            {
                _slug = value;
            }
        }
    }

    public class UrlShortenRequest
    {
        public string _token { get; set; } = "Vx4NPAjwR6eK3qmTZ2YSWohfda0ThZ2STNLcPf7C"; // default token may be deprecated in the future
        public string url { get; set; }
    }
}
