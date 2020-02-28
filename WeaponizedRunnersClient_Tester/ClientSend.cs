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
        WelcomeContent welcomePackageContent = new WelcomeContent();
        var endpoint = (IPEndPoint)client.tcp.tcpClient.Client.LocalEndPoint;
        welcomePackageContent.IP = endpoint.Address.MapToIPv4().ToString();
        welcomePackageContent.UDPPort = client.udpReceive.Port;

        Packet responsePacket = new Packet((int)PacketType.welcomeClient);
        responsePacket.PacketContent = welcomePackageContent;
        responsePacket.ClientId = client.Id;

        //packageContent.Message = "TestExampleUsername1";
        client.tcp.SendData(responsePacket);
    }

    public void Message(Client client, string message)
    {
        MessageContent packageContent = new MessageContent();
        packageContent.Message = message;

        Packet packet = new Packet((int)PacketType.message);
        packet.ClientId = client.Id;
        packet.PacketContent = packageContent;
       
        client.udpSend.SendData(packet);
    }
}
