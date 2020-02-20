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
        private UdpClient udpClient;

        private int _port;
        private string _ip;

        public IPEndPoint endPoint;
        private IClient _parentClient;
        private Action<Packet> _receivePackageAction;

        public UDP(int id, IClient client, Action<Packet> action)
        {
            _parentClient = client;
            _receivePackageAction = action;
            //endPoint = new IPEndPoint(IPAddress.Parse(_parentClient.ServerIP), _parentClient.ServerPort);
        }

        public UdpClient Connect(string ip, int port)
        {
            try
            {
                _ip = ip;
                _port = port;
                

                Console.WriteLine("Attempting UDP Connection");
                //if(endpoint == null)
                    this.endPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
                //else 
                //    this.endPoint = endpoint;

                udpClient = new UdpClient(_port);
                udpClient.Connect(_ip, _port);
                udpClient.BeginReceive(ReceiveCallback, null);
                Console.WriteLine($"UDP Connected (Endpoint: {this.endPoint})");
                return udpClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                Console.WriteLine("Receiving data from: " + endPoint);
                byte[] data = udpClient.EndReceive(_result, ref endPoint);
                udpClient.BeginReceive(ReceiveCallback, null);

                Packet packet = new Packet(data);

                _receivePackageAction(packet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while receiving UDP data: " + ex.ToString());
                _parentClient.Disconnect();
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
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
