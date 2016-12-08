using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.Models
{
    public class VM : INotifyPropertyChanged
    {

        public static readonly ulong NSECS_PER_SECOND = 1000000000;
        public static readonly double KB_PER_GB = 1048576.0;
        vm_state _powerState;
        int _State;
        string _Name;
        string _UUID;
        string _Os;
        ulong _MaxMem;
        ulong _Memory;
        ulong _CpuTime;
        double _MemoryGB;
        double _MaxMemGB;
        uint _Id;
        int _VCpus;
        int _VNCPort;
        ulong _CpuTimeCanonical;
        string _CpuTimeStamp;
        List<VcpuInfo> _VcpuStats;


        //prop change
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }


        #region Properties
        public string Name {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");

            }
        }

        public uint Id {
            get
            { return _Id; }
            set
            {
                _Id = value;
                NotifyPropertyChanged("Id");

            }
        }

        public string UUID
        {
            get { return _UUID; }
            set
            {
                _UUID = value;
                NotifyPropertyChanged("UUID");

            }

        }

        public string Os { get; set; }

        public int State
        {
            get { return _State; }
            set
            {
                _State = value;
                PowerState = VMPowerState.getEnumeratePowerState(value);
                
                NotifyPropertyChanged("State");
            }
        }

        public ulong MaxMem
        {
            get { return _MaxMem; }
            set
            {
                _MaxMem = value;
                _MaxMemGB = _MaxMem / KB_PER_GB;
                NotifyPropertyChanged("MaxMem");

            }
        }

        public int VCpus {
            get { return _VCpus; }
            set
            {
                _VCpus = value;
                NotifyPropertyChanged("VCpus");

            }
        }

        public vm_state PowerState
        {
            get
            {
                return _powerState;
            }

            set
            {
                _powerState = value;
                NotifyPropertyChanged("PowerState");
            }
        }


        public ulong Memory
        {
            get
            {
                return _Memory;
            }

            set
            {
                _Memory = value;
                _MemoryGB = _Memory / KB_PER_GB;
                NotifyPropertyChanged("Memory");

            }
        }

        public ulong CpuTime
        {
            get
            {
                return _CpuTime;
            }

            set
            {
                _CpuTime = value;
                _CpuTimeCanonical = _CpuTime / NSECS_PER_SECOND;
                double cputime_seconds = _CpuTime / 1000000000;
                double cputime_minutes = cputime_seconds / 60;
                double cputime_hours = cputime_minutes / 60;
                string cpu_timestamp = string.Format("{0:0.0#}", cputime_hours) + " Hours " + (int)cputime_minutes % 60 + " Minutes ";
                CpuTimeStamp = cpu_timestamp;
                NotifyPropertyChanged("CpuTime");

            }
        }

        public ulong CpuTimeCanonical
        {
            get
            {
                return _CpuTimeCanonical;
            }

            set
            {
                _CpuTimeCanonical = value;
                NotifyPropertyChanged("CpuTimeCanonical");

            }
        }

        public string CpuTimeStamp
        {
            get
            {
                return _CpuTimeStamp;
            }
            set
            {
                _CpuTimeStamp = value;
                NotifyPropertyChanged("CpuTimeStamp");
            }
        }

        public double MemoryGB
        {
            get
            {
                return _MemoryGB;
            }

            set
            {
                _MemoryGB = value;
                NotifyPropertyChanged("MemoryGB");

            }
        }

        public double MaxMemGB
        {
            get
            {
                return _MaxMemGB;
            }

            set
            {
                _MaxMemGB = value;
                NotifyPropertyChanged("MaxMemGB");

            }
        }

        public int VNCPort
        {
            get
            {
                return _VNCPort;
            }

            set
            {
                _VNCPort = value;
            }
        }

        public List<VcpuInfo> VcpuStats
        {
            get
            {
                return _VcpuStats;
            }

            set
            {
                _VcpuStats = value;
                NotifyPropertyChanged("VcpuStats");
            }
        }
        #endregion

        #region Constructors

        public VM(string uuid)
        {
            UUID = uuid;
        }

        public VM(string name, string uuid)
        {
            Name = name;
            UUID = uuid;
        }

        public VM(string name, uint id, string uuid, string os, int state)
        {
            _Name = name;
            _Id = id;
            _UUID = uuid;
            _Os = os;
            _State = state;

        }

        public VM(string name, string uuid, uint id, string os, int state, uint maxmem, int vcpus)
        {
            _Name = name;
            _Id = id;
            _UUID = uuid;
            _Os = os;
            _State = state;
            _MaxMem = maxmem;
            _VCpus = vcpus;
        }


        public VM()
        {

        }
        #endregion

        /// <summary>
        /// Updates the fields of this VM with new values of the matching newVm object, that is
        /// stale data is replaced with data from a fresh call to the Xen host while still maintaining
        /// the object reference of the original vm object
        /// </summary>
        /// <param name="newVm"></param>
        public void update(VM newVm)
        {
            Name = newVm.Name;
            State = newVm.State;
            MaxMem = newVm.MaxMem;
            VCpus = newVm.VCpus;
            Memory = newVm.Memory;
            CpuTime = newVm.CpuTime;
        }

        public void updateVcpuStats(List<VcpuInfo> stats)
        {
            VcpuStats = stats;
        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append("Vm: " + Name);
            sb.Append(",");
            sb.Append("State: " + PowerState);
            sb.Append(",");
            sb.Append("Vcpus: " + VCpus);
            sb.Append(",");
            sb.Append("Max Memory: " + MaxMem);
            sb.Append("]");

            return sb.ToString();


        }

    }
}
