﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIMWebClient.Core.Item
{
    public class BaseItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        public string name { get; set; }
        public List<string> keywords { get; set; }
        public int? keyValue { get; set; }
        public bool hidden { get; set; }
        public string location { get; set; }
        public bool equipable { get; set; }
        public string slot { get; set; }

        public Action actions { get; set; }
        public Description description { get; set; }
        public Stats stats { get; set; }
        public bool container { get; set; }
        public bool locked { get; set; }

    }
}