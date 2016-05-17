﻿namespace MIMEngine.Core.Events
{
    using System.Text;

    using PlayerSetup;

    class Score
    {

        public static void ReturnScore(Player playerData)
        {
            string scoreTest = "Score:\r\n Name: " + playerData.Name + " Race: " + playerData.Race;



           HubProxy.MimHubServer.Invoke("SendToClient", scoreTest);

        }
    }
}
