using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XenMaster.LibvirtAPI.XMLObjects;

namespace XenMaster.LibvirtAPI.XMLMessages
{
    [Serializable]
    [XmlRoot("DiskUsage")]
    public class LibvirtDiskUsage
    {
        [XmlElement("DiskStats")]
        public List<LibvirtDiskStats> Stats { get; set; }

        public LibvirtDiskUsage()
        {
            Stats = new List<LibvirtDiskStats>();
        }
    }
}
