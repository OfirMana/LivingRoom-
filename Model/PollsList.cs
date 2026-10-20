using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [CollectionDataContract]
    public class PollsList : List<Polls> 
    {
        public PollsList() { }
        public PollsList(IEnumerable<Polls> list) : base(list) { }
        public PollsList(IEnumerable<BaseEntity> list) : base(list.Cast<Polls>().ToList()) { }
    }
}
