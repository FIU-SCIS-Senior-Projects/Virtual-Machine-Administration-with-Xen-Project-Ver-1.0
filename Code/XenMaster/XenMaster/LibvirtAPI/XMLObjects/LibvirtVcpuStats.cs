using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XenMaster.LibvirtAPI.XMLObjects
{
    [Serializable,XmlRoot("VcpuStats")]
    public class LibvirtVcpuStats
    {
        [XmlElement("VCpus")]
        public List<LibvirtVCpu> cpus { get; set; }

        public LibvirtVcpuStats() { }
    }
}
