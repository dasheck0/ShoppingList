using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShoppingList
{
    abstract class AbstractShoppingList
    {
        protected List<String> shoppingList;
        //protected int typeOfList; //0 -> buy, 1 -> sell, 2 -> trade 

        public AbstractShoppingList()
        {
            shoppingList = new List<String>();
        }

        public bool exportList(String filename)
        {
            StreamWriter file;

            try
            {
                file = new System.IO.StreamWriter(filename);
            }
            catch(IOException e)
            {
                //should do some kind of error logging
                return false;
            }            

            file.WriteLine("listtype:" + getType());

            foreach (String scroll in shoppingList)
            {
                file.WriteLine(scroll);
            }

            file.Close();
            return true;
        }

        public bool importList(String filename)
        {
            StreamReader file;

            try
            {
                file = new System.IO.StreamReader(filename);
            }
            catch (IOException e)
            {
                //should do some kind of error logging
                return false;
            }

            String scroll;

            while((scroll = file.ReadLine()) != null)
            {
                if(!scroll.StartsWith("listtype:"))
                {
                    shoppingList.Add(scroll);
                }
            }

            file.Close();
            return true;
        }

        public bool addItem(String item)
        {
            item = item.ToLower();

            if (shoppingList.Contains(item))
            {
                //if scroll already in list then return false
                return false;
            }

            shoppingList.Add(item);
            return true;
        }

        public bool removeItem(String item)
        {
            item = item.ToLower();

            if (!shoppingList.Contains(item))
            {
                //if scroll not in list then return false
                return false;
            }

            shoppingList.Remove(item);
            return true;
        }

        public void clearList()
        {
            shoppingList.Clear();
        }

        public List<String> getList()
        {
            return shoppingList;
        }

        public virtual int getType()
        {
            //-1 -> unknown type
            return -1;
        }

        public virtual String getListName()
        {
            return "unknown List";
        }
    }
}