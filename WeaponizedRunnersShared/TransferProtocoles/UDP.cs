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
    public class UDP2Way
    {
        private UdpClient _udpClient;
        private UdpClient _udpClientSend;

        //private Socket _socketSend;

        private int _portReceive;

        private int _portSend;
        private string _ip;

        public IPEndPoint endPoint;
        private IClient _parentClient;
        private Action<Packet> _receivePackageAction;

        public UDP2Way(int id, IClient client, Action<Packet> action)
        {
            _parentClient = client;
            _receivePackageAction = action;
            //endPoint = new IPEndPoint(IPAddress.Parse(_parentClient.ServerIP), _parentClient.ServerPort);
        }

        public UdpClient Connect(string ip, int portReceive, int portSend)
        {
            if(!Constants.AllowUDP)
                throw new Exception("UDP is disabled");
            try
            {
                _ip = ip;
                _portReceive = portReceive;
                _portSend = portSend;

                Console.WriteLine("Attempting UDP Connection");

                var ipAddress = IPAddress.Parse(_ip);

                endPoint = new IPEndPoint(ipAddress, _portReceive);

                //_socketSend = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                //_socketSend.Connect(ipAddress, _portSend);

                _udpClientSend = new UdpClient(_portSend);
                _udpClientSend.Connect(_ip, _portSend);

                _udpClient = new UdpClient(_portReceive);
                _udpClient.Connect(_ip, _portReceive);
                _udpClient.BeginReceive(ReceiveCallback, null);
                Console.WriteLine($"UDP Connected (Endpoint: {endPoint})");
                return _udpClient;
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
                byte[] data = _udpClient.EndReceive(_result, ref endPoint);
                _udpClient.BeginReceive(ReceiveCallback, null);

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
            if(!Constants.AllowUDP)
                throw new Exception("UDP is disabled");
                
            try
            {
                if (_udpClientSend != null)
                {
                    var bytes = packet.GetPacketBytes();
                    _udpClientSend.BeginSend(bytes, bytes.Length, null, null);
                }
                else
                    Console.WriteLine("SendData: _socketSend is null");
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
            _udpClient?.Close();
            _udpClient = null;
        }
    }
}
