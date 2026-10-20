using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [CollectionDataContract]
    public class EventsList : List<Events>
    {
        public EventsList() { }
        public EventsList(IEnumerable<Events> list) : base(list) { }
        public EventsList(IEnumerable<BaseEntity> list) : base(list.Cast<Events>().ToList()) { }
    }
}
