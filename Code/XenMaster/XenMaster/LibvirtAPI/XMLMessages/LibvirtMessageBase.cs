using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XenMaster.LibvirtAPI.XMLObjects;

namespace XenMaster.LibvirtAPI.XMLMessages
{
    [Serializable]
    public abstract class LibvirtMessageBase
    {
        public enum LIBVIRT_MESSAGE_TYPE
        {
            HELLO,
            LIST,
            START,
            SHUTDOWN,
            PAUSE,
            STOP,
            HOST_DETAILS,
            LIST_DISKS,
            LIST_IMAGES,
            CREATE_VM,
            FORCE_SHUTDOWN,
            DELETE_VM,
            GET_VNC_PORT,
            TRANSFER_FILE,
            LIST_VM_STATISTICS,
            CLONE_VM,
            LIST_DISK_USAGE,
            LIST_VHD_STORAGE,
            RESUME,
            UNKNOWN,
        }

        [XmlElement("Command")]
        public string Command { get; set; }

        [XmlElement("Requestor")]
        public string Requestor { get; set; }

        [XmlElement("RequestorId")]
        public string RequestorId { get; set; }

        [XmlElement("Response")]
        public string Response { get; set; }

        [XmlElement("ResponseCode")]
        public int ResponseCode { get; set; }

        [XmlElement("ResponseMessage")]
        public string ResponseMessage { get; set; }

        [XmlElement("Host")]
        public LibvirtHost Host { get; set; }

        [XmlElement("VirtualMachineBuilder")]
        public VirtualMachineBuilder VirtualMachineBuilder
        {
            get
            {
                return _VirtualMachineBuilder;
            }
            set
            {
                _VirtualMachineBuilder = value;
            }
        }
        private VirtualMachineBuilder _VirtualMachineBuilder;

        public static LibvirtMessageWrapper BuildMessage(LIBVIRT_MESSAGE_TYPE type, object Payload, string requestor, string requestorid)
        {
            LibvirtMessageWrapper wrapper = new LibvirtMessageWrapper();

            switch (type)
            {
                case LIBVIRT_MESSAGE_TYPE.CREATE_VM:
                    LibvirtCommand createVmMessage = new LibvirtCommand();
                    wrapper.Message = createVmMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;

                    VirtualMachineBuilder builder = (VirtualMachineBuilder)Payload;
                    wrapper.VirtualMachineBuilder = builder;
                    break;
            }

            return wrapper;

        }


        public static LibvirtMessageWrapper BuildMessage(LIBVIRT_MESSAGE_TYPE type, string requestor, string requestorid)
        {
            LibvirtMessageWrapper wrapper = new LibvirtMessageWrapper();

            switch (type)
            {
                case LIBVIRT_MESSAGE_TYPE.LIST:
                    LibvirtCommand listDomainsMessage = new LibvirtCommand();
                    wrapper.Message = listDomainsMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.START:
                    LibvirtCommand startDomainMessage = new LibvirtCommand();
                    wrapper.Message = startDomainMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.STOP:
                    LibvirtCommand stopDomainMessage = new LibvirtCommand();
                    wrapper.Message = stopDomainMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.PAUSE:
                    LibvirtCommand pauseDomainMessage = new LibvirtCommand();
                    wrapper.Message = pauseDomainMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.RESUME:
                    LibvirtCommand resumeDomainMessage = new LibvirtCommand();
                    wrapper.Message = resumeDomainMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.HOST_DETAILS:
                    LibvirtCommand hostDetailsMessage = new LibvirtCommand();
                    wrapper.Message = hostDetailsMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.LIST_DISKS:
                    LibvirtCommand listDisksMessage = new LibvirtCommand();
                    wrapper.Message = listDisksMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.LIST_IMAGES:
                    LibvirtCommand listIsoMessage = new LibvirtCommand();
                    wrapper.Message = listIsoMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.CREATE_VM:
                    LibvirtCommand createVmMessage = new LibvirtCommand();
                    wrapper.Message = createVmMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.FORCE_SHUTDOWN:
                    LibvirtCommand ForceShutdownVmMessage = new LibvirtCommand();
                    wrapper.Message = ForceShutdownVmMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.GET_VNC_PORT:
                    LibvirtCommand vncPortMessage = new LibvirtCommand();
                    wrapper.Message = vncPortMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.TRANSFER_FILE:
                    LibvirtCommand transferFileMessage = new LibvirtCommand();
                    wrapper.Message = transferFileMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.CLONE_VM:
                    LibvirtCommand cloneMessage = new LibvirtCommand();
                    wrapper.Message = cloneMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;

                case LIBVIRT_MESSAGE_TYPE.DELETE_VM:
                    LibvirtCommand deletMessage = new LibvirtCommand();
                    wrapper.Message = deletMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.LIST_VM_STATISTICS:
                    LibvirtCommand listStatsMessage = new LibvirtCommand();
                    wrapper.Message = listStatsMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.LIST_DISK_USAGE:
                    LibvirtCommand listDiskUsageMessage = new LibvirtCommand();
                    wrapper.Message = listDiskUsageMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
                case LIBVIRT_MESSAGE_TYPE.LIST_VHD_STORAGE:
                    LibvirtCommand listVHDMessage = new LibvirtCommand();
                    wrapper.Message = listVHDMessage;
                    wrapper.Command = MessageTypeToString(type);
                    wrapper.Requestor = requestor;
                    wrapper.RequestorId = requestorid;
                    break;
            }

            return wrapper;

        }

