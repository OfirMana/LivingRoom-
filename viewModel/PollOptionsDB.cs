using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class PollOptionsDB : BaseDB
    {
        public override BaseEntity newEntity()
        {
            return new PollOptions();
        }

        public override BaseEntity CreateModel(BaseEntity entity)
        {
            PollOptions pollOption = entity as PollOptions;

            if (pollOption != null)
            {
                pollOption.Id = (int)reader["ID"];
                pollOption.PollID = (int)reader["pollID"];
                pollOption.OptionNumber = (int)reader["optionNumber"];
                pollOption.OptionText = reader["optionText"].ToString();
                pollOption.Votes = (int)reader["votes"]; // <-- read Votes
            }

            return pollOption;
        }

        public PollOptionsList SelectAll()
        {
            command.CommandText = "SELECT * FROM PollOptionsTbl";
            List<BaseEntity> baseList = base.Select();
            PollOptionsList pollOptionsList = new PollOptionsList();

            foreach (BaseEntity entity in baseList)
            {
                pollOptionsList.Add(entity as PollOptions);
            }

            return pollOptionsList;
        }

        public PollOptions SelectByID(int id)
        {
            command.CommandText = "SELECT * FROM PollOptionsTbl WHERE ID = @ID";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", id);

            List<BaseEntity> baseList = base.Select();
            if (baseList.Count == 1)
                return baseList[0] as PollOptions;

            return null;
        }

        public override void Insert(BaseEntity entity)
        {
            PollOptions pollOption = entity as PollOptions;

            if (pollOption != null)
            {
                insert.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        public override void Update(BaseEntity entity)
        {
            PollOptions pollOption = entity as PollOptions;

            if (pollOption != null)
            {
                update.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        public override void Delete(BaseEntity entity)
        {
            PollOptions pollOption = entity as PollOptions;

            if (pollOption != null)
            {
                delete.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            PollOptions pollOption = entity as PollOptions;
            string sqlString = string.Format(
                "INSERT INTO PollOptionsTbl (pollID, optionNumber, optionText, votes) " +
                "VALUES ({0}, {1}, '{2}', {3})",
                pollOption.PollID,
                pollOption.OptionNumber,
                pollOption.OptionText.Replace("'", "''"), // escape single quotes
                pollOption.Votes
            );
            // DEBUG:
            System.Diagnostics.Debug.WriteLine("SQL: " + sqlString);
            return sqlString;
        }
        public void IncrementVote(int optionId)
        {
            command.CommandText = "UPDATE PollOptionsTbl SET votes = votes + 1 WHERE ID = @ID";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", optionId);

            command.ExecuteNonQuery();
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            PollOptions pollOption = entity as PollOptions;

            string sqlString = string.Format(
                "UPDATE PollOptionsTbl SET pollID = {0}, optionNumber = {1}, optionText = '{2}', votes = {3} " +
                "WHERE ID = {4}",
                pollOption.PollID,
                pollOption.OptionNumber,
                SqlText(pollOption.OptionText),
                pollOption.Votes,
                pollOption.Id
            );

            return sqlString;
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            PollOptions pollOption = entity as PollOptions;

            string sqlString = $"DELETE FROM PollOptionsTbl WHERE ID = {pollOption.Id}";
            return sqlString;
        }
    }

}
