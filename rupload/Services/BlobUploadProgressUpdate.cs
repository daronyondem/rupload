using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rupload.Services
{
    public class BlobUploadProgressUpdate
    {
        public int Percentage { get; set; }
        public string Description { get; set; }
    }
}
