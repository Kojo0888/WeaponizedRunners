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
        var clientId = packet.ClientId;
        var packageContent = packet.GetPacketContent<MessageContent>();
        Console.WriteLine("Message from Server: " + packageContent.Message);

        client.Id = clientId;
        Console.WriteLine("Received ID from Server: " + clientId);

        packageContent.Message = "TestExampleUsername1";
        client.tcp.SendData(packet);
        //var endpoint = (IPEndPoint)client.tcp.tcpClient.Client.LocalEndPoint;
    }

    public static void Message(Client client, Packet packet)
    {
        var clientId = packet.ClientId;
        var packageContent = packet.GetPacketContent<MessageContent>();
        string message = packageContent.Message;

        Console.WriteLine(DateTime.Now.ToString() + $"(ClientId:{clientId})" + "\t" + message);
    }
}
