using System;
using System.Collections.Generic;
using System.Text;

namespace WeaponizedRunnersShared.PacketContents
{
    public abstract class PacketContentBase {

        public virtual void SetBytes(byte[] bytes){}

        public virtual byte[] GetBytes()
        {
            return new byte[0];
        }
    }
}