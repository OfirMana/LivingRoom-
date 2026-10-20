using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using Model;

namespace ViewModel
{
    /// <summary>
    /// MembersDB – CRUD על טבלת MembersTbl + Join עם UserTbl  
    /// </summary>
    public class MembersDB : UserDB
    {
        /*──────────────────────── בסיסי ────────────────────────*/

        public override BaseEntity newEntity()          /* יוצר אובייקט ריק */
        {
            return new Members();
        }

        public override BaseEntity CreateModel(BaseEntity entity)
        {
            /* ממיר שורה מה-DB לאובייקט Members */
            Members member = (Members)entity;

            base.CreateModel(member);                   // מאפייני UserTbl
            member.MemberName = reader["memberName"].ToString();
            member.BirthDate = (DateTime)reader["birthDate"];
            member.IsManager = (bool)reader["isManager"];

            int famId = (int)reader["familyID"];
            member.Family = new FamiliesDB().SelectByID(famId);  // קשר למשפחה
            return member;
        }

        /*──────────────────────── SELECT-ים ─────────────────────*/

        public MemberList SelectAll()
        {
            command.CommandText =
                "SELECT *, UserTbl.ID AS ID " +
                "FROM  UserTbl INNER JOIN MembersTbl ON UserTbl.ID = MembersTbl.ID";
            return new MemberList(base.Select());
        }

        public MemberList SelectByName(string userName, int familyId)
        {
            command.CommandText =
                "SELECT *, UserTbl.ID AS ID " +
                "FROM UserTbl INNER JOIN MembersTbl ON UserTbl.ID = MembersTbl.ID " +
                "WHERE UserTbl.userName = @userName AND MembersTbl.familyID = @familyId";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@userName", userName);
            command.Parameters.AddWithValue("@familyId", familyId);

            return new MemberList(base.Select().Cast<Members>().ToList());
        }

        public Members SelectByID(int id)
        {
            command.CommandText =
                "SELECT *, UserTbl.ID AS ID " +
                "FROM  UserTbl INNER JOIN MembersTbl ON UserTbl.ID = MembersTbl.ID " +
                "WHERE UserTbl.ID = " + id;

            var list = base.Select();
            return list.Count == 1 ? (Members)list[0] : null;
        }

        /*──────────────────────── שינוי נתונים ─────────────────*/

        /*   Insert/Update/Delete מוסיפים את ה-SQL לרשימות  */
        public override void Insert(BaseEntity e)
        {
            Members m = e as Members;
            if (m == null) return;

            insert.Add(new ChangeEntity(base.CreateInsertSQL, e));  // UserTbl
            insert.Add(new ChangeEntity(CreateInsertSQL, e));  // MembersTbl
        }

        public override void Update(BaseEntity e)
        {
            Members m = e as Members;
            if (m == null) return;

            update.Add(new ChangeEntity(CreateUpdateSQL, e));  // MembersTbl
            update.Add(new ChangeEntity(base.CreateUpdateSQL, e));  // UserTbl
        }

        public override void Delete(BaseEntity e)
        {
            Members m = e as Members;
            if (m == null) return;

            delete.Add(new ChangeEntity(CreateDeleteSQL, e));  // MembersTbl
            delete.Add(new ChangeEntity(base.CreateDeleteSQL, e));  // UserTbl
        }

        /*──────────────────────── SQL Builders ────────────────*/

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            /* מכין פקודת INSERT לטבלת MembersTbl – ללא Role */
            Members m = (Members)entity;
            int famId = (m.Family != null) ? m.Family.Id : 0;            // בטיחות
            string name = (m.MemberName ?? "").Replace("'", "''");
            string iso = m.BirthDate.ToString("yyyy-MM-dd");

            return string.Format(
                "INSERT INTO MembersTbl " +
                "(ID, memberName, birthDate, isManager, familyID) " +
                "VALUES ({0}, '{1}', '{2}', {3}, {4})",
                m.Id, name, iso, m.IsManager ? 1 : 0, famId);
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            /* UPDATE ללא Role */
            Members m = (Members)entity;
            string name = m.MemberName.Replace("'", "''");
            string iso = m.BirthDate.ToString("yyyy-MM-dd");

            return string.Format(
                "UPDATE MembersTbl SET " +
                "memberName = '{0}', " +
                "birthDate  = '{1}', " +
                "isManager  = {2}, " +
                "familyID   = {3} " +
                "WHERE ID   = {4}",
                name, iso, m.IsManager ? 1 : 0, m.Family.Id, m.Id);
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            return "DELETE FROM MembersTbl WHERE ID = " + ((Members)entity).Id;
        }

        /*──────────────────────── Login ───────────────────────*/

