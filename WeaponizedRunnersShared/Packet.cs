﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace WeaponizedRunnersShared
{
    public class Packet : IDisposable
    {
        private List<byte> buffer;

        public int ClientId;
        public int PacketTypeId;
        private bool isValid = false;

        public Packet()
        {
            buffer = new List<byte>();
        }

        public Packet(int packetTypeId) : this()
        {
            PacketTypeId = packetTypeId;
        }

        public Packet(int packetTypeId, int clientId) : this()
        {
            PacketTypeId = packetTypeId;
            ClientId = clientId;
        }

        public Packet(byte[] incomingBytes) : this()
        {
            SetPacketBytes(incomingBytes);
        }

        public void SetPacketBytes(byte[] incomingBytes)
        {
            buffer = new List<byte>();
            if (incomingBytes.Length < 4)
            {
                isValid = false;
                return;
            }
            PacketTypeId = BitConverter.ToInt32(incomingBytes, 0);
            ClientId = BitConverter.ToInt32(incomingBytes, 4);
            buffer.AddRange(incomingBytes.Skip(8));
            isValid = true;
        }

        public byte[] GetPacketBytes()
        {
            var returnBuffer = new List<byte>();
            returnBuffer.AddRange(BitConverter.GetBytes(PacketTypeId));
            returnBuffer.AddRange(BitConverter.GetBytes(ClientId));
            returnBuffer.AddRange(buffer);
            return returnBuffer.ToArray();
        }

        public bool IsValid()
        {
            return isValid;
        }

        public void Dispose()
        {
            buffer.Clear();
            GC.SuppressFinalize(this);
        }
    }
}