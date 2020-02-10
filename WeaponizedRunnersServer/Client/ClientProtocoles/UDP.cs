using GameServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Numerics;
using System.Net.Sockets;
using WeaponizedRunnersShared;

namespace WeaponizedRunnersServer.Server.Protocoles
{
    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;
        private Client _parentClient;

        private int id;

        public UDP(int _id, Client client)
        {
            id = _id;
            _parentClient = client;
            
        }

        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
            socket = new UdpClient(endPoint);
            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);
            Console.WriteLine($"ClientId: {id}: UDP has connected.");
        }

        public void HandleData(Packet _packetData)
        {
            byte[] packetBytes = _packetData.ReadAllBytes();

            GameServer.Server.ReceiveManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(packetBytes))
                {
                    int packetId = packet.ReadInt();
                    GameServer.Server.ReceiveManager.ProcessPacket(packetId, id, packet);
                }
            });
        }

        public void SendData(Packet _packet)
        {
            try
            {
                //_packet.InsertInt(_parentClient.id); 
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

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

        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            GameServer.Server.ReceiveManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    GameServer.Server.ReceiveManager.ProcessPacket(_packetId, _parentClient.id, _packet); // Call appropriate method to handle the packet
                }
            });
        }

        /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
        public void Disconnect()
        {
            _parentClient.Disconnect();

            endPoint = null;
            socket = null;

            Console.WriteLine($"ClientId: {id}: UDP has disconnected.");
        }
    }
}
