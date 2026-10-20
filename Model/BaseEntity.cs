using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    [KnownType(typeof(User))]
    [KnownType(typeof(Cities))]
    [KnownType(typeof(Events))]
    [KnownType(typeof(Families))]
    [KnownType(typeof(PollOptions))]
    [KnownType(typeof(Polls))]
    [KnownType(typeof(PollVotes))]
    [KnownType(typeof(Routine))]
    [KnownType(typeof(Shopping))]
    [KnownType(typeof(Tasks))]
    [KnownType(typeof(Members))]
    [KnownType(typeof(Admin))]
    public class BaseEntity
    {
        private int id;

        [DataMember]

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
    }

}
