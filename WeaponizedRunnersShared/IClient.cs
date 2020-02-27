using System;
using System.Collections.Generic;
using System.Text;

namespace WeaponizedRunnersShared
{
    public interface IClient
    {
        int Id {get;set;}
        void Disconnect();
    }
}
