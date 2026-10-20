using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class RoutineDB : BaseDB
    {
        public override BaseEntity newEntity()
        {
            return new Routine();
        }

        public override BaseEntity CreateModel(BaseEntity entity)
        {
            Routine routine = entity as Routine;

            if (routine != null)
            {
                routine.Id = (int)reader["ID"];
                routine.RoutineName = reader["routineName"].ToString();
                routine.RepeatType = reader["repeatType"].ToString();
                routine.DaysOfWeek = reader["daysOfWeek"].ToString();
                routine.Description = reader["description"].ToString();
                routine.Hour = reader["hour"].ToString();
                routine.FamilyID = (Families)reader["familyID"];
            }

            return routine;
        }

        public RoutineList SelectAll()
        {
            command.CommandText = "SELECT * FROM RoutineTbl";
            List<BaseEntity> baseList = base.Select();
            RoutineList routineList = new RoutineList();

            foreach (BaseEntity entity in baseList)
            {
                routineList.Add(entity as Routine);
            }

            return routineList;
        }

        public Routine SelectByID(int id)
        {
            command.CommandText = "SELECT * FROM RoutineTbl WHERE ID = @ID";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", id);

            List<BaseEntity> baseList = base.Select();
            if (baseList.Count == 1)
                return baseList[0] as Routine;

            return null;
        }

        public override void Insert(BaseEntity entity)
        {
            Routine routine = entity as Routine;

            if (routine != null)
            {
                insert.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        public override void Update(BaseEntity entity)
        {
            Routine routine = entity as Routine;

            if (routine != null)
            {
                update.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        public override void Delete(BaseEntity entity)
        {
            Routine routine = entity as Routine;

            if (routine != null)
            {
                delete.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            Routine routine = entity as Routine;

            string sqlString = string.Format(
                "INSERT INTO RoutineTbl (routineName, repeatType, daysOfWeek, description, hour) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                routine.RoutineName,
                routine.RepeatType,
                routine.DaysOfWeek,
                routine.Description,
                routine.Hour
            );

            return sqlString;
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            Routine routine = entity as Routine;

            string sqlString = string.Format(
                "UPDATE RoutineTbl SET routineName = '{0}', repeatType = '{1}', daysOfWeek = '{2}', " +
                "description = '{3}', hour = '{4}' WHERE ID = {5}",
                routine.RoutineName,
                routine.RepeatType,
                routine.DaysOfWeek,
                routine.Description,
                routine.Hour,
                routine.Id
            );

            return sqlString;
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            Routine routine = entity as Routine;

            string sqlString = $"DELETE FROM RoutineTbl WHERE ID = {routine.Id}";
            return sqlString;
        }
    }

}
