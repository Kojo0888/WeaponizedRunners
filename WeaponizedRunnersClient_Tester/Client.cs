﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using WeaponizedRunnersShared;
using WeaponizedRunnersClient_Tester;
using WeaponizedRunnersShared.TransferProtocoles;

namespace WeaponizedRunnersClient_Tester
{
    public class Client : IClient
    {
        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 26950;
        public int Id { get; set; } = 0;
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

            Action<Packet> receivePacketAction = (packet) =>
            {
                ClientReceiveManager.ExecuteOnMainThread(() =>
                {
                     ClientReceiveManager.ProcessPacket(packet.PacketTypeId, this, packet);
                });
            };
            tcp = new TCP(Id, this, receivePacketAction);
            udp = new UDP(Id, this, receivePacketAction);
        }

        public void Connect()
        {
            isConnected = true;
            tcp.Connect();
            //udp.Connect();
        }

        public void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp?.Disconnect();
                udp?.Disconnect();

                Console.WriteLine("Disconnected from server.");
            }
        }
    }

}