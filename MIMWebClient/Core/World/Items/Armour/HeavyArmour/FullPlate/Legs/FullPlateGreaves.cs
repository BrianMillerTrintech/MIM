﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Components.DictionaryAdapter;
using MIMWebClient.Core.Item;

namespace MIMWebClient.Core.World.Items.Armour.HeavyArmour.FullPlate.Legs
{
    public class FullPlateGreaves
    {
        public static Item.Item SteelGreevesOfTyr()
        {

            var SteelGreevesOfTyr = new Item.Item
            {
                armourType = Item.Item.ArmourType.PlateMail,
                eqSlot = Item.Item.EqSlot.Legs,
                description = new Description()
                {
                    look =
                        "",
                    exam =
                        "",
                    smell = "It doesn't seem to smell",
                    room = "",
                    taste = "It tastes like metal",
                    touch = "It feels cold to touch"
                },
                location = Item.Item.ItemLocation.Room,
                slot = Item.Item.EqSlot.Legs,               
                type = Item.Item.ItemType.Armour,
                name = "Greaves of Tyr",
                ArmorRating = new ArmourRating()
                {
                    Armour = 20,
                    Magic = 7
                },
                itemFlags = new EditableList<Item.Item.ItemFlags>()
                {
                    Item.Item.ItemFlags.equipable,
                    Item.Item.ItemFlags.glow,
                    Item.Item.ItemFlags.bless,
                },
                Weight = 15,
                equipable = true,

            };

            return SteelGreevesOfTyr;
        }

        public static Item.Item BronzeGreaves()
        {

            var BronzeGreaves = new Item.Item
            {
                armourType = Item.Item.ArmourType.PlateMail,
                eqSlot = Item.Item.EqSlot.Body,
                description = new Description()
                {
                    look = "Bronze platemail Greaves",
                    exam = "Bronze platemail Greaves",
                    smell = "Bronze platemail Greaves",
                    room = "Bronze platemail Greaves",
                    taste = "",
                    touch = ""
                },
                location = Item.Item.ItemLocation.Room,
                slot = Item.Item.EqSlot.Legs,
                type = Item.Item.ItemType.Armour,
                name = "Bronze platemail Greaves",
                stats = new Stats()
                {
                    minUsageLevel = 7
                },
                ArmorRating = new ArmourRating()
                {
                    Armour = 5,
                    Magic = 1
                },
                itemFlags = new EditableList<Item.Item.ItemFlags>()
                {
                    Item.Item.ItemFlags.equipable,

                },
                Weight = 15,
                equipable = true,
                Gold = 80

            };

            return BronzeGreaves;
        }
    }
}