using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XenMaster.LibvirtAPI.XMLObjects;

namespace XenMaster.LibvirtAPI.XMLMessages
{  /// <summary>
   /// List all Domains on a Xen Host
   /// </summary>
    [Serializable]
    [XmlRoot("VMList")]
    public class LibvirtCommand
    {
        [XmlElement("VM")]
        public List<LibvirtVM> Vms { get; set; }

        public LibvirtCommand()
        {
            Vms = new List<LibvirtVM>();
        }
    }
}
