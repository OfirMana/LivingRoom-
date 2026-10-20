using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class User: BaseEntity
    {
        protected string userName;
        protected string passwordd;
        [DataMember]
        public string UserName { get => userName; set => userName = value; }
        [DataMember]
        public string Passwordd { get => passwordd; set => passwordd = value; }
    }
}
