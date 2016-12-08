using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XenMaster.LibvirtAPI
{
    [Serializable]
    public sealed class VirtualMachineBuilder
    {
        public static readonly string XEN_PATH_DEFAULT_ISO = "/var/lib/xen/images/";
        public static readonly string XEN_PATH_DEFAULT_DISK = "/home/guest_images/";
        public static readonly string XEN_PATH_DEFAULT_STORAGE = "/home/guest_images/storage";
        public static readonly string XEN_PATH_DEFAULT_EMULATOR = "/usr/lib64/xen/bin/qemu-system-i386";

        /// <summary>
        /// Transient domains only exist until the domain is shutdown or when the host server is restarted.
        //  Persistent domains last indefinitely and are repesentative of the normal use of a virtual machine.
        /// </summary>
        public enum DomainType
        {
            TRANSIENT,
            PERSISTENT
        }

        public enum ClockTime
        {

            utc,
            localtime, 
            timezone,
            variable 
        }
        DomainType _DomType = DomainType.PERSISTENT;
        ClockTime _ClockOffset = ClockTime.utc;


        string _VmName;
        string _Iso;
        string _VHD;
        int _Memory = 500000;
        int _HDMemory = 8000; //megabytes
        int _Vcpu = 1;
        bool _UseExistingDisk = false;
        bool _DiskCopy = false;
        bool _CreateForIntrospection = true;

        HypervisorFeatures _HypervisorFeatures;
        Power _Power;
        DeviceList _Devices;

        #region Public Properties
        public ClockTime ClockOffset
        {
            get
            {
                return _ClockOffset;
            }

            set
            {
                _ClockOffset = value;
            }
        }

        public bool UseExistingDisk
        {
            get
            {
                return _UseExistingDisk;
            }

            set
            {
                _UseExistingDisk = value;
            }
        }

        public DomainType DomType
        {
            get
            {
                return _DomType;
            }

            set
            {
                _DomType = value;
            }
        }

        public string VmName
        {
            get
            {
                return _VmName;
            }

            set
            {
                _VmName = value;
            }
        }
        public string VHD
        {
            get
            {
                return _VHD;
            }

            set
            {
                _VHD = value;
            }
        }

        public bool CreateForIntrospection
        {
            get
            {
                return _CreateForIntrospection;
            }

            set
            {
                _CreateForIntrospection = value;
            }
        }

        public string Iso
        {
            get
            {
                return _Iso;
            }

            set
            {
                _Iso = value;
            }
        }

        public int Memory
        {
            get
            {
                return _Memory;
            }

            set
            {
                _Memory = value;
            }
        }

        public int HDMemory
        {
            get
            {
                return _HDMemory;
            }

            set
            {
                _HDMemory = value;
            }
        }

        public int Vcpu
        {
            get
            {
                return _Vcpu;
            }

            set
            {
                _Vcpu = value;
            }
        }

        public HypervisorFeatures HypervisorFeatures
        {
            get
            {
                return _HypervisorFeatures;
            }

            set
            {
                _HypervisorFeatures = value;
            }
        }

        public Power Power
        {
            get
            {
                return _Power;
            }

            set
            {
                _Power = value;
            }
        }

        
        public DeviceList Devices
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

        public bool DiskCopy
        {
            get
            {
                return _DiskCopy;
            }

            set
            {
                _DiskCopy = value;
            }
        }

     

        #endregion

        public VirtualMachineBuilder()
        {

        }

        /// <summary>
        ///  Templated constructor for easy start up of a virtual machine from ISO file
        /// </summary>
        /// <param name="VmName"></param>
        /// <param name="memory"></param>
        /// <param name="vcpu"></param>
        /// <param name="iso"></param>
        public VirtualMachineBuilder(string VmName, int memory, int vcpu, string iso, DomainType type)
        {
            HypervisorFeatures = new HypervisorFeatures();
            Power = new Power();
            _Devices = new DeviceList(XEN_PATH_DEFAULT_EMULATOR);
            this.VmName = VmName;
            Memory = memory;
            Vcpu = vcpu;
            Iso = iso;
            DomType = type;

            //add Default boot cdrom
            Disk cdrom = new Disk(Disk.DiskTypes.file, Disk.DiskDevices.cdrom);
            cdrom.SourceFile = iso;
            cdrom.BackingStore = false;
            cdrom.ReadOnly = true;
            cdrom.TargetDevice = Disk.DiskBoot.hdc;
            cdrom.TargetBus = Disk.DiskBus.ide;
            cdrom.AddressType = "drive";
            cdrom.AddressController = 0;
            cdrom.AddressBus = 0;
            cdrom.AddressTarget = 0;
            cdrom.AddressUnit = 0;
            cdrom.Role = Disk.DiskRole.secondary;
            _Devices.Disks.Add(cdrom);

        }

        /// <summary>
        ///  Templated constructor for easy start up of a virtual machine from VHD file
        /// </summary>
        /// <param name="VmName"></param>
        /// <param name="memory"></param>
        /// <param name="vcpu"></param>
        /// <param name="iso"></param>
        public VirtualMachineBuilder(string VmName, string vhd, int memory, int vcpu, DomainType type)
        {
            HypervisorFeatures = new HypervisorFeatures();
            Power = new Power();
            _Devices = new DeviceList(XEN_PATH_DEFAULT_EMULATOR);
            this.VmName = VmName;
            Memory = memory;
            Vcpu = vcpu;
            Iso = "";
            VHD = vhd;
            DomType = type;

            //add Default boot cdrom
            Disk cdrom = new Disk(Disk.DiskTypes.file, Disk.DiskDevices.cdrom);
            cdrom.SourceFile = Iso;
            cdrom.BackingStore = false;
            cdrom.ReadOnly = true;
            cdrom.TargetDevice = Disk.DiskBoot.hdc;
            cdrom.TargetBus = Disk.DiskBus.ide;
            cdrom.AddressType = "drive";
            cdrom.AddressController = 0;
            cdrom.AddressBus = 0;
            cdrom.AddressTarget = 0;
            cdrom.AddressUnit = 0;
            cdrom.Role = Disk.DiskRole.secondary;
            _Devices.Disks.Add(cdrom);

        }

        public void addDisk(Disk.DiskTypes type, Disk.DiskDevices device, Disk.DiskBoot targetDev, Disk.DiskBus targetBus,
            bool diskReadonly, bool backingStore, string sourceFile,string addrType, int addrController, int addrBus, int addrTarget, int addrUnit)
        {
            Disk disk = new Disk(type, device);
            disk.SourceFile = sourceFile;
            disk.BackingStore = backingStore;
            disk.ReadOnly = diskReadonly;
            disk.TargetDevice = targetDev;
            disk.TargetBus = targetBus;
            disk.AddressType = addrType;
            disk.AddressController = addrController;
            disk.AddressBus = addrBus;
            disk.AddressTarget = addrTarget;
            disk.AddressUnit = addrUnit;
            _Devices.Disks.Add(disk);
        }

        public void addDisk(Disk.DiskTypes type, Disk.DiskDevices device, Disk.DiskBoot targetDev, Disk.DiskBus targetBus, Disk.DiskRole role,
            bool diskReadonly, bool backingStore, string sourceFile,string addrType, int addrController, int addrBus, int addrTarget, int addrUnit)
        {
            Disk disk = new Disk(type, device);
            disk.SourceFile = sourceFile;
            disk.BackingStore = backingStore;
            disk.ReadOnly = diskReadonly;
            disk.TargetDevice = targetDev;
            disk.TargetBus = targetBus;
            disk.AddressType = addrType;
            disk.AddressController = addrController;
            disk.AddressBus = addrBus;
            disk.AddressTarget = addrTarget;
            disk.AddressUnit = addrUnit;
            disk.Role = role;
            _Devices.Disks.Add(disk);
        }

        /// <summary>
        /// Set the hard drive to a pre-existing already installed virtual machine image and whether or not it should use a backingstore
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backingStore"></param>
        public void setHardDriveExisting(bool backingStore, bool diskCopy, string path)
        {
            Disk hd = new Disk(Disk.DiskTypes.file, Disk.DiskDevices.disk);
            hd.SourceFile = path;
            hd.BackingStore = backingStore;
            hd.TargetDevice = Disk.DiskBoot.hda;
            hd.TargetBus = Disk.DiskBus.ide;
            hd.AddressType = "drive";
            hd.AddressController = 0;
            hd.AddressBus = 0;
            hd.AddressTarget = 0;
            hd.AddressUnit = 0;
            hd.Role = Disk.DiskRole.primary;
            _Devices.Disks.Add(hd);
            _UseExistingDisk = true;
            _DiskCopy = diskCopy;


        }

        /// <summary>
        /// Set the path to the hard drive image and whether or not it should use a backingstore
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backingStore"></param>
        public void setHardDrive(int size, bool backingStore)
        {
            if (size == 0) throw new ArgumentException("The HD Memory size must be greater than 0");

            Disk hd = new Disk(Disk.DiskTypes.file, Disk.DiskDevices.disk);
            hd.SourceFile = XEN_PATH_DEFAULT_DISK+VmName+".img";
            hd.BackingStore = backingStore;
            hd.TargetDevice = Disk.DiskBoot.hda;
            hd.TargetBus = Disk.DiskBus.ide;
            hd.AddressType = "drive";
            hd.AddressController = 0;
            hd.AddressBus = 0;
            hd.AddressTarget = 0;
            hd.AddressUnit = 0;
            hd.Role = Disk.DiskRole.primary;
            HDMemory = size;
            _Devices.Disks.Add(hd);
            
        }

        /// <summary>
        /// The contents of the emulator element specify the fully qualified path to the device model emulator binary.
        /// </summary>
        /// <param name="path"></param>
        public void setEmulator(string path)
        {
           // _Devices = new DeviceList(path);
        }

        /// <summary>
        /// Sets the virtual machine memory (RAM) each where each value 1,2,3,...n is Mega Bytes MB
        /// </summary>
        /// <param name="memory"></param>
        public void setMemory(int memory)
        {
            if (memory == 0) throw new ArgumentException("The memory value cannot be 0");
            _Memory = memory;
        }

        /// <summary>
        /// Sets the path to the virtual machine iso image
        /// </summary>
        /// <param name="iso"></param>
        public void setIsoImage(string iso)
        {
            _Iso = iso;
        }

        /// <summary>
        /// Sets the virtual machine's name
        /// </summary>
        /// <param name="name"></param>
        public void setVmName(string name)
        {
            _VmName = name;
        }

        /// <summary>
        /// The content of this element defines the maximum number of virtual CPUs allocated for the guest OS, which must be between 1 and the maximum supported by the hypervisor.
        /// </summary>
        /// <param name="count"></param>
        public void setVcpus(int count)
        {
            _Vcpu = count;
        }

        /// <summary>
        /// The guest clock will always be synchronized to UTC when booted
        /// The guest clock will be synchronized to the host's configured timezone when booted
        /// (Not Implemented) The guest clock will be synchronized to the requested timezone using the timezone attribute
        /// (Not Implement) The guest clock will have an arbitrary offset applied relative to UTC or localtime, depending on the basis attribute. The delta relative to UTC (or localtime) is specified in seconds, using the adjustment attribute.
        /// </summary>
        /// <param name="offset"></param>
        public void setClockOffset(ClockTime offset)
        {
            if (offset == ClockTime.timezone || offset == ClockTime.variable) throw new ArgumentException(offset.ToString() + " Is not implemented");
            ClockTime _ClockOffset = offset;
        }

        /// <summary>
        /// pae - Physical address extension mode allows 32-bit guests to address more than 4 GB of memory.
        /// acpi - ACPI is useful for power management, for example, with KVM guests it is required for graceful shutdown to work.
        /// apic - APIC allows the use of programmable IRQ management.
        /// </summary>
        /// <param name="acpi"></param>
        /// <param name="apic"></param>
        /// <param name="pae"></param>
        public void setHypervisorFeatures(bool acpi, bool apic, bool pae)
        {
            HypervisorFeatures = new HypervisorFeatures(acpi, apic, pae);
        }


        public static XElement RemoveAllNamespaces(XElement e)
        {
            return new XElement(e.Name.LocalName,
               (from n in e.Nodes()
                select ((n is XElement) ? RemoveAllNamespaces(n as XElement) : n)),
               (e.HasAttributes) ? (from a in e.Attributes()
                                    where (!a.IsNamespaceDeclaration)
                                    select new XAttribute(a.Name.LocalName, a.Value)) : null);
        }

        public string toXMLString()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.NewLineHandling = NewLineHandling.None;
            settings.Indent = false;
            StringWriter Swriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(Swriter, settings);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            XmlSerializer serializer = new XmlSerializer(this.GetType());

            serializer.Serialize(writer, this, namespaces);
            string s = Swriter.ToString();
            return s;
        }
    }
}
