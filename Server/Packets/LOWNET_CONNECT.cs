/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2020 by Kuxii
*/
using LowNet.Data;
using LowNet.Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowNet.Server.Packets
{
    /// <summary>
    /// On client Connect Packet
    /// </summary>
    public class LOWNET_CONNECT
    {
        /// <summary>
        /// Send Packet
        /// </summary>
        internal static void Send()
        {
            //Packet is Direct in the Client
            //so is not so hard to querry many Server info's
        }

        /// <summary>
        /// Read Packet
        /// </summary>
        /// <param name="client"></param>
        /// <param name="store"></param>
        public static void Read(Client client, Store store)
        {
            int senderId = store.PopInt();
            int Checksum = store.PopInt();
            string uuid = store.PopAscii();
            if (uuid == client.Session.GetUUID)
            {
                string passwd = store.PopAscii();
                if (passwd == LowNetServer.server.GetServerpassword)
                {
                    int clientId = store.PopInt();
                    if (clientId == client.ClientId)
                    {
                        //We Check has Client Read and Stored all Server Infos, when Yes we Beginn with Data.
                        LOWNET_DATA.Send(client);
                    }
                    else
                    {
                        client.Disconnect();
                        LowNetServer.server.Error("Client ClientId missmatch with Server!", LowNetServer.server);
                    }
                }
                else
                {
                    client.Disconnect();
                    LowNetServer.server.Error("Client Password missmatch with Server!", LowNetServer.server);
                }
            }
            else
            {
                client.Disconnect();
                LowNetServer.server.Error("Client GUID missmatch with Server!", LowNetServer.server);
            }
        }
    }
}