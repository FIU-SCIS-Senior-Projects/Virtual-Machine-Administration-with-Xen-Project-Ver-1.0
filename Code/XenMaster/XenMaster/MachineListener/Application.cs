using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.MachineListener
{
    public class Application
    {
        public string name { get; set; }
        public string execution_name { get; set; }
        public ApplicationType type { get; set; }
        public string description { get; set; }
        public long size { get; set; }
        public string os { get; set; }
        public string category { get; set; }
        public string comments { get; set; }
        public string extension { get; set; }


        public Application() { }
    }
    public enum ApplicationType
    {
        BENIGN,
        MALWARE,
        UNKOWN
    }
    public enum OSype0
    {
        LIN0X,
        WINDOWS,
        UNKOWN
    }
}
