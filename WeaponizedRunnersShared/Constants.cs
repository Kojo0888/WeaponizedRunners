using System;
using System.Collections.Generic;
using System.Text;

namespace WeaponizedRunnersShared
{
    public class Constants
    {
        public const int TICKS_PER_SEC = 30;
        public const float MS_PER_TICK = 1000f / TICKS_PER_SEC; 
        public static int PACKET_DATA_BUFFER_SIZE = 4096;
    }
}
