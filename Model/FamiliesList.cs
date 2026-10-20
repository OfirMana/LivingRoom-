using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [CollectionDataContract]
    public class FamiliesList : List<Families>
    {
        public FamiliesList() { }
        public FamiliesList(IEnumerable<Families> list) : base(list) { }
        public FamiliesList(IEnumerable<BaseEntity> list) : base(list.Cast<Families>().ToList()) { }
    }
}
