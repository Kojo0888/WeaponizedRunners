using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using WeaponizedRunnersShared;

namespace WeaponizedRunnersShared.TransferProtocoles
{
    public class TCP
    {
        byte[] receiveBuffer;

        public TcpClient tcpClient;

        private Action<Packet> _receivePackageAction;

        private IClient _parentClient;

        private string _ip;
        private int _port;

        public TCP(int id, IClient client, Action<Packet> action)
        {
            _parentClient = client;
            _receivePackageAction = action;
        }

        public void Connect(string ip, int port)
        {
            _port = port;
            _ip = ip;

            this.tcpClient = new TcpClient();

            tcpClient.ReceiveBufferSize = Constants.PACKET_DATA_BUFFER_SIZE;
            tcpClient.SendBufferSize = Constants.PACKET_DATA_BUFFER_SIZE;

            receiveBuffer = new byte[Constants.PACKET_DATA_BUFFER_SIZE];
            tcpClient.BeginConnect(ip, port, ConnectCallback, tcpClient);
        }

        public void Connect(TcpClient client)
        {
            tcpClient = client;
            tcpClient.ReceiveBufferSize = Constants.PACKET_DATA_BUFFER_SIZE;
            tcpClient.SendBufferSize = Constants.PACKET_DATA_BUFFER_SIZE;

            receiveBuffer = new byte[Constants.PACKET_DATA_BUFFER_SIZE];

            NetworkStream stream = tcpClient.GetStream();
            stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                tcpClient.EndConnect(result);

                Console.WriteLine($"TPC Connected (Address: {_ip}, Port: {_port})");
                NetworkStream stream = tcpClient.GetStream();
                stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to find server endpoint");
                Environment.Exit(0);
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (tcpClient != null)
                {
                    var bytes = packet.GetPacketBytes();
                    NetworkStream stream = tcpClient.GetStream();
                    stream.BeginWrite(bytes, 0, bytes.Length, null, null);
                }
                else
                    Console.WriteLine("tpcClient is null");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to send packet ({ex.Message})");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                if (tcpClient != null)
                {
                    NetworkStream stream = tcpClient.GetStream();
                    int byteLength = stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        _parentClient.Disconnect();
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);
                    Packet packet = new Packet(data);
                    _receivePackageAction(packet);

                    stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
                }
                else
                    Console.WriteLine("tpcClient is null");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Disconnect();
            }
        }

        public void Disconnect()
        {
            //_parentClient.Disconnect();
            tcpClient?.Close();
            receiveBuffer = null;
            tcpClient = null;
        }
    }
}
