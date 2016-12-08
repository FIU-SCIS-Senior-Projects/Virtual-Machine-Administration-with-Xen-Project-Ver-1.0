using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using XenMaster.Models;

namespace VMAX
{
    /// <summary>
    /// Interaction logic for VMViewerWindow.xaml
    /// </summary>
    public partial class VMViewerWindow : Window
    {
        Host _host;
        VM _vm;
        SshClient client;
        ForwardedPortLocal port;
        string _HostUsername = "root";
        string _HostPassword = "TCCTCC9";
        string _HostName = "localhost";
        string _LocalName = "localhost";
        int _vncport;

        public VMViewerWindow(Host host,VM vm, int vncport)
        {
            InitializeComponent();
            _host = host;
            _vncport = vncport;
            _vm = vm;
            CreateSecureTunnel();
            startSession();
        }

        private void CreateSecureTunnel()
        {
            textblock_status.Text = "Creating Connection...";
            client = new SshClient(_host.Ip, _HostUsername, _HostPassword);
            client.Connect();
            port = new ForwardedPortLocal(_LocalName, (uint)_vncport, _HostName, (uint)_vncport);
            client.AddForwardedPort(port);
            //port.Exception += portError;
            port.Start();
        }

        private void startSession()
        {
            textblock_status.Text = "Starting Session...";
            string host = "localhost";
            int monitor = _vncport-5900;
            viewer.ConnectSSH(host, monitor, false, true);
            label_title.Content = _vm.Name;
            textblock_status.Text = "Connected";
            border_status.Background = Brushes.Green;

        }

    }
}
