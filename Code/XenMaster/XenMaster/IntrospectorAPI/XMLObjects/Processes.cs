using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XenMaster.IntrospectorAPI.XMLObjects
{
    [Serializable]
    public class Processes
    {
        string _Name;
        int _Pid;
        bool _significant = true;

        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
            }
        }

        public int Pid
        {
            get
            {
                return _Pid;
            }

            set
            {
                _Pid = value;
            }
        }

        [XmlIgnore]
        public bool Significant
        {
            get
            {
                return _significant;
            }

            set
            {
                _significant = value;
            }
        }

        public Processes() { }


    }
}
