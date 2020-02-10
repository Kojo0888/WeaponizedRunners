using System;
using System.Collections.Generic;
using System.Text;
using WeaponizedRunnersShared;

namespace GameServer
{
    public class ServerReceiveManager
    {
        private readonly List<Action> executeOnMainThread = new List<Action>();
        private readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private bool actionToExecuteOnMainThread = false;

        public delegate void PacketHandler(int _fromClient, Packet _packet);
        private Dictionary<int, PacketHandler> packetHandlers { get; set; }

        public ServerReceiveManager()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPacketType.welcome, ServerReceive.Welcome },
                { (int)ClientPacketType.playerMovement, ServerReceive.PlayerMovement },
                { (int)ClientPacketType.message, ServerReceive.Message },
            };
        }

        public void ProcessPacket(int packetHandlerId, int clientId, Packet packet)
        {
            packetHandlers[packetHandlerId](clientId, packet);
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
