
namespace rupload.Services
{
    public class ClipboardService : rupload.Services.IClipboardService
    {
        public void SetUriToClipboard(string Uri)
        {
            Uri = System.Web.HttpUtility.UrlPathEncode(Uri);
            System.Windows.Clipboard.SetText(Uri);
        }
    }
}
