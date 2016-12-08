using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XenMaster.IntrospectorAPI.XMLObjects;

namespace XenMaster.IntrospectorAPI.XMLMessages
{
    [Serializable]
    public class Message
    {
        [XmlElement("FeatureScan", typeof(FeatureScan))]
        [XmlElement("HostSettings", typeof(HostSettings))]
        [XmlElement("ProcessList", typeof(ProcessList))]
        [XmlElement("FeatureScanArguments", typeof(FeatureScanArguments))]
        [XmlElement("MalwareList", typeof(MalwareList))]
        public object Data
        {
            get
            {
                return _Data;
            }
            set
            {
                _Data = value;
            }
        }

        
        private object _Data;

        public Message() { }
    }
}
