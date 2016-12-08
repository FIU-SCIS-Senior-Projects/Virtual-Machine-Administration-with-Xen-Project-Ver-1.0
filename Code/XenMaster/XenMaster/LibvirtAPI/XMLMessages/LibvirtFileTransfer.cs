using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XenMaster.LibvirtAPI.XMLMessages
{
    [Serializable]
    [XmlRoot("FileTransfer")]
    public class LibvirtFileTransfer
    {
        public string Filename
        {
            get
            {
                return _Filename;
            }

            set
            {
                _Filename = value;
            }
        }

        public string RemoteDirectory
        {
            get
            {
                return _RemoteDirectory;
            }

            set
            {
                _RemoteDirectory = value;
            }
        }

        public long Size
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


        /// <summary>
        /// Local Filename including directory
        /// </summary>
        string _Filename;
        /// <summary>
        /// Remote Directory location to place the file
        /// </summary>
        string _RemoteDirectory;
        /// <summary>
        /// Size of the file to be transferred
        /// </summary>
        long _Size;

        public LibvirtFileTransfer()
        {

        }

        public LibvirtFileTransfer(string _filename, string remoteDirectory, long size)
        {
            _Filename = _filename;
            _RemoteDirectory = remoteDirectory;
            _Size = size;
        }

    
    }
}
