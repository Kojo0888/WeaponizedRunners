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
        private int _port;

        private Action<Packet> _receivePackageAction;

        private UdpClient _udpListener;

        public UDPReceive(Action<Packet> action)
        {
            _receivePackageAction = action;
        }

        public void StartReceiving(int port)
        {
            _port = port;
            _udpListener = new UdpClient(port);
            Console.WriteLine($"UDP Receive has Started (Port: {_port}).");
            _udpListener.BeginReceive(UDPReceiveCallback, null);
        }

        private void UDPReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, _port);
                byte[] bytes = _udpListener.EndReceive(result, ref _clientEndPoint);
                _udpListener.BeginReceive(UDPReceiveCallback, null);

                using (Packet packet = new Packet(bytes))
                {
                    _receivePackageAction(packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving UDP data: {ex}");
            }
        }
    }
}
