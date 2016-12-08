using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.IntrospectorAPI.XMLObjects
{
    [Serializable]
    public class FeatureScan
    {
        public string Type { get; set; }
        public int Duration { get; set; }
        public VmInfo VmInfo { get; set; }
        public ProcessList ProcessList { get; set; }

        public FeatureScan() { }

    }

    public class VmInfo
    {
        public string Name { get; set; }
        public string UUID { get; set; }

        public VmInfo() { }
    }


    public enum ScanTypes
    {
        LINUX_PROCESS_LIST_SCAN,
        WINDOWS_PROCESS_LIST_SCAN
    }

}
