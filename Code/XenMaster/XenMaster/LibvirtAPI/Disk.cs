using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.LibvirtAPI
{
    /// <summary>
    /// <disk type='file' device='disk'>
    ///          <source file = "/home/guest_images/BuntuD.img" ></ source >
    ///          < backingStore ></ backingStore >
    ///          < target dev="hda" bus="ide"></target>
    ///          <address type = "drive" controller="0" bus="0" target="0" unit="0"></address>
    ///      </disk>
    /// </summary>
    [Serializable]
    public class Disk
    {
        public enum DiskTypes
        {
           file,device, rawio, sigio, snapshot
        }

        public enum DiskBoot
        {
            hdc, vda, vdb,hda
        }

        public enum DiskSources
        {
            file, block, dir, network, volume
        }

        public enum DiskDevices
        {
            floppy, disk, cdrom, lun
        }

        public enum DiskBus
        {
             ide, scsi, virtio, xen, usb, sata, sd 
        }

        public enum DiskRole
        {
            primary,secondary
        }

        DiskTypes _type;
        DiskDevices _device;
        DiskBoot _targetDevice;
        DiskBus _targetBus;
        DiskRole _role;

        bool _backingStore;
        bool _readOnly;

        string _sourceFile;
        string _addressType;
        int _addressController;
        int _addressBus;
        int _addressTarget;
        int _addressUnit;
        
        #region Public Properties

        public bool BackingStore
        {
            get
            {
                return _backingStore;
            }

            set
            {
                _backingStore = value;
            }
        }

        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }

            set
            {
                _readOnly = value;
            }
        }

        public string SourceFile
        {
            get
            {
                return _sourceFile;
            }

            set
            {
                _sourceFile = value;
            }
        }


        public string AddressType
        {
            get
            {
                return _addressType;
            }

            set
            {
                _addressType = value;
            }
        }

        public int AddressController
        {
            get
            {
                return _addressController;
            }

            set
            {
                _addressController = value;
            }
        }

        public int AddressBus
        {
            get
            {
                return _addressBus;
            }

            set
            {
                _addressBus = value;
            }
        }

        public int AddressTarget
        {
            get
            {
                return _addressTarget;
            }

            set
            {
                _addressTarget = value;
            }
        }

        public int AddressUnit
        {
            get
            {
                return _addressUnit;
            }

            set
            {
                _addressUnit = value;
            }
        }

        public DiskTypes Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }

        public DiskDevices Device
        {
            get
            {
                return _device;
            }

            set
            {
                _device = value;
            }
        }

        public DiskBoot TargetDevice
        {
            get
            {
                return _targetDevice;
            }

            set
            {
                _targetDevice = value;
            }
        }

        public DiskBus TargetBus
        {
            get
            {
                return _targetBus;
            }

            set
            {
                _targetBus = value;
            }
        }

        public DiskRole Role
        {
            get
            {
                return _role;
            }

            set
            {
                _role = value;
            }
        }


        #endregion

        public Disk() { }

        public Disk(DiskTypes type, DiskDevices device)
        {
            Type = type;
            Device = device;
        }
        
    }
}
