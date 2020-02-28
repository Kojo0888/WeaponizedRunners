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
        public int Port;

        private Action<Packet> _receivePackageAction;

        private UdpClient _udpListener;

        public UDPReceive(Action<Packet> action)
        {
            _receivePackageAction = action;
        }

        public void StartListening(int port)
        {
            Port = port;
            _udpListener = new UdpClient(port);
            Console.WriteLine($"UDP Receive has Started (Port: {Port}).");
            _udpListener.BeginReceive(UDPReceiveCallback, null);
        }

        private void UDPReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, Port);
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
