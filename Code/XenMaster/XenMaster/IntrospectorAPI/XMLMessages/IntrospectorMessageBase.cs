using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace XenMaster.IntrospectorAPI.XMLMessages
{
    /*
     * Super class for all messages that will be exiting the server
     */
    [Serializable]
    public abstract class IntrospectorMessageBase
    {
        public enum MessageTypes
        {
            ACKNOWLEDGE,
            HELLO,
            RESYNC_REQUEST,
            SYNC_REQUEST,
            SYNC_DATA,
            VM_MANAGEMENT,
            PROCESS_LIST_SCAN,
            LIST_LINUX_VM_PROCESSES,
            LIST_MALWARE,
            UKNOWN
               
        }
        

        [XmlIgnore]
        public static readonly string MESSAGE_ACKNOWLEDGE = "ACKNOWLEDGE";
        [XmlIgnore]
        public static readonly string MESSAGE_SYNC_REQUEST = "SYNC REQUEST";
        [XmlIgnore]
        public static readonly string MESSAGE_PROCESS_LIST_SCAN = "PROCESS LIST SCAN";
        [XmlIgnore]
        public MessageTypes _enumeratedType = MessageTypes.UKNOWN;

        private string _command;
        private string _requestor;
        private string _requestorId;
        private string _response;
        private int _responseCode;
        private string _responseMessage;
        private string _message;            //not an actual string type



       [XmlElement("Command")]
        public string Command
        {
            get { return _command; }
            set
            {
                setEnumeratedMessageType(value);
                _command = value;
            }
        }

        [XmlElement("Requestor")]
        public string Requestor
        {
            get { return _requestor; }
            set
            {
                setEnumeratedMessageType(value);
                _requestor = value;
            }
        }

        [XmlElement("RequestorId")]
        public string RequestorId
        {
            get { return _requestorId; }
            set
            {
                setEnumeratedMessageType(value);
                _requestorId = value;
            }
        }

        [XmlElement("Response")]
        public string Response
        {
            get { return _response; }
            set
            {
                setEnumeratedMessageType(value);
                _response = value;
            }
        }

        [XmlElement("ResponseCode")]
        public int ResponseCode
        {
            get { return _responseCode; }
            set
            {

                _responseCode = value;
            }
        }

        [XmlElement("ResponseMessage")]
        public string ResponseMessage
        {
            get { return _responseMessage; }
            set
            {
                setEnumeratedMessageType(value);
                _responseMessage = value;
            }
        }



        /*
        [XmlElement("MessageType")]
        public MessageTypes MessageType { get; set; }
         */

        public static IntrospectorMessageWrapper BuildMessage(MessageTypes type, string requestor, string requestorid)
        {
            IntrospectorMessageWrapper wrapper = new IntrospectorMessageWrapper();

            switch (type)
            {
                case MessageTypes.SYNC_REQUEST:
                    wrapper.Command = getEnumeratedMessageType(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case MessageTypes.HELLO:
                    wrapper.Command = getEnumeratedMessageType(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case MessageTypes.PROCESS_LIST_SCAN:
                    wrapper.Command = getEnumeratedMessageType(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case MessageTypes.LIST_LINUX_VM_PROCESSES:
                    wrapper.Command = getEnumeratedMessageType(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
            }

            return wrapper;

        }

        public static string getEnumeratedMessageType(MessageTypes type)
        {
            string msg ="NOT DEFINED";
            if (type == MessageTypes.SYNC_REQUEST)
                msg = "SYNC REQUEST";
            else if (type == MessageTypes.ACKNOWLEDGE)
                msg = "ACKNOWLEDGE";
            else if (type == MessageTypes.PROCESS_LIST_SCAN)
                msg = "PROCESS LIST SCAN";
            else if (type == MessageTypes.HELLO)
                msg = "HELLO";
            else if (type == MessageTypes.RESYNC_REQUEST)
                msg = "RESYNC REQUEST";
            else if (type == MessageTypes.SYNC_REQUEST)
                msg = "SYNC REQUEST";
            else if (type == MessageTypes.SYNC_DATA)
                msg = "SYNC DATA";
            else if (type == MessageTypes.LIST_LINUX_VM_PROCESSES)
                msg = "LIST LINUX VM PROCESSES";

            return msg;
        }


        public void setEnumeratedMessageType(string value)
        {
            if (value.Equals("SYNC REQUEST"))
                _enumeratedType = MessageTypes.SYNC_REQUEST;
            else if (value.Equals("ACKNOWLEDGE"))
                _enumeratedType = MessageTypes.ACKNOWLEDGE;
            else if (value.Equals("HELLO"))
                _enumeratedType = MessageTypes.HELLO;
            else if (value.Equals("RESYNC REQUEST"))
                _enumeratedType = MessageTypes.RESYNC_REQUEST;
            else if (value.Equals("SYNC REQUEST"))
                _enumeratedType = MessageTypes.SYNC_REQUEST;
            else if (value.Equals("SYNC DATA"))
                _enumeratedType = MessageTypes.SYNC_DATA;
            else if (value.Equals("PROCESS LIST SCAN"))
                _enumeratedType = MessageTypes.PROCESS_LIST_SCAN;
            else if (value.Equals("LIST LINUX VM PROCESSES"))
                _enumeratedType = MessageTypes.PROCESS_LIST_SCAN;

        }


        public string toXMLString()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.NewLineHandling = NewLineHandling.None;
            settings.Indent = false;
            StringWriter swriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(swriter, settings);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            XmlSerializer serializer = new XmlSerializer(this.GetType());

            serializer.Serialize(writer, this, namespaces);
            string s = swriter.ToString();

            return s;
        }

        //default constructor
        public IntrospectorMessageBase()
        {

        }
    }
}
