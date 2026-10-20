using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [CollectionDataContract]
    public class PollOptionsList : List<PollOptions>
    {
        public PollOptionsList() { }
        public PollOptionsList(IEnumerable<PollOptions> list) : base(list) { }
        public PollOptionsList(IEnumerable<BaseEntity> list) : base(list.Cast<PollOptions>().ToList()) { }
    }
}
