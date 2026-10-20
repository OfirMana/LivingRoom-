using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class FamiliesDB : BaseDB
    {
        public override BaseEntity newEntity()
        {
            return new Families();
        }

        public override BaseEntity CreateModel(BaseEntity entity)
        {
            Families family = entity as Families;
            if (family != null)
            {
                family.Id = (int)reader["ID"];
                family.FamilyName = reader["familyName"].ToString();
                family.FamPass = reader["famPass"].ToString();

                int cityId = Convert.ToInt32(reader["cityID"]);
                family.CityID = new CitiesDB().SelectById(cityId);

                // ← READ the new column (assumes column name is 'googleCalendarId')
                if (reader["googleCalendarId"] != DBNull.Value)
                    family.GoogleCalendarId = reader["googleCalendarId"].ToString();
            }
            return family;
        }


        public FamiliesList SelectAll()
        {
            command.CommandText = "SELECT * FROM FamiliesTbl";
            List<BaseEntity> baseList = base.Select();
            FamiliesList familiesList = new FamiliesList();

            foreach (BaseEntity entity in baseList)
            {
                familiesList.Add(entity as Families);
            }

            return familiesList;
        }

        public Families SelectByID(int id)
        {
            command.CommandText = "SELECT * FROM FamiliesTbl WHERE ID = @ID";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", id);

            List<BaseEntity> baseList = base.Select();
            if (baseList.Count == 1)
                return baseList[0] as Families;

            return null;
        }

        public override void Insert(BaseEntity entity)
        {
            Families family = entity as Families;

            if (family != null)
            {
                insert.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        public override void Update(BaseEntity entity)
        {
            Families family = entity as Families;

            if (family != null)
            {
                update.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        public override void Delete(BaseEntity entity)
        {
            Families family = entity as Families;

            if (family != null)
            {
                delete.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            var family = (Families)entity;
            return ParameterizedSql("INSERT INTO [FamiliesTbl] ([familyName], [cityID], [famPass], [googleCalendarId]) VALUES (?, ?, ?, ?)",
                family.FamilyName, family.CityID.Id, family.FamPass, family.GoogleCalendarId ?? "");
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            var family = (Families)entity;
            return ParameterizedSql("UPDATE [FamiliesTbl] SET [familyName] = ?, [cityID] = ?, [famPass] = ?, [googleCalendarId] = ? WHERE [ID] = ?",
                family.FamilyName, family.CityID.Id, family.FamPass, family.GoogleCalendarId ?? "", family.Id);
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            var family = (Families)entity;
            return ParameterizedSql("DELETE FROM [FamiliesTbl] WHERE [ID] = ?", family.Id);
        }
    }

}
