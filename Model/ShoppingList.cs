using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Model
{
    [CollectionDataContract]
    public class ShoppingList : List<Shopping>
    {
        public ShoppingList() { }
        public ShoppingList(IEnumerable<Shopping> list) : base(list) { }
        public ShoppingList(IEnumerable<BaseEntity> list) : base(list.Cast<Shopping>().ToList()) { }
    }
}
