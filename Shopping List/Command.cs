using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace ShoppingList
{
    class Command
    {
        public virtual bool hooksSend(RoomChatMessageMessage rcmm, List<AbstractShoppingList> lists)
        {            
            if (rcmm.text.StartsWith("/buylist"))
            {
                return proceedList(rcmm, lists[0]);
            }
            else if (rcmm.text.StartsWith("/buy"))
            {
                return proceedBuySell(rcmm, lists[0]);
            }
            else if (rcmm.text.StartsWith("/selllist"))
            {
                return proceedList(rcmm, lists[1]);
            }
            else if (rcmm.text.StartsWith("/sell"))
            {
                return proceedBuySell(rcmm, lists[1]);
            }
            else if(rcmm.text.StartsWith("/export"))
            {
                foreach (AbstractShoppingList list in lists)
                {
                    String filename = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + list.getListName() + ".txt"; 

                    if(!list.exportList(filename))
                    {
                        msg("Couldn't export " + list.getListName());
                    }
                    else
                    {
                        msg("Exported " + list.getListName() + " to " + filename);
                    }
                }

                return true;
            }
            else if (rcmm.text.StartsWith("/import"))
            {
                foreach (AbstractShoppingList list in lists)
                {
                    String filename = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + list.getListName() + ".txt";

                    if (!list.importList(filename))
                    {
                        msg("Couldn't import " + list.getListName());
                    }
                    else
                    {
                        msg("Imported " + list.getListName() + " from " + filename);
                    }
                }

                return true;
            }
            else if (rcmm.text.StartsWith("/!buy"))
            {
                return proceedUnbuyUnsell(rcmm, lists[0]);
            }
            else if (rcmm.text.StartsWith("/!sell"))
            {
                return proceedUnbuyUnsell(rcmm, lists[1]);
            }
            else
            {
                return false;
            }
        }

        private bool proceedList(RoomChatMessageMessage rcmm, AbstractShoppingList list)
        {
            String temp = "\n" + list.getListName() + ":\n";
            List<String> listOfScrolls = list.getList();

            foreach (String scroll in listOfScrolls)
            {
                temp += scroll.ToLower() + "\n";
            }

            msg(temp);

            return true;
        }

        private bool proceedBuySell(RoomChatMessageMessage rcmm, AbstractShoppingList list)
        {
            String[] splitted = rcmm.text.Split(',');
            if (splitted.Length > 1)
            {
                String notAdded = "";

                for (int i = 0; i < splitted.Length; i++)
                {
                    String temp = splitted[i];

                    if (i == 0)
                    {
                        String[] whiteSpaceSplitted = temp.Split(' ');
                        temp = "";
                        for (int j = 1; j < whiteSpaceSplitted.Length; j++)
                        {
                            temp += whiteSpaceSplitted[j] + " ";
                        }
                    }

                    if (!list.addItem(temp.Trim()))
                    {
                        if (!notAdded.Equals(""))
                        {
                            notAdded += ", ";
                        }

                        notAdded += temp;
                    }
                }

                if (!notAdded.Equals(""))
                {
                    msg("Couldn't add all scrolls to " + list.getListName().ToLower() + ". Exception(s): " + notAdded);
                }
                else
                {
                    msg("All scrolls added to " + list.getListName().ToLower());
                }
            }
            else
            {
                String[] whiteSpaceSplitted = splitted[0].Split(' ');
                if (whiteSpaceSplitted.Length > 1)
                {
                    String scrollName = "";

                    for (int i = 1; i < whiteSpaceSplitted.Length; i++)
                    {
                        scrollName += whiteSpaceSplitted[i] + " ";
                    }

                    if (!list.addItem(scrollName.Trim()))
                    {
                        msg("Couldn't add all scrolls to " + list.getListName().ToLower() + ". Exception(s): " + scrollName);
                    }
                    else
                    {
                        msg("All scrolls added to " + list.getListName().ToLower());
                    }
                }
                else
                {
                    msg("Unknown command! Correct syntax: " + whiteSpaceSplitted[0].Trim() + " scroll (, anotherscroll)* ");
                }
            }

            return true;
        }

        private bool proceedUnbuyUnsell(RoomChatMessageMessage rcmm, AbstractShoppingList list)
        {
            String[] splitted = rcmm.text.Split(',');
            if (splitted.Length > 1)
            {
                String notRemoved = "";

                for (int i = 0; i < splitted.Length; i++)
                {
                    String temp = splitted[i];

                    if (i == 0)
                    {
                        String[] whiteSpaceSplitted = temp.Split(' ');
                        temp = "";
                        for (int j = 1; j < whiteSpaceSplitted.Length; j++)
                        {
                            temp += whiteSpaceSplitted[j] + " ";
                        }
                    }

                    if (!list.removeItem(temp.Trim()))
                    {
                        if (!notRemoved.Equals(""))
                        {
                            notRemoved += ", ";
                        }

                        notRemoved += temp;
                    }
                }

                if (!notRemoved.Equals(""))
                {
                    msg("Couldn't remove all desired scrolls from " + list.getListName().ToLower() + ". Exceptions: " + notRemoved);
                }
                else
                {
                    msg("All desired scrolls removed from " + list.getListName().ToLower());
                }
            }
            else
            {
                String[] whiteSpaceSplitted = splitted[0].Split(' ');
                if (whiteSpaceSplitted.Length > 1)
                {
                    if (whiteSpaceSplitted[1].Trim().Equals("*"))
                    {
                        list.clearList();
                        msg("Cleared " + list.getListName().ToLower());
                    }
                    else
                    {
                        String scrollName = "";

                        for (int i = 1; i < whiteSpaceSplitted.Length; i++)
                        {
                            scrollName += whiteSpaceSplitted[i] + " ";
                        }

                        if (!list.removeItem(scrollName.Trim()))
                        {
                            msg("Couldn't remove all desired scrolls from " + list.getListName().ToLower() + ". Exceptions: " + scrollName);
                        }
                        else
                        {
                            msg("All desired scrolls removed from " + list.getListName().ToLower());
                        }
                    }  
                }
                else
                {
                    msg("Unknown command! Correct syntax: " + whiteSpaceSplitted[0] + " scroll (, anotherscroll)* ");
                }
            }

            return true;
        }

        private bool proceedClear(RoomChatMessageMessage rcmm, AbstractShoppingList list)
        {
            //ersetzen durch /!buy *
            return true;
        }

        public virtual bool hooksReceive(RoomChatMessageMessage rcmm, List<AbstractShoppingList> lists)
        { 
   
            return false;
        }

        public virtual string help()
        {
            return "";
        }

        protected void msg(String txt)
        {
            RoomChatMessageMessage rcmm = new RoomChatMessageMessage();
            rcmm.from = "Shopping List";
            rcmm.text = txt;
            rcmm.roomName = App.ArenaChat.ChatRooms.GetCurrentRoom();

            App.ChatUI.handleMessage(rcmm);
            App.ArenaChat.ChatRooms.ChatMessage(rcmm);
        }
    }
}