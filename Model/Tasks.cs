using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class Tasks : BaseEntity
    {
        private string taskName;
        private DateTime taskDeadline;
        private bool done;
        private string taskInstructions;
        private int familyID;


        // Properties
        [DataMember]
        public string TaskName { get => taskName; set => taskName = value; }
        [DataMember]
        public DateTime TaskDeadline { get => taskDeadline; set => taskDeadline = value; }
        [DataMember]
        public bool Done { get => done; set => done = value; }
        [DataMember]
        public string TaskInstructions { get => taskInstructions; set => taskInstructions = value; }
        [DataMember]
        public int FamilyID { get => familyID; set => familyID = value;}
    }
}
