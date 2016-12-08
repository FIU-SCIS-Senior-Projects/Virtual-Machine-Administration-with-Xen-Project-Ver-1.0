using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.Models
{
    public class VirtualDisk
    {
        public static readonly double KB_PER_GB = 1073741824.0;

        public enum DiskFormats
        {
            ISO,
            RAW,
            VHD,
            VDI,
            WINDOWS,
            LINUX,
            UNKNOWN
        }
        DiskFormats _DiskFormat = DiskFormats.UNKNOWN;
        string _Name;
        ulong _Size;
        double _SizeGB;
        string _NameCanonical;
        string _Extension;
        string _Format;

        #region Public Properties

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                getParts();
            }
        }


        public ulong Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
               _SizeGB =  getSizeGB();
            }
        }

        public string NameCanonical
        {
            get
            {
                return _NameCanonical;
            }

            set
            {
                _NameCanonical = value;
            }
        }

        public string Extension
        {
            get
            {
                return _Extension;
            }

            set
            {
                _Extension = value;
            }
        }

        public double SizeGB
        {
            get
            {
                return _SizeGB;
            }

            set
            {
                _SizeGB = value;
            }
        }

        public string Format
        {
            get
            {
                return _Format;
            }

            set
            {
                _Format = value;
            }
        }

        public DiskFormats DiskFormat
        {
            get
            {
                return _DiskFormat;
            }

            set
            {
                _DiskFormat = value;
            }
        }
        #endregion

        public VirtualDisk()
        {

        }

        /// <summary>
        /// Split the file name to determine the format and extension of the file
        /// </summary>
        public void getParts()
        {
            char delim = '.';
            string[] parts = _Name.Split(delim);
            _NameCanonical = parts[0];
            _Extension = parts[1];
            _Extension = _Extension.ToUpper();

            switch(_Extension.ToLower())
            {
                case "iso":
                    Format = DiskFormats.ISO.ToString();
                    //ISO disks are typically windows or linux distributions and thus the format will reflect that
                    string discName = _NameCanonical.ToLower();
                    if (discName.Contains("win"))
                    {
                        Format = "Windows";
                        DiskFormat = DiskFormats.WINDOWS;
                    }
                    else if (discName.Contains("buntu"))
                    {
                        Format = "Linux";
                        DiskFormat = DiskFormats.LINUX;

                    }
                    else if (discName.Contains("centos"))
                    {
                        Format = "Linux";
                        DiskFormat = DiskFormats.LINUX;
                    }
                    else if (discName.Contains("rhel"))
                    {
                        Format = "Linux";
                        DiskFormat = DiskFormats.LINUX;
                    }
                    else
                    {
                        Format = "Unknown";
                    }

                    break;
                case "img":
                    Format = DiskFormats.RAW.ToString();
                    break;
                case "vhd":
                    Format = DiskFormats.VHD.ToString();
                    break;
                case "vdi":
                    Format = DiskFormats.VDI.ToString();
                    break;
                default:
                    Format = DiskFormats.UNKNOWN.ToString();
                    break;

            }
        }

        public double getSizeGB()
        {
            return _Size / KB_PER_GB;
        }
    }
}
