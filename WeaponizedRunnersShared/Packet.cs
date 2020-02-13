using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using WeaponizedRunnersShared.PacketContents;

namespace WeaponizedRunnersShared
{
    public class Packet<PacketContentType> : IDisposable where PacketContentType : class, IPacketContent, new() 
    {
        public IPacketContent PacketContent;

        public int ClientId;
        public int PacketTypeId;
        private bool isValid = false;

        public Packet()
        {
            PacketContent = new PacketContentType();
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
            PacketContent.SetBytes(incomingBytes);
        }

        public void SetPacketBytes(byte[] incomingBytes)
        {

            if (incomingBytes.Length < 4)
            {
                isValid = false;
                return;
            }
            PacketTypeId = BitConverter.ToInt32(incomingBytes, 0);
            ClientId = BitConverter.ToInt32(incomingBytes, 4);
            PacketContent.SetBytes(incomingBytes.Skip(8).ToArray());
            isValid = true;
        }

        public byte[] GetPacketBytes()
        {
            var returnBuffer = new List<byte>();
            returnBuffer.AddRange(BitConverter.GetBytes(PacketTypeId));
            returnBuffer.AddRange(BitConverter.GetBytes(ClientId));
            returnBuffer.AddRange(PacketContent.GetBytes());
            return returnBuffer.ToArray();
        }

        public bool IsValid()
        {
            return isValid;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}