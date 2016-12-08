using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XenMaster.LibvirtAPI.XMLObjects
{
    [Serializable, XmlRoot("DiskStats")]
    public class LibvirtDiskStats
    {
        private string _Filesystem;
        private string _Size;
        private string _UsedSpace;
        private string _AvailableSpace;
        private int _UsedPercentage;
        private string _Mount;

        public string Filesystem
        {
            get
            {
                return _Filesystem;
            }

            set
            {
                _Filesystem = value;
            }
        }

        public string Size
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

        public string UsedSpace
        {
            get
            {
                return _UsedSpace;
            }

            set
            {
                _UsedSpace = value;
            }
        }

        public string AvailableSpace
        {
            get
            {
                return _AvailableSpace;
            }

            set
            {
                _AvailableSpace = value;
            }
        }

        public int UsedPercentage
        {
            get
            {
                return _UsedPercentage;
            }

            set
            {
                _UsedPercentage = value;
            }
        }

        public string Mount
        {
            get
            {
                return _Mount;
            }

            set
            {
                _Mount = value;
            }
        }

        public LibvirtDiskStats()
        {

        }
    }
}
