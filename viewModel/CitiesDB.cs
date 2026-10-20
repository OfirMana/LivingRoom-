using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class CitiesDB : BaseDB
    {
        static private CitiesList list = null;
        public CitiesList SelectAll()
        {
            command.CommandText = "SELECT * FROM CitiesTbl";
            list = new CitiesList(Select());
            return list;

        }
        public Cities SelectById(int id)
        {
            if (list == null)
            {
                CitiesDB dB = new CitiesDB();
                list = dB.SelectAll();
            }
            Cities city = list.Find(item => item.Id == id);
            return city;
        }
        public override BaseEntity CreateModel(BaseEntity entity)
        {
            Cities city = entity as Cities;
            if (city != null)
            {
                city.Id = (int)reader["ID"];
                city.CityName = reader["cityName"].ToString();
            }
            return city;
        }
        public override BaseEntity newEntity()
        {
            return new Cities();
        }

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            Cities city = entity as Cities;

            string sqlString = string.Format(
                "INSERT INTO CitiesTbl (cityName) " +
                "VALUES ('{0}')",
                city.CityName
            );

            return sqlString;
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            throw new NotImplementedException();
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            throw new NotImplementedException();
        }

    }
}
