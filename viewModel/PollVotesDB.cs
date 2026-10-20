using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;

namespace viewModel
{
    public class PollVotesDB : BaseDB
    {
        public override BaseEntity newEntity()
        {
            return new PollVotes();
        }

        public override BaseEntity CreateModel(BaseEntity entity)
        {
            PollVotes vote = entity as PollVotes;
            if (vote != null)
            {
                vote.Id = (int)reader["ID"];
                vote.PollID = (int)reader["pollID"];
                vote.OptionID = (int)reader["optionID"];
                vote.MemberID = (int)reader["memberID"];
            }
            return vote;
        }

        public PollVotesList SelectAll()
        {
            command.CommandText = "SELECT * FROM PollVotesTbl";
            List<BaseEntity> baseList = base.Select();
            PollVotesList voteList = new PollVotesList();
            foreach (BaseEntity entity in baseList)
                voteList.Add(entity as PollVotes);
            return voteList;
        }

        public PollVotesList SelectByPoll(int pollId)
        {
            command.CommandText = "SELECT * FROM PollVotesTbl WHERE pollID = @PollID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@PollID", pollId);
            List<BaseEntity> baseList = base.Select();
            return new PollVotesList(baseList);
        }

        public PollVotesList SelectByMember(int memberId)
        {
            command.CommandText = "SELECT * FROM PollVotesTbl WHERE memberID = @MemberID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@MemberID", memberId);
            List<BaseEntity> baseList = base.Select();
            return new PollVotesList(baseList);
        }

        public PollVotes SelectByPollAndMember(int pollId, int memberId)
        {
            command.CommandText = "SELECT * FROM PollVotesTbl WHERE pollID = @PollID AND memberID = @MemberID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@PollID", pollId);
            command.Parameters.AddWithValue("@MemberID", memberId);

            List<BaseEntity> baseList = base.Select();
            if (baseList.Count > 0)
                return baseList[0] as PollVotes;
            return null;
        }

        public override void Insert(BaseEntity entity)
        {
            PollVotes vote = entity as PollVotes;
            if (vote != null)
            {
                insert.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }
        public override void Update(BaseEntity entity)
        {
            PollVotes vote = entity as PollVotes;
            if (vote != null)
            {
                update.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }
        public override void Delete(BaseEntity entity)
        {
            PollVotes vote = entity as PollVotes;
            if (vote != null)
            {
                delete.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            var vote = (PollVotes)entity;
            return ParameterizedSql("INSERT INTO [PollVotesTbl] ([pollID], [optionID], [memberID]) VALUES (?, ?, ?)",
                vote.PollID, vote.OptionID, vote.MemberID);
        }
        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            var vote = (PollVotes)entity;
            return ParameterizedSql("UPDATE [PollVotesTbl] SET [pollID] = ?, [optionID] = ?, [memberID] = ? WHERE [ID] = ?",
                vote.PollID, vote.OptionID, vote.MemberID, vote.Id);
        }
        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            var vote = (PollVotes)entity;
            return ParameterizedSql("DELETE FROM [PollVotesTbl] WHERE [ID] = ?", vote.Id);
        }
    }

}
