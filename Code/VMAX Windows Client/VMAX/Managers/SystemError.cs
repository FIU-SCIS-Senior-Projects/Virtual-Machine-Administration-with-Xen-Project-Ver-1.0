using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMAX.Managers
{
    public class SystemError
    {
        public enum SystemErrorType
        {
            Add_New_Host_Failure,
            Success
        }

        string _Message;
        SystemErrorType _Type;

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

        public SystemErrorType Type
        {
            get
            {
                return _Type;
            }

            set
            {
                _Type = value;
            }
        }

        public SystemError(string message, SystemErrorType type)
        {
            Message = message;
            Type = type;
        }
    }
}
