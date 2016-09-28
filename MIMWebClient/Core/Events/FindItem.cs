﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MIMWebClient.Core.Events
{
    public class FindItem
    {
        public object Item (List<Item.Item> collection, int findNth, string itemToFind)
        {

            if (findNth == -1)
            {
                return collection.Find(x => x.name.ToLower().Contains(itemToFind));
            }


            return collection.FindAll(x => x.name.ToLower().Contains(itemToFind)).Skip(findNth - 1).FirstOrDefault();

        }

        public object Player(List<PlayerSetup.Player> collection, int findNth, string itemToFind)
        {

            if (findNth == -1)
            {
                return collection.Find(x => x.Name.ToLower().Contains(itemToFind));
            }


            return collection.FindAll(x => x.Name.ToLower().Contains(itemToFind)).Skip(findNth - 1).FirstOrDefault();

        }
    }
}