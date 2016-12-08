using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VMAX.ApplicationSettings;
using VMAX.Helpers;
using XenMaster;
using XenMaster.Helpers;
using XenMaster.LibvirtAPI;
using XenMaster.Models;

namespace VMAX
{
    /// <summary>
    /// Interaction logic for NewVMWizard.xaml
    /// </summary>
    public partial class NewVMWizard : Window
    {
        Host _selectedHost;
        HostList _hostList;
        Settings _settings;

        public NewVMWizard(HostList hostList, Settings settings)
        {
            InitializeComponent();
            _hostList = hostList;
            _settings = settings;
            DataContext = this;
            addHostsToComboBox();
        }

        private void addHostsToComboBox()
        {
            foreach (KeyValuePair<string, Host> entry in _hostList)
            {
                Host host = entry.Value;
                combobox_host.Items.Add(entry.Value.Name);

            }
        }

        private Host getHostByName(string name)
        {
            foreach (KeyValuePair<string, Host> entry in _hostList)
            {
                Host host = entry.Value;
                if (host.Name.Equals(name))
                    return host;

            }

            return null;
        }

        private void ComboBox_Host_DropDownClosed(object sender, EventArgs e)
        {
            //MessageBox.Show(combobox_host.Text);
            _selectedHost = getHostByName(combobox_host.Text);
            XenConnect conn = new XenConnect(_selectedHost, _settings.SystemIdentifier,_settings.SystemIp);
            List<VirtualDisk> isoImages = conn.getISOStorageList();

            combobox_os_image.Items.Clear();

            foreach(VirtualDisk disk in isoImages)
            {
                combobox_os_image.Items.Add(disk.NameCanonical);
            }

        }

        private void btn_create_vm_Click(object sender, RoutedEventArgs e)
        {
            XenConnect conn = new XenConnect(_selectedHost, _settings.SystemIdentifier, _settings.SystemIp);

            string iso = (string)combobox_os_image.SelectedItem;
            string vmName = textbox_vm_name.Text;
            int memory = int.Parse(textbox_vm_memory.Text.Replace(",",""));
            int vcpus = int.Parse(textbox_vm_vcpu.Text.Replace(",", ""));
            int diskMem = int.Parse(textbox_vm_hd.Text.Replace(",", ""));

            VirtualMachineBuilder builder = null;


            if (!(bool)checkbox_existing.IsChecked)
            {
                VirtualMachineBuilder.DomainType domType = VirtualMachineBuilder.DomainType.TRANSIENT;
                builder = new VirtualMachineBuilder(vmName, memory * 1000, vcpus, VirtualMachineBuilder.XEN_PATH_DEFAULT_ISO + iso + ".iso", domType);
                builder.setHardDrive(diskMem * 1000, true); //set hard drive size to n GB (nk MB)
            }else
            {

                List<VirtualDisk> disks = conn.getISOStorageList();

                string filename = null;

                //hard coded for fluff
                foreach (VirtualDisk disk in disks)
                {
                    if (disk.NameCanonical.Contains("Ubuntu14"))
                    {
                        filename = disk.Name;
                        break;
                    }
                }

                VirtualMachineBuilder.DomainType domType = VirtualMachineBuilder.DomainType.TRANSIENT;
                builder = new VirtualMachineBuilder(vmName, memory * 1000, vcpus, VirtualMachineBuilder.XEN_PATH_DEFAULT_ISO + filename, domType);
                builder.setHardDriveExisting(true, false, VirtualMachineBuilder.XEN_PATH_DEFAULT_DISK + ((string)combobox_os_image.SelectedItem)+ ".img");
                conn.createVm(builder);
            }

            XenError status = conn.createVm(builder);

            MessageBoxResult mbr;

            if (status.Type == XenError.XenErrorType.SUCCESS)
            {
                 mbr = MessageBox.Show("Succesessfully created virtual machine "+vmName,"Create Virtual Machine", MessageBoxButton.OK, MessageBoxImage.Information);
            }else
            {
                 mbr = MessageBox.Show(status.Message, "Create Virtual Machine", MessageBoxButton.OK, MessageBoxImage.Information);

            }

            if (mbr.CompareTo(MessageBoxResult.OK) == 0)
            {
                if(status.Type == XenError.XenErrorType.SUCCESS)
                {
                    Close();
                }
            }
        }
        
        private void checkbox_existing_Checked(object sender, RoutedEventArgs e)
        {
            XenConnect conn = new XenConnect(_selectedHost, _settings.SystemIdentifier, _settings.SystemIp);
            List<VirtualDisk> isoImages = conn.getVirtualDiskList();
            combobox_os_image.Items.Clear();
            slider_hd.IsEnabled = false;
            textbox_vm_hd.IsEnabled = false;
            textblock_slider_value_display.Opacity = 0;


            foreach (VirtualDisk disk in isoImages)
            {
                combobox_os_image.Items.Add(disk.NameCanonical);
            }

        }

        private void checkbox_existing_Unchecked(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(combobox_host.Text);
            _selectedHost = getHostByName(combobox_host.Text);
            XenConnect conn = new XenConnect(_selectedHost, _settings.SystemIdentifier, _settings.SystemIp);
            List<VirtualDisk> isoImages = conn.getISOStorageList();

            combobox_os_image.Items.Clear();

            foreach (VirtualDisk disk in isoImages)
            {
                combobox_os_image.Items.Add(disk.NameCanonical);
            }

            slider_hd.IsEnabled = true;
            textbox_vm_hd.IsEnabled = true;
            textblock_slider_value_display.Opacity = 100;

        }
    }
}
