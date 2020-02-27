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
        //string msg = packet.ReadString();
        //int myId = packet.ReadInt();

        Console.WriteLine($"Welcome to server");
        //client.myId = myId;
        //client.Send.Welcome(client);

        // Now that we have the client's id, connect UDP
        //client.udp.Connect((IPEndPoint)client.tcp.tcpClient.Client.LocalEndPoint);
    }

    public static void Message(Client client, Packet packet)
    {
        string msg = packet.ReadString();
        //int myId = packet.ReadInt();

        Console.WriteLine(DateTime.Now.ToString() + "\t" + msg);
    }
}
