using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text;
using Model;

namespace ViewModel
{
    public abstract class UserDB : BaseDB
    {
     
        public override BaseEntity CreateModel(BaseEntity entity)
        {
            User user = entity as User;
            user.Id = (int)reader["ID"];
            user.UserName = reader["userName"].ToString();
            user.Passwordd = reader["password"].ToString();
            //user.LastLogin = (DateTime)reader["lastLogin"]; 
            
            return user; // Remove base.CreateModel(user) as it's not defined in the base class
        }

        

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            User user = entity as User;
            string sqlStr = string.Format("INSERT INTO UserTbl(userName, [password]) VALUES('{0}','{1}')", SqlText(user.UserName), SqlText(user.Passwordd));
            return sqlStr;
          
        }


        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            User user = entity as User;
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("DELETE FROM UserTbl WHERE ID = {0}", user.Id);
            return sqlBuilder.ToString();
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            var u = entity as Members;  // assuming your base class is for Members
            return string.Format(
                "UPDATE UserTbl " +
                "   SET userName = '{0}', " +
                "       [password] = '{1}' " +    // wrap in brackets
                " WHERE ID = {2}",              // leading space before WHERE
                u.UserName.Replace("'", "''"),
                u.Passwordd.Replace("'", "''"),
                u.Id
            );
        }

    }
}
//public UserList SelectAll()
//{
//    command.CommandText = "SELECT * FROM UserTbl";
//    UserList userList = new UserList(base.Select());
//    return userList;
//}

//public User SelectByID(int id)
//{
//    command.CommandText = $"SELECT * FROM UserTbl WHERE ID = {id}";
//    List<BaseEntity> userList = base.Select();
//    if (userList.Count == 1)
//        return userList[0] as User;
//    return null;
//}
//protected override string CreateInsertSQL(BaseEntity entity)
//{
//    User user = entity as User;

//    string sqlStr = string.Format("INSERT INTO UserTbl(userName, password) VALUES('{0}','{1}')", SqlText(user.UserName), SqlText(user.Passwordd));
//    return sqlStr;

//    //string sqlStr = string.Format("INSERT INTO UserTbl(userName, password)" +
//    //   "VALUES('{0}','{1}')", SqlText(user.UserName), SqlText(user.Passwordd));
//    //return sqlStr;

//    //string sqlStr = string.Format("INSERT INTO UserTbl(userName, password) VALUES('{0}','{1}')", SqlText(user.UserName), SqlText(user.Passwordd));
//    //return sqlStr;
//}

