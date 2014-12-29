using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rupload.Services
{
    public class CommandLineArgsService : rupload.Services.ICommandLineArgsService
    {
        public string GetFirstCommand()
        {
            string[] args = Environment.GetCommandLineArgs();
#if DEBUG
                        args = new string[2] {"", @"C:\Videos\test.mp4"};
#endif
            string filename = "";
            if (args.Length > 1)
            {
                filename = args[1];
            }
            return filename;
        }
    }
}
