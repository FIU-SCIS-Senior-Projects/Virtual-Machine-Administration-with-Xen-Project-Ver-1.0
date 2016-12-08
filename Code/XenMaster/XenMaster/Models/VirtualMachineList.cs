using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.Models
{
    public class VirtualMachineList : ObservableConcurrentDictionary<string, VM>
    {
      
    }
}
