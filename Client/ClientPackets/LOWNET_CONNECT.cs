using LowNet.Enums;
using LowNet.Unity3D;
using LowNet.Utils;
using System;

namespace LowNet.ClientPackets
{
    /// <summary>
    /// First Server/Client Sequenze on Connecting.
    /// </summary>
    internal class LOWNET_CONNECT
    {
        /// <summary>
        /// Read Server Response
        /// </summary>
        /// <param name="store"></param>
        internal static void Readpacket(Store store)
        {
            int clientId = store.PopInt();
            string servername = store.PopAscii();
            string serverpassword = store.PopAscii();
            int maxplayer = store.PopInt();

            if (serverpassword != ClientNetworkmanager.Instance.ServerPassword)
            {
                Client.Disconnect();
                if (NetworkUIManager.Instance != null && NetworkUIManager.Instance.OnEnterserverPassword != null)
                    NetworkUIManager.Instance.OnEnterserverPassword.gameObject.SetActive(true);
                Client.Log("Invalid Password! Disconnect from Server.", LogType.LogError);
                return;
            }

            ClientNetworkmanager.Instance.ConnectionId = clientId;

            if (!String.IsNullOrEmpty(Client.GetPlayername))
            {
                Client.Log("Connected to server: " + servername, LogType.LogNormal);
                Sendpacket();
            }
            else
            {
                if (NetworkUIManager.Instance != null && NetworkUIManager.Instance.OnEnterplayername != null)
                    NetworkUIManager.Instance.OnEnterplayername.gameObject.SetActive(true);
                Client.Log("Empty Playername, Call Send packet Self from 'LOWNET_CONNECT' Packet!", LogType.LogWarning);
            }
        }

        /// <summary>
        /// Send Player Infos ConnectionId, Name, Current GUID You can Send packet via LOWNET_CONNECT.Sendpacket(*name*) Need only Empty name in the Client Networkmanager
        /// to Stopp Auto Send
        /// </summary>
        public static void Sendpacket(string playername = null)
        {
            string User = Client.GetPlayername;
            if (!String.IsNullOrEmpty(playername))
                User = playername;
            if (String.IsNullOrEmpty(User))
                User = "LowNet-Player";
            Store store = new Store((int)Packet.LOWNET_CONNECT);
            store.PushInt(Client.GetPlayerId);
            store.PushAscii(User);
            store.PushAscii(Guid.NewGuid().ToString());
            Client.SendTCP(store);
        }
    }
}