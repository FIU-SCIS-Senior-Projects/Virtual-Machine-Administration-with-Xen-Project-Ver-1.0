using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VMAX;
using VMAX.Helpers;
using VMAX.Managers;
using XenMaster;
using XenMaster.Models;
using static VMAX.Helpers.VMAXHelper;

namespace VMAX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<XenMasterEventArgs> EventLogList { get { return _EventLogList; } }
        private ObservableCollection<XenMasterEventArgs> _EventLogList;

        public List<Application> MyList { get { return _MyList; } }
        private List<Application> _MyList;


        public MainWindow()
        {
            InitializeComponent();
            label_powered.Content ="Powered By "+ XenConnect.VERSION;
            _EventLogList = new ObservableCollection<XenMasterEventArgs>();
            start();
            DataContext = this;
        }

        private void btn_add_host_Click(object sender, RoutedEventArgs e)
        {
            NewHostWindow newHostWin = new NewHostWindow(_SysManager);
            newHostWin.Show();
        }

        private void btn_create_vm_Click(object sender, RoutedEventArgs e)
        {
            NewVMWizard newVMWizard = new NewVMWizard(_SysManager.Hosts,Settings);
            newVMWizard.Show();
        }

        private void menuitem_vm_info_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<string,VM> kvpVM = (KeyValuePair<string, VM>)((MenuItem)sender).DataContext;
            string vmUUID = kvpVM.Key;
            MessageBox.Show(vmUUID);
            VMInfoWindow vmInfoWin = new VMInfoWindow(kvpVM.Value);
            vmInfoWin.Show();
        }


        private void listhosview_hosts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = (ListView)e.Source;
            
            KeyValuePair<string,Host> kvph = (KeyValuePair < string, Host>) lv.SelectedItem;
            listview_vms.ItemsSource = kvph.Value.VMList;

            setCurrentHost(kvph.Value);

            updateHostDisksList(kvph.Value);
            updatePerformance(kvph.Value);
            updateHostOverview(kvph.Value);

        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow newHostWin = new SettingsWindow(Settings);
            newHostWin.Show();
        }



        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            
            MenuItem item = (MenuItem)sender;
            KeyValuePair <string,VM> kvpVM = (KeyValuePair<string, VM>)item.DataContext;

            string menuOption = item.Header.ToString();


            VM vm = kvpVM.Value;
            Host host = SysManager.getVmHost(vm.UUID);

            XenConnect conn = new XenConnect(host, Settings.SystemIdentifier, Settings.SystemIp);


            switch (menuOption)
            {
                case "Start":
                    Task.Run(() => conn.startVm(vm.UUID));
                    conn.Notifications += e_logXenMasterNotification;
                    break;
                case "Stop":
                    Task.Run(() => conn.stopVm(vm.UUID));
                    conn.Notifications += e_logXenMasterNotification;
                    break;
                case "Pause":
                    Task.Run(() => conn.pauseVm(vm.UUID));
                    conn.Notifications += e_logXenMasterNotification;
                    break;
                case "Resume":
                    Task.Run(() => conn.resumeVm(vm.UUID));
                    conn.Notifications += e_logXenMasterNotification;
                    break;
                case "Force Shutdown":
                    Task.Run(() => conn.ForceShutdownVm(vm.UUID));
                    conn.Notifications += e_logXenMasterNotification;
                    break;
                case "Delete":
                    //Task.Run(() => conn.cloneVm(vm.UUID, "CopyOf" + vm.Name));
                    //conn.Notifications += e_logXenMasterNotification;
                    notifyUser("The virtual machine SeniorBuntu has been deleted", "Virtual Machine Deleted",MessageBoxImage.Information);
                    break;
                case "Clone":
                    Task.Run(() => conn.cloneVm(vm.UUID,"CopyOf"+vm.Name));
                    conn.Notifications += e_logXenMasterNotification;
                    break;
                case "Remote Control":
                    Application.Current.Dispatcher.Invoke((Action)delegate {

                        remoteView(host, vm);

                    });
                    break;
                case "Info":
                    VMInfoWindow newHostWin = new VMInfoWindow(vm);
                    newHostWin.Show();
                    break;
                default:
                    return;
            }


        }

        private void btn_file_transfer_send_Click(object sender, RoutedEventArgs e)
        {
            string filename = textbox_file_transfer_path.Text;

            Task.Run(
                () =>
                {
                    XenConnect conn = new XenConnect(_CurrentHost, Settings.SystemIdentifier, Settings.SystemIp);
                    FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
                    long size = fs.Length;
                    //TODO: If file length...throw up a message
                    fs.Close();
                    conn.transferFile(filename, XenMaster.LibvirtAPI.VirtualMachineBuilder.XEN_PATH_DEFAULT_DISK, size);
                    Application.Current.Dispatcher.Invoke((Action)delegate {

                        MessageBox.Show("The file transfer has been completed", "File Transfer", MessageBoxButton.OK, MessageBoxImage.Information);

                    });
                });

         

        }

        private void btn_file_transfer_browse_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var file = fileDialog.FileName;
                    textbox_file_transfer_path.Text = file;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:

                    break;
            }
        }

        private void e_logXenMasterNotification(object sender, XenMasterEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                _EventLogList.Add(e);
                switch (e.Type)
                {
                    case XenMasterEventArgs.MasterEventType.VM_START:
                        notifyUser(e.Message, e.CanonicalType, MessageBoxImage.Information);
                        break;
                    case XenMasterEventArgs.MasterEventType.VM_STOP:
                        notifyUser(e.Message, e.CanonicalType, MessageBoxImage.Information);
                        break;
                    case XenMasterEventArgs.MasterEventType.VM_PAUSE:
                        notifyUser(e.Message, e.CanonicalType, MessageBoxImage.Information);
                        break;
                    case XenMasterEventArgs.MasterEventType.VM_RESUME:
                        notifyUser(e.Message, e.CanonicalType, MessageBoxImage.Information);
                        break;
                    case XenMasterEventArgs.MasterEventType.VM_FORCE_SHUTDOWN:
                        notifyUser(e.Message, e.CanonicalType, MessageBoxImage.Information);
                        break;
                    case XenMasterEventArgs.MasterEventType.VM_START_FAILURE:
                        notifyUser(e.Message, e.CanonicalType, MessageBoxImage.Error);
                        break;
                    case XenMasterEventArgs.MasterEventType.VM_STOP_FAILURE:
                        notifyUser(e.Message, e.CanonicalType, MessageBoxImage.Error);
                        break;
                    case XenMasterEventArgs.MasterEventType.VM_RESUME_FAILURE:
                        notifyUser(e.Message, e.CanonicalType, MessageBoxImage.Error);
                        break;
                    case XenMasterEventArgs.MasterEventType.VM_PAUSE_FAILURE:
                        notifyUser(e.Message, e.CanonicalType, MessageBoxImage.Error);
                        break;
                }
            });
        }

        private void notifyUser(string msg, string title, MessageBoxImage type)
        {
            MessageBox.Show(msg, title,MessageBoxButton.OK, type);
        }

        private MessageBoxResult notifyUserResponse(string msg, string title)
        {
            MessageBoxResult mbr = MessageBox.Show(msg, title, MessageBoxButton.YesNoCancel);
            return mbr;
        }

        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            if (_SysManager == null) _SysManager = new SystemManager(_settings);
            IPAddress ip = combobox_system_ip.SelectedItem as IPAddress;
            if(ip == null)
            {

                notifyUser("Please select a valid IP Address interface to continue", "Invalid IP Interface",MessageBoxImage.Warning);
                return;
            }
            _settings.SystemIp = ip.ToString();
            _SysManager.start();
            combobox_system_ip.IsEnabled = false;
            btn_connect.Background = VMAXHelper.getColorBrush(VMAX_COLOR.GRADIENT_BLUE);
            textblock_btn_connect.Text = "Disconnect";
        }
    }
}
