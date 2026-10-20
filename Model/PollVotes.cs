using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class PollVotes : BaseEntity
    {
        private int pollID;
        private int optionID;
        private int memberID;

        [DataMember]
        public int PollID { get => pollID; set => pollID = value; }
        [DataMember]
        public int OptionID { get => optionID; set => optionID = value; }
        [DataMember]
        public int MemberID { get => memberID; set => memberID = value; }
    }
}
