/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2020 by Kuxii
*/
using LowNet.Packets;
using LowNet.Server.Packets;
using System;
using System.Collections.Generic;
using static LowNet.Server.LowNetServer;

namespace LowNet.ServerPackets
{
    /// <summary>
    /// LowNet Message/Packet Handler
    /// </summary>
    public class LowNetServerPackethander
    {
        /// <summary>
        /// Init Networking System Packets
        /// </summary>
        public static void InitPackets()
        {
            Dictionary<int, PacketHandler> packets = new Dictionary<int, PacketHandler>()
            {
                { (int)LowNetpacketOrder.LOWNET_CONNECT, LOWNET_CONNECT.Read},
                { (int)LowNetpacketOrder.LOWNET_HANDSHAKE, LOWNET_HANDSHAKE.Read},
                { (int)LowNetpacketOrder.LOWNET_DATA, LOWNET_DATA.Read},
                { (int)LowNetpacketOrder.LOWNET_PLAYER, LOWNET_PLAYER.Read},
                { (int)LowNetpacketOrder.LOWNET_OBJECT, LOWNET_OBJECT.Read},
                { (int)LowNetpacketOrder.LOWNET_CONNECT_UDP, LOWNET_CONNECT_UDP.Read},
            };

            Serverpackets = packets;

            #region Debug Printing
            if (server == null)
                return;

            foreach (var item in Serverpackets)
            {
                server.Debug("LowNetpacket: [" + item.Key.ToString() + "] " + "LowNetpacketOrder." + ((LowNetpacketOrder)item.Key).ToString() + " Relay to() =>" + item.Value.Method.DeclaringType + "." + item.Value.Method.Name + "(Client client, Store store)", server);
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
        public static void AddCustomPackets(int packetId, PacketHandler packet, string Customenum = null)
        {
            try
            {
                if (packet == null || packetId == 0)
                    return;

                Serverpackets.Add(packetId + 1, packet);

                if (Customenum != null)
                    server.Debug("Add Custompacket: [" + (packetId + 1).ToString() + "] " + "CustomPacketOrder." + Customenum + " Relay to() =>" + packet.Method.Name + "(Client client, Store store)", packet.GetType());
                else
                    server.Debug("Add Custompacket: [" + (packetId + 1).ToString() + "] " + "CustomPacketOrder.PacketEnum_" + packetId.ToString() + " Relay to() =>" + packet.Method.DeclaringType + "." + packet.Method.Name + "(Client client, Store store)", server);
            }
            catch (Exception ex)
            {
                server.Error("Packethandler Inject Custompackets: " + ex.ToString(), server);
            }
        }

        /// <summary>
        /// Only for Debugging intressting.
        /// This Function Prints all Server Network Packets
        /// Contains System Packets plus Custom Packets was are Injected.
        /// </summary>
        public static void WriteallPackets()
        {
            foreach (var item in Serverpackets)
            {
                try
                {
                    server.Debug("LowNetpacket: [" + item.Key.ToString() + "] " + "LowNetpacketOrder." + ((LowNetpacketOrder)item.Key).ToString() + " Relay to() =>" + item.Value.Method.DeclaringType + "." + item.Value.Method.Name + "(Client client, Store store)", server);
                }
                catch
                {
                    server.Debug("Custompacket: [" + item.ToString() + "] " + "CustomPacketOrder.PacketEnum_" + item.ToString() + " Relay to() =>" + item.Value.Method.DeclaringType + "." + item.Value.Method.Name + "(Client client, Store store)", server);
                }
            }
        }
        #endregion
    }
}