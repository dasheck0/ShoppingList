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
                        msg("Couldn't export " + list.getListName(), "Shopping List");
                    }
                    else
                    {
                        msg("Exported " + list.getListName() + " to " + filename, "Shopping List");
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
                        msg("Couldn't import " + list.getListName(), "Shopping List");
                    }
                    else
                    {
                        msg("Imported " + list.getListName() + " from " + filename, "Shopping List");
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
            else if (rcmm.text.StartsWith("/print"))
            {
                return proceedPrint(rcmm, lists);
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

            msg(temp, "Shopping List");

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
                    msg("Couldn't add all scrolls to " + list.getListName().ToLower() + ". Exception(s): " + notAdded, "Shopping List");
                }
                else
                {
                    msg("All scrolls added to " + list.getListName().ToLower(), "Shopping List");
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
                        msg("Couldn't add all scrolls to " + list.getListName().ToLower() + ". Exception(s): " + scrollName, "Shopping List");
                    }
                    else
                    {
                        msg("All scrolls added to " + list.getListName().ToLower(), "Shopping List");
                    }
                }
                else
                {
                    msg("Unknown command! Correct syntax: " + whiteSpaceSplitted[0].Trim() + " scroll (, anotherscroll)* ", "Shopping List");
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
                    msg("Couldn't remove all desired scrolls from " + list.getListName().ToLower() + ". Exceptions: " + notRemoved, "Shopping List");
                }
                else
                {
                    msg("All desired scrolls removed from " + list.getListName().ToLower(), "Shopping List");
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
                        msg("Cleared " + list.getListName().ToLower(), "Shopping List");
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
                            msg("Couldn't remove all desired scrolls from " + list.getListName().ToLower() + ". Exceptions: " + scrollName, "Shopping List");
                        }
                        else
                        {
                            msg("All desired scrolls removed from " + list.getListName().ToLower(), "Shopping List");
                        }
                    }  
                }
                else
                {
                    msg("Unknown command! Correct syntax: " + whiteSpaceSplitted[0] + " scroll (, anotherscroll)* ", "Shopping List");
                }
            }

            return true;
        }

        private bool proceedPrint(RoomChatMessageMessage rcmm, List<AbstractShoppingList> lists)
        {
            String[] splitted = rcmm.text.Split(' ');
            if(splitted.Length == 1)
            {
                // /print
                String message = "WTB: ";

                foreach(String scroll in lists[0].getList())
                {
                    message += scroll + ", ";
                }

                message += "\nWTS: ";

                foreach(String scroll in lists[1].getList())
                {
                    message += scroll + ", ";
                }
                
                msg(message, App.MyProfile.ProfileInfo.name);
                return true;
            }
            else if(splitted.Length == 2)
            {
                String message = "";
                // /print buy, /print sell
                if(splitted[1].Equals("buy"))
                {
                    message = "WTB: ";

                    foreach (String scroll in lists[0].getList())
                    {
                        message += scroll + ", ";
                    }
                }
                else if (splitted[1].Equals("sell"))
                {
                    message = "WTS: ";

                    foreach (String scroll in lists[1].getList())
                    {
                        message += scroll + ", ";
                    }
                }
                else
                {
                    msg("Unknown command. Correct syntax: /print buy or /print sell", "Shopping List");
                }

                msg(message, App.MyProfile.ProfileInfo.name);
                return true;
            }

            return false;
        }

        protected void msg(String txt, String from)
        {
            RoomChatMessageMessage rcmm = new RoomChatMessageMessage();
            rcmm.from = from;
            rcmm.text = txt;
            rcmm.roomName = App.ArenaChat.ChatRooms.GetCurrentRoom();

            App.ChatUI.handleMessage(rcmm);
            App.ArenaChat.ChatRooms.ChatMessage(rcmm);
        }
    }
}