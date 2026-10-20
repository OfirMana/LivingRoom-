using Model;
using ofirManaFamily.ServiceReference1;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ofirManaFamily
{
    public partial class HomePage : Page
    {
        private Members currentUser;

        public HomePage()
        {
            InitializeComponent();
            currentUser = App.Current.Properties["CurrentUser"] as Members;

            if (currentUser != null)
            {
                greetingText.Text = $"Welcome Home, {currentUser.MemberName}!";
                familyNameText.Text = $"שם משפחה: {currentUser.Family?.FamilyName ?? "לא ידוע"}";
                familyIdText.Text = $"מזהה משפחה: {currentUser.Family?.Id.ToString() ?? "?"}";
            }
        }

        private void Polls_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PollsPage());
        }

        private void Tasks_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TasksPage());
        }

        private void Shopping_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ShoppingPage());
        }

        private void Events_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CalendarPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Properties["CurrentUser"] = null;
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.myFrame.Navigate(new Login());
        }
    }
}
