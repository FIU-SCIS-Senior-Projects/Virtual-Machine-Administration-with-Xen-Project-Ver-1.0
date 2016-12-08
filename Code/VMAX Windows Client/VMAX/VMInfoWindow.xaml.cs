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
using XenMaster.Models;

namespace VMAX
{
    /// <summary>
    /// Interaction logic for VMInfoWindow.xaml
    /// </summary>
    public partial class VMInfoWindow : Window
    {
        VM _vm;
        public VMInfoWindow(VM vm)
        {
            InitializeComponent();
            _vm = vm;
            Title = _vm.Name + " Virtual Machine Information";
            label_memory.Content = string.Format("{0:0.0#}", vm.MaxMemGB);
            label_machine.Content = vm.Id.ToString();
            label_power.Content = vm.PowerState.ToString();
            label_name.Content = vm.Name.ToString();
            label_uuid.Content = vm.UUID.ToString();
            label_vcpus.Content = vm.VCpus.ToString();
            label_current_memory.Content = string.Format("{0:0.0#}", vm.MemoryGB);

            //double cputime_seconds = vm.CpuTime / 1000000000;
           // double cputime_minutes = cputime_seconds / 60;
            //double cputime_hours = cputime_minutes / 60;
           // string cpu_timestamp = string.Format("{0:0.0#}", cputime_hours) + " HRS " + (int)cputime_minutes % 60 + " MINS ";

            label_cpu_time.Content = vm.CpuTimeStamp;

        }
    }
}
