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
using Microsoft.Win32;
using ofirManaFamily.ServiceReference1;

namespace ofirManaFamily
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetBrowserFeatureControl();
        }

        private void SetBrowserFeatureControl()
        {
            try
            {
                string appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                using (var key = Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"))
                {
                    key.SetValue(appName, 11001, RegistryValueKind.DWord); // Emulates IE11
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Browser emulation setting failed: " + ex.Message);
            }
        }
        private void myFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}
