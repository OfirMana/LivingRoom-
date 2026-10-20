using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class Shopping : BaseEntity
    {
        private string itemName;
        private int quantity;
        private int addedByMember;
        private bool purchased;
        private Families familyID;

        // Properties
        [DataMember]
        public string ItemName { get => itemName; set => itemName = value; }
        [DataMember]
        public int Quantity { get => quantity; set => quantity = value; }
        [DataMember]
        public int AddedByMember { get => addedByMember; set => addedByMember = value; }
        [DataMember]
        public bool Purchased { get => purchased; set => purchased = value; }
        [DataMember]
        public Families FamilyID { get => familyID; set => familyID = value;}
    }
}
