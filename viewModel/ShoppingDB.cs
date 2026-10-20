using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class ShoppingDB : BaseDB
    {
        public override BaseEntity newEntity()
        {
            return new Shopping();
        }

        public override BaseEntity CreateModel(BaseEntity entity)
        {
            Shopping shopping = entity as Shopping;

            if (shopping != null)
            {
                shopping.Id = (int)reader["ID"];
                shopping.ItemName = reader["itemName"].ToString();
                shopping.Quantity = (int)reader["quantity"];
                shopping.Purchased = (bool)reader["purchased"];

                int familyId = (int)reader["familyID"];
                shopping.FamilyID = new FamiliesDB().SelectByID(familyId);
            }

            return shopping;
        }

        public ShoppingList SelectAll()
        {
            command.CommandText = "SELECT * FROM ShoppingTbl";
            List<BaseEntity> baseList = base.Select();
            ShoppingList shoppingList = new ShoppingList();

            foreach (BaseEntity entity in baseList)
            {
                shoppingList.Add(entity as Shopping);
            }

            return shoppingList;
        }

        public Shopping SelectByID(int id)
        {
            command.CommandText = "SELECT * FROM ShoppingTbl WHERE ID = @ID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", id);

            List<BaseEntity> baseList = base.Select();
            if (baseList.Count == 1)
                return baseList[0] as Shopping;

            return null;
        }

        public ShoppingList SelectByNotPurchased()
        {
            command.CommandText = "SELECT * FROM ShoppingTbl WHERE purchased = 0";
            command.Parameters.Clear();

            List<BaseEntity> baseList = base.Select();
            ShoppingList result = new ShoppingList();

            foreach (BaseEntity entity in baseList)
            {
                result.Add(entity as Shopping);
            }

            return result;
        }

        public override void Insert(BaseEntity entity)
        {
            Shopping shopping = entity as Shopping;
            if (shopping != null)
            {
                insert.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        public override void Update(BaseEntity entity)
        {
            Shopping shopping = entity as Shopping;
            if (shopping != null)
            {
                update.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        public override void Delete(BaseEntity entity)
        {
            Shopping shopping = entity as Shopping;
            if (shopping != null)
            {
                delete.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            var item = (Shopping)entity;
            return ParameterizedSql("INSERT INTO [ShoppingTbl] ([itemName], [quantity], [purchased], [familyID]) VALUES (?, ?, ?, ?)",
                item.ItemName, item.Quantity, item.Purchased, item.FamilyID.Id);
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            var item = (Shopping)entity;
            return ParameterizedSql("UPDATE [ShoppingTbl] SET [itemName] = ?, [quantity] = ?, [purchased] = ?, [familyID] = ? WHERE [ID] = ?",
                item.ItemName, item.Quantity, item.Purchased, item.FamilyID.Id, item.Id);
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            var item = (Shopping)entity;
            return ParameterizedSql("DELETE FROM [ShoppingTbl] WHERE [ID] = ?", item.Id);
        }
    }


}
