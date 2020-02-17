using System;
using System.Collections.Generic;
using System.Text;

namespace WeaponizedRunnersShared.PacketContents
{
    public class MessageContent : PacketContentBase
    {
        public string Message { get; set; }
        
        public override byte[] GetBytes()
        {
            return PacketContentParser.ToBytes<string>(Message);
        }

        public override void SetBytes(byte[] bytes)
        {
            Message = PacketContentParser.ToType<string>(bytes);
        }
    }
}