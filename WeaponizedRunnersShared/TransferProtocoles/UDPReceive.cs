using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Net.Sockets;
using WeaponizedRunnersShared;
using WeaponizedRunnersShared.PacketContents;
using System.Net;

namespace WeaponizedRunnersShared.TransferProtocoles
{
    public class UDPReceive
    {
        private UdpClient _udpClient;

        private int _port;
        private string _ip;

        private Action<Packet> _receivePackageAction;

        public UdpClient udpListener;

        public UDPReceive(Action<Packet> action)
        {
            _receivePackageAction = action;
        }

        public void StartReceiving(int port){
            _port = port;
            udpListener = new UdpClient(port);
            udpListener.BeginReceive(UDPReceiveCallback, null);
        }

        private void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, _port);
                byte[] bytes = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                using (Packet packet = new Packet(bytes))
                {
                    int clientId = packet.ClientId;
                    if (clientId == 0)
                    {
                        Console.WriteLine("500: ClientID: " + clientId);
                        return;
                    }

                    _receivePackageAction(packet);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }
    }
}
