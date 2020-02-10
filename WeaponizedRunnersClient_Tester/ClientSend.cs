using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using WeaponizedRunnersClient_Tester;
using WeaponizedRunnersShared;

public class ClientSend
{
    public void Welcome(Client client)
    {
        using (Packet packet = new Packet((int)ClientPacketType.welcome))
        {
            packet.Write(client.myId);
            packet.Write("Test username");

            client.tcp.SendData(packet);
        }
    }

    public void PlayerMovement(Client client, bool[] inputs)
    {
        using (Packet packet = new Packet((int)ClientPacketType.playerMovement))
        {
            packet.Write(inputs.Length);
            foreach (bool input in inputs)
            {
                packet.Write(input);
            }
            //packet.Write(GameManager.players[client.myId].transform.rotation);

            client.udp.SendData(packet);
        }
    }

    public void Message(Client client, string message)
    {
        using (Packet packet = new Packet((int)ClientPacketType.message))
        {
            packet.Write(client.myId);
            packet.Write(message);

            client.tcp.SendData(packet);
        }
    }
}
