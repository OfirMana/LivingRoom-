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
    /// Interaction logic for FamilyDashboard.xaml
    /// </summary>
    public partial class FamilyDashboard : Page
    {
        public FamilyDashboard()
        {
            InitializeComponent();

            // Example data
            TasksList.ItemsSource = new List<TaskItem>
        {
            new TaskItem { Description = "Take out trash", IsDone = false },
            new TaskItem { Description = "Clean kitchen", IsDone = true }
        };

            // טען את 3 הפריטים הראשונים מרשימת הקניות
            var currentUser = App.Current.Properties["CurrentUser"] as Members;
            if (currentUser != null)
            {
                // קבל את כל פריטי הקניות שלא נרכשו עבור המשפחה
                var shoppingItems = new ShoppingDB().SelectAll()
                                                .Where(s => s.FamilyID.Id == currentUser.Family.Id && !s.Purchased)
                                                .Take(3)  // קח רק את 3 הפריטים הראשונים
                                                .ToList();

                // הגדר את מקור הנתונים של רשימת הקניות
                ShoppingList.ItemsSource = shoppingItems;
            }
            else
            {
                // אם המשתמש לא מחובר, הצג הודעה או השאר ריק
                ShoppingList.ItemsSource = new List<Shopping>();
            }

            LatestPoll.Text = "What movie should we watch this weekend?";
        }

        // Task title clicked
        private void TasksTitle_Click(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService?.Navigate(new TasksPage());
        }

        // Shopping title clicked
        private void ShoppingTitle_Click(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService?.Navigate(new ShoppingPage());
        }

        // Polls title clicked
        private void PollsTitle_Click(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService?.Navigate(new PollsPage());
        }

        // Calendar title clicked
        private void CalendarTitle_Click(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService?.Navigate(new CalendarPage());
        }
    }

    // Supporting models
    public class TaskItem
    {
        public string Description { get; set; }
        public bool IsDone { get; set; }
    }

    public class ShoppingItem
    {
        public string ItemName { get; set; }
        public bool IsBought { get; set; }
    }
}


