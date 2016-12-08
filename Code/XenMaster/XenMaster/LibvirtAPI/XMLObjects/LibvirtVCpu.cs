using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XenMaster.LibvirtAPI.XMLObjects
{
    [Serializable, XmlRoot("VCpus")]
    public class LibvirtVCpu
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

        public int VcpuNumber
        {
            get
            {
                return _VcpuNumber;
            }

            set
            {
                _VcpuNumber = value;
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
            }
        }

        public LibvirtVCpu()
        {

        }
    }
}
