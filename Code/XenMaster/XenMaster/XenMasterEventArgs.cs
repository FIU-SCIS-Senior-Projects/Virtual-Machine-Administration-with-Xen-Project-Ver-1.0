using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenMaster.Helpers;

namespace XenMaster
{
    public class XenMasterEventArgs
    {
        public enum Serverity
        {
            Critical,
            Warning,
            Error,
            Notification,
        }

        public enum MasterEventType
        {
            CONNECTION_FAILURE,
            VM_START_FAILURE,
            VM_STOP_FAILURE,
            VM_PAUSE_FAILURE,
            VM_RESUME_FAILURE,
            VM_CLONE_FAILURE,
            VM_LIST_FAILURE,
            VM_DELETED,
            VM_DELETE_FAILURE,
            VM_FORCE_SHUTDOWN_FAILURE,
            VM_FORCE_SHUTDOWN,
            VM_STOP,
            VM_PAUSE,
            VM_RESUME,
            CREATE_VM_FAILURE,
            CREATE_VM,
            HOST_DETAILS_FAILURE,
            HOST_DETAILS,
            VM_LIST,
            VM_START,
            VM_CLONE,
            ISO_RETRIEVAL,
            ISO_RETRIEVAL_FAILURE,
            VHD_RETRIEVAL,
            VHD_RETRIEVAL_FAILURE,
            FILE_TRANSFER_FAILURE,
            VM_PROCESS_LIST_RETRIEVAL_FAILURE,
            HOST_OFFLINE,
            ERROR,
        }

        string _Message;
        string _CanonicalType;
        MasterEventType _Type;
        Serverity _Severity;

        public string Message
        {
            get
            {
                return _Message;
            }

            set
            {
                _Message = value;
            }
        }

        public MasterEventType Type
        {
            get
            {
                return _Type;
            }

            set
            {
                _Type = value;
                _CanonicalType = _Type.ToString().Replace("_"," ");
            }
        }

        public Serverity Severity
        {
            get
            {
                return _Severity;
            }

            set
            {
                _Severity = value;
            }
        }

        public string CanonicalType
        {
            get
            {
                return _CanonicalType;
            }

            set
            {
                _CanonicalType = value;
            }
        }

        public XenMasterEventArgs(string message, MasterEventType type, Serverity severity)
        {
            Message = message;
            Type = type;
            Severity = severity;
            CanonicalType = _Type.ToString().Replace("_", " ");
        }


    }
}
