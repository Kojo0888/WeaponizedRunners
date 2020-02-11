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

        public Packet(byte[] _data) : this()
        {
            buffer.AddRange(_data);
        }

        public byte[] GetPacketBytes()
        {
            var returnBuffer = new List<byte>();
            returnBuffer.AddRange(BitConverter.To);
        }

        public void Dispose()
        {
            buffer.Clear();
            GC.SuppressFinalize(this);
        }
    }
}