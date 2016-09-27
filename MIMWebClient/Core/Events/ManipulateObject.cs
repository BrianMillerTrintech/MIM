﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIMWebClient.Core.Events
{
    using System.Runtime.Remoting.Channels;

    using MIMWebClient.Core.Item;
    using MIMWebClient.Core.PlayerSetup;
    using MIMWebClient.Core.Room;

    public class ManipulateObject
    {
        private static string FindItem { get; } = "item";
        private static string FindInventory { get; } = "inventory";
        private static string Findkillable { get; } = "killable";
        private static string FindAll { get; } = "all";

        /// <summary>
        /// Finds Objects!!
        /// </summary>
        /// <param name="room">the room the player is in</param>
        /// <param name="player">the player calling the method</param>
        /// <param name="command">command used</param>
        /// <param name="thingToFind">Object to find</param>
        /// <param name="objectTypeToFind">can only be item, inventory or all</param>
        /// <returns></returns>
        public static object FindObject(Room room, Player player, string command, string thingToFind, string objectTypeToFind = "")
        {
            /*words to stripout
             * 
             *  from / in / inside / the  ?
            /*
             *  sayto geof good morning
             *  say to geof good morning <- nah
             *  > geof hello
             *  
             *  get 'long sword' / "long sword" / long_sword
             *  get sword -done
             *  get sword chest - done
             *  get 2.dagger -done
             *  
             *  drop potion
             *  give dagger geof
             *  put dagger chest - done
             *  
             *  open north
             *  lock chest
             *  
             * */


            // gets if it;s 2.sword or not and returns the item

            string item = thingToFind;
            var itemContainer = string.Empty;
            //checks for spaces

            //get sword bag - text after the 1st space is the container
            item = FindFirstAndLast.FindFirstAndLastIndex(thingToFind).Key;
            itemContainer = FindFirstAndLast.FindFirstAndLastIndex(thingToFind).Value;

            //get Item
            var findObject = Events.FindNth.Findnth(item);
            int nth = findObject.Key;
            string itemToFind = findObject.Value;


            //get container
            var findContainer = new KeyValuePair<int, string>();
            int nthContainer = 0;
            string comntainerToFind = string.Empty;
            if (!string.IsNullOrEmpty(itemContainer))
            {
                findContainer = Events.FindNth.Findnth(itemContainer);
                nthContainer = findContainer.Key;
                comntainerToFind = findContainer.Value;
            }

            Item foundItem = null;
            Player foundMob = null;
            Player foundPlayer = null;

            List<Item> roomItems = room.items;
            List<Item> playerInv = player.Inventory;
            List<Player> mobList = room.mobs;
            List<Player> playerList = room.players;

            #region find Item searching Room and Player Inventory
            if (objectTypeToFind == FindItem && itemToFind != "all")
            {
                // if its not a container
                if (string.IsNullOrEmpty(itemContainer))
                {
                    //search room items 1st
                    foundItem = (nth == -1) ? roomItems.Find(x => x.name.ToLower().Contains(itemToFind))
                                        : roomItems.FindAll(x => x.name.ToLower().Contains(itemToFind)).Skip(nth - 1).FirstOrDefault();

                    if (foundItem != null) { return new KeyValuePair<Item, Item>(null, foundItem); ; }


                    HubContext.SendToClient("You don't see " + AvsAnLib.AvsAn.Query(itemToFind) + " " + itemToFind + " here and you are not carrying " + AvsAnLib.AvsAn.Query(itemToFind) + " " + itemToFind, player.HubGuid);
                    HubContext.broadcastToRoom(player.Name + " rummages around for an item but finds nothing", room.players, player.HubGuid, true);

                }
                else
                {

                    //look in room
                    var foundContainer = (nthContainer == -1) ? roomItems.Find(x => x.name.ToLower().Contains(comntainerToFind) && x.container == true)
                                          : roomItems.FindAll(x => x.name.ToLower().Contains(comntainerToFind) && x
                      .container == true).Skip(nthContainer - 1).FirstOrDefault();



                    if (foundContainer != null)
                    {
                        //inside found container
                        if (foundContainer.containerItems != null)
                        {
                            foundItem = (nth == -1)
                                            ? foundContainer.containerItems.Find(x => x.name.ToLower().Contains(itemToFind))
                                            : foundContainer.containerItems.FindAll(
                                                x => x.name.ToLower().Contains(itemToFind))
                                                  .Skip(nth - 1)
                                                  .FirstOrDefault();
                        }
                        else
                        {
                            HubContext.SendToClient("You don't see that inside the container", player.HubGuid);
                            HubContext.broadcastToRoom(player.Name + " searches around inside the container but finds nothing", room.players, player.HubGuid, true);

                            return null;
                        }
                    }
                    else
                    {
                        HubContext.SendToClient("You don't see that container here and you are not carrying such an item", player.HubGuid);
                        HubContext.broadcastToRoom(player.Name + " searches for a container but finds nothing", room.players, player.HubGuid, true);
                        return null;
                    }

                    //return item found in container
                    if (foundItem != null || itemToFind.Equals("all", StringComparison.OrdinalIgnoreCase))
                    {
                        return new KeyValuePair<Item, Item>(foundContainer, foundItem);
                    }
                    else
                    {
                        HubContext.SendToClient("You don't see that item inside the container", player.HubGuid);
                        HubContext.broadcastToRoom(player.Name + " searches around inside the container but finds nothing", room.players, player.HubGuid, true);

                    }
                }



            }
            else if (itemToFind == "all" && roomItems.Count == 0 && command == "get") {
                HubContext.SendToClient("There is nothing here to get", player.HubGuid);
            }
            else if (itemToFind == "all" && playerInv.Count == 0 && command == "drop" || itemToFind == "all" && playerInv.Count == 0 && command == "put")
            {
                HubContext.SendToClient("You have nothing to drop", player.HubGuid);
            }

            #endregion
            #region find item in player inventory for commands such as drop, equip, wield etc
            else if (objectTypeToFind == FindInventory)
            {
                Item foundContainer = null;
                if (string.IsNullOrEmpty(itemContainer))
                {

                    foundItem = (nth == -1) ? playerInv.Find(x => x.name.ToLower().Contains(itemToFind))
                                    : playerInv.FindAll(x => x.name.ToLower().Contains(itemToFind)).Skip(nth - 1).FirstOrDefault();

                    if (foundItem != null || itemToFind.Equals("all", StringComparison.OrdinalIgnoreCase))
                    {
                        return new KeyValuePair<Item, Item>(foundContainer, foundItem);
                    }
                    else
                    {
                        HubContext.SendToClient("you are not carrying such an item", player.HubGuid);
                        HubContext.broadcastToRoom(player.Name + " tries to get an item but can't find it.", room.players, player.HubGuid, true);
                    }
                }
                else
                {
                    //look in inv
                    if (itemToFind != "all")
                    {
                        foundItem = (nthContainer == -1) ? playerInv.Find(x => x.name.ToLower().Contains(itemToFind))
                                       : playerInv.FindAll(x => x.name.ToLower().Contains(itemToFind)).Skip(nthContainer - 1).FirstOrDefault();
                    }
                    else
                    {
                        // foundItem = playerInv;
                    }

                    if (foundItem != null || itemContainer != null && itemToFind == "all")
                    {
                        //find container
                        foundContainer = (nth == -1)
                                       ? roomItems.Find(x => x.name.ToLower().Contains(itemContainer))
                                       : roomItems.FindAll(x => x.name.ToLower().Contains(itemContainer))
                                             .Skip(nth - 1)
                                             .FirstOrDefault();
                    }
                    else
                    {
                        HubContext.SendToClient("You are not carrying such an item", player.HubGuid);
                        HubContext.broadcastToRoom(player.Name + " tries to get an item but can't find it.", room.players, player.HubGuid, true);
                    }

                    //return item found in container
                    if (foundItem != null || foundContainer != null)
                    {
                        return new KeyValuePair<Item, Item>(foundContainer, foundItem);
                    }
                    else
                    {
                        HubContext.SendToClient("You don't see that item inside the container", player.HubGuid);
                        HubContext.broadcastToRoom(player.Name + " tries to get an item from a container but can't find it.", room.players, player.HubGuid, true);
                    }
                }
            }
            #endregion
            #region find killable mob or player
            else if (objectTypeToFind == Findkillable)
            {


                //search mob 
                foundMob = (nth == -1) ? mobList.Find(x => x.Name.ToLower().Contains(itemToFind))
                                      : mobList.FindAll(x => x.Name.ToLower().Contains(itemToFind)).Skip(nth - 1).FirstOrDefault();

                if (foundMob != null)
                {
                    return foundMob;
                }
                //search player
                foundPlayer = (nth == -1) ? playerList.Find(x => x.Name.ToLower().Contains(itemToFind))
                                    : playerList.FindAll(x => x.Name.ToLower().Contains(itemToFind)).Skip(nth - 1).FirstOrDefault();

                if (foundPlayer != null)
                {
                    return foundPlayer;
                }
                else
                {
                    HubContext.SendToClient("you don't see " + itemToFind + " here", player.HubGuid);
                    HubContext.broadcastToRoom(player.Name + " tries to kill x but can't find them.", room.players, player.HubGuid, true);
                }


            }
            #endregion
            else if (objectTypeToFind == FindAll)
            {

                if (comntainerToFind == null)
                {


                    //general stuff? sayto command? whsper? cast

                    //search room items 1st
                    foundItem = (nth == -1) ? roomItems.Find(x => x.name.ToLower().Contains(itemToFind))
                                    : roomItems.FindAll(x => x.name.ToLower().Contains(itemToFind)).Skip(nth - 1).FirstOrDefault();

                    if (foundItem != null) { return foundItem; }

                    //search player inventory
                    foundItem = (nth == -1) ? playerInv.Find(x => x.name.ToLower().Contains(itemToFind))
                                    : playerInv.FindAll(x => x.name.ToLower().Contains(itemToFind)).Skip(nth - 1).FirstOrDefault();

                    if (foundItem != null)
                    {
                        return foundItem;
                    }


                    //search mob
                    foundMob = (nth == -1) ? mobList.Find(x => x.Name.ToLower().Contains(itemToFind))
                                   : mobList.FindAll(x => x.Name.ToLower().Contains(itemToFind)).Skip(nth - 1).FirstOrDefault();

                    if (foundMob != null)
                    {
                        return foundMob;
                    }

                    //search players
                    foundPlayer = (nth == -1) ? playerList.Find(x => x.Name.ToLower().Contains(itemToFind))
                                   : playerList.FindAll(x => x.Name.ToLower().Contains(itemToFind)).Skip(nth - 1).FirstOrDefault();

                    if (foundPlayer != null)
                    {
                        return foundPlayer;
                    }
                    else
                    {
                        HubContext.SendToClient("You don't see anything by that name here", player.HubGuid);
                        HubContext.broadcastToRoom(player.Name + " something something...", room.players, player.HubGuid, true);
                    }
                }
                else
                {
                    //look in room
                    var foundContainer = (nthContainer == -1) ? roomItems.Find(x => x.name.ToLower().Contains(comntainerToFind) && x.container == true)
                                          : roomItems.FindAll(x => x.name.ToLower().Contains(comntainerToFind) && x
                      .container == true).Skip(nthContainer - 1).FirstOrDefault();



                    if (foundContainer != null)
                    {
                        //inside found container
                        if (foundContainer.containerItems != null)
                        {
                            

                            var containerItemsCount = foundContainer.containerItems.Count;

                            for (int i = containerItemsCount - 1; i >= 0; i--)
                            {
                                if (foundContainer.containerItems[i].type != Item.ItemType.Gold || foundContainer.containerItems[i].type != Item.ItemType.Silver
                                    || foundContainer.containerItems[i].type != Item.ItemType.Copper)
                                {



                                    foundContainer.containerItems[i].location = Item.ItemLocation.Inventory;
                                    player.Inventory.Add(foundContainer.containerItems[i]);

                                }
                                else
                                {
                                    if (foundContainer.containerItems[i].type == Item.ItemType.Gold)
                                    {
                                        player.Gold += foundContainer.containerItems[i].count;
                                    }

                                    if (foundContainer.containerItems[i].type == Item.ItemType.Silver)
                                    {
                                        player.Silver += foundContainer.containerItems[i].count;
                                    }

                                    if (foundContainer.containerItems[i].type == Item.ItemType.Copper)
                                    {
                                        player.Copper += foundContainer.containerItems[i].count;
                                    }

                                }
                                HubContext.getHubContext.Clients.Client(player.HubGuid).addNewMessageToPage("You pick up a " + foundContainer.containerItems[i].name);
                                HubContext.getHubContext.Clients.AllExcept(player.HubGuid).addNewMessageToPage(player.Name + " picks up a " + foundContainer.containerItems[i].name);
                                foundContainer.containerItems.Remove(foundContainer.containerItems[i]);
                            }
                        }
                        else
                        {
                            HubContext.SendToClient("You don't see that inside the container", player.HubGuid);
                            HubContext.broadcastToRoom(player.Name + " searches around inside the container but finds nothing", room.players, player.HubGuid, true);

                            return null;
                        }
                    }


                    }

            }



            return new KeyValuePair<Item, Item>(null, null);
        }

        /// <summary>
        /// Adds item from room to player inventory
        /// </summary>
        /// <param name="room">Room Object</param>
        /// <param name="player">Player Object</param>
        /// <param name="userInput">Text user entered</param>
        public static void GetItem(Room room, Player player, string userInput, string commandKey, string type)
        {

            //TODO handle container

            var currentRoom = room;
            var currentPlayer = player;
            string[] all = userInput.Split();

            if (all[0].Equals("all", StringComparison.InvariantCultureIgnoreCase))
            {

                var returnedItem = (KeyValuePair<Item, Item>)FindObject(room, player, commandKey, userInput, type);
                var container = returnedItem.Key;
                var item = returnedItem.Value;


                if (container == null)
                {

                    var roomItems = room.items;
                    var roomItemsCount = roomItems.Count;

                    for (int i = roomItemsCount - 1; i >= 0; i--)
                    {
                        if (roomItems[i].type != Item.ItemType.Gold || roomItems[i].type != Item.ItemType.Silver
                            || roomItems[i].type != Item.ItemType.Copper)
                        {



                            roomItems[i].location = Item.ItemLocation.Inventory;
                            player.Inventory.Add(roomItems[i]);

                        }
                        else
                        {
                            if (roomItems[i].type == Item.ItemType.Gold)
                            {
                                player.Gold += roomItems[i].count;
                            }

                            if (roomItems[i].type == Item.ItemType.Silver)
                            {
                                player.Silver += roomItems[i].count;
                            }

                            if (roomItems[i].type == Item.ItemType.Copper)
                            {
                                player.Copper += roomItems[i].count;
                            }

                        }
                        HubContext.getHubContext.Clients.Client(player.HubGuid).addNewMessageToPage("You pick up a " + roomItems[i].name);
                        HubContext.getHubContext.Clients.AllExcept(player.HubGuid).addNewMessageToPage(player.Name + " picks up a " + roomItems[i].name);
                        room.items.Remove(roomItems[i]);
                    }

                    //save to cache
                    Cache.updateRoom(room, currentRoom);
                    Cache.updatePlayer(player, currentPlayer);
                }
                else
                {

                    var containerItems = container.containerItems;
                    var containerCount = containerItems.Count;


                    for (int i = containerCount - 1; i >= 0; i--)
                    {
                        containerItems[i].location = Item.ItemLocation.Inventory;
                        player.Inventory.Add(containerItems[i]);
                        HubContext.getHubContext.Clients.Client(player.HubGuid).addNewMessageToPage("You get a " + containerItems[i].name + " from a " + container.name);
                        HubContext.getHubContext.Clients.AllExcept(player.HubGuid).addNewMessageToPage(player.Name + " get a " + containerItems[i].name + " from a " + container.name);
                        containerItems.Remove(containerItems[i]);
                    }

                    //save to cache
                    Cache.updateRoom(room, currentRoom);
                    Cache.updatePlayer(player, currentPlayer);

                }
            }
            else
            {

                KeyValuePair<Item, Item> returnedItem = (KeyValuePair<Item, Item>)FindObject(room, player, commandKey, userInput, type);

                var container = returnedItem.Key;
                var item = returnedItem.Value;

                if (container == null)
                {
                    if (item == null)
                    {
                        return;
                    }

                    room.items.Remove(item);
                    item.location = Item.ItemLocation.Inventory;
                    player.Inventory.Add(item);


                    //save to cache
                    Cache.updateRoom(room, currentRoom);
                    Cache.updatePlayer(player, currentPlayer);

                    HubContext.getHubContext.Clients.Client(player.HubGuid)
                        .addNewMessageToPage("You pick up a " + item.name);
                    HubContext.getHubContext.Clients.AllExcept(player.HubGuid)
                        .addNewMessageToPage(player.Name + " picks up a " + item.name);
                }
                else
                {
                    if (item == null)
                    {
                        return;
                    }


                    container.containerItems.Remove(item);
                    container.location = Item.ItemLocation.Inventory;
                    player.Inventory.Add(item);


                    //save to cache


                    Cache.updateRoom(room, currentRoom);
                    Cache.updatePlayer(player, currentPlayer);

                    HubContext.getHubContext.Clients.Client(player.HubGuid)
                        .addNewMessageToPage("You get a " + item.name + " from a " + container.name);
                    HubContext.getHubContext.Clients.AllExcept(player.HubGuid)
                        .addNewMessageToPage(player.Name + " gets a " + item.name + " from a " + container.name);
                }

            }

        }

        /// <summary>
        /// Drops item from player inventory
        /// </summary>
        /// <param name="room">room object</param>
        /// <param name="player">player object</param>
        /// <param name="userInput">text entered by user</param>
        /// <param name="commandKey">command entered</param>
        public static void DropItem(Room room, Player player, string userInput, string commandKey)
        {

            var currentRoom = room;
            var currentPlayer = player;
            string[] all = userInput.Split();

            if (all[0].Equals("all", StringComparison.InvariantCultureIgnoreCase))
            {
                var returnedItem = (KeyValuePair<Item, Item>)FindObject(room, player, commandKey, userInput, FindInventory);

                var container = returnedItem.Key;
                var item = returnedItem.Value;
                var playerInv = player.Inventory;

                if (container == null)
                {


                    var playerInvCount = player.Inventory.Count;

                    for (int i = playerInvCount - 1; i >= 0; i--)
                    {
                        playerInv[i].location = Item.ItemLocation.Room;
                        room.items.Add(playerInv[i]);
                        HubContext.getHubContext.Clients.Client(player.HubGuid).addNewMessageToPage("You drop a " + playerInv[i].name);
                        HubContext.getHubContext.Clients.AllExcept(player.HubGuid).addNewMessageToPage(player.Name + " drops a " + playerInv[i].name);
                        player.Inventory.Remove(playerInv[i]);
                    }


                    //save to cache
                    Cache.updateRoom(room, currentRoom);
                    Cache.updatePlayer(player, currentPlayer);

                }
                else
                {

                    var playerInvCount = playerInv.Count;

                    for (int i = playerInvCount - 1; i >= 0; i--)
                    {
                        playerInv[i].location = Item.ItemLocation.Room;
                        container.containerItems.Add(playerInv[i]);
                        HubContext.getHubContext.Clients.Client(player.HubGuid).addNewMessageToPage("You drop a " + playerInv[i].name + " into a " + container.name);
                        HubContext.getHubContext.Clients.AllExcept(player.HubGuid).addNewMessageToPage(player.Name + " drops a " + playerInv[i].name + " into a " + container.name);
                        player.Inventory.Remove(playerInv[i]);
                    }


                    //save to cache
                    Cache.updateRoom(room, currentRoom);
                    Cache.updatePlayer(player, currentPlayer);
                }
            }
            else
            {
                KeyValuePair<Item, Item> returnedItem = (KeyValuePair<Item, Item>)FindObject(room, player, commandKey, userInput, "inventory");

                var container = returnedItem.Key;
                var item = returnedItem.Value;

                if (container == null)
                {

                    if (item == null)
                    {
                        return;
                    }

                    player.Inventory.Remove(item);
                    item.location = Item.ItemLocation.Room;
                    room.items.Add(item);

                    HubContext.getHubContext.Clients.Client(player.HubGuid).addNewMessageToPage("You drop  a " + item.name);
                    HubContext.getHubContext.Clients.AllExcept(player.HubGuid).addNewMessageToPage(player.Name + " drops  a " + item.name);

                    //save to cache
                    Cache.updateRoom(room, currentRoom);
                    Cache.updatePlayer(player, currentPlayer);

                }
                else
                {

                    if (item == null)
                    {
                        return;
                    }

                    player.Inventory.Remove(item);
                    item.location = Item.ItemLocation.Room;
                    container.containerItems.Add(item);

                    HubContext.getHubContext.Clients.Client(player.HubGuid).addNewMessageToPage("You put a " + item.name + " inside the " + container.name);
                    HubContext.getHubContext.Clients.AllExcept(player.HubGuid).addNewMessageToPage(player.Name + " puts a " + item.name + " inside the " + container.name);

                    //save to cache
                    Cache.updateRoom(room, currentRoom);
                    Cache.updatePlayer(player, currentPlayer);
                }




            }
        }
    }
}
