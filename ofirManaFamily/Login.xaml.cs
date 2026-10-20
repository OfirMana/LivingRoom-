using System;
using System.Windows;
using System.Windows.Controls;
using ofirManaFamily.ServiceReference1;

namespace ofirManaFamily
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = textBox.Text;
            string password = showPasswordCheckBox.IsChecked == true
                ? visiblePasswordBox.Text
                : passwordBox.Password;

            var client = new Service1Client();
            var member = client.Login(username, password);

            if (member != null)
            {
                // Get full details with Family included
                var fullMember = client.GetMemberDetails(member.Id);
                client.Close();

                if (fullMember?.Family != null && fullMember.Family.Id > 0)
                {
                    App.Current.Properties["CurrentUser"] = fullMember;
                    NavigationService?.Navigate(new MainPage());
                }
                else
                {
                    textBlock.Text = "המשתמש לא משויך למשפחה.";
                    textBlock.Visibility = Visibility.Visible;
                }
            }
            else
            {
                client.Close();
                textBlock.Text = "שם משתמש או סיסמה שגויים";
                textBlock.Visibility = Visibility.Visible;
            }
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            visiblePasswordBox.Text = passwordBox.Password;
            passwordBox.Visibility = Visibility.Collapsed;
            visiblePasswordBox.Visibility = Visibility.Visible;
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordBox.Password = visiblePasswordBox.Text;
            passwordBox.Visibility = Visibility.Visible;
            visiblePasswordBox.Visibility = Visibility.Collapsed;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ForgotPasswordPage());
        }
    }
}