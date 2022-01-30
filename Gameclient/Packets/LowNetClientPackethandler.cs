/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Gameclient;
using LowNet.Gameclient.Packets;
using LowNet.Packets;
using System;
using System.Collections.Generic;
using static LowNet.Gameclient.GameClient;

namespace LowNet.ClientPackets
{
    /// <summary>
    /// LowNet Client Packet Handler
    /// </summary>
    public class LowNetClientPackethandler
    {
        private static GameClient client;

        /// <summary>
        /// Init System Packets
        /// </summary>
        public static void InitPackets(GameClient gameClient)
        {
            client = gameClient;

            Dictionary<int, PacketHandler> packets = new Dictionary<int, PacketHandler>()
            {
                { (int)LowNetpacketOrder.LOWNET_CONNECT, LOWNET_CONNECT.Read},
                { (int)LowNetpacketOrder.LOWNET_HANDSHAKE, LOWNET_HANDSHAKE.Read},
                { (int)LowNetpacketOrder.LOWNET_DATA, LOWNET_DATA.Read},
                { (int)LowNetpacketOrder.LOWNET_PLAYER, LOWNET_PLAYER.Read},
                { (int)LowNetpacketOrder.LOWNET_OBJECT, LOWNET_OBJECT.Read},
                { (int)LowNetpacketOrder.LOWNET_CONNECT_UDP, LOWNET_CONNECT_UDP.Read},
            };

            GameClient.ClientPackets = packets;

            #region Debug Printing
            if (client == null)
                return;

            foreach (var item in GameClient.ClientPackets)
            {
                client.Debug("LowNetpacket: [" + item.Key.ToString() + "] " + "LowNetpacketOrder." + ((LowNetpacketOrder)item.Key).ToString() + " Relay to() =>" + item.Value.Method.DeclaringType + "." + item.Value.Method.Name + "(Client client, Store store)", client);
            }
            #endregion
        }

        #region User Manipulation
        /// <summary>
        /// Inject Custom Network Packets info Server
        /// Its Allow to Use Own Network Packets without Overwriting System Packets.
        /// When you Overwrite System Packets so you can not use Unity3d LowNet Components.
        /// </summary>
        /// <param name="packetId">Packet Id</param>
        /// <param name="packet">Packet relay Sender Function</param>
        /// <param name="Customenum">Optional, for Debugging, Custom Name from Enum or was you Want</param>
        public static void AddPackets(int packetId, PacketHandler packet, string Customenum = null)
        {
            try
            {
                if (packet == null || packetId == 0)
                    return;

                GameClient.ClientPackets.Add(GameClient.ClientPackets.Count + packetId, packet);

                if (Customenum != null)
                    client.Debug("Add Custompacket: [" + (packetId + 1).ToString() + "] " + "CustomPacketOrder." + Customenum + " Relay to() =>" + packet.Method.Name + "(Client client, Store store)", packet.GetType());
                else
                    client.Debug("Add Custompacket: [" + (packetId + 1).ToString() + "] " + "CustomPacketOrder.PacketEnum_" + packetId.ToString() + " Relay to() =>" + packet.Method.DeclaringType + "." + packet.Method.Name + "(Client client, Store store)", client);
            }
            catch (Exception ex)
            {
                client.Error("Packethandler Inject Custompackets: " + ex.ToString(), client);
            }
        }

        /// <summary>
        /// Only for Debugging intressting.
        /// This Function Prints all Server Network Packets
        /// Contains System Packets plus Custom Packets was are Injected.
        /// </summary>
        public static void WriteallPackets()
        {
            foreach (var item in GameClient.ClientPackets)
            {
                try
                {
                    client.Debug("LowNetpacket: [" + item.Key.ToString() + "] " + "LowNetpacketOrder." + ((LowNetpacketOrder)item.Key).ToString() + " Relay to() =>" + item.Value.Method.DeclaringType + "." + item.Value.Method.Name + "(Client client, Store store)", client);
                }
                catch
                {
                    client.Debug("Custompacket: [" + item.ToString() + "] " + "CustomPacketOrder.PacketEnum_" + item.ToString() + " Relay to() =>" + item.Value.Method.DeclaringType + "." + item.Value.Method.Name + "(Client client, Store store)", client);
                }
            }
        }
        #endregion

    }
}