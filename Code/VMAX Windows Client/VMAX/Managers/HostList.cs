using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenMaster.Models;

namespace VMAX.Helpers
{
    public class HostList: ObservableConcurrentDictionary<string, Host>
    {

    }
}
