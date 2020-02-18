using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Numerics;
using System.Net.Sockets;
using WeaponizedRunnersShared;
using WeaponizedRunnersShared.PacketContents;

namespace WeaponizedRunnersShared.TransferProtocoles
{
    public class UDP
    {
        public UdpClient udpClient;
        public IPEndPoint endPoint;
        private IClient _parentClient;
        private Action<Packet> _receivePackageAction;

        public UDP(int id, IClient client, Action<Packet> action)
        {
            _parentClient = client;
            _receivePackageAction = action;
            endPoint = new IPEndPoint(IPAddress.Parse(_parentClient.ServerIP), _parentClient.ServerPort);
        }

        public void Connect(IPEndPoint endPoint)
        {
            try
            {
                Console.WriteLine("Attempting UDP Connection");
                this.endPoint = endPoint;
                udpClient = new UdpClient();
                udpClient.Connect("127.0.0.1", endPoint.Port);
                udpClient.BeginReceive(ReceiveCallback, null);
                Console.WriteLine("UDP Connected");
                //FinishInitializingConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void FinishInitializingConnection()
        {
            try
            {
                var packetContent = new MessageContent();
                packetContent.Message = "Welcome to server :)";

                Packet packet = new Packet((int)PacketType.welcome);
                packet.ClientId = _parentClient.Id;
                packet.PacketContent = packetContent;

                if (udpClient != null)
                {
                    var bytes = packet.GetPacketBytes();
                    udpClient.BeginSend(bytes, bytes.Length, null, null);
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
                byte[] data = udpClient.EndReceive(_result, ref endPoint);
                udpClient.BeginReceive(ReceiveCallback, null);

                Packet packet = new Packet(data);

                ReceiveData(packet);
            }
            catch (Exception)
            {
                _parentClient.Disconnect();
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                //_packet.InsertInt((int)ClientPacketType.message); // Insert the client's ID at the start of the packet
                if (udpClient != null)
                {
                    var bytes = packet.GetPacketBytes();
                    udpClient.BeginSend(bytes, bytes.Length, null, null);
                }
                else
                    Console.WriteLine("SendData: udpClient is null");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to send packet ({ex.Message}).");
                //Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }

        public void ReceiveData(Packet packet)
        {
            _receivePackageAction(packet);
        }

        public void Disconnect()
        {
            //_parentClient.Disconnect();

            endPoint = null;
            udpClient?.Close();
            udpClient = null;
        }
    }
}
