using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XenMaster.LibvirtAPI.XMLObjects
{
    [Serializable, XmlRoot("VM")]
    public class LibvirtVM
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Id")]
        public string Id { get; set; }

        [XmlElement("CpuTime")]
        public string CpuTime { get; set; }

        [XmlElement("UUID")]
        public string UUID { get { return _uuid; } set { _uuid = value; } }

        [XmlElement("Os")]
        public string Os { get; set; }

        [XmlElement("State")]
        public string State { get; set; }

        [XmlElement("MaxMemory")]
        public string MaxMemory { get; set; }

        [XmlElement("Memory")]
        public string Memory { get; set; }

        [XmlElement("VCpus")]
        public string VCpus { get; set; }

        [XmlElement("VNC")]
        public string VNC { get; set; }

        [XmlElement("VcpuStats")]
        public LibvirtVcpuStats VcpuStats { get; set; }

        private string _uuid;

        public LibvirtVM()
        {

        }

        public LibvirtVM(string uuid)
        {
            _uuid = uuid;
        }
    }
 }
