using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using VMAX.ApplicationSettings;
using VMAX.Helpers;
using XenMaster;
using XenMaster.Helpers;
using XenMaster.Models;

namespace VMAX.Managers
{
   
    public class SystemManager
    {
        public int MS_PER_SECOND = 1000;

        public static int NODECOUNT = 0;

        public HostList Hosts { get { return _hostList; } }

        private HostList _hostList = new HostList();
        private Settings _settings;

        //Timer for vm synchronization
        System.Timers.Timer vmUpdateTimer = new System.Timers.Timer();
        System.Timers.Timer vmCpuUpdateTimer = new System.Timers.Timer();

        public SystemManager(Settings settings)
        {
            _settings = settings;

        }

        public void start()
        {
            //addHost("172.16.10.115",29171,"ID-GIBBERISH","TEST-TCC-9-CHANGE ME","OMG ITS TCC9");//TODO: Remvoe this test

            foreach(Node node in _settings.Hosts)
            {
                loadHosts(node.Ip, node.Port, node.Id, node.Name, node.Description);
            }

            vmUpdateTimer.Elapsed += new ElapsedEventHandler(VmUpdateTimedEvent);
            vmUpdateTimer.Interval = _settings.ReSyncInterval*MS_PER_SECOND;
            vmUpdateTimer.Enabled = true;

            vmCpuUpdateTimer.Elapsed += new ElapsedEventHandler(VmUpdateInfoTimedEvent);
            vmCpuUpdateTimer.Interval = (_settings.ReSyncInterval * MS_PER_SECOND);
            vmCpuUpdateTimer.Enabled = true;

            VmUpdateTimedEvent(null, null);


            //_settings.save();

        }

        public void loadHosts(string ip, int port, string id, string name, string description)
        {
            Host host = new Host(ip, port, id, name, description);
            host.HostNumber = ++NODECOUNT;
            _hostList.Add(id, host);
        }

        public SystemError addHost(string ip, int port, string id, string name, string description)
        {
            //check if the host exsits
            SystemError err = new SystemError("Successfully added new host "+id+" name "+name, SystemError.SystemErrorType.Success);

            Host host = null;

            if(_hostList.TryGetValue(id,out host))
            {
                err = new SystemError("Unable to add new host, a host with Id: " + id + " already exists", SystemError.SystemErrorType.Add_New_Host_Failure);
                return err;
            }

            host = new Host(ip, port);
            host.Id = id;
            host.Name = name;
            host.Description = description;
            host.HostNumber = ++NODECOUNT;
            _hostList.Add(id, host);
            _settings.addNewNode(ip, port, id, name, description);
            _settings.save();

            updateHostsVMList();

            return err;

        }
        private void VmUpdateTimedEvent(object source, ElapsedEventArgs e)
        {
            updateHostsVMList();
        }

        private void VmUpdateInfoTimedEvent(object source, ElapsedEventArgs e)
        {
            //updateVMCpuInfo();
        }

        public void updateVMCpuInfo()
        {
            foreach (KeyValuePair<string, Host> entry in _hostList)
            {
                Host host = entry.Value;
                foreach (KeyValuePair<string, VM> vmentry in host.VMList)
                {
                    VM vm = vmentry.Value;
                    if (!(vm.PowerState == vm_state.Running)) continue;
                    XenConnect connect = new XenConnect(host, "VMAX", "172.16.10.76"); //TODO: Unhardcode me please!
                    List<VcpuInfo> info = null;
                    connect.listVmStats(vm.UUID, out info);
                    if (info == null) continue;
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {

                        vm.updateVcpuStats(info);


                    });
                            Thread.Sleep(1000);

                }
            }
        }

        public Settings getSettings()
        {
            return _settings;
        }

        public void updateHostsVMList()
        {
            foreach(KeyValuePair<string,Host> entry in _hostList)
            {
                Host host = entry.Value;
                XenConnect connect = new XenConnect(host, "VMAX", "172.16.10.76"); //TODO: Unhardcode me please!
                VirtualMachineList vms = connect.getHostVirtualMachineList();
                host.setVMList(vms);
            }
        }

        public Host getVmHost(string uuid)
        {
            foreach (KeyValuePair<string, Host> entry in _hostList)
            {
                Host host = entry.Value;
                VM vm;
                if(host.VMList.TryGetValue(uuid, out vm))
                {
                    return host;
                }

            }
            return null;
        }
    }
}
