using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XenMaster.IntrospectorAPI.XMLMessages
{

    [Serializable]
    [XmlRoot("Introspector")]
    public class IntrospectorMessageWrapper : IntrospectorMessageBase
    {

        private object _message;

        [XmlElement("Message", typeof(Message))]
        public object Message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
            }
        }

        public IntrospectorMessageWrapper()
            : base()
        {

        }


        ///<summary>
        ///Converts the wrapped message to an XML String by removing all the namespace data
        ///</summary>
        public string toXmlString()
        {
            string result = "";
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, this);
                result = textWriter.ToString();
            }

            var doc = RemoveAllNamespaces(XElement.Parse(result));

            return doc.ToString();

        }


        public override string ToString()
        {
            StringBuilder output = new StringBuilder();

            output.Append("Introspector Message\n===============");
            output.Append("Message Type: " + this.Command + "\n");
            output.Append("Requestor: " + this.Requestor + "\n");

            return output.ToString();
        }

        public static XElement RemoveAllNamespaces(XElement e)
        {
            return new XElement(e.Name.LocalName,
               (from n in e.Nodes()
                select ((n is XElement) ? RemoveAllNamespaces(n as XElement) : n)),
               (e.HasAttributes) ? (from a in e.Attributes()
                                    where (!a.IsNamespaceDeclaration)
                                    select new XAttribute(a.Name.LocalName, a.Value)) : null);
        }
    }
}