        public Members Login(string username, string password)
        {
            Members member = null;
            try
            {
                command.CommandText =
                    "SELECT *, UserTbl.ID AS ID " +
                    "FROM UserTbl INNER JOIN MembersTbl ON UserTbl.ID = MembersTbl.ID " +
                    "WHERE UserTbl.userName = @u AND UserTbl.[password] = @p";

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@u", username);
                command.Parameters.AddWithValue("@p", password);

                connection.Open();
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    member = new Members
                    {
                        Id = (int)reader["ID"],
                        UserName = reader["userName"].ToString(),
                        Passwordd = reader["password"].ToString(),
                        MemberName = reader["memberName"].ToString(),
                        BirthDate = (DateTime)reader["birthDate"],
                        IsManager = (bool)reader["isManager"],
                        Family = new FamiliesDB()
                                     .SelectByID((int)reader["familyID"])
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Login Error: " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (connection.State == ConnectionState.Open) connection.Close();
            }
            return member;
        }
    }
}
//public MemberList SelectAll()
//{
//    command.CommandText = command.CommandText = "SELECT * FROM (UserTbl INNER JOIN"+
//    "MembersTbl ON UserTbl.ID = MembersTbl.ID)";
//    MemberList memberList = new MemberList();

//    try
//    {
//        command.Connection = connection;
//        connection.Open();
//        reader = command.ExecuteReader();

//        Members member;
//        while (reader.Read())
//        {
//            member = new Members();
//            member.ID = (int)reader["ID"];
//            member.MemberName = reader["MemberName"].ToString();
//            member.BirthYear = (int)reader["BirthYear"];
//            member.BirthMonth = (int)reader["BirthMonth"];
//            member.BirthDay = (int)reader["BirthDay"];
//            member.Role = reader["Role"].ToString();
//            member.Permissions = (int)reader["Permission"];
//            member.Family = (Families)reader["Family"];

//            //int city = (int)reader["City"];
//            //student.City = CityDB.SelectByID(city);

//            memberList.Add(member);
//        }
//    }
//    catch (Exception e)
//    {
//        System.Diagnostics.Debug.WriteLine(e.Message);
//    }
//    finally
//    {
//        if (reader != null)
//            reader.Close();

//        if (connection.State == ConnectionState.Open)
//            connection.Close();
//    }

//    return memberList;
//}
//public MemberList SelectByName(string MemberName)
//{
//    command.CommandText = string.Format("SELECT *,PeopleTbl. ID as ID FROM (PeopleTbl " +
//    "INNER JOIN StudentTbl ON PeopleTbl. ID = StudentTbl. ID) " +
//    "WHERE MemberName='{0}''", MemberName);
//    MemberList memberList = new MemberList();

//    try
//    {
//        command.Connection = connection;
//        connection.Open();
//        reader = command.ExecuteReader();

//        Members member;
//        while (reader.Read())
//        {
//            member = new Members();
//            member.ID = (int)reader["ID"];
//            member.MemberName = reader["MemberName"].ToString();
//            member.BirthYear = (int)reader["BirthYear"];
//            member.BirthMonth = (int)reader["BirthMonth"];
//            member.BirthDay = (int)reader["BirthDay"];
//            member.Role = reader["Role"].ToString();
//            member.Permissions = (int)reader["Permission"];
//            member.Family = (Families)reader["Family"];

//            //int city = (int)reader["City"];
//            //student.City = CityDB.SelectByID(city);

//            memberList.Add(member);
//        }
//    }
//    catch (Exception e)
//    {
//        System.Diagnostics.Debug.WriteLine(e.Message);
//    }
//    finally
//    {
//        if (reader != null)
//            reader.Close();

//        if (connection.State == ConnectionState.Open)
//            connection.Close();
//    }

//    return memberList;
//}
//public Members SelectById(int id)
//{
//    command.CommandText = "SELECT *,PersonTbl.ID as ID FROM (PersonTbl INNER JOIN" +
//    "StudentTbl ON PersonTbl.ID StudentTbl.ID)" + $"WHEREW PersonTbl.ID={id}";
//    MemberList memberList = new MemberList();

//    try
//    {
//        command.Connection = connection;
//        connection.Open();
//        reader = command.ExecuteReader();

//        Members member;
//        while (reader.Read())
//        {
//            member = new Members();
//            member.ID = (int)reader["ID"];
//            member.MemberName = reader["MemberName"].ToString();
//            member.BirthYear = (int)reader["BirthYear"];
//            member.BirthMonth = (int)reader["BirthMonth"];
//            member.BirthDay = (int)reader["BirthDay"];
//            member.Role = reader["Role"].ToString();
//            member.Permissions = (int)reader["Permission"];
//            member.Family = (Families)reader["Family"];

//            //int city = (int)reader["City"];
//            //student.City = CityDB.SelectByID(city);

//            memberList.Add(member);
//        }
//    }
//    catch (Exception e)
//    {
//        System.Diagnostics.Debug.WriteLine(e.Message);
//    }
//    finally
//    {
//        if (reader != null)
//            reader.Close();

//        if (connection.State == ConnectionState.Open)
//            connection.Close();
//    }
//    if (memberList.Count == 1)
//        return memberList[0];
//    else
//        return null;
//}


