
namespace rupload.Services
{
    public class ClipboardService : rupload.Services.IClipboardService
    {
        public void SetClipboard(string text)
        {
            System.Windows.Clipboard.SetText(text);
        }
    }
}
