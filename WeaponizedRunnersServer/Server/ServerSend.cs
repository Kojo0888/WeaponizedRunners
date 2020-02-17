using System;
using System.Collections.Generic;
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
            packet.ClientId = client.id;
            packet.PacketContent = packetContent;

            client.tcp.SendData(packet);
        }

        public void Message(Client client, string message)
        {
            MessageContent packetContent = new MessageContent();
            packetContent.Message = message;

            Packet packet = new Packet((int)PacketType.message);
            packet.ClientId = client.id;
            packet.PacketContent = packetContent;

            client.tcp.SendData(packet);
        }
    }
}
