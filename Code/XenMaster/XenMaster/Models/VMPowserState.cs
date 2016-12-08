using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.Models
{
    public enum vm_state
    {
        Running, Blocked, Paused, Shutdown, Shutoff, Crashed, Suspended, Unknown
    }
    public class VMPowerState
    {

        public static string ToString(vm_state x)
        {
            switch (x)
            {
                case vm_state.Running:
                    return "Running";
                case vm_state.Blocked:
                    return "Blocked";
                case vm_state.Paused:
                    return "Paused";
                case vm_state.Shutdown:
                    return "Shutting Down";
                case vm_state.Shutoff:
                    return "Halted";
                case vm_state.Crashed:
                    return "Crashed";
                case vm_state.Suspended:
                    return "Suspended";
                default:
                    return "Unknown";
            }
        }

        public static vm_state getEnumeratePowerState(int state)
        {
            switch (state)
            {
                case 1:
                    return vm_state.Running;
                case 2:
                    return vm_state.Blocked;
                case 3:
                    return vm_state.Paused;
                case 4:
                    return vm_state.Shutdown;
                case 5:
                    return vm_state.Shutoff;
                case 6:
                    return vm_state.Crashed;
                case 7:
                    return vm_state.Suspended;
                default:
                    return vm_state.Unknown;
            }
        }
    }
}
