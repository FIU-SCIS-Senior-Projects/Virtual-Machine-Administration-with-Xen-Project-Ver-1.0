using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XenMaster.LibvirtAPI.XMLMessages;

namespace XenMaster.LibvirtAPI.XMLObjects
{
    [Serializable, XmlRoot("Host")]
    public class LibvirtHost
    {

        [XmlElement("CPU")]
        public string CPU { get; set; }

        [XmlElement("Cores")]
        public int Cores { get; set; }

        [XmlElement("MemoryKB")]
        public ulong MemoryKB { get; set; }

        [XmlElement("MHz")]
        public int Megahertz { get; set; }

        [XmlElement("Nodes")]
        public int Nodes { get; set; }

        [XmlElement("Sockets")]
        public int Sockets { get; set; }

        [XmlElement("Model")]
        public string Model { get; set; }

        [XmlElement("Threads")]
        public int Threads { get; set; }

        [XmlElement("VirtualDisks")]
        public LibvirtDiskList VirtualDisks { get; set; }

        [XmlElement("StorageList")]
        public LibvirtVHDList StorageList { get; set; }

        public LibvirtHost() { }
    }
}
