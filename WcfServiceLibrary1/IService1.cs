using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ViewModel;
using Model;


namespace WcfServiceLibrary1
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        MemberList SelectAllMembers();

        [OperationContract]
        MemberList SelectMembersByName(string userName, int familyId);

        [OperationContract]
        Members GetMemberDetails(int id);

        [OperationContract]
        int InsertMember(Members member);

        [OperationContract]
        int UpdateMember(Members member);

        [OperationContract]
        int DeleteMember(Members member);

        [OperationContract]
        FamiliesList SelectAllFamilies();

        [OperationContract]
        Families SelectFamilyByID(int id);

        [OperationContract]
        CitiesList SelectAllCities();

        [OperationContract]
        Cities SelectCityById(int id);

        [OperationContract]
        EventsList SelectAllEvents();

        [OperationContract]
        PollOptionsList SelectAllPollOptions();

        [OperationContract]
        PollOptions SelectPollOptionByID(int id);

        [OperationContract]
        PollsList SelectAllPolls();

        [OperationContract]
        Polls SelectPollByID(int id);

        [OperationContract]
        RoutineList SelectAllRoutines();

        [OperationContract]
        Routine SelectRoutineByID(int id);

        [OperationContract]
        ShoppingList SelectAllShopping();

        [OperationContract]
        Shopping SelectShoppingByID(int id);

        [OperationContract]
        ShoppingList SelectShoppingByNotPurchased(bool False);

        [OperationContract]
        int InsertShopping(Shopping shopping);

        [OperationContract]
        TasksList SelectAllTasks();

        [OperationContract]
        Tasks SelectTaskByID(int id);

        // ─── Calendar & Events ───

        [OperationContract]
        EventsList GetFamilyEvents(int familyId, DateTime from, DateTime to);

        [OperationContract]
        string CreateFamilyEvent(int familyId, Events ev);

        [OperationContract]
        void UpdateFamilyEvent(int familyId, Events ev);

        [OperationContract]
        void DeleteFamilyEvent(int familyId, string eventId);
        [OperationContract]
        Members Login(string username, string password);
        [OperationContract]
        int DeleteShopping(Shopping shopping);
        [OperationContract]
        int InsertTask(Tasks task);

        [OperationContract]
        int DeleteTask(Tasks task);
        [OperationContract]
        PollVotes SelectVoteByPollAndMember(int pollId, int memberId);

        [OperationContract]
        PollVotesList SelectVotesByPoll(int pollId);

        [OperationContract]
        int InsertPoll(Polls poll);

        [OperationContract]
        int InsertPollOption(PollOptions option);

        [OperationContract]
        int InsertPollVote(PollVotes vote);

        [OperationContract]
        int UpdatePollOption(PollOptions option);

        [OperationContract]
        int DeletePoll(Polls poll);

        [OperationContract]
        int DeletePollOption(PollOptions option);

        [OperationContract]
        int DeletePollVote(PollVotes vote);
        [OperationContract]
        int InsertFamily(Families family);
        [OperationContract]
        int UpdateFamily(Families family);
        [OperationContract]
        string EnsureFamilyCalendar(int familyId);
        [OperationContract]
        int UpdateTask(Tasks t);
        [OperationContract]
        int UpdateShopping(Shopping s);
    }
}
