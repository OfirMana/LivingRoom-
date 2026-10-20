using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class Families : BaseEntity
    {
        private string familyName; // שם המשפחה, לא יוניק
        private string famPass;
        private Cities cityID; //עיר המגורים
        private ShoppingList shoppingList; //רשימת המוצרים שצריך לקנות שמקושרים למשפחה
        private RoutineList routineList; //רשימת המטלות החוזרות שמקושרות לבני המשפחה
        private TasksList tasksList; //רשימת המטלות שמקושרות לבני המשפחה
        private EventsList eventsList; //רשימת האירועים שמקושרים לבני המשפחה
        private PollsList pollsList; //רשימת הסקרים שמקושרים לבני המשפחה
        private MemberList memberList; //רשימת בני המשפחה
        private string googleCalendarId;

        [DataMember]
        public string FamilyName { get => familyName; set => familyName = value; }
        [DataMember]
        public string FamPass { get => famPass; set => famPass = value; }
        [DataMember]
        public Cities CityID { get => cityID; set => cityID = value; }
        [DataMember]
        public ShoppingList ShoppingList { get => shoppingList; set => shoppingList = value; }
        [DataMember]
        public RoutineList RoutineList { get => routineList; set => routineList = value; }
        [DataMember]
        public TasksList TasksList { get => tasksList; set => tasksList = value; }
        [DataMember]
        public EventsList EventsList { get => eventsList; set => eventsList = value; }
        [DataMember]
        public PollsList PollsList { get => pollsList; set => pollsList = value; }
        [DataMember]
        public MemberList MemberList { get => memberList; set => memberList = value; }
        [DataMember]
        public string GoogleCalendarId
        {
            get => googleCalendarId;
            set => googleCalendarId = value;
        }
    }
}
