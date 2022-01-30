/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LowNet.Gameclient.Transport
{
    /// <summary>
    /// Network Discovery Worker
    /// </summary>
    public class DiscoveryLayer
    {
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="client"></param>
        public DiscoveryLayer(GameClient client)
        {
            Client = client;
        }
        private GameClient Client;
        private UdpClient discoveryClient;

        /// <summary>
        /// Start Worker
        /// </summary>
        public void StartDiscovery()
        {
            try
            {
                Client.Log("Starting Network Discovery....", this);

                discoveryClient = new UdpClient();
                discoveryClient.Client.SendTimeout = 500;
                discoveryClient.Client.ReceiveTimeout = 600;

                var RequestData = Encoding.ASCII.GetBytes("querry");
                var ServerEp = new IPEndPoint(IPAddress.Any, 0);

                discoveryClient.EnableBroadcast = true;
                discoveryClient.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 4901));

                var ServerResponseData = discoveryClient.Receive(ref ServerEp);
                var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                ParseDiscoveryServer(ServerResponse, ServerEp.Address.ToString());
                discoveryClient.Close();
            }
            catch
            {
                Client.Log("No Network Server found, Feels Sorry but you can Connect to an Other :)", this);
            }
        }

        /// <summary>
        /// Stop Discovery Layer
        /// </summary>
        public void Shutdown()
        {
            try
            {
                discoveryClient.Close();
                Client.Log("Stopped Discovery Service", this);
            }
            catch { }
        }

        /// <summary>
        /// Parse Server data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="serverip"></param>
        public void ParseDiscoveryServer(string message, string serverip)
        {
            string[] data = message.Split('/'); // player/maxplayer/name/password/port
            string DiscoveryServer = $"Found: {data[2]} Player {data[0]}/{data[1]} Adresse: {serverip + ":" + data[4]}";
            Client.Log("Discoveryworker: " + DiscoveryServer);
        }
    }
}