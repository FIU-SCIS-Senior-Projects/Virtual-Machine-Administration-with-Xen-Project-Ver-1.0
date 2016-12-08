using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.Models
{
    public class VcpuInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Virtual Cpu number
        /// </summary>
        int _VcpuNumber;
        /// <summary>
        /// Virtual CPU state
        /// </summary>
        int _State;
        /// <summary>
        /// CPU usage time in nano seconds
        /// </summary>
        ulong _CpuTime;
        /// <summary>
        /// Real CPU number
        /// </summary>
        int _Cpu;

        //prop change
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public int VcpuNumber
        {
            get
            {
                return _VcpuNumber;
            }

            set
            {
                _VcpuNumber = value;
                NotifyPropertyChanged("VcpuNumber");
            }
        }

        public int State
        {
            get
            {
                return _State;
            }

            set
            {
                _State = value;
                NotifyPropertyChanged("State");
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
                NotifyPropertyChanged("CpuTime");
            }
        }

        public int Cpu
        {
            get
            {
                return _Cpu;
            }

            set
            {
                _Cpu = value;
                NotifyPropertyChanged("Cpu");
            }
        }

        public VcpuInfo(int number, int state, ulong time, int cpu)
        {
            VcpuNumber = number;
            State = state;
            CpuTime = time;
            Cpu = cpu;
        }
    }
}
