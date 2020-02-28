﻿using System;
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

        public delegate void PacketHandler(Packet _packet);
        private Dictionary<int, PacketHandler> packetHandlers { get; set; }

        public ServerReceiveManager()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)PacketType.welcomeClient, ServerReceive.Welcome },
                { (int)PacketType.message, ServerReceive.Message },
            };
        }

        public void ProcessPacket(int packetHandlerId, Packet packet)
        {
            if(packetHandlers.ContainsKey(packetHandlerId))
                packetHandlers[packetHandlerId](packet);
            else 
                Console.WriteLine("There is no package receive for packageTypeID: " + packetHandlerId);
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
