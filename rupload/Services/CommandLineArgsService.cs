using System;

namespace rupload.Services
{
    public class CommandLineArgsService : rupload.Services.ICommandLineArgsService
    {
        public string GetFirstCommand()
        {
            string[] args = Environment.GetCommandLineArgs();
#if DEBUG
            args = new string[2] { "", @"C:\Users\Garen\Desktop\1.jpg" };
#endif
            var filename = string.Empty;
            if (args.Length > 1)
            {
                filename = args[1];
            }
            return filename;
        }
    }
}
