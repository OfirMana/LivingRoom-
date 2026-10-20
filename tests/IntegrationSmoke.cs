using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Data.OleDb;
using Model;
using ViewModel;
using WcfServiceLibrary1;

// Runs only against a disposable copy of the sanitized demo database.
internal static class IntegrationSmoke
{
    static int checks;
    private sealed class FailingMembersDB : MembersDB
    {
        protected override string CreateInsertSQL(BaseEntity entity)
        {
            throw new InvalidOperationException("Intentional second-stage failure for rollback test.");
        }
    }
    static void Check(bool ok, string label)
    {
        if (!ok) throw new Exception("FAIL: " + label);
        Console.WriteLine("PASS: " + label);
        checks++;
    }
    static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1 || !File.Exists(args[0])) throw new Exception("Pass the sanitized database template path.");
            var folder = Path.Combine(Path.GetTempPath(), "LivingRoom-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            var database = Path.Combine(folder, "test.accdb");
            File.Copy(args[0], database);
            Environment.SetEnvironmentVariable("LIVINGROOM_DATABASE_PATH", database);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(folder, "not-configured.json"));
            var service = new Service1();
            Check(service.SelectAllFamilies().Count == 0, "demo contains no families");
            Check(service.SelectAllMembers().Count == 0, "demo contains no user accounts");
            var cities = service.SelectAllCities();
            Check(cities.Count > 0, "Access provider and city lookup");
            var family = new Families { FamilyName = "Demo O'Brien family", CityID = cities[0], FamPass = "demo-only" };
            family.Id = service.InsertFamily(family);
            Check(family.Id > 0 && service.SelectFamilyByID(family.Id) != null, "create and retrieve family");
            var member = new Members { UserName = "demo-test", Passwordd = "demo'test-only", MemberName = "Demo member", BirthDate = new DateTime(2000, 1, 1), Family = family, IsManager = true };
            member.Id = service.InsertMember(member);
            Check(member.Id > 0 && service.GetMemberDetails(member.Id) != null, "create member across linked tables");
            Check(service.Login(member.UserName, member.Passwordd) != null, "valid login");
            Check(service.Login(member.UserName, "wrong") == null, "invalid password rejected");
            family.FamilyName = "Updated O'Brien family";
            Check(service.UpdateFamily(family) == 1 && service.SelectFamilyByID(family.Id).FamilyName == family.FamilyName, "update family preserves quoted text");
            member.MemberName = "Updated O'Brien member";
            Check(service.UpdateMember(member) == 2 && service.GetMemberDetails(member.Id).MemberName == member.MemberName, "update linked member and login records");
            var task = new Tasks { TaskName = "Demo's task", TaskInstructions = "Someone's disposable integration test", TaskDeadline = DateTime.Today.AddDays(1), FamilyID = family.Id };
            Check(service.InsertTask(task) > 0 && service.SelectTaskByID(task.Id) != null, "create task");
            task.Done = true;
            Check(service.UpdateTask(task) > 0 && service.SelectTaskByID(task.Id).Done, "complete task and persist");
            var shopping = new Shopping { ItemName = "Someone's milk", Quantity = 2, FamilyID = family };
            Check(service.InsertShopping(shopping) > 0 && service.SelectShoppingByID(shopping.Id) != null, "create shopping item");
            shopping.Purchased = true;
            Check(service.UpdateShopping(shopping) > 0 && service.SelectShoppingByID(shopping.Id).Purchased, "purchase shopping item and persist");
            var poll = new Polls { Question = "What's for dinner?", Description = "Today's test", FamilyID = family, Results = 0 };
            Check(service.InsertPoll(poll) > 0 && service.SelectPollByID(poll.Id) != null, "create poll");
            var option = new PollOptions { PollID = poll.Id, OptionNumber = 1, OptionText = "Demo option", Votes = 0 };
            Check(service.InsertPollOption(option) > 0 && service.SelectPollOptionByID(option.Id) != null, "create poll option");
            option.OptionText = "Everyone's option";
            option.Votes = 1;
            Check(service.UpdatePollOption(option) == 1 && service.SelectPollOptionByID(option.Id).OptionText == option.OptionText, "update poll option preserves quoted text");
            poll.Description = "Updated family's poll";
            var pollRepository = new PollsDB();
            pollRepository.Update(poll);
            Check(pollRepository.SaveChanges() == 1 && service.SelectPollByID(poll.Id).Description == poll.Description, "update poll uses typed parameters");
            var vote = new PollVotes { PollID = poll.Id, OptionID = option.Id, MemberID = member.Id };
            Check(service.InsertPollVote(vote) > 0 && service.SelectVoteByPollAndMember(poll.Id, member.Id) != null, "cast and retrieve vote");
            Check(service.DeletePollVote(vote) > 0 && service.SelectVotesByPoll(poll.Id).Count == 0, "delete vote");
            Check(service.DeletePollOption(option) > 0, "delete poll option");
            Check(service.DeletePoll(poll) > 0 && service.SelectPollByID(poll.Id) == null, "delete poll");
            Check(service.DeleteShopping(shopping) > 0 && service.SelectShoppingByID(shopping.Id) == null, "delete shopping item");
            Check(service.DeleteTask(task) > 0 && service.SelectTaskByID(task.Id) == null, "delete task");
            bool calendarDisabled = false;
            try { service.EnsureFamilyCalendar(family.Id); }
            catch (FaultException) { calendarDisabled = true; }
            Check(calendarDisabled, "calendar without credentials returns a clear fault");
            using (var host = new ServiceHost(typeof(Service1)))
            {
                host.Open();
                var client = new ofirManaFamily.ServiceReference1.Service1Client();
                Check(client.SelectAllCities().Count == cities.Count, "WCF request and response serialization");
                Check(client.Login(member.UserName, member.Passwordd).Family.Id == family.Id, "WCF login with nested family data");
                client.Close(); host.Close();
            }
            Check(service.DeleteMember(member) > 0 && service.Login(member.UserName, member.Passwordd) == null, "delete member and linked user");
            var pendingA = new FamiliesDB();
            var pendingB = new FamiliesDB();
            var familyA = new Families { FamilyName = "Queue A", CityID = cities[0], FamPass = "test" };
            var familyB = new Families { FamilyName = "Queue B", CityID = cities[0], FamPass = "test" };
            pendingA.Insert(familyA);
            pendingB.Insert(familyB);
            Check(pendingB.SaveChanges() == 1 && familyA.Id == 0 && familyB.Id > 0, "pending writes are isolated between repository instances");
            Check(pendingA.SaveChanges() == 1 && familyA.Id > 0, "saving another repository preserves this instance's pending writes");
            var failing = new FailingMembersDB();
            var failedMember = new Members { UserName = "rollback-check", Passwordd = "test", MemberName = "Rollback", BirthDate = new DateTime(2000,1,1), Family = family };
            failing.Insert(failedMember);
            Check(failing.SaveChanges() == 0 && failedMember.Id == 0, "failed linked insert rolls back identity and reports zero saved rows");
            using (var connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + database))
            {
                connection.Open();
                using (var query = connection.CreateCommand())
                {
                    query.CommandText = "SELECT COUNT(*) FROM UserTbl WHERE userName = ?";
                    query.Parameters.AddWithValue("@name", failedMember.UserName);
                    Check(Convert.ToInt32(query.ExecuteScalar()) == 0, "failed member insert leaves no orphan login record");
                }
            }
            Console.WriteLine("SUCCESS: " + checks + " integration checks passed. Original databases were not modified.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
