using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.IntrospectorAPI.XMLObjects
{
    [Serializable]
    public class ProcessList
    {

        string _VM;
        List<Processes> _ProcessBlock;

        public string VM
        {
            get
            {
                return _VM;
            }

            set
            {
                _VM = value;
            }
        }

        public List<Processes> ProcessBlock
        {
            get
            {
                return _ProcessBlock;
            }

            set
            {
                _ProcessBlock = value;
            }
        }

        public ProcessList()
        {

        }
    }
}
