using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class Members : User
    {
        protected string memberName; 
        protected DateTime birthDate; 
        protected bool isManager;
        protected Families family;


        [DataMember]
        public string MemberName { get => memberName; set => memberName = value; }
        [DataMember]
        public DateTime BirthDate { get => birthDate; set => birthDate = value; }
        [DataMember]
        public bool IsManager { get => isManager; set => isManager = value; }
        [DataMember]
        public Families Family { get => family; set => family = value; }
        
    }
}
//protected Families family;


//public DateTime? DateOfBirth
//{
//    get
//    {
//        try
//        {
//            return new DateTime(birthYear, birthMonth, birthDay);
//        }
//        catch
//        {
//            return null; // Return null if date is invalid
//        }
//    }
//    set
//    {
//        if (value.HasValue)
//        {
//            birthYear = value.Value.Year;
//            birthMonth = value.Value.Month;
//            birthDay = value.Value.Day;
//        }
//        else
//        {
//            birthYear = 0;
//            birthMonth = 0;
//            birthDay = 0;
//        }
//    }
//}
//public byte[] ProfilePictureFileData { get => profilePictureFileData; set => profilePictureFileData = value; }
//public string ProfilePictureFileName { get => profilePictureFileName; set => profilePictureFileName = value; }
//public string ProfilePictureFileType { get => profilePictureFileType; set => profilePictureFileType = value; }
//public int Permissions { get => permissions; set => permissions = value; }
//public Families Family { get => family; set => family = value; }