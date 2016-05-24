﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIMEngine.Core
{
    using System.Collections.Concurrent;

    using MIMEngine.Core.Events;
    using MIMEngine.Core.Player;
    using MIMEngine.Core.Room;

    using Newtonsoft.Json.Linq;

    public static class Command
    {
    //public static Dictionary<string, Action> commandList { get; set; }
        public static Dictionary<string, Action> Commands(string commandOptions, string commandKey, PlayerSetup.Player playerData, Room.Room room)
        {
            var commandList = new Dictionary<String, Action>(); 
            commandList.Add("north", () => Movement.Move(playerData, room, "North"));
            commandList.Add("south", () => Movement.Move(playerData, room, "South"));
            commandList.Add("east", () => Movement.Move(playerData, room, "East"));
            commandList.Add("west", () => Movement.Move(playerData, room, "West"));
            commandList.Add("down", () => Movement.Move(playerData, room, "Down"));
            commandList.Add("up", () => Movement.Move(playerData, room, "Up"));
            commandList.Add("look", () => LoadRoom.ReturnRoom(playerData, room, commandOptions, "look"));
            commandList.Add("examine", () => LoadRoom.ReturnRoom(playerData, room, commandOptions, "examine"));
            commandList.Add("touch", () => LoadRoom.ReturnRoom(playerData, room, commandOptions, "touch"));
            commandList.Add("smell", () => LoadRoom.ReturnRoom(playerData, room, commandOptions, "smell"));
            commandList.Add("taste", () => LoadRoom.ReturnRoom(playerData, room, commandOptions, "taste"));
            commandList.Add("score", () => Score.ReturnScore(playerData));
            commandList.Add("inventory", () => Inventory.ReturnInventory(playerData.Inventory, playerData));
            commandList.Add("get", () => ManipulateObject.GetItem(room, playerData, commandOptions, commandKey, "item"));
            commandList.Add("save", () =>  Save.UpdatePlayer(playerData));
            commandList.Add("'", () => Communicate.Say(commandOptions, playerData, false));
            commandList.Add("say", ()=> Communicate.Say(commandOptions, playerData, false));
            commandList.Add("sayto", () => Communicate.SayTo(commandOptions, room, playerData));
            commandList.Add(">", () => Communicate.Say(commandOptions, playerData, true));

            return commandList;
        }

 

        public static void ParseCommand(string input, PlayerSetup.Player playerData, Room.Room room = null)
        {

            //testing
            string enteredCommand = input;
            string[] commands = enteredCommand.Split(' ');
            string commandKey = commands[0];
            string commandOptions = string.Empty;
            // testing
 
            if (commands.Length >= 2)
            {
                commandOptions = enteredCommand.Substring(enteredCommand.IndexOf(' ', 1)).Trim();
            }
 
             //TODO: do this only once
            var command = Commands(commandOptions, commandKey, playerData, room);
 
            var fire = command.FirstOrDefault(x => x.Key.StartsWith(commandKey));

            if (fire.Value != null)
            {
                fire.Value();
            }
            else
            { 

                HubProxy.MimHubServer.Invoke("SendToClient", "Sorry you can't do that.");
            }
           
        }
    }
}