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
    [XmlRoot("VirtualDisks")]
    public class LibvirtDiskList
    {

        [XmlElement("Disks")]
        public List<LibvirtDisks> VirtualDisks { get; set; }

        public LibvirtDiskList()
        {
            VirtualDisks = new List<LibvirtDisks>();
        }
    }
}
