using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Model;
using ofirManaFamily.ServiceReference1;
using Google;  // for GoogleApiException

namespace ofirManaFamily
{
    public partial class ManageEventsDialog : Window
    {
        private readonly Service1Client _svc = new Service1Client();
        private readonly int _familyId;
        public ObservableCollection<Events> Events { get; } = new ObservableCollection<Events>();

        public ManageEventsDialog(int familyId)
        {
            InitializeComponent();
            DataContext = this;
            _familyId = familyId;
            LoadEvents();
        }

        private void LoadEvents()
        {
            Events.Clear();
            DateTime from = DateTime.Today.AddYears(-1);
            DateTime to = DateTime.Today.AddYears(1);

            foreach (var ev in _svc.GetFamilyEvents(_familyId, from, to))
            {
                ev.EventStartDate = DateTime.SpecifyKind(ev.EventStartDate, DateTimeKind.Utc)
                                            .ToLocalTime();
                ev.EventEndDate = DateTime.SpecifyKind(ev.EventEndDate, DateTimeKind.Utc)
                                            .ToLocalTime();
                Events.Add(ev);
            }
        }

        private void EditEvent_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var ev = btn?.Tag as Events;
            if (ev == null) return;

            var hadId = !string.IsNullOrWhiteSpace(ev.GoogleEventId);
            var dlg = new AddEventDialog(ev) { Owner = this };
            if (dlg.ShowDialog() != true) return;

            ev.EventName = dlg.EventTitle;
            ev.EventDescription = dlg.EventDescription;
            ev.EventStartDate = dlg.StartDateTime;
            ev.EventEndDate = dlg.EndDateTime;

            try
            {
                if (hadId)
                    _svc.UpdateFamilyEvent(_familyId, ev);
                else
                    _svc.CreateFamilyEvent(_familyId, ev);
            }
            catch (GoogleApiException ex) when (ex.Message.Contains("eventId"))
            {
                _svc.CreateFamilyEvent(_familyId, ev);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving event:\n{ex.Message}",
                                "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            LoadEvents();
        }

        private void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var ev = btn?.Tag as Events;
            if (ev == null) return;

            if (MessageBox.Show(
                    $"Really delete '{ev.EventName}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning)
                != MessageBoxResult.Yes)
                return;

            try
            {
                _svc.DeleteFamilyEvent(_familyId, ev.GoogleEventId);
            }
            catch (GoogleApiException ex)
            {
                MessageBox.Show($"Calendar API error:\n{ex.Message}",
                                "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting event:\n{ex.Message}",
                                "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            LoadEvents();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
            => LoadEvents();

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            // Signal success so CalendarPage will reload its WebView
            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}