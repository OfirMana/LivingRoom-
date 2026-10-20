using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [CollectionDataContract]
    public class MemberList : List<Members> 
    {
        public MemberList() { }
        public MemberList(IEnumerable<Members> list) : base(list) { }
        public MemberList(IEnumerable<BaseEntity> list) : base(list.Cast<Members>().ToList()) { }
    }
}
