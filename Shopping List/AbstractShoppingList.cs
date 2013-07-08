using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ShoppingList
{
    abstract class AbstractShoppingList
    {
        protected List<String> shoppingList;
        protected Dictionary<String, int> priceList;
        //protected int typeOfList; //0 -> buy, 1 -> sell, 2 -> trade 

        public AbstractShoppingList()
        {
            shoppingList = new List<String>();
            priceList = new Dictionary<String,int>();
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
                file.WriteLine(scroll + " " + (hasPrice(scroll) ? "" + getPrice(scroll) : "0"));
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
                    String[] splitted = scroll.Split(' ');
                    String scrollName = "";
                    int price = 0;

                    Regex regex = new Regex(@"[0-9]+");

                    for (int i = 0; i < splitted.Length; i++)
                    {
                        if (!regex.Match(splitted[i]).Success)
                        {
                            scrollName += splitted[i] + " ";
                        }
                        else
                        {                           
                            price = Convert.ToInt32(splitted[i]);                          
                        }
                    }

                    scrollName = scrollName.Remove(scrollName.Length - 1, 1);
                    setPrice(scrollName, price);  
                    shoppingList.Add(scrollName);
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
            priceList.Add(item, -1);
            return true;
        }

        public bool addItem(String item, int price)
        {
            bool flag = addItem(item);
            flag &= setPrice(item, price);

            return flag;
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
            priceList.Remove(item);
            return true;
        }

        public bool hasScroll(String item)
        {
            return shoppingList.Contains(item);
        }

        public bool hasPrice(String item)
        {
            if(!priceList.ContainsKey(item))
            {
                return false;
            }

            return (priceList[item] > 0);
        }

        public int getPrice(String item)
        {
            if (hasPrice(item))
            {
                return priceList[item];
            }

            return 0;            
        }

        public bool setPrice(String item, int price)
        {
            priceList[item] = price;
            return true;
        }

        public bool alterPrice(String item, int value)
        {
            if (priceList.ContainsKey(item))
            {
                priceList[item] += value;
                return true;
            }

            return false;
        }

       /* public bool alterPrice(String item, float percent)
        {
            if (priceList.ContainsKey(item))
            {
                priceList[item] *= percent;
                return true;
            }

            return false;
        }*/

      /*  public void alterPrice(float percent)
        {
            foreach (KeyValuePair<String, int> pair in priceList)
            {
                pair.Value *= percent;
            }
        }*/

        public void clearList()
        {
            shoppingList.Clear();
            priceList.Clear();
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