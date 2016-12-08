using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenMaster.IntrospectorAPI.XMLObjects;

namespace XenMaster.Models
{
    public class Host : INotifyPropertyChanged
    {
        public enum HOST_POWER_STATE
        {
            Online,
            Offline
        }

        #region Properties
        public HOST_POWER_STATE PowerState
        {
            get
            {
                return _PowerState;
            }

            set
            {
                _PowerState = value;
                NotifyPropertyChanged("PowerState");
            }
        }


        public Host(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        public Host(string ip, int port, string name)
        {
            Name = name;
            Ip = ip;
            Port = port;
        }

        public Host(string ip, int port, string id, string name, string description)
        {
            Name = name;
            Ip = ip;
            Port = port;
            Description = description;
            Id = id;
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
                NotifyPropertyChanged("Id");
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string Ip
        {
            get
            {
                return _Ip;
            }

            set
            {
                _Ip = value;
                NotifyPropertyChanged("Ip");
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }

            set
            {
                _Description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public int Port
        {
            get
            {
                return _Port;
            }

            set
            {
                _Port = value;
                NotifyPropertyChanged("Port");
            }
        }

        public int LibvirtPort
        {
            get
            {
                return _LibvirtPort;
            }

            set
            {
                _LibvirtPort = value;
                NotifyPropertyChanged("LibvirtPort");
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
                NotifyPropertyChanged("LibvirtIp");
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

        public int AgentListenerPort
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

        public string ConfigLastUpdate
        {
            get
            {
                return _ConfigLastUpdate;
            }

            set
            {
                _ConfigLastUpdate = value;
            }
        }

        public bool IsSynced
        {
            get
            {
                return isSynced;
            }

            set
            {
                isSynced = value;
            }
        }
        #endregion

        public VirtualMachineList VMList { get { return _VMList; } }

        public int VmCount
        {
            get
            {
                return _vmCount;
            }

            set
            {
                _vmCount = value;
                NotifyPropertyChanged("VmCount");
            }
        }

        public int HostNumber
        {
            get
            {
                return _HostNumber;
            }

            set
            {
                _HostNumber = value;
            }
        }

        private VirtualMachineList _VMList = new VirtualMachineList();

        private HOST_POWER_STATE _PowerState = HOST_POWER_STATE.Offline;

        /// <summary>
        /// Determines whether or not this host has been synchronized 
        /// </summary>
        private bool isSynced = false;

        private int _HostNumber = 0;

        private string _Id = "-";
        private string _Name = "-";
        private string _Ip;
        private string _Description = "-";
        private int _Port;

        private int _LibvirtPort;
        private string _LibvirtIp;

        private string _AgentListenerIp;
        private int _AgentListenerPort;

        private string _ConfigLastUpdate;

        private int _vmCount = 0;



        //prop change
        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append("Host:" + Name);
            sb.Append(",");
            sb.Append("Ip: " + Ip);
            sb.Append(",");
            sb.Append("Libvirt: " + LibvirtIp);
            sb.Append(",");
            sb.Append("Last Update: " +ConfigLastUpdate);
            sb.Append("]");

            return sb.ToString();
        }

        /// <summary>
        /// Update this hosts machine list with a list retrieved from the remote host
        /// </summary>
        /// <param name="listOfVms"></param>
        public void setVMList(VirtualMachineList listOfVms)
        {
            int totalUpdate = 0;
             
            foreach(KeyValuePair<string,VM> pair in listOfVms)
            {
                string uuid = pair.Key;
                VM newVm = pair.Value;

                VM current;

                //vm does not exist
                if (!_VMList.TryGetValue(uuid, out current))
                {
                    _VMList.Add(uuid, newVm);
                    VmCount++;
                    continue;
                }

                //vm does exist
                current.update(newVm);
                totalUpdate++;
            };

            //vm missing from remote host (deleted etc)
            if(totalUpdate != VmCount)
            {
                //find the vm that was removed from the remote host and remote it from this view of the host
                foreach(KeyValuePair<string, VM> pair in _VMList)
                {
                    VM vm;
                    if(!listOfVms.TryGetValue(pair.Key,out vm))
                    {
                        _VMList.Remove(pair.Key);
                    }
                }
            }


        }

        /// <summary>
        /// Update the host settings using an XML HostSettings object
        /// </summary>
        /// <param name="settings"></param>
        public void setHostSettings(HostSettings settings)
        {
            _AgentListenerIp = settings.AgentListenerIp;
            _AgentListenerPort = int.Parse(settings.AgentListenerPort);
            _LibvirtPort = int.Parse(settings.LibvirtPort);
            _LibvirtIp = settings.LibvirtIp;
            _ConfigLastUpdate = settings.LastUpdate;
            _Id = settings.Id;
            PowerState = HOST_POWER_STATE.Online;
            IsSynced = true;
        }

    }
}
