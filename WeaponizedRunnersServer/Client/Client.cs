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
        public int ServerId {get;set;}
        public Player player;
        public TCP tcp;
        public UDP udp;
        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 27001;

        public Client(int _clientId)
        {
            ServerId = _clientId;

            Action<Packet> receivePacketAction = (packet) =>
            {
                Server.ReceiveManager.ExecuteOnMainThread(() =>
                {
                    Server.ReceiveManager.ProcessPacket(packet.PacketTypeId, packet);
                });
            };
            tcp = new TCP(ServerId, this, receivePacketAction);
            udp = new UDP(ServerId, this, receivePacketAction);

            player = new Player(ServerId, "NewPlayer", new Vector3(0, 0, 0), this);
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
