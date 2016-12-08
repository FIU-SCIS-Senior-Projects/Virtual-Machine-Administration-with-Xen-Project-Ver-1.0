using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using XenMaster.Models;

namespace VMAX.Controls.Cpu
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        int _cpus = 0;
        ulong _cputime = 0;
        string _vm;

        //Height/16 (max cpus at this time)
        public static readonly int BAR_INCREMENTS = 18;


        public UserControl1(int cpus, ulong cputime, string vm)
        {
            InitializeComponent();
            _cpus = cpus;
            _cputime = cputime;
            _vm = vm;
            buildGraph();

        }

        public void buildGraph()
        {
            textblock_vm_name.Text = _vm;
            bar_left_cpu_count.Margin = new Thickness(0, 290 -(BAR_INCREMENTS * _cpus), 0, 0);
            bar_right_cpu_time.Margin = new Thickness(0, 290 - (_cputime%290), 0, 0);
        }

     
    }
}
