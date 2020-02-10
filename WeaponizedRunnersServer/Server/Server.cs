using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WeaponizedRunnersShared;

namespace GameServer
{
    public static class Server
    {
        private static bool isRunning = false;
        public static int MaxPlayers { get; set; }
        public static int Port { get; set; }
        public static string ServerIP = "127.0.0.1";

        public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static ServerReceiveManager ReceiveManager;
        public static ServerSend Send;

        private static int _currentClientId;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");

            ReceiveManager = new ServerReceiveManager();
            Send = new ServerSend();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            isRunning = true; 
            mainThread.Start();
            Console.WriteLine($"Server started on port {Port}.");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient tpcClient = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Console.WriteLine($"Incoming connection from {tpcClient.Client.RemoteEndPoint}...");

            if(Clients.Values.Count >= MaxPlayers)
            {
                Console.WriteLine($"{tpcClient.Client.RemoteEndPoint} failed to connect: Server full!");
                return;
            }
            _currentClientId++;
            var client = new Client(_currentClientId);
            Clients[_currentClientId] = client;
            client.tcp.Connect(tpcClient);
            Send.Welcome(client, "Welcome to the server!");
        }

        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, Port);
                byte[] bytes = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (bytes.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(bytes))
                {
                    int _clientId = _packet.ReadInt();

                    if (_clientId == 0)
                    {
                        return;
                    }

                    if (Clients[_clientId].udp.endPoint == null)
                    {
                        Clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if (Clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        Clients[_clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime nextLoop = DateTime.Now;

            while (isRunning)
            {
                //try
                //{
                    while (nextLoop < DateTime.Now)
                    {
                        Update();

                        nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK);
                        if (nextLoop > DateTime.Now)
                        {
                            Thread.Sleep(nextLoop - DateTime.Now);
                        }
                    }
                //}
                //catch(Exception ex)
                //{
                //    Console.WriteLine(ex.ToString());
                //}
            }
        }

        private static void Update()
        {
            foreach (Client _client in Server.Clients.Values)
            {
                if (_client.player != null)
                {
                    //_client.player.Update();
                }
            }

            ReceiveManager.RunAwaitingActions();
        }
    }
}
