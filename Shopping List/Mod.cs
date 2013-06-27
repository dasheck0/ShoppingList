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

		//initialize everything here, Game is loaded at this point
		public Mod ()
		{
            command = new Command();

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

                String keywords = "(";

                foreach(String keyword in lists[0].getList())
                {
                    keywords += keyword + @"|";
                }

                keywords += ")";

                Regex regex = new Regex(keywords, RegexOptions.IgnoreCase);
                message.text = regex.Replace(message.text, "<color=#f1f425>$&</color>");

               // message.text = keywords;

                 /*foreach (String buyItem in lists[0].getList())
                {
                    if (message.text.ToLower().Contains(buyItem.ToLower()))
                    {
                        message.text = message.text.Replace(buyItem, "<color=#ff4343>" + buyItem + "</color>");
                    }
                }

               foreach (String sellItem in lists[1].getList())
                {
                    if (message.text.ToLower().Contains(sellItem.ToLower()))
                    {
                        message.text = message.text.Replace(sellItem, "<color=#ff4343>" + sellItem + "</color>");
                    }
                }*/

                return false;//hooks(false, message);
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
            bool flag = false;
            bool hooksSingle = sending ? command.hooksSend(message, lists) : command.hooksReceive(message, lists);

            return flag | hooksSingle;
        }
	}
}
