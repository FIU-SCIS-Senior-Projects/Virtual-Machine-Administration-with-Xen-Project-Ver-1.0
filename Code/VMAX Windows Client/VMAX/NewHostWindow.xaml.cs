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
using VMAX.Managers;
using static VMAX.Managers.SystemError;

namespace VMAX
{
    /// <summary>
    /// Interaction logic for NewHostWindow.xaml
    /// </summary>
    public partial class NewHostWindow : Window
    {
        SystemManager _manager;

        public NewHostWindow(SystemManager manager)
        {
            InitializeComponent();
            DataContext = this;
            _manager = manager;
            //generate an id for the user
            textbox_id.Text = getRandomId();
        }


        private string getRandomId()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }

        private void btn_add_host_Click(object sender, RoutedEventArgs e)
        {
           SystemError err = _manager.addHost(textbox_ip.Text, _manager.getSettings().SystemPort, textbox_id.Text, textbox_name.Text, textbox_description.Text);


            if (err.Type == SystemErrorType.Success)
            {
                MessageBox.Show(err.Message, "Add New Host", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();

            }else
            {
                MessageBox.Show(err.Message, "Add New Host", MessageBoxButton.OK, MessageBoxImage.Error);

            }


        }
    }


}
