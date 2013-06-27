using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingList
{
     class BuyingList : AbstractShoppingList
    {
        public BuyingList()
        {
        }

        public override int getType()
        {
            return 0;
        }

         public override string getListName()
         {
             return "Buying list";
         }
    }
}