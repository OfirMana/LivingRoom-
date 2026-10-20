using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [CollectionDataContract]
    public class PollVotesList : List<PollVotes>
    {
        public PollVotesList() { }
        public PollVotesList(IEnumerable<PollVotes> list) : base(list) { }
        public PollVotesList(IEnumerable<BaseEntity> list) : base(list.Cast<PollVotes>().ToList()) { }
    }
}
