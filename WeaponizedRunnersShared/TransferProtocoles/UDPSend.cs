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
    public class UDPSend
    {
        private UdpClient _udpClient;

        //private Socket _socketSend;
        private int _port;
        private string _ip;

        public IPEndPoint endPoint;
        private IClient _parentClient;
        private Action<Packet> _receivePackageAction;

        public UDPSend(int id, IClient client, Action<Packet> action)
        {
            _parentClient = client;
            _receivePackageAction = action;
            //endPoint = new IPEndPoint(IPAddress.Parse(_parentClient.ServerIP), _parentClient.ServerPort);
        }

        public UdpClient Connect(string ip, int port)
        {
            if(!Constants.AllowUDP)
                throw new Exception("UDP is disabled");
            try
            {
                _ip = ip;
                _port = port;

                var ipAddress = IPAddress.Parse(_ip);

                //endPoint = new IPEndPoint(ipAddress, _portReceive);


                //_socketSend = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                //_socketSend.Connect(ipAddress, _portSend);

                Console.WriteLine($"Attempting UDP Connection (IP: {ipAddress} Port: {port})");
                _udpClient = new UdpClient();
                _udpClient.Connect(_ip, _port);
                //_udpClient.BeginReceive(ReceiveCallback, null);
                Console.WriteLine($"UDP Connected (IP: {ipAddress} Port: {port})");
                return _udpClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

   

        public void SendData(Packet packet)
        {
            if(!Constants.AllowUDP)
                throw new Exception("UDP is disabled");
                
            try
            {
                if (_udpClient != null)
                {
                    var bytes = packet.GetPacketBytes();
                    _udpClient.BeginSend(bytes, bytes.Length, null, null);
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
