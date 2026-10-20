using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class EventsDB : BaseDB
    {
        public override BaseEntity newEntity() => new Events();

        /*────────────────────────────  MAPPING  ────────────────────────────*/
        public override BaseEntity CreateModel(BaseEntity entity)
        {
            var ev = entity as Events;

            if (ev != null)
            {
                ev.Id = (int)reader["ID"];
                ev.EventName = reader["eventName"].ToString();
                ev.EventStartDate = (DateTime)reader["eventStartDate"];
                ev.EventEndDate = (DateTime)reader["eventEndDate"];
                ev.EventDescription = reader["eventDescription"].ToString();
                ev.ForMembers = (MemberList)reader["forMembers"];
                ev.GoogleEventId = reader["googleEventId"].ToString();     // ← NEW
            }
            return ev;
        }

        /*────────────────────────────  SELECT  ────────────────────────────*/
        public EventsList SelectAll()
        {
            command.CommandText = "SELECT * FROM EventsTbl";
            var list = new EventsList();
            foreach (var b in base.Select())
                list.Add(b as Events);
            return list;
        }

        public Events SelectByID(int id)
        {
            command.CommandText = "SELECT * FROM EventsTbl WHERE ID=@ID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", id);

            var rows = base.Select();
            return rows.Count == 1 ? rows[0] as Events : null;
        }

        /*────────────────────────────  C-/U-/D  ────────────────────────────*/
        public override void Insert(BaseEntity entity)
        {
            if (entity is Events ev)
                insert.Add(new ChangeEntity(CreateInsertSQL, ev));
        }

        public override void Update(BaseEntity entity)
        {
            if (entity is Events ev)
                update.Add(new ChangeEntity(CreateUpdateSQL, ev));
        }

        public override void Delete(BaseEntity entity)
        {
            if (entity is Events ev)
                delete.Add(new ChangeEntity(CreateDeleteSQL, ev));
        }

        /*────────────────────────────  SQL  ───────────────────────────────*/
        protected override string CreateInsertSQL(BaseEntity entity)
        {
            var e = entity as Events;

            return string.Format(
                "INSERT INTO EventsTbl " +
                "(eventName, eventStartDate, eventEndDate, eventDescription, forMembers, tasksLinked, googleEventId) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', {4}, {5}, '{6}')",
                e.EventName.Replace("'", "''"),
                e.EventStartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                e.EventEndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                e.EventDescription.Replace("'", "''"),
                e.ForMembers,          // keep your existing logic
                e.TasksLinked,         // keep your existing logic
                (e.GoogleEventId ?? "").Replace("'", "''")      // ← NEW
            );
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            var e = entity as Events;

            return string.Format(
                "UPDATE EventsTbl SET " +
                "eventName        = '{0}', " +
                "eventStartDate   = '{1}', " +
                "eventEndDate     = '{2}', " +
                "eventDescription = '{3}', " +
                "forMembers       = {4}, " +
                "tasksLinked      = {5}, " +
                "googleEventId    = '{6}' " +           // ← NEW
                "WHERE ID = {7}",
                e.EventName.Replace("'", "''"),
                e.EventStartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                e.EventEndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                e.EventDescription.Replace("'", "''"),
                e.ForMembers,
                e.TasksLinked,
                (e.GoogleEventId ?? "").Replace("'", "''"),
                e.Id
            );
        }

        protected override string CreateDeleteSQL(BaseEntity entity) =>
            $"DELETE FROM EventsTbl WHERE ID={((Events)entity).Id}";
    }
}