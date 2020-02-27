using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Text;
using WeaponizedRunnersClient_Tester;
using WeaponizedRunnersShared;
using WeaponizedRunnersShared.PacketContents;

public class ClientSend
{
    public void Welcome(Client client)
    {
        MessageContent packageContent = new MessageContent();
        packageContent.Message = "Test username";

        Packet packet = new Packet((int)PacketType.welcome);
        packet.ClientId = client.Id;
        packet.PacketContent = packageContent;

        client.tcp.SendData(packet);
    }

    public void Message(Client client, string message)
    {
        MessageContent packageContent = new MessageContent();
        packageContent.Message = message;

        Packet packet = new Packet((int)PacketType.message);
        packet.ClientId = client.Id;
        packet.PacketContent = packageContent;
       
        client.udp.SendData(packet);
    }
}
