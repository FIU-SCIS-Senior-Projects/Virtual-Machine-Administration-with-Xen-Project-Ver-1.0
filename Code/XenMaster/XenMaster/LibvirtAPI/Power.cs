using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.LibvirtAPI
{
    [Serializable]
    public class Power
    {
        public enum PowerSettings {
            destroy,
            restart,
            preserve
        }

        private PowerSettings powerOff = PowerSettings.destroy;
        private PowerSettings reboot = PowerSettings.restart;
        private PowerSettings crash = PowerSettings.destroy;

        #region Public Properties

        public PowerSettings PowerOff
        {
            get
            {
                return powerOff;
            }

            set
            {
                powerOff = value;
            }
        }

        public PowerSettings Reboot
        {
            get
            {
                return reboot;
            }

            set
            {
                reboot = value;
            }
        }

        public PowerSettings Crash
        {
            get
            {
                return crash;
            }

            set
            {
                crash = value;
            }
        }

        #endregion

        public Power() { }

    }
}
