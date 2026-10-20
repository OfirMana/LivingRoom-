using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [CollectionDataContract]
    public class CitiesList : List<Cities>
    {
        public CitiesList() { }
        public CitiesList(IEnumerable<Cities> list) : base(list) { }
        public CitiesList(IEnumerable<BaseEntity> list) : base(list.Cast<Cities>().ToList()) { }
    }
}
