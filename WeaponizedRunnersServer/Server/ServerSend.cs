using System;
using System.Collections.Generic;
using System.Text;
using WeaponizedRunnersShared;

namespace GameServer
{
    public class ServerSend
    {
        public void Welcome(Client client, string msg)
        {
            using (Packet packet = new Packet((int)ServerPacketType.welcome))
            {
                packet.Write(msg);
                //packet.Write(client.id);

                //packet.WriteLength();

                client.tcp.SendData(packet);
            }
        }

        public void SpawnPlayer(Client client)
        {
            using (Packet packet = new Packet((int)ServerPacketType.spawnPlayer))
            {
                packet.Write(client.player.id);
                packet.Write(client.player.username);
                packet.Write(client.player.position);
                packet.Write(client.player.rotation);

                packet.WriteLength();

                client.tcp.SendData(packet);
            }
        }

        public void PlayerPosition(Client client)
        {
            using (Packet packet = new Packet((int)ServerPacketType.playerPosition))
            {
                packet.Write(client.player.id);
                packet.Write(client.player.position);

                packet.WriteLength();

                client.tcp.SendData(packet);
            }
        }

        public void Message(Client client, string message)
        {
            using (Packet packet = new Packet())
            {
                packet.Write((int)ServerPacketType.message);
                packet.Write(message);
                //packet.Write(client.id);

                //packet.WriteLength();

                client.tcp.SendData(packet);
            }
        }
    }
}
