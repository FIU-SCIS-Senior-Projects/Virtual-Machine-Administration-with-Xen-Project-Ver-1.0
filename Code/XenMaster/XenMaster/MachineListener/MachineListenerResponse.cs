using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.MachineListener
{
    public class MachineListenerResponse
    {
        public string message { get; set; }
        public List<Application> processlist { get; set; }
        public MachineListenerResponse() { }
    }
}
