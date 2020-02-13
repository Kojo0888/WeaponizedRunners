using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Text;
using WeaponizedRunnersClient_Tester;
using WeaponizedRunnersShared;

public class ClientReceive
{
    public static void Welcome(Client client, Packet packet)
    {
        var bytes = packet.GetPacketContentBytes();
        string msg = Encoding.UTF8.GetString(bytes);
        int myId = packet.ClientId;

        Console.WriteLine($"Message from server: {msg}");
        client.myId = myId;
        client.Send.Welcome(client);

        // Now that we have the client's id, connect UDP
        client.udp.Connect(((IPEndPoint)client.tcp.tcpClient.Client.LocalEndPoint).Port);
    }

    public static void Message(Client client, Packet packet)
    {
        string msg = packet.ReadString();
        int myId = packet.ReadInt();

        Console.WriteLine(DateTime.Now.ToString() + "\t" + msg);
    }

    public static void SpawnPlayer(Client client, Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        //GameManager.instance.SpawnPlayer(id, username, position, rotation);
    }

    public static void PlayerPosition(Client client, Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        //GameManager.players[id].transform.position = position;
    }

    public static void PlayerRotation(Client client, Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();

        //GameManager.players[id].transform.rotation = rotation;
    }
}
