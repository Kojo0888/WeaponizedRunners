using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using WeaponizedRunnersShared;
using WeaponizedRunnersServer.Server.Protocoles;
using WeaponizedRunnersClient_Tester;

namespace WeaponizedRunnersClient_Tester
{
    public class Client
    {
        public string ServerIP = "127.0.0.1";
        public int ServerPort = 26950;
        public int myId = 0;
        public TCP tcp;
        public UDP udp;

        public ClientReceiveManager ClientReceiveManager;
        public ClientSend Send;

        private bool isConnected = false;

        public Client(string ip, int port)
        {
            ServerIP = ip;
            ServerPort = port;
            ClientReceiveManager = new ClientReceiveManager();
            Send = new ClientSend();
            tcp = new TCP(myId, this);
            udp = new UDP(myId, this);
        }

        public void ConnectToServer()
        {
            isConnected = true;
            tcp.Connect();
        }

        public void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp.socket.Close();
                udp.socket.Close();

                Console.WriteLine("Disconnected from server.");
            }
        }
    }

}