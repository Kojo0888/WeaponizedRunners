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
        public string IP { get; set; } 
        public int Port { get; set; } 
        public int Id { get; set; } = 0;
        public string ServerIP { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ServerPort { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ServerId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TCP tcp;
        public UDP2Way udp;

        public ClientReceiveManager ClientReceiveManager;
        public ClientSend Send;

        private bool isConnected = false;

        public Client()
        {
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
            udp = new UDP2Way(Id, this, receivePacketAction);
        }

        public void Connect(string ip, int tpcPort, int udpPort)
        {
            isConnected = true;
            tcp.Connect(ip, tpcPort);
            if (Constants.AllowUDP)
                udp.Connect(ip, Constants.ClientPortUDP, Constants.ServerPortUDP);
        }

        public void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp?.Disconnect();
                udp?.Disconnect();

                Console.WriteLine("Disconnected from server.");
                Environment.Exit(-1);
            }
        }
    }

}