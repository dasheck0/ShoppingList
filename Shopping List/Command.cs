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
        private Config config;

        public void setConfig(Config theConfig)
        {
            config = theConfig;
        }

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
            else if (rcmm.text.StartsWith("/price"))
            {
                return proceedPrice(rcmm, lists);
            }
            else if (rcmm.text.StartsWith("/config"))
            {
                return proceedConfig(rcmm);
            }
            else
            {
                return false;
            }
        }

        private bool proceedPrice(RoomChatMessageMessage rcmm, List<AbstractShoppingList> lists)
        {
            String[] splitted = rcmm.text.Split(' ');
            int mode = 0;
            if (splitted[splitted.Length - 1][0].Equals('+'))
            {
                mode = 1;
                splitted[splitted.Length - 1] = splitted[splitted.Length-1].Remove(0, 1);
            }
            else if (splitted[splitted.Length-1][0].Equals('-'))
            {
                mode = 2;
                splitted[splitted.Length-1] = splitted[splitted.Length-1].Remove(0, 1);
            }
            
            //msg(splitted[1], "ich");
            
            Regex regex = new Regex(@"[1-9][0-9]*");
            Match match = regex.Match(splitted[splitted.Length-1]);

            if (match.Success)
            {
                int price = Convert.ToInt32(splitted[splitted.Length - 1]);

                String scrollName = "";

                for (int i = 1; i < splitted.Length-1; i++)
                {
                    scrollName += splitted[i] + " ";
                }

                scrollName = scrollName.Trim();

                if (mode > 0)
                {
                    if (mode == 2)
                    {
                        price *= -1;
                    }

                    if (lists[0].hasScroll(scrollName))
                    {
                        if (lists[0].alterPrice(scrollName, price)) 
                        {
                            msg((mode == 1 ? "Added " : "Removed ") + Math.Abs(price) + "g" + (mode == 1 ? " to " : " from ") + scrollName + " in your buylist", "Shopping List");
                        }
                        else
                        {
                            msg("Couldn't adjust price for " + scrollName + " in your buylist", "Shopping List");
                        }
                    }
                    else if (lists[1].hasScroll(scrollName))
                    {
                        if (lists[1].alterPrice(scrollName, price))
                        {
                            msg((mode == 1 ? "Added " : "Removed ") + Math.Abs(price) + "g" + (mode == 1 ? " to " : " from ") + scrollName + " in your selllist", "Shopping List");
                        }
                        else
                        {
                            msg("Couldn't adjust price for " + scrollName + " in your selllist", "Shopping List");
                        }
                    }
                    else
                    {
                        msg("Couldn't find " + scrollName + " in any of your lists", "Shopping List");
                    }

                }
                else if(mode == 0)
                {
                    if (lists[0].hasScroll(scrollName))
                    {
                        if (lists[0].setPrice(scrollName, price))
                        {
                            msg("Set price to " + splitted[1] + "g for " + scrollName + " in your buylist", "Shopping List");
                        }
                        else
                        {
                            msg("Couldn't set price for " + scrollName + " in your buylist", "Shopping List");
                        }
                    }
                    else if (lists[1].hasScroll(scrollName))
                    {
                        if (lists[1].setPrice(scrollName, price))
                        {
                            msg("Set price to " + splitted[1] + "g for " + scrollName + " in your selllist", "Shopping List");
                        }
                        else
                        {
                            msg("Couldn't set price for " + scrollName + " in your selllist", "Shopping List");
                        }
                    }
                    else
                    {
                        msg("Couldn't find " + scrollName + " in any of your lists", "Shopping List");
                    }
                }

                return true;
            }
            else
            {
                msg("Unknown Command", "Shopping List");
                return true;
            }
        }

        private bool proceedConfig(RoomChatMessageMessage rcmm)
        {
            String[] splitted = rcmm.text.Split(' ');
            bool error = false;

            if (splitted.Length == 3)
            {
                if (splitted[1].Equals("autosave"))
                {
                    config.AutoSave = Convert.ToBoolean(splitted[2]);
                }
                else if (splitted[1].Equals("autoprice"))
                {
                    config.AutoPrice = Convert.ToBoolean(splitted[2]);
                }
                else if (splitted[1].Equals("opener"))
                {
                    config.Opener = splitted[2];
                }
                else if (splitted[1].Equals("endingmessage"))
                {
                    config.EndingMessage = splitted[2];
                }
                else if (splitted[1].Equals("separator"))
                {
                    config.Separator = splitted[2];
                }
                else
                {
                    msg("Unknown Command. Correct Syntax: /config (autoprice|autosave|opener|endingmessage|separator) value", "Shopping List");
                    error = true;
                }
            }
            else
            {
                if (splitted[1].Equals("endingmessage"))
                {
                    String temp = "";

                    for (int i = 2; i < splitted.Length; i++)
                    {
                        temp += splitted[i] + " ";
                    }

                    config.EndingMessage = temp;
                }
                else
                {
                    msg("Unknown Command. Correct Syntax: /config (autoprice|autosave|opener|endingmessage|separator) value", "Shopping List");
                    error = true;
                }
            }

            if (!error)
            {
                //config.saveConfigFile(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "shoppinglist_config.txt");
                //msg("saved", "jkolfd");
            }

            return true;
        }

        private bool proceedList(RoomChatMessageMessage rcmm, AbstractShoppingList list)
        {
            String temp = "\n" + list.getListName() + ":\n";
            List<String> listOfScrolls = list.getList();

            foreach (String scroll in listOfScrolls)
            {
                temp += scroll.ToLower() + " - " + (list.hasPrice(scroll.ToLower()) ? "" + list.getPrice(scroll.ToLower()) : "0")  + "g\n";
            }

            msg(temp, "Shopping List");

            return true;
        }

        private bool proceedBuySell(RoomChatMessageMessage rcmm, AbstractShoppingList list)
        {
            Regex regex = new Regex(@"[0-9]+");
            String[] splitted = rcmm.text.Split(',');

            if (splitted.Length > 1)
            {
                String notAdded = "";

                for (int i = 0; i < splitted.Length; i++)
                {
                    String temp = splitted[i];
                    int price = 0;
                    int start = 0;

                    if (i == 0)
                    {
                        start = 1;
                    }
                    else
                    {
                        start = 0;
                    }

                    String[] whiteSpaceSplitted = temp.Split(' ');
                    temp = "";
                    for (int j = start; j < whiteSpaceSplitted.Length; j++)
                    {
                        if (j == whiteSpaceSplitted.Length - 1)
                        {
                            Match match = regex.Match(whiteSpaceSplitted[j]);
                            if (match.Success)
                            {
                                price = Convert.ToInt32(match.Groups[0].Value);
                                break;
                            }
                        }

                        temp += whiteSpaceSplitted[j] + " ";
                    } 

                    if (!list.addItem(temp.Trim(), price))
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
                    int price = 0;
                    
                    for (int i = 1; i < whiteSpaceSplitted.Length; i++)
                    {
                        if (i == whiteSpaceSplitted.Length - 1)
                        {
                            Match match = regex.Match(whiteSpaceSplitted[i]);
                            if (match.Success)
                            {
                                price = Convert.ToInt32(match.Groups[0].Value);
                                break;
                            }
                        }

                        scrollName += whiteSpaceSplitted[i] + " ";
                    }

                    if (!list.addItem(scrollName.Trim(), price))
                    {
                        msg("Couldn't add all scrolls to " + list.getListName().ToLower() + ". Exception: " + scrollName, "Shopping List");
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
                String message = "WTB" + config.Opener + " ";

                foreach(String scroll in lists[0].getList())
                {
                    message += scroll + (lists[0].hasPrice(scroll) ? " " + lists[0].getPrice(scroll) + "g" : "") + " " + config.Separator + " ";
                }

                message += "\nWTS" + config.Opener + " ";

                foreach(String scroll in lists[1].getList())
                {
                    message += scroll + (lists[1].hasPrice(scroll) ? " " + lists[1].getPrice(scroll) + "g" : "") + " " + config.Separator + " ";
                }

                message += "\n" + config.EndingMessage;
                
                msg(message, App.MyProfile.ProfileInfo.name);
                return true;
            }
            else if(splitted.Length == 2)
            {
                String message = "";
                // /print buy, /print sell
                if(splitted[1].Equals("buy"))
                {
                    message = "WTB" + config.Opener + " ";

                    foreach (String scroll in lists[0].getList())
                    {
                        message += scroll + (lists[0].hasPrice(scroll) ? " " + lists[0].getPrice(scroll) + "g" : "") + " " + config.Separator + " ";
                    }
                }
                else if (splitted[1].Equals("sell"))
                {
                    message = "WTS" + config.Opener + " ";

                    foreach (String scroll in lists[1].getList())
                    {
                        message += scroll + (lists[1].hasPrice(scroll) ? " " + lists[1].getPrice(scroll) + "g" : "") + " " + config.Separator + " ";
                    }
                }
                else
                {
                    msg("Unknown command. Correct syntax: /print buy or /print sell", "Shopping List");
                }

                message += "\n" + config.EndingMessage;
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