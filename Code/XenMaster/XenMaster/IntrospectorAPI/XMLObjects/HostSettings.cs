using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenMaster.IntrospectorAPI.XMLObjects
{
    [Serializable]
    public class HostSettings
    {
        private string _Ip;
        private string _Port;
        private string _Id;

        private string _LibvirtIp;
        private string _LibvirtPort;

        private string _AgentListenerIp;
        private string _AgentListenerPort;

        private string _LastUpdate;

        public HostSettings() { }


        public string Ip
        {
            get
            {
                return _Ip;
            }

            set
            {
                _Ip = value;
            }
        }

        public string Port
        {
            get
            {
                return _Port;
            }

            set
            {
                _Port = value;
            }
        }

        public string Id
        {
            get
            {
                return _Id;
            }

            set
            {
                _Id = value;
            }
        }

        public string LibvirtIp
        {
            get
            {
                return _LibvirtIp;
            }

            set
            {
                _LibvirtIp = value;
            }
        }

        public string LibvirtPort
        {
            get
            {
                return _LibvirtPort;
            }

            set
            {
                _LibvirtPort = value;
            }
        }

        public string AgentListenerIp
        {
            get
            {
                return _AgentListenerIp;
            }

            set
            {
                _AgentListenerIp = value;
            }
        }

        public string AgentListenerPort
        {
            get
            {
                return _AgentListenerPort;
            }

            set
            {
                _AgentListenerPort = value;
            }
        }

        public string LastUpdate
        {
            get
            {
                return _LastUpdate;
            }

            set
            {
                _LastUpdate = value;
            }
        }
    }
}
