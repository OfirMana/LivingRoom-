using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using Model;
using ofirManaFamily.ServiceReference1;

namespace ofirManaFamily
{
    public partial class CalendarPage : Page
    {
        private readonly Service1Client _client = new Service1Client();
        private readonly Members _user;

        public CalendarPage()
        {
            InitializeComponent();

            _user = App.Current.Properties["CurrentUser"] as Members;
            if (_user?.Family == null)
            {
                MessageBox.Show("User or family not set. Please log in.");
                NavigationService?.Navigate(new Login());
                return;
            }

            CalendarWebView.NavigationCompleted += CalendarWebView_NavigationCompleted;
            try
            {
                EmbedGoogleCalendar();
            }
            catch (System.ServiceModel.FaultException ex)
            {
                IsEnabled = false;
                MessageBox.Show(ex.Message, "Google Calendar", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CalendarWebView_NavigationCompleted(object sender,
                                                         CoreWebView2NavigationCompletedEventArgs e)
        { }

        private void EmbedGoogleCalendar()
        {
            string calId = _client.EnsureFamilyCalendar(_user.Family.Id);
            const string israelTz = "Asia/Jerusalem";

            string url = "https://calendar.google.com/calendar/embed?"
                       + "src=" + Uri.EscapeDataString(calId)
                       + "&mode=MONTH"
                       + "&ctz=" + Uri.EscapeDataString(israelTz)
                       + "&hl=he";

            CalendarWebView.Source = new Uri(url);
        }

        private void ReloadCalendarFrame()
        {
            if (CalendarWebView.CoreWebView2 != null)
                CalendarWebView.CoreWebView2.Reload();
        }

        /*──────── toolbar buttons ────────*/

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddEventDialog { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;

            _client.CreateFamilyEvent(_user.Family.Id, new Events
            {
                EventName = dlg.EventTitle,
                EventDescription = dlg.EventDescription,
                EventStartDate = dlg.StartDateTime,
                EventEndDate = dlg.EndDateTime
            });
            ReloadCalendarFrame();
        }

        private void ManageEvents_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ManageEventsDialog(_user.Family.Id) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true) ReloadCalendarFrame();
        }

        private void SyncDbToGoogle_Click(object s, RoutedEventArgs e) =>
            MessageBox.Show("Not implemented – handled automatically when you create events.");

        private void SyncGoogleToDb_Click(object s, RoutedEventArgs e) =>
            MessageBox.Show("Not implemented – pull via the service if you need it.");

        /*──────── NEW: info ❓ ────────*/
        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            string msg =
                "✨  יומן המשפחה – מה אפשר לעשות?\n\n" +
                "• ‎Add Event‎ – פותח חלון להוספת אירוע חדש (שם, תיאור, תאריכים).\n" +
                "• ‎Manage Events‎ – מציג רשימת אירועים לעריכה או מחיקה.\n" +
                "• כל שינוי מסונכרן אוטומטית אל יומן Google המשפחתי המוטמע בצד ימין.\n" +
                "• ניתן לגרור או ללחוץ על אירועים ישירות ביומן Google לצפייה מהירה.\n\n" +
                "שימוש נעים!";
            MessageBox.Show(msg, "מידע על יומן המשפחה",
                            MessageBoxButton.OK, MessageBoxImage.Information,
                            MessageBoxResult.OK,
                            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }
    }
}




