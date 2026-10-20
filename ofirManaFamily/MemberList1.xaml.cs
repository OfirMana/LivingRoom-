using Model;
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
using ViewModel;

namespace ofirManaFamily
{
    /// <summary>
    /// Interaction logic for MemberList1.xaml
    /// </summary>


    public enum Mode { None, Update, Insert };

    public partial class MemberList1 : Page
    {

        private Members member; // "עצם "בטיפול
        private MemberList list;
        MembersDB db;

        private Mode mode;

        public MemberList1()
        {
            InitializeComponent();
            db = new MembersDB();
            list = db.SelectAll();
            this.lstView.ItemsSource = list;
            mode = Mode.None;
        }

        private void lstView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            member = this.lstView.SelectedItem as Members;   // קבלה של העצם שנבחר מהרשימה

        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            member = new Members();
            mode = Mode.Insert;
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new MemberDetails1(member));  //נשלח את פעולת ההמשך כפרמטר לבנאי
        }


        private void MenuItem_Upd(object sender, RoutedEventArgs e)
        {
            mode = Mode.Update;

            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new MemberDetails1(member));   // נשלח את המשתמש שנבחר כפרמטר לבנאי של הדף
        }

        private void MenuItem_Del(object sender, RoutedEventArgs e)
        {
            db.Delete(member);
            int x = db.SaveChanges();
            if (x > 0)  // OK
            {
                this.list.Remove(member);
                RefreshUserList();
            }

            else
            {


            }
            //this.lstView.ItemsSource = null;  // force refresh
            //this.lstView.ItemsSource = lst;


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigating += NavigationService_Navigating;

        }

        private void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (mode == Mode.Insert)  //insert Mode
                {
                    this.list.Add(member);
                }
                if (mode == Mode.Update)
                {
                    // .....   לא צריך לעשות כלום, כרגע, כי זה אותו העצם
                }


                //this.lstView.ItemsSource = null;  // force refresh
                //this.lstView.ItemsSource = list;
                RefreshUserList();

                mode = Mode.None;
            }


        }

        private void RefreshUserList()
        {
            //this.lstView.Items.Refresh();
            this.lstView.ItemsSource = null;  // force refresh
            this.lstView.ItemsSource = list;
        }


    }
}
