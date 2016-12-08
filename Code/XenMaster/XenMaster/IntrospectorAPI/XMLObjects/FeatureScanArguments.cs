using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.IntrospectorAPI.XMLObjects
{
    [Serializable]
    public class FeatureScanArguments
    {
        private ScanTypes _type;
        private int _duration;
        private string _vm;
        private string _uuid;
        private ProcessList _exclusionList { get; set; }


        public ScanTypes Type
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

        public int Duration
        {
            get
            {
                return _duration;
            }

            set
            {
                _duration = value;
            }
        }

        public string Vm
        {
            get
            {
                return _vm;
            }

            set
            {
                _vm = value;
            }
        }

        public string Uuid
        {
            get
            {
                return _uuid;
            }

            set
            {
                _uuid = value;
            }
        }

        public ProcessList ExclusionList
        {
            get
            {
                return _exclusionList;
            }

            set
            {
                _exclusionList = value;
            }
        }


        public FeatureScanArguments(string vmName, int duration, ScanTypes type)
        {
            Vm = vmName;
            Duration = duration;
            Type = type;
        }

        public FeatureScanArguments(string vmName, string uuid, int duration, ScanTypes type)
        {
            Vm = vmName;
            Duration = duration;
            Type = type;
            Uuid = uuid;
        }
        public FeatureScanArguments(){}

        public void setExclusionList(ProcessList list)
        {
            if (list == null) throw new NullReferenceException("The Exclusion list cannot be null");
            _exclusionList = list;
        }

        public ProcessList getExclusionList()
        {
            return _exclusionList;
        }



    }
}
