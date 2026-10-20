using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [CollectionDataContract]
    public class RoutineList : List<Routine> 
    {
        public RoutineList() { }
        public RoutineList(IEnumerable<Routine> list) : base(list) { }
        public RoutineList(IEnumerable<BaseEntity> list) : base(list.Cast<Routine>().ToList()) { }
    }
}
