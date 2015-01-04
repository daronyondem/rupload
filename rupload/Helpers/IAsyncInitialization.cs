using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rupload.Helpers
{
    public interface IAsyncInitialization
    {
        Task Initialization { get; }
    }
}
