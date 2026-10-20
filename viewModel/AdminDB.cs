using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class AdminDB : UserDB
    {
        public override BaseEntity newEntity()
        {
            return new Admin();
        }

        public override BaseEntity CreateModel(BaseEntity entity)
        {
            Admin admin = entity as Admin;

            if (admin == null)
                admin = new Admin();

            // Populate common properties from UserTbl (inherited logic)
            admin = (Admin)base.CreateModel(admin);

            return admin;
        }

        public AdminList SelectAll()
        {
            command.CommandText = "SELECT * FROM AdminTbl";
            AdminList adminList = new AdminList(base.Select());
            return adminList;
        }

        public Admin SelectByID(int id)
        {
            command.CommandText = "SELECT * FROM AdminTbl WHERE ID = @ID";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", id);

            List<BaseEntity> userList = base.Select();
            if (userList.Count == 1)
                return userList[0] as Admin;

            return null;
        }

        public override void Insert(BaseEntity entity)
        {
            Admin admin = entity as Admin;
            if (admin != null)
            {
                insert.Add(new ChangeEntity(base.CreateInsertSQL, entity));
                insert.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        public override void Update(BaseEntity entity)
        {
            Admin admin = entity as Admin;
            if (admin != null)
            {
                update.Add(new ChangeEntity(base.CreateUpdateSQL, entity));
            }
        }

        public override void Delete(BaseEntity entity)
        {
            Admin admin = entity as Admin;
            if (admin != null)
            {
                delete.Add(new ChangeEntity(base.CreateDeleteSQL, entity));
                delete.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            Admin admin = entity as Admin;
            string sqlString = "INSERT INTO AdminTbl (ID) VALUES (@ID);";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", admin.Id);

            return sqlString;
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            Admin admin = entity as Admin;
            string sqlString = "DELETE FROM AdminTbl WHERE ID = @ID;";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", admin.Id);

            return sqlString;
        }
    }

}
