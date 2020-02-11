using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using WeaponizedRunnersShared;

namespace GameServer
{
    class ServerReceive
    {
        public static void Welcome(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();

            Console.WriteLine($"{Server.Clients[fromClient].tcp.tcpClient.Client.RemoteEndPoint} connected successfully and is now player {fromClient}. Username: \"{username}\"");
            if (fromClient != clientIdCheck)
            {
                Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
            }
            Server.Clients[fromClient].player.SendIntoGame();
        }

        public static void PlayerMovement(int fromClient, Packet packet)
        {
            bool[] inputs = new bool[packet.ReadInt()];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = packet.ReadBool();
            }
            Quaternion rotation = packet.ReadQuaternion();

            Server.Clients[fromClient].player.SetInput(inputs, rotation);
        }

        public static void Message(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string message = packet.ReadString();

            Console.WriteLine(DateTime.Now.ToString() + "\t" + message);
        }
    }
}
