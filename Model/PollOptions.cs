using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class PollOptions : BaseEntity
    {
        private int pollID;
        private int optionNumber;
        private string optionText;
        private int votes;

        // Properties
        [DataMember]
        public int PollID { get => pollID; set => pollID = value; }
        [DataMember]
        public int OptionNumber { get => optionNumber; set => optionNumber = value; }
        [DataMember]
        public string OptionText { get => optionText; set => optionText = value; }
        [DataMember]
        public int Votes { get => votes; set => votes = value; }
    }
}
