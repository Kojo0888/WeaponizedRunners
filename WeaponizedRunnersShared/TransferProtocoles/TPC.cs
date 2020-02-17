using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using WeaponizedRunnersShared;

namespace WeaponizedRunnersShared.TransferProtocoles
{
    public class TCP
    {
        public TcpClient tcpClient;

        private Action<Packet> _receivePackageAction;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;
        private IClient _parentClient;
        public TCP(int id, IClient client, Action<Packet> action)
        {
            _parentClient = client;
            _receivePackageAction = action;
        }

        public void Connect(TcpClient _socket)
        {
            tcpClient = _socket;
            tcpClient.ReceiveBufferSize = Constants.PACKET_DATA_BUFFER_SIZE;
            tcpClient.SendBufferSize = Constants.PACKET_DATA_BUFFER_SIZE;

            stream = tcpClient.GetStream();

            receivedData = new Packet();
            receiveBuffer = new byte[Constants.PACKET_DATA_BUFFER_SIZE];

            stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
        }

        public void Connect()
        {
            tcpClient = new TcpClient
            {
                ReceiveBufferSize = Constants.PACKET_DATA_BUFFER_SIZE,
                SendBufferSize = Constants.PACKET_DATA_BUFFER_SIZE
            };

            receiveBuffer = new byte[Constants.PACKET_DATA_BUFFER_SIZE];
            tcpClient.BeginConnect(_parentClient.ServerIP, _parentClient.ServerPort, ConnectCallback, tcpClient);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {

                tcpClient.EndConnect(result);

                if (!tcpClient.Connected)
                {
                    return;
                }

                stream = tcpClient.GetStream();
                receivedData = new Packet();
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
                    stream.BeginWrite(bytes, 0, bytes.Length, null, null);
                }
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
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            tcpClient = null;
        }
    }
}
