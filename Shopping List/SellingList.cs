using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingList
{
    class SellingList : AbstractShoppingList
    {
        public SellingList()
        {
        }

        public override int getType()
        {
            return 1;
        }

        public override string getListName()
        {
            return "Selling list";
        }
    }
}