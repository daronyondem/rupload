using System;

namespace rupload.Services
{
    public class CommandLineArgsService : rupload.Services.ICommandLineArgsService
    {
        public string GetFirstCommand()
        {
            string[] args = Environment.GetCommandLineArgs();
#if DEBUG
            args = new string[2] { "", @"C:\Videos\test.mp4" };
#endif
            return (args.Length > 1) ? args[1] : string.Empty;
        }
    }
}
