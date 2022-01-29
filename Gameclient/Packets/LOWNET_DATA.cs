/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2020 by Kuxii
*/
using LowNet.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowNet.Gameclient.Packets
{
    /// <summary>
    /// DataQuerry Maybe an tiny Store about Player Data or so?
    /// </summary>
    public class LOWNET_DATA
    {
        /// <summary>
        /// Read Packet
        /// </summary>
        /// <param name="client"></param>
        /// <param name="store"></param>
        internal static void Read(GameClient client, Store store)
        {
            int Checksum = store.PopInt();
            int clietnt = store.PopInt();
            byte Question = store.PopByte();
            switch(Question)
            {
                case 0:
                    {
                        client.Log("Connection accepted", client);
                        Send(client);
                    }
                    break;
                case 1:
                    {
                        client.Warning("Registration Required, Not installed yet", client);
                    }
                    break;
                case 2:
                    {
                        client.Warning("Login Required, Not installed yet", client);
                    }
                    break;
                case 3:
                    {
                        client.Error("Connection denied", client);
                        client.Disconnect();
                        return;
                    }
            }
        }

        /// <summary>
        /// Send Data 
        /// </summary>
        /// <param name="client"></param>
        internal static void Send(GameClient client)
        {
            Store store = new Store(LowNet.Packets.LowNetpacketOrder.LOWNET_DATA);
            //TODO: Give User from Networksystem the Option for Register Loging if Wandet
            //btw 1 == Registration Infos, 2 == Login Infos, 0 == Process Connection
            store.PushInt(client.GetConnectionId);
            store.PushByte(0x00);
            client.SendTCP(store);
        }
    }
}