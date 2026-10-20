using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class TasksDB : BaseDB
    {
        public override BaseEntity newEntity()
        {
            return new Tasks();
        }

        public override BaseEntity CreateModel(BaseEntity entity)
        {
            Tasks task = entity as Tasks;

            if (task != null)
            {
                task.Id = (int)reader["ID"];
                task.TaskName = reader["taskName"].ToString();
                task.TaskDeadline = (DateTime)reader["taskDeadline"];
                task.Done = (bool)reader["done"];
                task.TaskInstructions = reader["instructions"].ToString();
                task.FamilyID = (int)reader["familyID"];
            }

            return task;
        }

        public TasksList SelectAll()
        {
            command.CommandText = "SELECT * FROM TasksTbl";
            List<BaseEntity> baseList = base.Select();
            TasksList tasksList = new TasksList();

            foreach (BaseEntity entity in baseList)
            {
                tasksList.Add(entity as Tasks);
            }

            return tasksList;
        }

        public Tasks SelectByID(int id)
        {
            command.CommandText = "SELECT * FROM TasksTbl WHERE ID = @ID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", id);

            List<BaseEntity> baseList = base.Select();
            if (baseList.Count == 1)
                return baseList[0] as Tasks;

            return null;
        }

        public override void Insert(BaseEntity entity)
        {
            Tasks task = entity as Tasks;

            if (task != null)
            {
                insert.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        public override void Update(BaseEntity entity)
        {
            Tasks task = entity as Tasks;

            if (task != null)
            {
                update.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        public override void Delete(BaseEntity entity)
        {
            Tasks task = entity as Tasks;

            if (task != null)
            {
                delete.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            Tasks task = entity as Tasks;

            string sqlString = string.Format(
                "INSERT INTO TasksTbl (taskName, taskDeadline, done, instructions, familyID) " +
                "VALUES ('{0}', '{1}', {2}, '{3}', {4})",
                SqlText(task.TaskName),
                task.TaskDeadline.ToString("yyyy-MM-dd HH:mm:ss"),
                task.Done ? 1 : 0,
                SqlText(task.TaskInstructions),
                task.FamilyID
            );

            return sqlString;
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            Tasks task = entity as Tasks;

            string sqlString = string.Format(
                "UPDATE TasksTbl SET taskName = '{0}', taskDeadline = '{1}', done = {2}, " +
                "instructions = '{3}', familyID = {4} WHERE ID = {5}",
                SqlText(task.TaskName),
                task.TaskDeadline.ToString("yyyy-MM-dd HH:mm:ss"),
                task.Done ? 1 : 0,
                SqlText(task.TaskInstructions),
                task.FamilyID,
                task.Id
            );

            return sqlString;
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            Tasks task = entity as Tasks;
            return $"DELETE FROM TasksTbl WHERE ID = {task.Id}";
        }
    }

}
