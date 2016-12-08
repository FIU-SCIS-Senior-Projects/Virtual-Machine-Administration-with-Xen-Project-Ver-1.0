using System.Windows;
using VMAX.ApplicationSettings;

namespace VMAX
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(Settings settings)
        {
            InitializeComponent();
            label_last_update.Content = settings.LastUpdated.ToString();
            label_retry.Content = settings.MaxRetry;
            label_auto_restart.Content = settings.AutoConnectOnStart;
            label_resync_interval.Content = settings.ReSyncInterval;
            label_system_port.Content = settings.SystemPort;
            label_system_id.Content = settings.SystemIdentifier;
            label_system_ip.Content = settings.SystemIp;
            listview_attached_hosts.ItemsSource = null;
            listview_attached_hosts.ItemsSource = settings.Hosts;

            
        }
    }
}
