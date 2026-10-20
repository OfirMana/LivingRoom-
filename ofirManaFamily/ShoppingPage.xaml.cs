using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Model;
using ofirManaFamily.ServiceReference1;

namespace ofirManaFamily
{
    public partial class ShoppingPage : Page
    {
        private readonly Members currentUser;
        private readonly bool isManager;
        private readonly Service1Client client = new Service1Client();
        private readonly List<Shopping> shoppingItems = new List<Shopping>();

        public ShoppingPage()
        {
            InitializeComponent();
            DataContext = this;

            currentUser = App.Current.Properties["CurrentUser"] as Members
                          ?? throw new InvalidOperationException("No user.");
            isManager = currentUser.IsManager;

            LoadItems();
        }

        public bool IsManager { get { return isManager; } }

        /*──────────  load  ─────────*/
        private void LoadItems()
        {
            shoppingItems.Clear();

            foreach (Shopping s in client.SelectAllShopping())
                if (s.FamilyID.Id == currentUser.Family.Id)
                    shoppingItems.Add(s);

            ShoppingItemsControl.ItemsSource = null;
            ShoppingItemsControl.ItemsSource = shoppingItems;
        }

        /*──────────  add  ─────────*/
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            int qty;
            if (string.IsNullOrWhiteSpace(ItemNameTextBox.Text) ||
                !int.TryParse(QuantityTextBox.Text, out qty))
            {
                MessageBox.Show("Please enter valid name and quantity.");
                return;
            }

            Shopping item = new Shopping
            {
                ItemName = ItemNameTextBox.Text.Trim(),
                Quantity = qty,
                Purchased = false,
                AddedByMember = currentUser.Id,
                FamilyID = currentUser.Family
            };

            client.InsertShopping(item);
            ItemNameTextBox.Clear();
            QuantityTextBox.Clear();
            LoadItems();
        }

        /*──────────  toggle purchased ✔ ─────────*/
        private void Check_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null) return;

            Shopping s = cb.Tag as Shopping;
            if (s == null) return;

            s.Purchased = cb.IsChecked == true;
            client.UpdateShopping(s);   // make sure method exists
            LoadItems();
        }

        /*──────────  delete 🗑 ─────────*/
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!isManager) return;     // button hidden anyway, extra safety

            Button btn = sender as Button;
            if (btn == null) return;

            Shopping s = btn.Tag as Shopping;
            if (s == null) return;

            client.DeleteShopping(s);   // immediate, no confirmation
            LoadItems();
        }

        /*──────────  info ❓ ─────────*/
        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            string msg =
                "✨  רשימת קניות – איך משתמשים?\n\n" +
                "• הוספת פריט: שם + כמות → ‎Add‎.\n" +
                "• ✔ מסמן שנקנה (קו-חוצה).\n" +
                "• 🗑 מוחק פריט – רק למנהל.\n\n" +
                "קנייה נעימה!";
            MessageBox.Show(msg, "מידע על רשימת הקניות",
                            MessageBoxButton.OK, MessageBoxImage.Information,
                            MessageBoxResult.OK,
                            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }
    }
}