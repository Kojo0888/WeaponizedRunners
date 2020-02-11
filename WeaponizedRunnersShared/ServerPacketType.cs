using System;
using System.Collections.Generic;
using System.Text;

namespace WeaponizedRunnersShared
{
    public enum ServerPacketType
    {
        welcome = 1,
        spawnPlayer,
        message,
        playerPosition,
        playerRotation
    }
}
