using System;
using System.Collections.Generic;
using System.Text;
using WeaponizedRunnersShared;

namespace WeaponizedRunnersClient_Tester
{
    public class ClientReceiveManager
    {
        private readonly List<Action> executeOnMainThread = new List<Action>();
        private readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private bool actionToExecuteOnMainThread = false;

        //public delegate void PacketHandler(int _fromClient, Packet _packet);
        private delegate void PacketHandler(Client client, Packet packet);
        private Dictionary<int, PacketHandler> packetHandlers { get; set; }

        public ClientReceiveManager()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ServerPacketType.welcome, ClientReceive.Welcome },
                { (int)ServerPacketType.spawnPlayer, ClientReceive.SpawnPlayer },
                { (int)ServerPacketType.playerPosition, ClientReceive.PlayerPosition },
                { (int)ServerPacketType.playerRotation, ClientReceive.PlayerRotation },
                { (int)ServerPacketType.message, ClientReceive.Message },
            };
        }

        public void ProcessPacket(int packetHandlerId, Client client, Packet packet)
        {
            packetHandlers[packetHandlerId](client, packet);
        }

        public void ExecuteOnMainThread(Action _action)
        {
            if (_action == null)
            {
                Console.WriteLine("No action to execute on main thread!");
                return;
            }

            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(_action);
                actionToExecuteOnMainThread = true;
            }
        }

        public void RunAwaitingActions()
        {
            if (actionToExecuteOnMainThread)
            {
                executeCopiedOnMainThread.Clear();
                lock (executeOnMainThread)
                {
                    executeCopiedOnMainThread.AddRange(executeOnMainThread);
                    executeOnMainThread.Clear();
                    actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
                {
                    executeCopiedOnMainThread[i]();
                }
            }
        }
    }
}
