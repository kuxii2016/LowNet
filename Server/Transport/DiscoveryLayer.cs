/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LowNet.Transport
{
    /// <summary>
    /// Network Discovery Servise
    /// </summary>
    public class DiscoveryLayer
    {
        /// <summary>
        /// Create new Service
        /// </summary>
        /// <param name="port"></param>
        /// <param name="server"></param>
        public DiscoveryLayer(int port, LowNetServer server)
        {
            this.Port = port;
            this.Mainserver = server;
            Mainserver.Debug("Init new Discoverylayer.", this);
        }

        private UdpClient Listner;
        /// <summary>
        /// Is Service Running or not
        /// </summary>
        public bool IsRunning { get; private set; } = false;
        /// <summary>
        /// Service Listen Port
        /// </summary>
        public int Port { get; private set; }
        private LowNetServer Mainserver { get; set; }

        /// <summary>
        /// Send Response to Client
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Client"></param>
        public void Send(string Message, IPEndPoint Client)
        {
            try
            {
                var Serverinfo = Encoding.ASCII.GetBytes(Message);
                Listner.Send(Serverinfo, Serverinfo.Length, Client);
                Listner.Client.SendTimeout = 1000;
                Listner.Client.ReceiveTimeout = 1000;
            }
            catch (SocketException so)
            {
                Mainserver.Error("Failed Send Client Data: " + so.Message, this);
            }
            catch (Exception ex)
            {
                Mainserver.Error("Failed Send Client Data: " + ex.Message, this);
            }
        }

        /// <summary>
        /// Stop Discovery Layer
        /// </summary>
        public void Shutdown()
        {
            IsRunning = false;
            if (IsRunning)
            {
                Listner.Close();
                Mainserver.Log("Stopped Discovery Service", this);
            }
        }

        /// <summary>
        /// Start Discovery Layer
        /// </summary>
        public void Start()
        {
            try
            {
                Listner = new UdpClient(Port);
            }
            catch (SocketException so)
            {
                Mainserver.Error("Failed Starting Discovery Service: " + so.Message, this);
                return;
            }
            catch (Exception ex)
            {
                Mainserver.Error("Failed Starting Discovery Service: " + ex.Message, this);
                return;
            }
            finally
            {
                IsRunning = true;
                Listner.BeginReceive(ReceiveCallback, null);
                Mainserver.Log("Started Discovery Service on Port: " + Port, this);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            if (IsRunning)
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

                try
                {
                    byte[] data = Listner.EndReceive(ar, ref clientEndPoint);
                    Listner.BeginReceive(ReceiveCallback, null);
                }
                catch (SocketException so)
                {
                    Mainserver.Error("Failed Read Client Data: " + so.Message, this);
                    return;
                }
                catch (Exception ex)
                {
                    Mainserver.Error("Failed Read Client Data: " + ex.Message, this);
                    return;
                }
                finally
                {
                    Send(BuildResponse(), clientEndPoint);
                }
            }
        }

        private string BuildResponse()
        {
            string Response = $"{Mainserver.GetPlayer}/{Mainserver.GetMaxplayer}/{Mainserver.GetServername}/{Mainserver.GetServerpassword}/{Mainserver.GetServerport}";
            return Response;
        }
    }
}
