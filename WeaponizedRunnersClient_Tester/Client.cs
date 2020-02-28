using System.Collections;
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
        public int Id { get; set; }
        public TCP tcp;
        public UDPSend udpSend;

        public UDPReceive udpReceive;
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
            tcp = new TCP(this, receivePacketAction);
            udpSend = new UDPSend(this, receivePacketAction);
            udpReceive = new UDPReceive(receivePacketAction);
        }

        public void Connect(string ip, int tpcPort, int udpReceivePort, int udpSendPort)
        {
            isConnected = true;
            tcp.Connect(ip, tpcPort);
            if (Constants.AllowUDP){
                udpReceive.StartListening(udpReceivePort);
                udpSend.Connect(ip, udpSendPort);
            }
        }

        public void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp?.Disconnect();
                udpSend?.Disconnect();

                Console.WriteLine("Disconnected from server.");
                Environment.Exit(-1);
            }
        }
    }

}