using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using WeaponizedRunnersShared;
using WeaponizedRunnersShared.PacketContents;

namespace GameServer
{
    class ServerReceive
    {
        public static void Welcome(Packet packet)
        {
            int fromClient = packet.ClientId;
            var packageContent = packet.GetPacketContent<MessageContent>();
            string message = packageContent.Message;

            Console.WriteLine($"{Server.Clients[fromClient].tcp.tcpClient.Client.RemoteEndPoint} connected successfully Username: \"{message}\"");

            //Server.Clients[fromClient].player.SendIntoGame();
        }

        public static void Message(Packet packet)
        {
            var clientId = packet.ClientId;
            var packageContent = packet.GetPacketContent<MessageContent>();
            string message = packageContent.Message;

            Console.WriteLine(DateTime.Now.ToString() +$"(ClientId:{clientId})"+ "\t" + message);
        }
    }
}
