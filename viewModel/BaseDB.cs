using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
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

        protected readonly List<ChangeEntity> insert = new List<ChangeEntity>();
        protected readonly List<ChangeEntity> update = new List<ChangeEntity>();
        protected readonly List<ChangeEntity> delete = new List<ChangeEntity>();

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
        /// <summary>Commits this repository's queued changes as one atomic operation.</summary>
        public int SaveChanges()
        {
            int records = 0;
            var identities = insert.Select(change => change.Entity).Distinct()
                .ToDictionary(entity => entity, entity => entity.Id);
            OleDbTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                foreach (var change in insert)
                {
                    command.Parameters.Clear();
                    command.CommandText = change.CreateSQL(change.Entity);
                    records += command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    command.CommandText = "SELECT @@IDENTITY";
                    change.Entity.Id = Convert.ToInt32(command.ExecuteScalar());
                }
                foreach (var change in update.Concat(delete))
                {
                    command.Parameters.Clear();
                    command.CommandText = change.CreateSQL(change.Entity);
                    records += command.ExecuteNonQuery();
                }
                transaction.Commit();
                return records;
            }
            catch (Exception error)
            {
                if (transaction != null)
                {
                    try { transaction.Rollback(); }
                    catch (Exception rollbackError) when (rollbackError is OleDbException || rollbackError is InvalidOperationException)
                    {
                        Trace.TraceError("Database rollback could not complete ({0}).", rollbackError.GetType().Name);
                    }
                }
                foreach (var original in identities) original.Key.Id = original.Value;
                // Never write SQL values, credentials, or database contents to logs.
                Trace.TraceError("Database save failed ({0}).", error.GetType().Name);
                return 0;
            }
            finally
            {
                command.Transaction = null;
                command.Parameters.Clear();
                transaction?.Dispose();
                insert.Clear();
                update.Clear();
                delete.Clear();
                if (connection.State != ConnectionState.Closed) connection.Close();
            }
        }

        // OLE DB binds parameters by position, regardless of their names.
        protected string ParameterizedSql(string sql, params object[] values)
        {
            command.Parameters.Clear();
            for (int index = 0; index < values.Length; index++)
            {
                object value = values[index] ?? string.Empty;
                var parameter = new OleDbParameter { ParameterName = "@p" + index };
                if (value is DateTime) parameter.OleDbType = OleDbType.DBTimeStamp;
                else if (value is bool) parameter.OleDbType = OleDbType.Boolean;
                else if (value is int) parameter.OleDbType = OleDbType.Integer;
                else
                {
                    parameter.OleDbType = OleDbType.VarWChar;
                    parameter.Size = Math.Max(Convert.ToString(value).Length, 1);
                }
                parameter.Value = value;
                command.Parameters.Add(parameter);
            }
            return sql;
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
