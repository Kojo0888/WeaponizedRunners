using System;
using System.Collections.Generic;
using System.Text;

namespace WeaponizedRunnersShared
{
    public interface IClient
    {
        string ServerIP { get; set; }
        int ServerPort { get; set; }

        int ServerId {get;set;}
        void Disconnect();
    }
}