        public static LIBVIRT_MESSAGE_TYPE CommandToMessageType(string command)
        {
            switch (command)
            {
                case "LIST VM":
                    return LIBVIRT_MESSAGE_TYPE.LIST;
                default:
                    return LIBVIRT_MESSAGE_TYPE.UNKNOWN;
            }
        }

        public static string MessageTypeToString(LIBVIRT_MESSAGE_TYPE type)
        {
            switch (type)
            {
                case LIBVIRT_MESSAGE_TYPE.LIST:
                    return "LIST VM";
                case LIBVIRT_MESSAGE_TYPE.START:
                    return "START VM";
                case LIBVIRT_MESSAGE_TYPE.STOP:
                    return "SHUTDOWN VM";
                case LIBVIRT_MESSAGE_TYPE.PAUSE:
                    return "PAUSE VM";
                case LIBVIRT_MESSAGE_TYPE.RESUME:
                    return "RESUME VM";
                case LIBVIRT_MESSAGE_TYPE.HOST_DETAILS:
                    return "HOST DETAILS";
                case LIBVIRT_MESSAGE_TYPE.LIST_DISKS:
                    return "LIST DISKS";
                case LIBVIRT_MESSAGE_TYPE.LIST_IMAGES:
                    return "LIST IMAGES";
                case LIBVIRT_MESSAGE_TYPE.CREATE_VM:
                    return "CREATE VM";
                case LIBVIRT_MESSAGE_TYPE.FORCE_SHUTDOWN:
                    return "FORCE SHUTDOWN VM";
                case LIBVIRT_MESSAGE_TYPE.GET_VNC_PORT:
                    return "GET VNC PORT";
                case LIBVIRT_MESSAGE_TYPE.TRANSFER_FILE:
                    return "TRANSFER FILE";
                case LIBVIRT_MESSAGE_TYPE.CLONE_VM:
                    return "CLONE VM";
                case LIBVIRT_MESSAGE_TYPE.DELETE_VM:
                    return "DELETE VM";
                case LIBVIRT_MESSAGE_TYPE.LIST_VM_STATISTICS:
                    return "LIST VM STATISTICS";
                case LIBVIRT_MESSAGE_TYPE.LIST_DISK_USAGE:
                    return "LIST DISK USAGE";
                case LIBVIRT_MESSAGE_TYPE.LIST_VHD_STORAGE:
                    return "LIST VHD STORAGE";
                default:
                    return "ERROR";
            }
        }
    }
}
