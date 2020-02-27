using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using WeaponizedRunnersShared;
using WeaponizedRunnersShared.TransferProtocoles;

namespace GameServer
{
    public class Client : IClient
    {
        public int Id {get;set;}
        public Player player;
        public TCP tcp;
        public UDPSend udp;
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }

        public Client(int _clientId)
        {
            Id = _clientId;

            Action<Packet> receivePacketAction = (packet) =>
            {
                Server.ReceiveManager.ExecuteOnMainThread(() =>
                {
                    Server.ReceiveManager.ProcessPacket(packet.PacketTypeId, packet);
                });
            };
            tcp = new TCP(Id, this, receivePacketAction);

            if(Constants.AllowUDP)
                udp = new UDPSend(Id, this, receivePacketAction);

            player = new Player(Id, "NewPlayer", new Vector3(0, 0, 0), this);
        }


        /// <summary>Disconnects the client and stops all network traffic.</summary>
        public void Disconnect()
        {
            Console.WriteLine($"{tcp?.tcpClient.Client.RemoteEndPoint} has disconnected.");

            player = null;

            tcp?.Disconnect();
            udp?.Disconnect();
        }
    }
}
