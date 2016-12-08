using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XenMaster.LibvirtAPI.XMLObjects
{
    [Serializable, XmlRoot("Disks")]
    public class LibvirtDisks
    {

        private string _Name;
        private ulong _Size;

        [XmlElement("Name")]
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
            }
        }
        [XmlElement("Size")]
        public ulong Size
        {
            get
            {
                return _Size;
            }

            set
            {
                _Size = value;
            }
        }


        public LibvirtDisks() { }
    }
}
