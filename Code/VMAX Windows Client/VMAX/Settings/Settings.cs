
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using XenMaster;

namespace VMAX.ApplicationSettings
{
    [Serializable]
    public class Settings
    {
        private static readonly string FILE_NAME = "\\VMAX.cfg";
        private static readonly string DIR_NAME = "\\VMAX";

        //The hosts that this VMAX application knows about
        //The last time this config file and settings was updated
        //Number of attempts to reconnect to a host node before giving up
        private int _MaxRetry = 5;
        private int _ReSyncInterval = 5;
        private int _SystemPort = 29171;

        private bool _autoConnectOnStart = true;

        private string _SystemIdentifier;
        private string _SystemIp;

        private List<Node> _Hosts;
        private DateTime _lastUpdated;


        #region Public Properties
        public string SystemIdentifier
        {
            get
            {
                return _SystemIdentifier;
            }

            set
            {
                _SystemIdentifier = value;
            }
        }

        public string SystemIp
        {
            get
            {
                return SystemIp1;
            }

            set
            {
                SystemIp1 = value;
            }
        }

        public int SystemPort
        {
            get
            {
                return _SystemPort;
            }

            set
            {
                _SystemPort = value;
            }
        }

        public int ReSyncInterval
        {
            get
            {
                return _ReSyncInterval;
            }

            set
            {
                _ReSyncInterval = value;
            }
        }

        public string SystemIp1
        {
            get
            {
                return _SystemIp;
            }

            set
            {
                _SystemIp = value;
            }
        }
        public int MaxRetry
        {
            get
            {
                return _MaxRetry;
            }

            set
            {
                _MaxRetry = value;
            }
        }
        public bool AutoConnectOnStart
        {
            get
            {
                return _autoConnectOnStart;
            }

            set
            {
                _autoConnectOnStart = value;
            }
        }

        public List<Node> Hosts
        {
            get
            {
                return _Hosts;
            }

            set
            {
                _Hosts = value;
            }
        }

        public DateTime LastUpdated
        {
            get
            {
                return _lastUpdated;
            }

            set
            {
                _lastUpdated = value;
            }
        }



        #endregion

        public Settings()
        {

        }

        public Settings(string _SystemIdentifier, string _SysstemIp, int _SystemPort)
        {
            this.SystemIdentifier = _SystemIdentifier;
            this.SystemIp = _SysstemIp;
            this.SystemPort = _SystemPort;
        }

        public void addNewNode(string ip, int port, string id, string name, string description)
        {
            Node node = new Node(ip,port,id,name,description);
            _Hosts.Add(node);
        }

        public void save()
        {
            //check if VMAX has a folder in C:\ProgramData
            if (!Directory.Exists(getFilepath()))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + DIR_NAME);

            SerializeObject(this, getFilepath());
        }

        public static Settings loadSettingsFile(string path)
        {
            Settings settings = null;
            if (File.Exists(path))
            {
                try
                {
                    settings = DeSerializeObject<Settings>(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                settings = new Settings();
                settings.Hosts = new List<Node>();
                settings.save();
            }
            return settings;
        }

        /// <summary>
        /// Deserializes the Settings file into an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Settings DeSerializeObject<Settings>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(Settings); }

            Settings objectOut = default(Settings);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(Settings);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (Settings)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return objectOut;
        }


        /// <summary>
        /// Serializes the settings file to text
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        public static void SerializeObject<Settings>(Settings serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static string getFilepath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + DIR_NAME + FILE_NAME;

        }

    }
}
