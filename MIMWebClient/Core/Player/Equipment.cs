﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIMWebClient.Core.Player
{
    using Events;
    using MIMWebClient.Core.PlayerSetup;
    public class Equipment
    {
        public string Floating { get; set; } = "Nothing";
        public string Head { get; set; } = "Nothing";
        public string Face { get; set; } = "Nothing";
        public string Eyes { get; set; } = "Nothing";
        public string LeftEar { get; set; } = "Nothing";
        public string RightEar { get; set; } = "Nothing";
        public string Neck { get; set; } = "Nothing";
        public string Neck2 { get; set; } = "Nothing";
        public string Cloak { get; set; } = "Nothing";
        public string AboutBody { get; set; } = "Nothing";
        public string Body { get; set; } = "Nothing";
        public string Waist { get; set; } = "Nothing";
        public string LeftSheath { get; set; } = "Nothing";
        public string RightSheath { get; set; } = "Nothing";
        public string BackSheath { get; set; } = "Nothing";
        public string Back { get; set; } = "Nothing";
        public string LeftWrist { get; set; } = "Nothing";
        public string RightWrist { get; set; } = "Nothing";
        public string LeftHand { get; set; } = "Nothing";
        public string RightHand { get; set; } = "Nothing";
        public string LeftRing { get; set; } = "Nothing";
        public string RightRing { get; set; } = "Nothing";
        public string Legs { get; set; } = "Nothing";
        public string Feet { get; set; } = "Nothing";

        /// <summary>
        /// Displays what the player is wearing and what slots are availaible
        /// </summary>
        /// <param name="player">The player</param>
        public static void ShowEquipment(Player player)
        {
            var eq = player.Equipment;
            var displayEquipment = new StringBuilder();

            HubContext.SendToClient("You are wearing:", player.HubGuid);

            displayEquipment.Append("<ul>")
               .Append("<li>Floating: ".PadRight(4)).Append(eq.Floating).Append("</li>")
               .Append("<li>Head: ".PadRight(8)).Append(eq.Head).Append("</li>")
               .Append("<li>Face: ".PadRight(8)).Append(eq.Face).Append("</li>")
               .Append("<li>Eyes: ".PadRight(8)).Append(eq.Eyes).Append("</li>")
               .Append("<li>Left Ear: ".PadRight(4)).Append(eq.LeftEar).Append("</li>")
               .Append("<li>Right Ear: ".PadRight(4)).Append(eq.RightEar).Append("</li>")
               .Append("<li>Neck: ".PadRight(8)).Append(eq.Neck).Append("</li>")
               .Append("<li>Neck: ".PadRight(8)).Append(eq.Neck2).Append("</li>")
               .Append("<li>Cloak: ".PadRight(8)).Append(eq.Cloak).Append("</li>")
               .Append("<li>About Body: ".PadRight(4)).Append(eq.AboutBody).Append("</li>")
               .Append("<li>Body: ".PadRight(8)).Append(eq.Body).Append("</li>")
               .Append("<li>Waist: ".PadRight(7)).Append(eq.Waist).Append("</li>");

            if (eq.LeftSheath != "Nothing")
            {
                displayEquipment.Append("<li>Left Sheath: ").Append(eq.LeftSheath).Append("</li>");
            }

            if (eq.RightSheath != "Nothing")
            {
                displayEquipment.Append("<li>Right Sheath: ").Append(eq.RightSheath).Append("</li>");
            }

            if (eq.BackSheath != "Nothing")
            {
                displayEquipment.Append("<li>Back Sheath: ").Append(eq.BackSheath).Append("</li>");
            }

            displayEquipment.Append("<li>Back: ".PadRight(8)).Append(eq.Back).Append("</li>")
                .Append("<li>Left Wrist: ".PadRight(4)).Append(eq.LeftWrist).Append("</li>")
                .Append("<li>Right Wrist: ".PadRight(4)).Append(eq.RightWrist).Append("</li>")
                .Append("<li>Left Hand: ".PadRight(4)).Append(eq.LeftHand).Append("</li>")
                .Append("<li>Right Hand: ".PadRight(4)).Append(eq.RightHand).Append("</li>")
                .Append("<li>Left Ring: ".PadRight(4)).Append(eq.LeftRing).Append("</li>")
                .Append("<li>Right Ring: ".PadRight(4)).Append(eq.RightRing).Append("</li>")
                .Append("<li>Legs: ".PadRight(8)).Append(eq.Legs).Append("</li>")
                .Append("<li>Feet: ".PadRight(8)).Append(eq.Feet).Append("</li>")
                .Append("</ul>");

            HubContext.SendToClient(displayEquipment.ToString(), player.HubGuid);
        }

        /// <summary>
        /// Wears an item
        /// </summary>
        /// <param name="player">The Player</param>
        /// <param name="itemToWear">Item to wear</param>
        public static void WearItem(Player player, string itemToWear, bool wield = false)
        {
            var oldPlayer = player;
            var foundItem = player.Inventory.Find(i => i.name.Contains(itemToWear));

            if (foundItem == null)
            {
                if (wield)
                {
                    HubContext.SendToClient("You do not have that item to wield.", player.HubGuid);
                    return;
                }

                HubContext.SendToClient("You do not have that item to wear.", player.HubGuid);
                return;
            }

            foundItem.location = "worn";
            var slot = foundItem.slot;

            var eqLocation = player.Equipment.GetType().GetProperty(slot);

            if (eqLocation == null){ return; }  // Log error?

            var hasValue = eqLocation.GetValue(player.Equipment);

            if (hasValue.ToString() != "Nothing")
            {
                RemoveItem(player, hasValue.ToString(), true);
            }

            eqLocation.SetValue(player.Equipment, foundItem.name);

            if (!wield)
            {
                HubContext.SendToClient("You wear." + foundItem.name, player.HubGuid);
            }
            else
            {
                HubContext.SendToClient("You wield." + foundItem.name, player.HubGuid);
            }

            Cache.updatePlayer(player, oldPlayer);

        }

        /// <summary>
        /// Remove worn item.
        /// </summary>
        /// <param name="player">The Player</param>
        /// <param name="itemToRemove">Item to Remove</param>
        public static void RemoveItem(Player player, string itemToRemove, bool replaceWithOtherEQ = false, bool unwield = false)
        {
            var oldPlayer = player;
            var foundItem = player.Inventory.Find(i => i.name.Contains(itemToRemove) && i.location.Equals("worn"));

            if (foundItem == null)
            {
                if (unwield)
                {
                    HubContext.SendToClient("You do not have that item to unwield.", player.HubGuid);
                    return;
                }

                HubContext.SendToClient("You are not wearing that item.", player.HubGuid);
                return;
            }

            foundItem.location = "inventory";
            var slot = foundItem.slot;

            var eqLocation = player.Equipment.GetType().GetProperty(slot);

            if (eqLocation == null) { return; }  // Log error?

          
             eqLocation.SetValue(player.Equipment, "Nothing");

            if (!unwield)
            {
                HubContext.SendToClient("You Remove." + foundItem.name, player.HubGuid);
            }
            else
            {
                HubContext.SendToClient("You Unwield." + foundItem.name, player.HubGuid);
            }
             

            if (replaceWithOtherEQ)
            {
                return; // we don't need to update the cache
            }
       

            Cache.updatePlayer(player, oldPlayer);

        }


    }
}
