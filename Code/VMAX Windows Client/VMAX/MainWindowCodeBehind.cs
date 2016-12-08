using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VMAX.ApplicationSettings;
using VMAX.Controls.Cpu;
using VMAX.Helpers;
using VMAX.Managers;
using XenMaster;
using XenMaster.Models;
using static VMAX.Helpers.VMAXHelper;

namespace VMAX
{
    public partial class MainWindow: Window
    {
        /// <summary>
        /// The currently selected host.
        /// </summary>
        Host _CurrentHost;

        public SystemManager SysManager { get { return _SysManager; } }
        public Settings Settings { get { return _settings; } }

        private SystemManager _SysManager;
        private Settings _settings = Settings.loadSettingsFile(Settings.getFilepath());

        private void start()
        {
            _SysManager = new SystemManager(_settings);

            loadIpInterfaceAddresses(_settings.SystemIp);
            //check ip connection status
            if (_settings.SystemIp == "" || _settings.SystemIp == null)
            {
                notifyUser("Please select an ip interface address to load available hosts","Ip Interface Missing",MessageBoxImage.Warning);
            }
            else
            {
                _SysManager.start();
            }


        }

        private void loadIpInterfaceAddresses(string systemIp)
        {
            // Get host name
            string strHostName = Dns.GetHostName();
            int systemIPIndex = 0;
            bool found = false;

            // Find host by name
            //IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

            List <IPAddress> addresses = new List<IPAddress>();
            // Enumerate IP addresses
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if(ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    //find system ip if it it was given
                    addresses.Add(ipaddress);
                    if (!string.Equals(ipaddress.ToString(), systemIp))
                    {
                        if(!found)   
                            systemIPIndex++;
                    }
                    else
                        found = true;

                }
            }

            combobox_system_ip.ItemsSource = addresses;
            if (!string.Equals(systemIp,"") && systemIp != null)
                if (addresses.Count > 0)
                {
                    combobox_system_ip.SelectedIndex = systemIPIndex;
                    combobox_system_ip.IsEnabled = false;

                    btn_connect.Background = VMAXHelper.getColorBrush(VMAX_COLOR.GRADIENT_BLUE);
                    textblock_btn_connect.Text = "Disconnect";

                }

        }

        private void updateHostDisksList(Host host)
        {
            XenConnect conn = new XenConnect(host, Settings.SystemIdentifier, Settings.SystemIp);
            conn.Notifications += e_logXenMasterNotification;
            List<VirtualDisk> isoImages = conn.getISOStorageList();
            List<VirtualDisk> vmDisks = conn.getVirtualDiskList();

            listview_host_disks.ItemsSource = vmDisks;
            listview_host_images.ItemsSource = isoImages;
        }

        private void updatePerformance(Host host)
        {
            XenConnect conn = new XenConnect(host, Settings.SystemIdentifier, Settings.SystemIp);
            foreach (KeyValuePair<string,VM> kvp in host.VMList)
            {
                VM vm = kvp.Value;
                if (vm.PowerState == vm_state.Shutoff) continue;
                List<VcpuInfo> cpuinfo = null;
                conn.listVmStats(vm.UUID, out cpuinfo);
                if (cpuinfo == null) continue;

                int cpuCount = 0;
                ulong cpuTime = 0;

                foreach(VcpuInfo info in cpuinfo)
                {
                    cpuCount++;
                    cpuTime += info.CpuTime;
                }
               
                listview_performance.Items.Add(new UserControl1(cpuCount,cpuTime,vm.Name));

            }

        }

        private void updateHostOverview(Host host)
        {
            label_overview_host_name.Content = host.Name;
            label_overview_host_vm_count.Content = host.VmCount;
            label_overview_host_power.Content = host.PowerState;
            label_overview_host_id.Content = host.Id;
            label_overview_host_libvirt_ip.Content = host.LibvirtIp;
            label_overview_host_libvirt_port.Content = host.LibvirtPort;
            label_overview_host_agentlistner_ip.Content = host.AgentListenerIp;
            label_overview_host_agentlistener_port.Content = host.AgentListenerPort;
            XenConnect conn = new XenConnect(host, Settings.SystemIdentifier, Settings.SystemIp);
            List<DiskStatistic> stats = null;
            conn.getHostDiskUsage(out stats);
            List<KeyValuePair<string,int>> valueList = new List<KeyValuePair<string, int>>();
            foreach(DiskStatistic stat in stats)
            {
                valueList.Add(new KeyValuePair<string, int>(stat.Filesystem, stat.UsedPercentage));
            }
            pieChart.DataContext= valueList;
            barChart.DataContext = valueList;
        }

        private void remoteView(Host host, VM vm)
        {
            XenConnect conn = new XenConnect(host, Settings.SystemIdentifier, Settings.SystemIp);
            conn.Notifications += e_logXenMasterNotification;
            int vmVncPort;
                conn.getVmVNCPort(vm.UUID, out vmVncPort);

            VMViewerWindow viewerWindow = new VMViewerWindow(host,vm,vmVncPort);
            viewerWindow.Show();

        }

        private void setCurrentHost(Host host)
        {
            _CurrentHost = host;
        }
    }
}
