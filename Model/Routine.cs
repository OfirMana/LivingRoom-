using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class Routine : BaseEntity
    {
        private string routineName;
        private string repeatType;
        private string daysOfWeek;
        private string description;
        private string hour;
        private Families familyID;

        // Properties
        [DataMember]
        public string RoutineName { get => routineName; set => routineName = value; }
        [DataMember]
        public string RepeatType { get => repeatType; set => repeatType = value; }
        [DataMember]
        public string DaysOfWeek { get => daysOfWeek; set => daysOfWeek = value; }
        [DataMember]
        public string Description { get => description; set => description = value; }
        [DataMember]
        public string Hour { get => hour; set => hour = value; }
        [DataMember]
        public Families FamilyID { get => familyID; set => familyID = value; }
    }
}
