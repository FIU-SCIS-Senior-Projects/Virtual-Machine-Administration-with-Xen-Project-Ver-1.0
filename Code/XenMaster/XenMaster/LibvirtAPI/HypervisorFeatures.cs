using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.LibvirtAPI
{
    [Serializable]
    public class HypervisorFeatures
    {
        bool _acpi = true;
        bool _apic = true;
        bool _pae = true;

        #region Public Properties

        public bool Acpi
        {
            get
            {
                return _acpi;
            }

            set
            {
                _acpi = value;
            }
        }

        public bool Apic
        {
            get
            {
                return _apic;
            }

            set
            {
                _apic = value;
            }
        }

        public bool Pae
        {
            get
            {
                return _pae;
            }

            set
            {
                _pae = value;
            }
        }

        #endregion

        public HypervisorFeatures() { }

        public HypervisorFeatures(bool acpi, bool apic, bool pae)
        {
            Acpi = acpi;
            Apic = apic;
            Pae = pae;
        }

    }
}
