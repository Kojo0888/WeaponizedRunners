using System;
using System.Collections.Generic;
using System.Text;

namespace WeaponizedRunnersShared
{
    public interface IClient
    {
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }

        public void Disconnect();
    }
}
