﻿using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIMWebClient.Core.Player;

namespace MIMWebClient.Core
{
    using System.Security.Cryptography.X509Certificates;

    using MIMWebClient.Core.Events;
    using MIMWebClient.Core.Player;
    using MIMWebClient.Hubs;
    public static class HubContext
    {
        private static IHubContext _getHubContext;
        /// <summary>
        /// gets the SignalR Hub connection
        /// </summary>
        public static IHubContext getHubContext
        {
            get
            {
                if (_getHubContext == null)
                {
                    _getHubContext = GlobalHost.ConnectionManager.GetHubContext<MIMHub>();
                }

                return _getHubContext;
            }
        }

        /// <summary>
        /// Send a message to connected clients
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="player">The active player</param>
        /// <param name="sendToAll">toggle to send to all or to caller</param>
        /// <param name="excludeCaller">toggle to send to all but exclude caller</param>
        public static void SendToClient(string message, string playerId, string recipientId = null, bool sendToAll = false, bool excludeCaller = false, bool excludeRecipient = false)
        {
            if (message != null)
            {

                //Send a message to all users
                if (sendToAll && excludeCaller == false)
                {
                    HubContext.getHubContext.Clients.All.addNewMessageToPage(message);
                }

                if (playerId != null)
                {
                    //send a message to all but caller
                    // x hits you - Fight message is being sent to all instead of person who's being hit
                    if (excludeCaller && sendToAll == false && excludeRecipient == false)
                    {
                        HubContext.getHubContext.Clients.AllExcept(playerId).addNewMessageToPage(message);
                    }

                    //send a message to all but recipient
                    if (excludeRecipient && recipientId != null)
                    {
                        HubContext.getHubContext.Clients.Client(recipientId).addNewMessageToPage(message);
                    }

                    //send only to caller
                    if (sendToAll == false && excludeCaller == false)
                    {
                        HubContext.getHubContext.Clients.Client(playerId).addNewMessageToPage(message);
                    }


                }
            }
        }

        public static void SendToAllExcept(string message, List<string> excludeThesePlayers, List<PlayerSetup.Player> players)
        {
            if (message != null)
            {
                int playerCount = players.Count;

                for (int i = 0; i < playerCount; i++)
                {
                    if (players[i].HubGuid != excludeThesePlayers.FirstOrDefault(x => x.Equals(players[i].HubGuid)))
                    {
                        HubContext.getHubContext.Clients.Client(players[i].HubGuid).addNewMessageToPage(message);
                    }
                    else
                    {
                        var playerInFight = players.FindAll(x => x.Status.Equals(PlayerSetup.Player.PlayerStatus.Fighting));

                        foreach (var fighter in playerInFight)
                        {
                            if (players[i].Status != PlayerSetup.Player.PlayerStatus.Fighting)
                            { 
                                HubContext.getHubContext.Clients.Client(players[i].HubGuid).addNewMessageToPage(message);
                            }
                        }
                    }

                }



            }
        }

        public static void broadcastToRoom(string message, List<PlayerSetup.Player> players, string playerId, bool excludeCaller = false)
        {
            int playerCount = players.Count;

            if (excludeCaller)
            {
                for (int i = 0; i < playerCount; i++)
                {
                    if (playerId != players[i].HubGuid)
                    {
                        HubContext.getHubContext.Clients.Client(players[i].HubGuid).addNewMessageToPage(message);
                    }
                }
            }
            else
            {
                for (int i = 0; i < playerCount; i++)
                {
                    HubContext.getHubContext.Clients.Client(players[i].HubGuid).addNewMessageToPage(message);
                }
            }


        }

        public static void Quit(string playerId, Room.Room room)
        {


            //remove player from room and player cache

            var oldRoom = room;

            int playerIndex = room.players.FindIndex(x => x.HubGuid == playerId);
            room.players.RemoveAt(playerIndex);

            Cache.updateRoom(room, oldRoom);

            PlayerSetup.Player playerData = null;
           MIMHub._PlayerCache.TryRemove(playerId, out playerData);

            if (playerData != null)
            {
                SendToClient("See you soon!", playerId);
                broadcastToRoom(playerData.Name + " has left the realm", room.players, playerId, true);

                Save.UpdatePlayer(playerData);

                HubContext.getHubContext.Clients.Client(playerId).quit();
            }






        }
    }
}
