 using System;

using ScrollsModLoader.Interfaces;
using UnityEngine;
using Mono.Cecil;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ShoppingList
{
	public class Mod : BaseMod
	{
        private Command command;
        private List<AbstractShoppingList> lists;
        private Config config;

		//initialize everything here, Game is loaded at this point
		public Mod ()
		{
            command = new Command();
            config = new Config();
            //config.loadConfigFile(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "shoppinglist_config.txt");

            command.setConfig(config);

            lists = new List<AbstractShoppingList>();
            lists.Add(new BuyingList());
            lists.Add(new SellingList());
		}


		public static string GetName ()
		{
			return "ShoppingList";
		}

		public static int GetVersion ()
		{
			return 1;
		}

		//only return MethodDefinitions you obtained through the scrollsTypes object
		//safety first! surround with try/catch and return an empty array in case it fails
		public static MethodDefinition[] GetHooks (TypeDefinitionCollection scrollsTypes, int version)
		{
            try
            {
                return new MethodDefinition[] {
                    scrollsTypes["ChatRooms"].Methods.GetMethod("ChatMessage", new Type[]{typeof(RoomChatMessageMessage)}),
                    scrollsTypes["Communicator"].Methods.GetMethod("sendRequest", new Type[]{typeof(Message)})
                };
            }
            catch
            {
                return new MethodDefinition[] { };
            }
		}

		
		public override bool BeforeInvoke (InvocationInfo info, out object returnValue)
		{
			returnValue = null;

            if(info.targetMethod.Equals("ChatMessage"))
            {
                RoomChatMessageMessage message = (RoomChatMessageMessage)info.arguments[0];

                //parse buylist
                String keywords = "(";
                
                if (lists[0].getList().Count > 0)
                {                   
                    foreach(String keyword in lists[0].getList())
                    {
                        keywords += keyword + @"|";
                    }
                }

                if(lists[0].getList().Count > 0)
                {
                    foreach (String keyword in lists[1].getList())
                    {
                        keywords += keyword + @"|";
                    }
                }

                keywords = keywords.Remove(keywords.Length-1,1);                  
                keywords += ")";

                Regex regex = new Regex(keywords, RegexOptions.IgnoreCase);
                Regex wtbRegex = new Regex(@"(buy|buying|wtb)", RegexOptions.IgnoreCase);
                Regex wtsRegex = new Regex(@"(sell|selling|wts)", RegexOptions.IgnoreCase);

                Match wtbMatch = wtbRegex.Match(message.text);
                Match wtsMatch = wtsRegex.Match(message.text);

                int wtbPosition = -1;
                int wtsPosition = -1;

                if (wtbMatch.Success)
                {
                    wtbPosition = wtbMatch.Index;
                }

                if (wtsMatch.Success)
                {
                    wtsPosition = wtsMatch.Index;
                }

                bool wtbFirst = (wtbPosition < wtsPosition);

                MatchCollection matches = regex.Matches(message.text);
                List<String> buying = new List<String>();
                List<String> selling = new List<String>();

                for (int i = 0; i < matches.Count; i++)
                {
                    if (matches[i].Index > wtbPosition &&
                        matches[i].Index < wtsPosition &&
                        wtbFirst)
                    {
                        buying.Add(matches[i].Value);
                    }
                    else if (matches[i].Index > wtbPosition &&
                            matches[i].Index > wtsPosition &&
                            wtbFirst)
                    {
                        selling.Add(matches[i].Value);
                    }
                    else if (matches[i].Index > wtbPosition &&
                             matches[i].Index > wtsPosition &&
                             !wtbFirst)
                    {
                        buying.Add(matches[i].Value);
                    }
                    else if (matches[i].Index < wtbPosition &&
                             matches[i].Index > wtsPosition &&
                             !wtbFirst)
                    {
                        selling.Add(matches[i].Value);
                    }
                }

                foreach (String scroll in buying)
                {
                    foreach (String item in lists[1].getList())
                    {
                        if (scroll.Equals(item))
                        {
                            message.text = message.text.Replace(scroll, "<color=#ffcc00>" + scroll + "</color>");
                        }
                    }                    
                }

                foreach (String scroll in selling)
                {
                    foreach (String item in lists[0].getList())
                    {
                        if (scroll.Equals(item))
                        {
                            message.text = message.text.Replace(scroll, "<color=#df3a01>" + scroll + "</color>");
                        }
                    } 
                }         
               
                return false;
            }
            else if(info.targetMethod.Equals("sendRequest"))
            {
                if(info.arguments[0] is RoomChatMessageMessage)
                {
                    RoomChatMessageMessage message = (RoomChatMessageMessage)info.arguments[0];

                    return hooks(true, message);
                }
            }

			return false;
		}

		public override void AfterInvoke (InvocationInfo info, ref object returnValue)
		{
			return;
		}

        private bool hooks(bool sending, RoomChatMessageMessage message)
        {
            return command.hooksSend(message, lists);
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

