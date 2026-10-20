using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class Events : BaseEntity
    {
        private string eventName; //שם האירוע
        private DateTime eventStartDate; //תחילת האירוע
        private DateTime eventEndDate; //סיום האירוע
        private string eventDescription; //תיאור האירוע
        private MemberList forMembers; //עבור מי האירוע
        private TasksList tasksLinked; //משימות שמקושרות
        private string googleEventId; //Holds Google’s event-ID that you get back from the Calendar API.

        [DataMember]
        public string EventName { get => eventName; set => eventName = value; }
        [DataMember]
        public DateTime EventStartDate { get => eventStartDate; set => eventStartDate = value; }
        [DataMember]
        public DateTime EventEndDate { get => eventEndDate; set => eventEndDate = value; }
        [DataMember]
        public string EventDescription { get => eventDescription; set => eventDescription = value; }
        [DataMember]
        public MemberList ForMembers { get => forMembers; set => forMembers = value; }
        [DataMember]
        public TasksList TasksLinked { get => tasksLinked; set => tasksLinked = value; }
        [DataMember]
        public string GoogleEventId { get => googleEventId; set => googleEventId = value; }
    }
}
