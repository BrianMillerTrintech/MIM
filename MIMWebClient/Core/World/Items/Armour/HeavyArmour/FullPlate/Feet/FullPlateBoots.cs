﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Components.DictionaryAdapter;
using MIMWebClient.Core.Item;

namespace MIMWebClient.Core.World.Items.Armour.HeavyArmour.FullPlate.Feet
{
    public class FullPlateBoots
    {
        public static Item.Item BootsOfTyr()
        {

            var BootsOfTyr = new Item.Item
            {
                armourType = Item.Item.ArmourType.PlateMail,
                eqSlot = Item.Item.EqSlot.Feet,
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
                slot = Item.Item.EqSlot.Feet,
                type = Item.Item.ItemType.Armour,
                name = "Boots Of Tyr",
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
                equipable = true

            };

            return BootsOfTyr;
        }

        public static Item.Item BronzeBoots()
        {

            var BronzeBoots = new Item.Item
            {
                armourType = Item.Item.ArmourType.PlateMail,
                eqSlot = Item.Item.EqSlot.Body,
                description = new Description()
                {
                    look = "Bronze platemail Boots",
                    exam = "Bronze platemail Boots",
                    smell = "Bronze platemail Boots",
                    room = "Bronze platemail Boots",
                    taste = "",
                    touch = ""
                },
                location = Item.Item.ItemLocation.Room,
                slot = Item.Item.EqSlot.Feet,
                type = Item.Item.ItemType.Armour,
                name = "Bronze platemail Boots",
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

            return BronzeBoots;
        }
    }
}