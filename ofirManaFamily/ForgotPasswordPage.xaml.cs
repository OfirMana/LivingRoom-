using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ofirManaFamily.ServiceReference1;
using Model;

namespace ofirManaFamily
{
    public partial class ForgotPasswordPage : Page
    {
        private readonly Service1Client client = new Service1Client();

        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        private void ChangeMemberPassword_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(FamIdBox_User.Text.Trim(), out int famId))
            {
                MessageBox.Show("מספר משפחה לא תקין.");
                return;
            }

            var family = client.SelectFamilyByID(famId);
            if (family == null || family.FamPass != FamPassBox_User.Password)
            {
                MessageBox.Show("מספר משפחה או סיסמה שגויים.");
                return;
            }

            string username = UserNameBox_User.Text.Trim();
            string newPass = NewPassBox_User.Password;
            string confirm = ConfirmPassBox_User.Password;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("נא להזין שם משתמש.");
                return;
            }

            if (newPass != confirm)
            {
                MessageBox.Show("הסיסמאות אינן תואמות.");
                return;
            }

            var member = client.SelectMembersByName(username, famId)?.FirstOrDefault();
            if (member == null)
            {
                MessageBox.Show("המשתמש לא נמצא במשפחה זו.");
                return;
            }

            member.Passwordd = newPass;
            int rows = client.UpdateMember(member);

            if (rows > 0)
            {
                MessageBox.Show("סיסמת המשתמש אופסה בהצלחה.");
                NavigationService?.Navigate(new Login());
            }
            else
            {
                MessageBox.Show("אירעה שגיאה בעת שמירת הסיסמה.");
            }
        }

        private void ChangeFamilyPassword_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(FamIdBox_Fam.Text.Trim(), out int famId))
            {
                MessageBox.Show("מספר משפחה לא תקין.");
                return;
            }

            string username = UserNameBox_Fam.Text.Trim();
            string userPass = UserPassBox_Fam.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(userPass))
            {
                MessageBox.Show("נא להזין שם משתמש וסיסמה.");
                return;
            }

            var member = client.Login(username, userPass);
            if (member == null || member.Family == null || member.Family.Id != famId)
            {
                MessageBox.Show("שם משתמש, סיסמה או מספר משפחה שגויים.");
                return;
            }

            string newFamPass = NewPassBox_Fam.Password;
            string confirmFamPass = ConfirmPassBox_Fam.Password;

            if (newFamPass != confirmFamPass)
            {
                MessageBox.Show("הסיסמאות אינן תואמות.");
                return;
            }

            var family = client.SelectFamilyByID(famId);
            if (family == null)
            {
                MessageBox.Show("משפחה לא נמצאה.");
                return;
            }

            family.FamPass = newFamPass;
            int rows = client.UpdateFamily(family);

            if (rows > 0)
            {
                MessageBox.Show("סיסמת המשפחה אופסה בהצלחה.");
                NavigationService?.Navigate(new Login());
            }
            else
            {
                MessageBox.Show("אירעה שגיאה בעת שמירת הסיסמה.");
            }
        }
    }
}
