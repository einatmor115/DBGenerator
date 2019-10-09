using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
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

namespace WPF_Project_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // public static MainWindow self;   
        private ViewModel vm;
        public MainWindow()
        {
            vm = new ViewModel();
            InitializeComponent();
            DataContext = vm;
           // self = this;
           // DataContext = new ViewModel(this.MyProgressBar, this.MyPercentageTB);
            
        }
    }
}
