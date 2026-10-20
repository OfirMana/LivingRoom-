using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ofirManaFamily.ServiceReference1;
using Model;

namespace ofirManaFamily
{
    public partial class RegisterPage : Page
    {
        private readonly Service1Client client = new Service1Client();

        public RegisterPage()
        {
            InitializeComponent();

            var cityList = client.SelectAllCities();
            Create_City.ItemsSource = cityList;
            if (cityList.Any())
                Create_City.SelectedIndex = 0;
        }

        private void CreateFamily_Click(object sender, RoutedEventArgs e)
        {
            if (!(Create_City.SelectedItem is Cities selectedCity))
            {
                MessageBox.Show("אנא בחר עיר", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newFamily = new Families
            {
                FamilyName = Create_FamilyName.Text.Trim(),
                CityID = selectedCity,
                FamPass = Create_Password.Password
            };

            int newFamilyId = client.InsertFamily(newFamily);
            if (newFamilyId > 0)
            {
                var fullFamily = client.SelectFamilyByID(newFamilyId);
                if (fullFamily != null)
                {
                    NavigationService.Navigate(new MemberDetails1(fullFamily.Id));
                }
                else
                {
                    MessageBox.Show("השגיאה: לא ניתן לשלוף את המשפחה שנוצרה", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("שגיאה ביצירת המשפחה", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void JoinFamily_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(Join_FamilyId.Text.Trim(), out var famId))
            {
                MessageBox.Show("הזן מספר משפחה תקין", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var fam = client.SelectFamilyByID(famId);
            if (fam == null || fam.FamPass != Join_Password.Password)
            {
                MessageBox.Show("מספר או סיסמה שגויים", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigationService.Navigate(new MemberDetails1(fam.Id));
        }
    }
}