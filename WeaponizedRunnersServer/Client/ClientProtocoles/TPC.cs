using GameServer;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using WeaponizedRunnersShared;

namespace WeaponizedRunnersServer.Server.Protocoles
{
    public class TCP
    {
        public TcpClient socket;

        private readonly int id;
        private NetworkStream stream;
        private Packet receivedPacket;
        private byte[] receiveBuffer;
        private Client _refClient;

        public TCP(int id, Client client)
        {
            this.id = id;
            _refClient = client;
        }

        /// <summary>Initializes the newly connected client's TCP-related info.</summary>
        /// <param name="_socket">The TcpClient instance of the newly connected client.</param>
        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = Constants.PACKET_DATA_BUFFER_SIZE;
            socket.SendBufferSize = Constants.PACKET_DATA_BUFFER_SIZE;

            stream = socket.GetStream();

            receivedPacket = new Packet();
            receiveBuffer = new byte[Constants.PACKET_DATA_BUFFER_SIZE];

            stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);

            Console.WriteLine($"ClientId: {id}: TCP has connected.");
        }

        /// <summary>Sends data to the client via TCP.</summary>
        /// <param name="_packet">The packet to send.</param>
        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null); // Send data to appropriate client
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error sending data to player {id} via TCP: {_ex}");
            }
        }

        /// <summary>Reads incoming data from the stream.</summary>
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Disconnect();
                    _refClient.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedPacket.Reset(HandleData(_data)); // Reset receivedData if all data was handled
                stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving TCP data: {_ex}");
                Disconnect();
            }
        }

        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="receivedBytes">The recieved data.</param>
        private bool HandleData(byte[] receivedBytes)
        {
            int packetLength = 0;

            receivedPacket.SetBytes(receivedBytes);

            packetLength = receivedPacket.UnreadLength();
            if (packetLength <= 0)
                return true;

            byte[] packetBytes = receivedPacket.ReadBytes(packetLength);
            GameServer.Server.ReceiveManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(packetBytes))
                {
                    int packetId = packet.ReadInt();
                    GameServer.Server.ReceiveManager.ProcessPacket(packetId, id, packet);
                }
            });

            packetLength = 0;
            if (receivedPacket.UnreadLength() >= 4)
            {
                packetLength = receivedPacket.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        public void Disconnect()
        {
            socket.Close();
            stream = null;
            receivedPacket = null;
            receiveBuffer = null;
            socket = null;

            Console.WriteLine($"ClientId: {id}: TCP has disconnected.");
        }
    }
}
