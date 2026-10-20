using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Model;
using ofirManaFamily.ServiceReference1;

namespace ofirManaFamily
{
    /// <summary>
    /// קוד-מאחורי לעמוד PollsPage – כולל:
    /// • כפתור מידע ❓
    /// • אפשרות מחיקה למנהלים בלבד
    /// </summary>
    public partial class PollsPage : Page
    {
        private readonly Members CurrentMember;
        private readonly Service1Client client = new Service1Client();
        private readonly List<TextBox> optionTextBoxes = new List<TextBox>();

        public PollsPage()
        {
            InitializeComponent();

            CurrentMember = App.Current.Properties["CurrentUser"] as Members;

            if (CurrentMember?.Family == null)
            {
                MessageBox.Show("אין משתמש מחובר. אנא התחבר מחדש.");
                NavigationService?.Navigate(new Login());
                return;
            }

            AddOptionInput();   // שדה אפשרות ראשון
            LoadPolls();        // בנייה ראשונית של הרשימה
        }

        /*────────────────────  יצירת סקר  ────────────────────*/
        private void AddOptionInput(string text = "")
        {
            var tb = new TextBox { Margin = new Thickness(0, 0, 0, 5), Text = text };
            optionTextBoxes.Add(tb);
            PollOptionsInputPanel.Items.Add(tb);
        }

        private void AddOption_Click(object sender, RoutedEventArgs e) => AddOptionInput();

        private void CreatePoll_Click(object sender, RoutedEventArgs e)
        {
            string q = NewPollQuestionBox.Text.Trim();
            string des = NewPollDescriptionBox.Text.Trim();
            var opts = optionTextBoxes.Select(t => t.Text.Trim())
                                        .Where(t => !string.IsNullOrEmpty(t))
                                        .ToList();

            if (string.IsNullOrEmpty(q) || opts.Count < 2)
            {
                MessageBox.Show("יש להזין שאלה ולפחות שתי אפשרויות.");
                return;
            }

            var poll = new Polls
            {
                Question = q,
                Description = des,
                Results = 0,
                FamilyID = CurrentMember.Family
            };

            client.InsertPoll(poll);

            // שליפת ה-ID שנוצר כרגע
            var latest = client.SelectAllPolls()
                               .Where(p => p.Question == q &&
                                           p.Description == des &&
                                           p.FamilyID.Id == poll.FamilyID.Id)
                               .OrderByDescending(p => p.Id)
                               .FirstOrDefault();

            if (latest == null) { MessageBox.Show("שגיאה ביצירת סקר."); return; }

            int num = 1;
            foreach (string txt in opts)
            {
                client.InsertPollOption(new PollOptions
                {
                    PollID = latest.Id,
                    OptionNumber = num++,
                    OptionText = txt,
                    Votes = 0
                });
            }

            // איפוס טופס
            NewPollQuestionBox.Clear();
            NewPollDescriptionBox.Clear();
            optionTextBoxes.Clear();
            PollOptionsInputPanel.Items.Clear();
            AddOptionInput();

            LoadPolls();
        }

