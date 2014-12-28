using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
