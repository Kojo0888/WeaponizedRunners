using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WeaponizedRunnersShared
{
    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private int readPos;

        public int ClientId;
        public int PacketTypeId;


        public Packet()
        {
            buffer = new List<byte>();
            readPos = 0;
        }

        public Packet(int packetTypeId) : this()
        {
            PacketTypeId = packetTypeId;
        }

        public Packet(byte[] incomingBytes) : this()
        {
            PacketTypeId = BitConverter. returnBuffer.AddRange(BitConverter.GetBytes());
            ClientId = returnBuffer.AddRange(BitConverter.GetBytes());
            buffer.AddRange(incomingBytes);
        }

        public byte[] GetPacketBytes()
        {
            var returnBuffer = new List<byte>();
            returnBuffer.AddRange(BitConverter.GetBytes(PacketTypeId));
            returnBuffer.AddRange(BitConverter.GetBytes(ClientId));
            returnBuffer.AddRange(buffer);
            return returnBuffer.ToArray();
        }

        public void Dispose()
        {
            buffer.Clear();
            GC.SuppressFinalize(this);
        }
    }
}