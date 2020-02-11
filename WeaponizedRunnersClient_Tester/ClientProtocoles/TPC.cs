﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using WeaponizedRunnersClient_Tester;
using WeaponizedRunnersShared;

namespace WeaponizedRunnersServer.Server.Protocoles
{
    public class TCP
    {
        public TcpClient tpcClient;

        private Action<byte[]> _receivePackageAction;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;
        private Client _parentClient;
        public TCP(int id, Client client, Action<byte[]> action)
        {
            _parentClient = client;
            _receivePackageAction = action;
        }

        public void Connect()
        {
            tpcClient = new TcpClient
            {
                ReceiveBufferSize = Constants.PACKET_DATA_BUFFER_SIZE,
                SendBufferSize = Constants.PACKET_DATA_BUFFER_SIZE
            };

            receiveBuffer = new byte[Constants.PACKET_DATA_BUFFER_SIZE];
            tpcClient.BeginConnect(_parentClient.ServerIP, _parentClient.ServerPort, ConnectCallback, tpcClient);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            tpcClient.EndConnect(result);

            if (!tpcClient.Connected)
            {
                return;
            }

            stream = tpcClient.GetStream();
            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (tpcClient != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                    _parentClient.Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);
                var shouldReset = HandleData(data);
                receivedData.Reset(shouldReset);
                stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0;

            receivedData.SetBytes(data);

            packetLength = receivedData.ReadInt();
            if (packetLength <= 0)
                return true;

            byte[] packetBytes = receivedData.ReadBytes(packetLength);
            _receivePackageAction(packetBytes);

            packetLength = 0;
            if (receivedData.UnreadLength() >= 4)
            {
                packetLength = receivedData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        private void Disconnect()
        {
            _parentClient.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            tpcClient = null;
        }
    }
}
