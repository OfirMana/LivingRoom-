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
            var user = (User)entity;
            return ParameterizedSql("INSERT INTO [UserTbl] ([userName], [password]) VALUES (?, ?)",
                user.UserName, user.Passwordd);
        }


        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            var user = (User)entity;
            return ParameterizedSql("DELETE FROM [UserTbl] WHERE [ID] = ?", user.Id);
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            var user = (User)entity;
            return ParameterizedSql("UPDATE [UserTbl] SET [userName] = ?, [password] = ? WHERE [ID] = ?",
                user.UserName, user.Passwordd, user.Id);
        }

    }
}
