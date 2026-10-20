using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class PollsDB : BaseDB
    {
        public override BaseEntity newEntity()
        {
            return new Polls();
        }

        public override BaseEntity CreateModel(BaseEntity entity)
        {
            Polls poll = entity as Polls;

            if (poll != null)
            {
                poll.Id = (int)reader["ID"];
                poll.Question = reader["question"].ToString();
                poll.Description = reader["Description"].ToString();
                //poll.CreatedByMember = (int)reader["createdByMember"];
                poll.Results = (int)reader["results"];
                int familyId = (int)reader["familyID"];
                poll.FamilyID = new FamiliesDB().SelectByID(familyId);
            }

            return poll;
        }

        public PollsList SelectAll()
        {
            command.CommandText = "SELECT * FROM PollsTbl";
            List<BaseEntity> baseList = base.Select();
            PollsList pollsList = new PollsList();

            foreach (BaseEntity entity in baseList)
            {
                pollsList.Add(entity as Polls);
            }

            return pollsList;
        }

        public Polls SelectByID(int id)
        {
            command.CommandText = "SELECT * FROM PollsTbl WHERE ID = @ID";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", id);

            List<BaseEntity> baseList = base.Select();
            if (baseList.Count == 1)
                return baseList[0] as Polls;

            return null;
        }
        public Polls SelectByCreator(string memberName)
        {
            command.CommandText = "SELECT * FROM PollsTbl WHERE createdByMember = @createdByMember";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@createdByMember", memberName);

            List<BaseEntity> baseList = base.Select();
            if (baseList.Count == 1)
                return baseList[0] as Polls;

            return null;
        }

        public override void Insert(BaseEntity entity)
        {
            Polls poll = entity as Polls;

            if (poll != null)
            {
                insert.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        public override void Update(BaseEntity entity)
        {
            Polls poll = entity as Polls;

            if (poll != null)
            {
                update.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        public override void Delete(BaseEntity entity)
        {
            Polls poll = entity as Polls;

            if (poll != null)
            {
                delete.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            var poll = (Polls)entity;
            return ParameterizedSql("INSERT INTO [PollsTbl] ([question], [description], [results], [familyID]) VALUES (?, ?, ?, ?)",
                poll.Question, poll.Description, poll.Results, poll.FamilyID.Id);
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            var poll = (Polls)entity;
            return ParameterizedSql("UPDATE [PollsTbl] SET [question] = ?, [description] = ?, [results] = ?, [familyID] = ? WHERE [ID] = ?",
                poll.Question, poll.Description, poll.Results, poll.FamilyID.Id, poll.Id);
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            var poll = (Polls)entity;
            return ParameterizedSql("DELETE FROM [PollsTbl] WHERE [ID] = ?", poll.Id);
        }
    }

}
