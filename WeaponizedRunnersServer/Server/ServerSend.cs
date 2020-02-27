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
            MessageContent packetContent = new MessageContent();
            packetContent.Message = message;

            Packet packet = new Packet((int)PacketType.welcome);
            packet.ClientId = client.Id;
            packet.PacketContent = packetContent;

            client.tcp.SendData(packet);
            var endpoint = (IPEndPoint)client.tcp.tcpClient.Client.LocalEndPoint;
            Console.WriteLine($"Welcome method: address: {endpoint.Address.ToString()}, port:{endpoint.Port}");
            //client.udp.Connect(endpoint.Address.ToString(), endpoint.Port);
        }

        public void Message(Client client, string message)
        {
            MessageContent packetContent = new MessageContent();
            packetContent.Message = message;

            Packet packet = new Packet((int)PacketType.message);
            packet.ClientId = client.Id;
            packet.PacketContent = packetContent;

            client.udp.SendData(packet);
        }
    }
}
