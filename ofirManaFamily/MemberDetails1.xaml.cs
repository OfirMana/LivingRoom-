using System;
using System.Windows;
using System.Windows.Controls;
using ofirManaFamily.ServiceReference1;
using Model;
using System.Windows.Navigation;

namespace ofirManaFamily
{
    /// <summary>
    /// Interaction logic for MemberDetails1.xaml
    /// </summary>
    public partial class MemberDetails1 : Page
    {
        Members member = new Members();
        Service1Client client = new Service1Client();

        // NEW MEMBER: default birth = today
        public MemberDetails1()
        {
            InitializeComponent();
            member.BirthDate = DateTime.Today;
            this.DataContext = member;
            birthDatePicker.SelectedDate = DateTime.Today;
            birthDatePicker.DisplayDateStart = new DateTime(1900, 1, 1);
            birthDatePicker.DisplayDate = DateTime.Today;
        }

        // EDIT EXISTING: load their real birth date
        public MemberDetails1(Members existingMember) : this()
        {
            member = existingMember;
            this.DataContext = member;
            birthDatePicker.SelectedDate = member.BirthDate;
        }

        // NEW MEMBER IN FAMILY
        public MemberDetails1(int familyId) : this()
        {
            var fam = client.SelectFamilyByID(familyId);
            member.Family = fam;
            this.DataContext = member;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            bool isNew = member.Id == 0;

            /*────────────────────────  הוספת משתמש חדש  ────────────────────────*/
            if (isNew)
            {
                /* 1. ודא שיש Family משובץ */
                if (member.Family == null || member.Family.Id == 0)
                {
                    // נשתמש במשפחה של המשתמש המחובר (אותה אחת שהוסיפה אותנו)
                    Members cur = App.Current.Properties["CurrentUser"] as Members;
                    if (cur == null || cur.Family == null)
                    {
                        MessageBox.Show("לא הוגדרה משפחה עבור המשתמש החדש.");
                        return;
                    }
                    member.Family = cur.Family;   // כעת יש familyID תקין
                }

                /* 2. קריאה לשירות */
                int newId = client.InsertMember(member);
                if (newId <= 0)
                {
                    MessageBox.Show("שגיאה בהוספת משתמש");
                    return;
                }

                /* 3. עדכון מזהה והמשך ניווט */
                member.Id = newId;
                App.Current.Properties["CurrentUser"] = member;
                NavigationService.Navigate(new MainPage());
                return;
            }

            /*────────────────────────  עדכון משתמש קיים  ───────────────────────*/
            int rows = client.UpdateMember(member);
            if (rows <= 0)
            {
                MessageBox.Show("שגיאה בעדכון משתמש");
                return;
            }

            var nav = NavigationService.GetNavigationService(this);
            if (nav != null && nav.CanGoBack)
                nav.GoBack();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
