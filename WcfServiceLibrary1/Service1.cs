using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using viewModel;    // MembersDB, FamiliesDB, EventsDB, RoutineDB, etc.
using Model;        // Events, Members, Families, Routine, etc.

// alias only the Google data types we need
using Calendar = Google.Apis.Calendar.v3.Data.Calendar;
using GEvent = Google.Apis.Calendar.v3.Data.Event;
using EventDateTime = Google.Apis.Calendar.v3.Data.EventDateTime;
using ViewModel;
using System.Diagnostics;
using Google.Apis.Calendar.v3.Data;
// ── resolve “Events” ambiguity ─────────────────────────────
using GCalEvent = Google.Apis.Calendar.v3.Data.Event;
using MEvent = Model.Events;        // alias for your model  (avoids ambiguity)
using System.Linq;
using System.ServiceModel;

namespace WcfServiceLibrary1
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
                 ConcurrencyMode = ConcurrencyMode.Single)]
    public class Service1 : IService1
    {
        private readonly CalendarService _calendarSvc;
        private readonly FamiliesDB _familiesDb = new FamiliesDB();

        // ─── Constructor ───
        public Service1()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string jsonPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            if (string.IsNullOrWhiteSpace(jsonPath))
                jsonPath = Path.Combine(baseDir, "Config", "GCKEY.json");

            if (!File.Exists(jsonPath))
                return; // Local features work without Google Calendar credentials.

            GoogleCredential cred;
            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                cred = GoogleCredential.FromStream(stream)
                         .CreateScoped(CalendarService.Scope.Calendar);
            }

            _calendarSvc = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred,
                ApplicationName = "FamilyManagerService"
            });
        }

        // ─────────────────────────────────────────────────────────────────────────────
        //  ensure each family has its own Google Calendar (public-read so WebView works)
        private string EnsureCalendar(int familyId)
        {
            if (_calendarSvc == null)
                throw new FaultException("Google Calendar is not configured. Set GOOGLE_APPLICATION_CREDENTIALS to your own service-account JSON file.");
            var fam = _familiesDb.SelectByID(familyId)
                      ?? throw new ArgumentException($"Family not found: {familyId}");

            // 1) create calendar when missing
            if (string.IsNullOrEmpty(fam.GoogleCalendarId))
            {
                var created = _calendarSvc.Calendars.Insert(new Calendar
                {
                    Summary = $"Family {familyId} Calendar",
                    TimeZone = "UTC"
                }).Execute();

                fam.GoogleCalendarId = created.Id;
                _familiesDb.Update(fam);
                _familiesDb.SaveChanges();
            }

            string calId = fam.GoogleCalendarId;

            // Calendar access is managed by its owner; do not make family calendars public.
            return calId;
        }

        // ─── Calendar & Events Methods ───

        public EventsList GetFamilyEvents(int familyId, DateTime from, DateTime to)
        {
            string calId = EnsureCalendar(familyId);

            var req = _calendarSvc.Events.List(calId);
            req.TimeMinDateTimeOffset = from;
            req.TimeMaxDateTimeOffset = to;
            req.SingleEvents = true;
            req.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            IList<GCalEvent> items = req.Execute().Items;
            var result = new EventsList();

            foreach (var ge in items)
            {
                result.Add(new MEvent
                {
                    EventName = ge.Summary,
                    EventDescription = ge.Description,
                    EventStartDate = ge.Start.DateTimeDateTimeOffset?.DateTime ?? DateTime.MinValue,
                    EventEndDate = ge.End.DateTimeDateTimeOffset?.DateTime ?? DateTime.MinValue,

                    // ← this is the key line:
                    GoogleEventId = ge.Id
                });
            }
            return result;
        }

        // ─── Creates a new Google event and returns its Google-ID ───
        public string CreateFamilyEvent(int familyId, MEvent ev)
        {
            string calId = EnsureCalendar(familyId);
            var tzOffset = TimeZoneInfo.Local.GetUtcOffset(ev.EventStartDate);

            var ge = new GEvent
            {
                Summary = ev.EventName,
                Description = ev.EventDescription,
                Start = new EventDateTime { DateTimeDateTimeOffset = new DateTimeOffset(ev.EventStartDate, tzOffset) },
                End = new EventDateTime { DateTimeDateTimeOffset = new DateTimeOffset(ev.EventEndDate, tzOffset) }
            };
            ge = _calendarSvc.Events.Insert(ge, calId).Execute();

            ev.GoogleEventId = ge.Id;
            var db = new EventsDB();
            db.Update(ev);
            db.SaveChanges();           // ← add this
            return ge.Id;
        }

        // ─── in Service1.UpdateFamilyEvent ───
        public void UpdateFamilyEvent(int familyId, MEvent ev)
        {
            string calId = EnsureCalendar(familyId);
            var tzOffset = TimeZoneInfo.Local.GetUtcOffset(ev.EventStartDate);

            var ge = _calendarSvc.Events.Get(calId, ev.GoogleEventId).Execute();
            ge.Summary = ev.EventName;
            ge.Description = ev.EventDescription;
            ge.Start = new EventDateTime { DateTimeDateTimeOffset = new DateTimeOffset(ev.EventStartDate, tzOffset) };
            ge.End = new EventDateTime { DateTimeDateTimeOffset = new DateTimeOffset(ev.EventEndDate, tzOffset) };
            _calendarSvc.Events.Update(ge, calId, ge.Id).Execute();

            var db = new EventsDB();
            db.Update(ev);
            db.SaveChanges();           // ← and this
        }

        public void DeleteFamilyEvent(int familyId, string googleEventId)
        {
            string calId = EnsureCalendar(familyId);

            // 1) Only call Google if we actually have an ID
            if (!string.IsNullOrWhiteSpace(googleEventId))
            {
                _calendarSvc.Events.Delete(calId, googleEventId).Execute();
            }

            // 2) Now delete the local record
            var db = new EventsDB();
            var local = db.SelectAll()
                          .FirstOrDefault(x => x.GoogleEventId == googleEventId);

            if (local != null)
            {
                db.Delete(local);
                db.SaveChanges();
            }
        }
        public string EnsureFamilyCalendar(int familyId)
        {
            return EnsureCalendar(familyId);      // your existing helper
        }

        // ─── All the other IService1 methods stay exactly as you already have them ───

        public MemberList SelectAllMembers()
        {
            MembersDB db = new MembersDB();
            return db.SelectAll();
        }
        public MemberList SelectMembersByName(string userName, int familyId)
        {
            MembersDB db = new MembersDB();
            return db.SelectByName(userName, familyId);
        }
        public Members GetMemberDetails(int id)
        {
            MembersDB db = new MembersDB();
            return db.SelectByID(id);
        }
        public int InsertMember(Members member)
        {
            var db = new MembersDB();
            db.Insert(member);
            db.SaveChanges();       // member.Id now contains @@IDENTITY
            return member.Id;       // ← send it back
        }
        public int UpdateMember(Members member)
        {
            MembersDB db = new MembersDB();
            db.Update(member);
            return db.SaveChanges();
        }
        public int DeleteMember(Members member)
        {
            MembersDB db = new MembersDB();
            db.Delete(member);
            return db.SaveChanges();
        }
        public FamiliesList SelectAllFamilies()
        {
            FamiliesDB db = new FamiliesDB();
            return db.SelectAll();
        }
        public Families SelectFamilyByID(int id)
        {
            FamiliesDB db = new FamiliesDB();
            return db.SelectByID(id);
        }
        public CitiesList SelectAllCities()
        {
            CitiesDB db = new CitiesDB();
            return db.SelectAll();
        }
        public int UpdateTask(Tasks t)
        {
            var db = new TasksDB();
            db.Update(t);
            return db.SaveChanges();
        }
        public int UpdateShopping(Shopping s)
        {
            var db = new ShoppingDB();
            db.Update(s);
            return db.SaveChanges();
        }
        public Cities SelectCityById(int id)
        {
            CitiesDB db = new CitiesDB();
            return db.SelectById(id);
        }
        public EventsList SelectAllEvents()
        {
            EventsDB db = new EventsDB();
            return db.SelectAll();
        }
        public PollOptionsList SelectAllPollOptions()
        {
            PollOptionsDB db = new PollOptionsDB();
            return db.SelectAll();
        }
        public PollOptions SelectPollOptionByID(int id)
        {
            PollOptionsDB db = new PollOptionsDB();
            return db.SelectByID(id);
        }
        public PollsList SelectAllPolls()
        {
            PollsDB db = new PollsDB();
            return db.SelectAll();
        }
        public Polls SelectPollByID(int id)
        {
            PollsDB db = new PollsDB();
            return db.SelectByID(id);
        }
        public RoutineList SelectAllRoutines()
        {
            RoutineDB db = new RoutineDB();
            return db.SelectAll();
        }
        public Routine SelectRoutineByID(int id)
        {
            RoutineDB db = new RoutineDB();
            return db.SelectByID(id);
        }
        public ShoppingList SelectAllShopping()
        {
            ShoppingDB db = new ShoppingDB();
            return db.SelectAll();
        }
        public Shopping SelectShoppingByID(int id)
        {
            ShoppingDB db = new ShoppingDB();
            return db.SelectByID(id);
        }
        public ShoppingList SelectShoppingByNotPurchased(bool False)
        {
            ShoppingDB db = new ShoppingDB();
            return db.SelectByNotPurchased();
        }
        public int InsertShopping(Shopping shopping)
        {
            ShoppingDB db = new ShoppingDB();
            db.Insert(shopping);
            return db.SaveChanges();
        }
        public TasksList SelectAllTasks()
        {
            TasksDB db = new TasksDB();
            return db.SelectAll();
        }
        public Tasks SelectTaskByID(int id)
        {
            TasksDB db = new TasksDB();
            return db.SelectByID(id);
        }
        public Members Login(string username, string password)
        {
            System.Diagnostics.Debug.WriteLine($"Pointer size: {IntPtr.Size * 8}-bit");
            // (optional) Debug log to make sure it’s being called:
            // Do not log credentials.

            var db = new MembersDB();
            var member = db.Login(username, password);

            Debug.WriteLine(member == null
                ? "[Service1.Login] FAILED"
                : $"[Service1.Login] OK → {member.Id}");

            return member;
        }
        public int DeleteShopping(Shopping shopping)
        {
            ShoppingDB db = new ShoppingDB();
            db.Delete(shopping);
            return db.SaveChanges();
        }
        public int InsertTask(Tasks task)
        {
            TasksDB db = new TasksDB();
            db.Insert(task);
            return db.SaveChanges();
        }

        public int DeleteTask(Tasks task)
        {
            TasksDB db = new TasksDB();
            db.Delete(task);
            return db.SaveChanges();
        }
        public PollVotes SelectVoteByPollAndMember(int pollId, int memberId)
        {
            PollVotesDB db = new PollVotesDB();
            return db.SelectByPollAndMember(pollId, memberId);
        }

        public PollVotesList SelectVotesByPoll(int pollId)
        {
            PollVotesDB db = new PollVotesDB();
            return db.SelectByPoll(pollId);
        }

        public int InsertPoll(Polls poll)
        {
            PollsDB db = new PollsDB();
            db.Insert(poll);
            return db.SaveChanges();
        }

        public int InsertPollOption(PollOptions option)
        {
            PollOptionsDB db = new PollOptionsDB();
            db.Insert(option);
            return db.SaveChanges();
        }

        public int InsertPollVote(PollVotes vote)
        {
            PollVotesDB db = new PollVotesDB();
            db.Insert(vote);
            return db.SaveChanges();
        }

        public int UpdatePollOption(PollOptions option)
        {
            PollOptionsDB db = new PollOptionsDB();
            db.Update(option);
            return db.SaveChanges();
        }

        public int DeletePoll(Polls poll)
        {
            PollsDB db = new PollsDB();
            db.Delete(poll);
            return db.SaveChanges();
        }

        public int DeletePollOption(PollOptions option)
        {
            PollOptionsDB db = new PollOptionsDB();
            db.Delete(option);
            return db.SaveChanges();
        }

        public int DeletePollVote(PollVotes vote)
        {
            PollVotesDB db = new PollVotesDB();
            db.Delete(vote);
            return db.SaveChanges();
        }
        public int InsertFamily(Families family)
        {
            FamiliesDB db = new FamiliesDB();
            db.Insert(family);
            int result = db.SaveChanges();
            return (result > 0) ? family.Id : -1;  // make sure family.Id is retrieved correctly
        }
        public int UpdateFamily(Families family)
        {
            FamiliesDB db = new FamiliesDB();
            db.Update(family);
            return db.SaveChanges();
        }
    }
}


