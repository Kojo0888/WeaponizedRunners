using System;
using System.Collections.Generic;
using System.Text;

namespace WeaponizedRunnersShared
{
    public enum PacketType
    {
        welcome = 1,
        spawnPlayer,
        playerPosition,
        playerRotation,
        message
    }
}
