using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XenMaster.LibvirtAPI.XMLMessages
{
    [Serializable, XmlRoot("StorageList")]
    public class LibvirtVHDList
    {
        [XmlElement("Disk")]
        public List<LibvirtVHDDisk> StorageList { get; set; }

        public LibvirtVHDList()
        {
            StorageList = new List<LibvirtVHDDisk>();
        }
    }
}
