using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XenMaster.LibvirtAPI.XMLMessages
{
    [Serializable]
    public class DataWrapper
    {
        [XmlElement("FileTransfer",typeof(LibvirtFileTransfer))]
        public object Data { get; set; }

        public DataWrapper()
        {

        }
    }
}
