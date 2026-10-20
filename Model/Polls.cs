using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class Polls : BaseEntity
    {
        private string question;
        private string description;
        private int results;
        private Families familyID;
        private MemberList voted;

        // Properties
        [DataMember]
        public string Question { get => question; set => question = value; }
        [DataMember]
        public string Description { get => description; set => description = value; }
        [DataMember]
        public int Results { get => results; set => results = value; }
        [DataMember]
        public Families FamilyID { get => familyID; set => familyID = value; }
        [DataMember]
        public MemberList Voted { get => voted; set => voted = value; }
    }
}
