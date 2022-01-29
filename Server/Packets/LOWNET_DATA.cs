/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2020 by Kuxii
*/
using LowNet.Data;
using LowNet.Server.Data;

namespace LowNet.Server.Packets
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
        internal static void Read(Client client, Store store)
        {
            //TODO: Check User Input Transport Over Handshake.
            //For now i Process this first 0 == Process, 1 Register Info, 2 Logininfo
            int Checksum = store.PopInt();
            int clietnt = store.PopInt();
            byte answer = store.PopByte();
            switch (answer)
            {
                case 0:
                    {
                        LowNetServer.server.Log("Connection accepted", LowNetServer.server);
                    }
                    break;
                case 1:
                    {
                        LowNetServer.server.Warning("Registration Required, Not installed yet", LowNetServer.server);
                    }
                    break;
                case 2:
                    {
                        LowNetServer.server.Warning("Login Required, Not installed yet", LowNetServer.server);
                    }
                    break;
                case 3:
                    {
                        LowNetServer.server.Error("Connection denied", LowNetServer.server);
                        client.Disconnect();
                        return;
                    }
            }
        }

        /// <summary>
        /// Send Data 
        /// </summary>
        /// <param name="client"></param>
        internal static void Send(Client client)
        {
            Store store = new Store(LowNet.Packets.LowNetpacketOrder.LOWNET_DATA);
            //TODO: Give User from Networksystem the Option for Register Loging if Wandet
            //btw 1 == Registration, 2 == Login, 3 == denied
            store.PushInt(client.ClientId);
            store.PushByte(0x00);
            LowNetServer.server.TCPLayer.Send(client, store);
        }
    }
}