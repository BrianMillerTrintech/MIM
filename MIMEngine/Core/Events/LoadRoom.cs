﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIMEngine.Core.Events
{
   public class LoadRoom
    {
       public string Region { get; set; }
        public string Area { get; set; }
        public int id { get; set; }


        public JObject LoadRoomFile()
        {
            JObject roomJson = JObject.Parse(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/World/Area/" + this.Region + "/" + this.Area + ".json"));

            return roomJson;
        }
 

        public static string DisplayRoom(JObject room)
        {

            var roomJson = room;

          string roomTitle = (string)roomJson["title"];
          string roomDescription = (string)roomJson["description"];
            var roomExitObj = roomJson.Property("exits").Children();
    

          
            string exitList = null;
            foreach (var exit in roomExitObj)
            {
                if (exit["North"] != null)
                {
                    exitList += exit["North"]["name"];
                }

                if (exit["East"] != null)
                {
                    exitList += exit["East"]["name"];
                }

                if (exit["South"] != null)
                {
                    exitList += exit["South"]["name"];
                }

                if (exit["West"] != null)
                {
                    exitList += exit["West"]["name"];
                }

                if (exit["Up"] != null)
                {
                    exitList += exit["Up"]["name"];
                }

                if (exit["Down"] != null)
                {
                    exitList += exit["Down"]["name"];
                }

            }

            var roomItems = string.Empty;
            var itemList = roomJson["items"];

            foreach (var item in itemList)
            {
                roomItems += item["name"] + "\r\n";
            }



                string displayRoom = roomTitle + "\r\n" + roomDescription + "\r\n" + "Obvious Exits:\r\n" + exitList + "\r\n Items:" + roomItems;

            return displayRoom;

        }

       public static void ReturnRoom(JObject room)
       {
           var roomInfo = DisplayRoom(room);

            HubProxy.MimHubServer.Invoke("SendToClient", roomInfo);
        }
    }
}
