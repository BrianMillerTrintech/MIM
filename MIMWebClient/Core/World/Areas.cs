﻿using System.Collections.Generic;

namespace MIMWebClient.Core.World
{
    using MIMWebClient.Core.Room;

    public static class Areas
    {
        public static List<Room> ListOfRooms()
        {
            var areas = new List<Room>

            {
                Anker.Anker.VillageSquare(),
                Anker.Anker.SquareWalkOutsideTavern(),
                Anker.Anker.SquareWalkOutsideStables(),
                Anker.Anker.RedLionStables(),
                Anker.Anker.SquareWalkCommerceCorner(),
                Anker.Anker.SquareWalkEastOfCentre(),
                Anker.Anker.SquareWalkEntrance(),
                Anker.Anker.SquareWalkSouthWestOfCentre(),
                Anker.Anker.SquareWalkWestOfCentre(),
                Anker.Anker.SquareWalkSouthOfCentre(),
                Anker.Anker.DrunkenSailor(),
                Anker.Anker.GeneralStore(),
                Anker.Anker.MetalMedley(),
                Anker.Anker.VillageHall(),
                Anker.Anker.VillageHallEntrance(),
                Anker.Anker.VillageHallEldersRoom(),
                Anker.Anker.TempleRoad(),
                Anker.Anker.TempleRoad2(),
                Anker.Anker.TempleEntrance(),
                Anker.Anker.PathToTheSquare(),
                Anker.Anker.AnkerLane(),
                Anker.Anker.AnkerLaneWest21(),
                Anker.Anker.AnkerLaneWest25(),
                Anker.Anker.AnkerLaneWest37(),
                Anker.Anker.AnkerLaneEast22(),
                Anker.Anker.AnkerLaneEast23(),
                Anker.Anker.AnkerLaneEast24(),
                Anker.Anker.AnkerHome(),
                Anker.Anker.AnkerHome2(),
                Anker.Anker.AnkerHome3(),
                Anker.Anker.AnkerHome4(),
                 Anker.Anker.AnkerHome5(),
                Anker.Anker.AnkerHome6()
            };

            return areas;
        } 
    }
}