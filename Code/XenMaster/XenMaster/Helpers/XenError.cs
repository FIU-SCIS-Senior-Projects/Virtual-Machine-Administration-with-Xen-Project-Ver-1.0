using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.Helpers
{    
    /// <summary>
    /// Error Wrapper for the XenMaster class
    /// </summary>
    public class XenError
    {
        public enum XenErrorType
        {
            CONNECTION_FAILURE,
            VM_START_FAILURE,
            VM_STOP_FAILURE,
            VM_PAUSE_FAILURE,
            VM_RESUME_FAILURE,
            HOST_DETAILS_FAILURE,
            FILE_TRANSFER_FAILURE,
            VM_PROCESS_LIST_RETRIEVAL_FAILURE,
            VM_CLONE_FAILURE,
            VM_DELETE_FAILURE,
            VM_FORCE_SHUTDOWN,
            VM_FORCE_SHUTDOWN_FAILURE,
            CREATE_VM_FAILURE,
            VM_STATS_FAILURE,
            SHARED_DRIVE_LIST_RETRIEVAL_FAILURE,
            MALWARE_NOT_FOUND,
            SUCCESS
        }

        private string _message;
        private XenErrorType _type;

        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
            }
        }

        public XenErrorType Type
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

        public XenError() {
            _type = XenErrorType.SUCCESS;
        }

        public XenError(string message, XenErrorType type)
        {
            _message = message;
            _type = type;
        }
    }
}
