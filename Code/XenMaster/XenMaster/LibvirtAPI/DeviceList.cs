using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.LibvirtAPI
{
    [Serializable]
    public class DeviceList
    {
        string _Emulator;

        List<Disk> _Devices;

        public string Emulator
        {
            get
            {
                return _Emulator;
            }

            set
            {
                _Emulator = value;
            }
        }

        public List<Disk> Disks
        {
            get
            {
                return _Devices;
            }

            set
            {
                _Devices = value;
            }
        }

        public DeviceList() { }

        public DeviceList(string emulator)
        {
            Emulator = emulator;
            _Devices = new List<Disk>();
        }
    }
}
