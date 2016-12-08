using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XenMaster.LibvirtAPI.XMLMessages
{
    [Serializable]
    [XmlRoot("LibvirtServerMessage")]
    public class LibvirtMessageWrapper : LibvirtMessageBase
    {
        [XmlElement("DiskUsage", typeof(LibvirtDiskUsage))]
        [XmlElement("Data", typeof(DataWrapper))]
        [XmlElement("VMList", typeof(LibvirtCommand))]
        public object Message { get; set; }


        public static XElement RemoveAllNamespaces(XElement e)
        {
            return new XElement(e.Name.LocalName,
               (from n in e.Nodes()
                select ((n is XElement) ? RemoveAllNamespaces(n as XElement) : n)),
               (e.HasAttributes) ? (from a in e.Attributes()
                                    where (!a.IsNamespaceDeclaration)
                                    select new XAttribute(a.Name.LocalName, a.Value)) : null);
        }

        public string toXMLString()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.NewLineHandling = NewLineHandling.None;
            settings.Indent = false;
            StringWriter Swriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(Swriter, settings);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            XmlSerializer serializer = new XmlSerializer(this.GetType());

            serializer.Serialize(writer, this, namespaces);
            string s = Swriter.ToString();
            return s;
        }
    }
}
