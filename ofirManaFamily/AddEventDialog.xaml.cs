using System;
using System.Windows;
using Model;

namespace ofirManaFamily
{
    public partial class AddEventDialog : Window
    {
        public string EventTitle { get; private set; } = "";
        public string EventDescription { get; private set; } = "";
        public DateTime StartDateTime { get; private set; }
        public DateTime EndDateTime { get; private set; }

        public AddEventDialog(Events src = null)
        {
            InitializeComponent();

            if (src != null)          // editing
            {
                TitleBox.Text = src.EventName;
                DescriptionBox.Text = src.EventDescription;
                StartDate.SelectedDate = src.EventStartDate.Date;
                StartTime.Text = src.EventStartDate.ToString("HH:mm");
                EndDate.SelectedDate = src.EventEndDate.Date;
                EndTime.Text = src.EventEndDate.ToString("HH:mm");
            }
            else                      // new
            {
                StartDate.SelectedDate = DateTime.Today;
                EndDate.SelectedDate = DateTime.Today;
            }
        }

        /* time HH:mm → hours / minutes */
        private static bool TryParseTime(string txt, out int h, out int m)
        {
            h = m = 0;
            var parts = txt.Split(':');
            int hh, mm;
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0], out hh)) return false;
            if (!int.TryParse(parts[1], out mm)) return false;
            if (hh < 0 || hh > 23) return false;
            if (mm < 0 || mm > 59) return false;
            h = hh; m = mm;
            return true;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            int sh, sm, eh, em;

            if (StartDate.SelectedDate == null || EndDate.SelectedDate == null ||
                !TryParseTime(StartTime.Text, out sh, out sm) ||
                !TryParseTime(EndTime.Text, out eh, out em))
            {
                MessageBox.Show("Enter valid dates and times (HH:mm).");
                return;
            }

            StartDateTime = new DateTime(
                StartDate.SelectedDate.Value.Year,
                StartDate.SelectedDate.Value.Month,
                StartDate.SelectedDate.Value.Day,
                sh, sm, 0, DateTimeKind.Local);

            EndDateTime = new DateTime(
                EndDate.SelectedDate.Value.Year,
                EndDate.SelectedDate.Value.Month,
                EndDate.SelectedDate.Value.Day,
                eh, em, 0, DateTimeKind.Local);

            if (EndDateTime <= StartDateTime)
            {
                MessageBox.Show("End must be after start.");
                return;
            }

            EventTitle = TitleBox.Text.Trim();
            EventDescription = DescriptionBox.Text.Trim();
            DialogResult = true;
        }
    }
}


