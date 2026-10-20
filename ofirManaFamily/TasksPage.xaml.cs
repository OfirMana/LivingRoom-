using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Model;
using ofirManaFamily.ServiceReference1;

namespace ofirManaFamily
{
    public partial class TasksPage : Page
    {
        /*──────────  fields  ─────────*/
        private readonly Members currentUser;
        private readonly bool isManager;
        private readonly Service1Client client;
        private readonly List<Tasks> taskItems;

        /*──────────  ctor  ─────────*/
        public TasksPage()
        {
            InitializeComponent();
            DataContext = this;   // מאפשר Binding ל-IsManager ב-XAML

            currentUser = App.Current.Properties["CurrentUser"] as Members
                          ?? throw new InvalidOperationException("No user.");
            isManager = currentUser.IsManager;

            client = new Service1Client();
            taskItems = new List<Tasks>();

            LoadTasks();

            // Storyboard fade-out בעת סגירת העמוד
            Unloaded += TasksPage_Unloaded;
        }

        /* חשיפת הדגל ל-XAML */
        public bool IsManager { get { return isManager; } }

        /*──────────  load  ─────────*/
        private void LoadTasks()
        {
            taskItems.Clear();

            List<Tasks> all = client.SelectAllTasks();
            foreach (Tasks t in all)
            {
                if (t.FamilyID == currentUser.Family.Id)
                    taskItems.Add(t);
            }

            taskItems.Sort((a, b) => a.TaskDeadline.CompareTo(b.TaskDeadline));

            TasksItemsControl.ItemsSource = null;
            TasksItemsControl.ItemsSource = taskItems;
        }

        /*──────────  add  ─────────*/
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskNameTextBox.Text))
            {
                MessageBox.Show("יש להזין שם משימה.");
                return;
            }

            Tasks newTask = new Tasks
            {
                TaskName = TaskNameTextBox.Text.Trim(),
                TaskDeadline = TaskDeadlinePicker.SelectedDate ?? DateTime.Now,
                TaskInstructions = InstructionsTextBox.Text.Trim(),
                FamilyID = currentUser.Family.Id,
                Done = false
            };

            client.InsertTask(newTask);

            TaskNameTextBox.Clear();
            InstructionsTextBox.Clear();
            TaskDeadlinePicker.SelectedDate = null;
            LoadTasks();
        }

        /*──────────  toggle done ✔  ─────────*/
        private void CompleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            Tasks t = btn.Tag as Tasks;
            if (t == null) return;

            t.Done = !t.Done;
            client.UpdateTask(t);         // ודא שהמתודה קיימת בשירות
            LoadTasks();
        }

        /*──────────  delete 🗑  ─────────*/
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!isManager)
            {
                MessageBox.Show("רק מנהל יכול למחוק משימות.",
                                "אין הרשאה", MessageBoxButton.OK,
                                MessageBoxImage.Information,
                                MessageBoxResult.OK,
                                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            Button btn = sender as Button;
            if (btn == null) return;

            Tasks t = btn.Tag as Tasks;
            if (t == null) return;

            MessageBoxResult res =
                MessageBox.Show("למחוק משימה זו?", "Delete Task",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning,
                                MessageBoxResult.No,
                                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

            if (res != MessageBoxResult.Yes) return;

            client.DeleteTask(t);
            LoadTasks();
        }

        /*──────────  info ❓  ─────────*/
        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            string msg =
                "✨  עמוד המשימות – איך משתמשים?\n\n" +
                "• הוספת משימה: מלאו פרטים ולחצו ‎Add‎.\n" +
                "• ✔ מסמן/מבטל ביצוע (קו חוצה בטקסט).\n" +
                "• 🗑 מחיקת משימה – רק למנהל.\n" +
                "• הרשימה ממוינת לפי דד־ליין.\n\n" +
                "עבודה נעימה!";
            MessageBox.Show(msg, "מידע על עמוד המשימות",
                            MessageBoxButton.OK, MessageBoxImage.Information,
                            MessageBoxResult.OK,
                            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }

        /*──────────  fade on unload  ─────────*/
        private void TasksPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["FadeOutStoryboard"] as Storyboard;
            if (sb != null) sb.Begin(this);
        }
    }
}
