using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using Model;

namespace ViewModel
{
    public abstract class BaseDB
    {
        private static string GetConnectionString()
        {
            var databasePath = Environment.GetEnvironmentVariable("LIVINGROOM_DATABASE_PATH");
            if (string.IsNullOrWhiteSpace(databasePath))
                databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DATA", "FamilyProjectOfir.accdb");
            if (!File.Exists(databasePath))
                throw new FileNotFoundException("LivingRoom database was not found. Run scripts/Start-LivingRoom.ps1 or set LIVINGROOM_DATABASE_PATH.", databasePath);
            return new OleDbConnectionStringBuilder
            {
                Provider = "Microsoft.ACE.OLEDB.16.0",
                DataSource = Path.GetFullPath(databasePath),
                PersistSecurityInfo = false
            }.ConnectionString;
        }
        protected OleDbConnection connection;
        protected OleDbCommand command;
        protected OleDbDataReader reader;

        public BaseDB()
        {
            connection = new OleDbConnection(GetConnectionString());
            command = new OleDbCommand();
            command.Connection = connection;

        }
        protected static string SqlText(string value) => (value ?? string.Empty).Replace("'", "''");

        public abstract BaseEntity newEntity();
        public abstract BaseEntity CreateModel(BaseEntity entity);

        public static List<ChangeEntity> insert = new List<ChangeEntity>();
        public static List<ChangeEntity> update = new List<ChangeEntity>();
        public static List<ChangeEntity> delete = new List<ChangeEntity>();

        protected abstract string CreateInsertSQL(BaseEntity entity);
        protected abstract string CreateUpdateSQL(BaseEntity entity);
        protected abstract string CreateDeleteSQL(BaseEntity entity);


        public List<BaseEntity> Select()
        {
            List<BaseEntity> list = new List<BaseEntity>();
            try
            {
                command.Connection = connection;
                connection.Open();

                reader = command.ExecuteReader();
                BaseEntity entity;
                while (reader.Read())
                {
                    entity = this.newEntity();
                    entity = this.CreateModel(entity);

                    list.Add(entity);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return list;
        }
        public int SaveChanges()
        {
            OleDbCommand command = new OleDbCommand();
            int records = 0;
            try
            {
                command.Connection = this.connection;
                connection.Open();
                //records = command.ExecuteNonQuery();

                //עוברים על כל הרשימה שצריך להוסיף
                foreach (var item in insert)
                {
                    command.CommandText = item.CreateSQL(item.Entity);
                    records += command.ExecuteNonQuery();
                    command.CommandText = "SELECT @@IDentity"; //Get last ID
                    item.Entity.Id = (int)command.ExecuteScalar();
                }
                foreach (var item in update)
                {
                    command.CommandText = item.CreateSQL(item.Entity);
                    records += command.ExecuteNonQuery();
                }
                foreach (var item in delete)
                {
                    command.CommandText = item.CreateSQL(item.Entity);
                    records += command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\nSQL: " + command.CommandText);
            }
            finally
            {
                insert.Clear();
                update.Clear();
                delete.Clear();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return records;
        }

        public virtual void Insert(BaseEntity entity)
        {
            BaseEntity reqEntity = this.newEntity();
            if (entity != null && entity.GetType() == reqEntity.GetType())
                insert.Add(new ChangeEntity(this.CreateInsertSQL, entity)); //עדכון רשימת הבקשות
        }

        public virtual void Update(BaseEntity entity)
        {
            BaseEntity reqEntity = this.newEntity();
            if (entity != null && entity.GetType() == reqEntity.GetType())
                update.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
        }

        public virtual void Delete(BaseEntity entity)
        {
            BaseEntity reqEntity = this.newEntity();
            if (entity != null && entity.GetType() == reqEntity.GetType())
                delete.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
        }


    }
}