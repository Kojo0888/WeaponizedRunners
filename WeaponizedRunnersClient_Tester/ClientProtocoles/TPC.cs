using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using WeaponizedRunnersClient_Tester;
using WeaponizedRunnersShared;

namespace WeaponizedRunnersServer.Server.Protocoles
{
    public class TCP
    {
        public TcpClient socket;

        

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;
        private Client _parentClient;
        public TCP(int id, Client client)
        {
            _parentClient = client;
        }

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = Constants.PACKET_DATA_BUFFER_SIZE,
                SendBufferSize = Constants.PACKET_DATA_BUFFER_SIZE
            };

            receiveBuffer = new byte[Constants.PACKET_DATA_BUFFER_SIZE];
            socket.BeginConnect(_parentClient.ServerIP, _parentClient.ServerPort, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            socket.EndConnect(result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();
            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                    _parentClient.Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);
                var shouldReset = HandleData(data);
                receivedData.Reset(shouldReset);
                stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0;

            receivedData.SetBytes(data);

            //packetLength = receivedData.ReadInt();
            //if (packetLength <= 0)
            //    return true;

            byte[] packetBytes = receivedData.ToArray();
            _parentClient.ClientReceiveManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(packetBytes))
                {
                    int packetId = packet.ReadInt();
                    _parentClient.ClientReceiveManager.ProcessPacket(packetId, _parentClient, packet);
                }
            });

            packetLength = 0;
            if (receivedData.UnreadLength() >= 4)
            {
                packetLength = receivedData.ReadInt();
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

        private void Disconnect()
        {
            _parentClient.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }
}
