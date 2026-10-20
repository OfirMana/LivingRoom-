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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.Threading;
using WcfServiceLibrary1;


namespace Host
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceHost serviceHost;
        private Mutex readySignal;

        public MainWindow()
        {
            InitializeComponent();
            serviceHost = new ServiceHost(typeof(WcfServiceLibrary1.Service1));
            serviceHost.Open();
            readySignal = new Mutex(false, @"Local\LivingRoomHostReady");
        }
        protected override void OnClosed(EventArgs e)
        {
            readySignal?.Dispose();
            if (serviceHost != null)
            {
                try { serviceHost.Close(); }
                catch { serviceHost.Abort(); }
            }
            base.OnClosed(e);
        }
    }
}
