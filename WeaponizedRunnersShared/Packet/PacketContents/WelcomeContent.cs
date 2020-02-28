using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeaponizedRunnersShared.PacketContents
{
    public class WelcomeContent : PacketContentBase
    {
        public string IP { get; set; }
        private int IP_MAXBYTELEGTH {get;set;} = 50;

        public int UDPPort { get; set; }
        
        public override byte[] GetBytes()
        {
            var bytesIP = PacketContentParser.ToBytes<string>(IP);
            var fullBytesIP = new byte[IP_MAXBYTELEGTH];
            Buffer.BlockCopy(bytesIP, 0, fullBytesIP, 0 , bytesIP.Length);

            Console.WriteLine(fullBytesIP.Length);

            var bytesUDPPort = PacketContentParser.ToBytes<int>(UDPPort);

            var bytesToReturn = fullBytesIP.Concat(bytesUDPPort).ToArray();

            Console.WriteLine("Client Welcome(Debug) Byte return Length: " + bytesToReturn.Length);
            return bytesToReturn;
        }

        public override void SetBytes(byte[] bytes)
        {
            IP = PacketContentParser.ToType<string>(bytes.Take(IP_MAXBYTELEGTH).ToArray());
            UDPPort = PacketContentParser.ToType<int>(bytes.Skip(IP_MAXBYTELEGTH).ToArray());
        }
    }
}