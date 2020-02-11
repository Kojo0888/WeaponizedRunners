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
        public int id;
        public Player player;
        public TCP tcp;
        public UDP udp;
        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 26950;

        public Client(int _clientId)
        {
            id = _clientId;

            Action<byte[]> receivePacketAction = (packetBytes) =>
            {
                Server.ReceiveManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Server.ReceiveManager.ProcessPacket(packetId, id, packet);
                    }
                });
            };
            tcp = new TCP(id, this, receivePacketAction);
            udp = new UDP(id, this, receivePacketAction);

            player = new Player(id, "NewPlayer", new Vector3(0, 0, 0), this);
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
