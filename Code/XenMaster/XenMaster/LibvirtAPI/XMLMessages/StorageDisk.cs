using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.LibvirtAPI.XMLMessages
{
    public class StorageDisk
    {
        private string name;
        private string os;
        private string distribution;
        private string size;
        private string rekall;
        private string kernel;
        private string description;

        #region Public Properties
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Os
        {
            get
            {
                return os;
            }

            set
            {
                os = value;
            }
        }

        public string Distribution
        {
            get
            {
                return distribution;
            }

            set
            {
                distribution = value;
            }
        }

        public string SizeGB
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        public string Rekall
        {
            get
            {
                return rekall;
            }

            set
            {
                rekall = value;
            }
        }

        public string Kernel
        {
            get
            {
                return kernel;
            }

            set
            {
                kernel = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }
        #endregion

        public StorageDisk()
        {

        }
    }
}