        /*────────────────────  בניית רשימה  ────────────────────*/
        private void LoadPolls()
        {
            PollsPanel.Children.Clear();

            var polls = client.SelectAllPolls()
                                .Where(p => p.FamilyID?.Id == CurrentMember.Family.Id);

            var options = client.SelectAllPollOptions();

            foreach (var poll in polls)
            {
                var stack = new StackPanel();
                stack.Children.Add(new TextBlock
                {
                    Text = poll.Question,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold
                });

                if (!string.IsNullOrWhiteSpace(poll.Description))
                    stack.Children.Add(new TextBlock
                    {
                        Text = poll.Description,
                        Foreground = Brushes.Gray,
                        FontSize = 13,
                        Margin = new Thickness(0, 0, 0, 5)
                    });

                /*–– כפתור מחיקה – מוצג רק למנהל –*/
                if (CurrentMember.IsManager)
                {
                    var delBtn = new Button
                    {
                        Content = "Delete Poll",
                        Background = Brushes.IndianRed,
                        Foreground = Brushes.White,
                        Padding = new Thickness(10, 2, 10, 2),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(0, 0, 0, 8),
                        Tag = poll
                    };
                    delBtn.Click += DeletePoll_Click;
                    stack.Children.Add(delBtn);
                }

                /*–– אפשרויות הצבעה –*/
                var pollOpts = options.Where(o => o.PollID == poll.Id).ToList();
                bool voted = client.SelectVoteByPollAndMember(poll.Id, CurrentMember.Id) != null;
                int maxVotes = pollOpts.Any() ? pollOpts.Max(o => o.Votes) : 1;
                int total = pollOpts.Sum(o => o.Votes);
                double barMaxW = 180;

                foreach (var opt in pollOpts)
                {
                    var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };

                    var btn = new Button
                    {
                        Content = opt.OptionText,
                        Tag = new { Option = opt, Poll = poll },
                        IsEnabled = !voted,
                        Background = Brushes.LightBlue,
                        Padding = new Thickness(8, 2, 8, 2),
                        MinWidth = 90,
                        FontSize = 15,
                        Margin = new Thickness(0, 0, 8, 0)
                    };
                    btn.Click += OptionButton_Click;
                    row.Children.Add(btn);

                    row.Children.Add(new Rectangle
                    {
                        Height = 18,
                        Width = opt.Votes * barMaxW / maxVotes,
                        Fill = Brushes.SteelBlue,
                        RadiusX = 3,
                        RadiusY = 3,
                        Margin = new Thickness(0, 0, 6, 0)
                    });

                    row.Children.Add(new TextBlock
                    {
                        Text = $"{opt.Votes} votes",
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 13
                    });

                    stack.Children.Add(row);
                }

                stack.Children.Add(new TextBlock
                {
                    Text = $"Total votes: {total}",
                    Foreground = Brushes.DarkGray,
                    FontSize = 11,
                    Margin = new Thickness(0, 4, 0, 0)
                });

                PollsPanel.Children.Add(new Border
                {
                    Child = stack,
                    Background = Brushes.White,
                    Padding = new Thickness(12),
                    CornerRadius = new CornerRadius(10),
                    Margin = new Thickness(0, 0, 0, 28)
                });
            }
        }

        /*────────────────────  הצבעה  ────────────────────*/
        private void OptionButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic tag = ((Button)sender).Tag;
            var opt = (PollOptions)tag.Option;
            var poll = (Polls)tag.Poll;

            if (client.SelectVoteByPollAndMember(poll.Id, CurrentMember.Id) != null)
            {
                MessageBox.Show("כבר הצבעת בסקר הזה.");
                LoadPolls();
                return;
            }

            client.InsertPollVote(new PollVotes
            {
                PollID = poll.Id,
                OptionID = opt.Id,
                MemberID = CurrentMember.Id
            });

            opt.Votes++;
            client.UpdatePollOption(opt);

            LoadPolls();
        }

        /*────────────────────  מחיקת סקר  ────────────────────*/
        private void DeletePoll_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentMember.IsManager)
            {
                MessageBox.Show("רק מנהל יכול למחוק סקרים.");
                return;
            }

            var poll = ((Button)sender).Tag as Polls;
            if (poll == null) return;

            if (MessageBox.Show("למחוק את הסקר?", "Delete Poll",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            foreach (var vote in client.SelectVotesByPoll(poll.Id))
                client.DeletePollVote(vote);

            foreach (var opt in client.SelectAllPollOptions().Where(o => o.PollID == poll.Id))
                client.DeletePollOption(opt);

            client.DeletePoll(poll);
            LoadPolls();
        }

        /*────────────────────  כפתור מידע ❓  ────────────────────*/
        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            string msg =
                "✨ עמוד הסקרים – איך משתמשים?\n\n" +
                "1. לחצו על \"Create a New Poll\" כדי לפתוח סקר חדש.\n" +
                "   • כתבו את שאלת הסקר (תיאור – רשות).\n" +
                "   • הוסיפו לפחות שתי אפשרויות תשובה.\n" +
                "2. סיימתם? לחצו \"Create Poll\" והסקר יתפרסם.\n" +
                "3. להצבעה – לחצו על האפשרות הרצויה (פעם אחת למשתמש).\n" +
                "4. מחיקת סקר זמינה למנהל בלבד דרך \"Delete Poll\".\n" +
                "5. מספר הקולות וגרף העמודות מתעדכנים בזמן אמת.\n\n" +
                "הצבעה נעימה!";
            MessageBox.Show(msg,
                "מידע על עמוד הסקרים",
                MessageBoxButton.OK,
                MessageBoxImage.Information,
                MessageBoxResult.OK,
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

        }
    }
}
