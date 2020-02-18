using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Text;
using WeaponizedRunnersClient_Tester;
using WeaponizedRunnersShared;
using WeaponizedRunnersShared.PacketContents;

public class ClientReceive
{
    public static void Welcome(Client client, Packet packet)
    {
        var packetContent = packet.GetPacketContent<MessageContent>();
        int myId = packet.ClientId;

        Console.WriteLine($"Message from server: {packetContent.Message}");
        client.Id = myId;
        client.Send.Welcome(client);

        // Now that we have the client's id, connect UDP
        client.udp.Connect((IPEndPoint)client.tcp.tcpClient.Client.LocalEndPoint);
    }

    public static void Message(Client client, Packet packet)
    {
        var packetContent = packet.GetPacketContent<MessageContent>();
        string msg = packetContent.Message;
        int myId = packet.ClientId;

        Console.WriteLine(DateTime.Now.ToString() + "\t" + msg);
    }
}
