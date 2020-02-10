using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using WeaponizedRunnersServer.Server.Protocoles;

namespace GameServer
{
    public class Client
    {
        public int id;
        public Player player;
        public TCP tcp;
        public UDP udp;

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id, this);
            udp = new UDP(id, this);

            player = new Player(id, "NewPlayer", new Vector3(0, 0, 0), this);
        }

        /// <summary>Disconnects the client and stops all network traffic.</summary>
        public void Disconnect()
        {
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");

            player = null;

            tcp.Disconnect();
            udp.Disconnect();
        }
    }
}
