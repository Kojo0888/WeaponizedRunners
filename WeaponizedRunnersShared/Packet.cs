using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using WeaponizedRunnersShared.PacketContents;

namespace WeaponizedRunnersShared
{
    public class Packet : IDisposable
    {
        public PacketContentBase PacketContent;

        public int ClientId;

        private int _packetTypeId;
        public int PacketTypeId{
            get {
                return _packetTypeId;
            }
            set {
                _packetTypeId = value;
                PacketContent = CreatePacketContent(value);
            }
        }

        private bool isValid = false;

        public Packet()
        {
            
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

        private PacketContentBase CreatePacketContent(int packetContentTypeId)
        {
            switch(packetContentTypeId){
                case (int)ClientPacketType.message:
                    return new MessageContent();
                default:
                    throw new FormatException($"Package type id {packetContentTypeId} is not defined.");
            }
        }

        public Type GetPacketContent<Type>() where Type : PacketContentBase {
            var packetType = PacketContent.GetType();
            var parsedPacketContent = PacketContent as Type;
            if (parsedPacketContent == null)
                throw new FormatException($"Packet tried to be parsed to {typeof(Type)} type, although it is {packetType} type");
            return parsedPacketContent;
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