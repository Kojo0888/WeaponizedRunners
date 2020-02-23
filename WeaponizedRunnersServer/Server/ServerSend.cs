using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using WeaponizedRunnersShared;
using WeaponizedRunnersShared.PacketContents;

namespace GameServer
{
    public class ServerSend
    {
        public void Welcome(Client client, string message)
        {
<<<<<<< HEAD
            MessageContent packetContent = new MessageContent();
            packetContent.Message = message;

            Packet packet = new Packet((int)PacketType.welcome);
            packet.ClientId = client.ServerId;
            packet.PacketContent = packetContent;
=======
            using (Packet packet = new Packet((int)ServerPacketType.welcome))
            {
                packet.Write(msg);
                //packet.Write(client.id);

                //packet.WriteLength();
>>>>>>> bc62f8670efd9b983411ea2c4d94d04b7d2b541b

            client.tcp.SendData(packet);
            var endpoint = (IPEndPoint)client.tcp.tcpClient.Client.LocalEndPoint;
            Console.WriteLine($"Welcome method: address: {endpoint.Address.ToString()}, port:{endpoint.Port}");
            //client.udp.Connect(endpoint.Address.ToString(), endpoint.Port);
        }

        public void Message(Client client, string message)
        {
<<<<<<< HEAD
            MessageContent packetContent = new MessageContent();
            packetContent.Message = message;
=======
            using (Packet packet = new Packet())
            {
                packet.Write((int)ServerPacketType.message);
                packet.Write(message);
                //packet.Write(client.id);
>>>>>>> bc62f8670efd9b983411ea2c4d94d04b7d2b541b

            Packet packet = new Packet((int)PacketType.message);
            packet.ClientId = client.ServerId;
            packet.PacketContent = packetContent;

            client.tcp.SendData(packet);
        }
    }
}
