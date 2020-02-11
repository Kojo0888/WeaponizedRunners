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
        private Action<byte[]> _receivePackageAction;

        public UDP(int id, IClient client, Action<byte[]> action)
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
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
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
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    _parentClient.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch
            {
                Disconnect();
            }
        }

        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="bytes">The recieved data.</param>
        private void HandleData(byte[] bytes)
        {
            //using (Packet _packet = new Packet(bytes))
            //{
            //    int _packetLength = _packet.ReadInt();
            //    bytes = _packet.ReadBytes(_packetLength);
            //}
            _receivePackageAction(bytes);
        }

        public void HandleData(Packet _packetData)
        {
            byte[] packetBytes = _packetData.ReadAllBytes();

            _receivePackageAction(packetBytes);
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
