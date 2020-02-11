using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Numerics;
using System.Net.Sockets;
using WeaponizedRunnersShared;

namespace WeaponizedRunnersShared.TransferProtocoles
{
    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;
        private IClient _parentClient;
        private Action<Packet> _receivePackageAction;

        public UDP(int id, IClient client, Action<Packet> action)
        {
            _parentClient = client;
            _receivePackageAction = action;
            endPoint = new IPEndPoint(IPAddress.Parse(_parentClient.ServerIP), _parentClient.ServerPort);
        }

        /// <summary>Attempts to connect to the server via UDP.</summary>
        /// <param name="_localPort">The port number to bind the UDP socket to.</param>
        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
            socket = new UdpClient(endPoint);
            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);
        }

        /// <summary>Sends data to the client via UDP.</summary>
        /// <param name="_packet">The packet to send.</param>
        public void SendData(Packet _packet)
        {
            try
            {
                //_packet.InsertInt((int)ClientPacketType.message); // Insert the client's ID at the start of the packet
                if (socket != null)
                {
                    var bytes = _packet.GetPacketBytes();
                    socket.BeginSend(bytes, bytes.Length, null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }

        /// <summary>Receives incoming UDP data.</summary>
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                Packet packet = new Packet(data);
                if (!packet.IsValid())
                {
                    _parentClient.Disconnect();
                    return;
                }

                _receivePackageAction(packet);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _parentClient.Disconnect();
            }
        }



        /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
        public void Disconnect()
        {
            //_parentClient.Disconnect();

            endPoint = null;
            socket.Close();
            socket = null;
        }
    }

    /// <summary>Initializes all necessary client data.</summary>
   
}
