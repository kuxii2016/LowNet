/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using LowNet.Packets;

namespace LowNet.Gameclient.Packets
{
    /// <summary>
    /// LowNet Connect Packets
    /// </summary>
    public class LOWNET_CONNECT
    {
        /// <summary>
        /// Read Connection Packet
        /// </summary>
        /// <param name="session"></param>
        /// <param name="store"></param>
        public static void Read(GameClient session, Store store)
        {
            int Checksum = store.PopInt();
            string Network = store.PopAscii();
            int ClientId = store.PopInt();
            int Player = store.PopInt();
            int Maxplayer = store.PopInt();
            string Serverpassword = store.PopAscii();
            string Servername = store.PopAscii();
            string ConnUUID = store.PopAscii();
            session.SetplayerId(ClientId);
            session.TempUUID = ConnUUID;

            if (GameClient.Instance.Serverpassword != Serverpassword)
            {
                GameClient.Instance.Error("Invalid Server Password!", GameClient.Instance);
                session.Disconnect();
                return;
            }

            ///Possible? i think not or i have Written Bad Code xD
            if (ClientId > Maxplayer)
            {
                GameClient.Instance.Error("Server is Full!", GameClient.Instance);
                session.Disconnect();
                return;
            }

            GameClient.Instance.Log("Connected to: " + Servername + $" Current Player(s) {Player}/{Maxplayer} are Online", GameClient.Instance);
            GameClient.Instance.Debug("Server based on: " + Network, GameClient.Instance);
        }

        /// <summary>
        /// Send Response
        /// </summary>
        /// <param name="client"></param>
        internal static void Send(GameClient client)
        {
            Store store = new Store(LowNetpacketOrder.LOWNET_CONNECT);
            store.PushInt(client.GetConnectionId);
            store.PushAscii(client.TempUUID);
            store.PushAscii(GameClient.Instance.Serverpassword);
            store.PushInt(client.GetConnectionId);
            client.SendTCP(store);
        }
    }
}