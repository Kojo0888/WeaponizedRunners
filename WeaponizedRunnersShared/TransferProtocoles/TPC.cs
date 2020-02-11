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

        private Action<byte[]> _receivePackageAction;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;
        private IClient _parentClient;
        public TCP(int id, IClient client, Action<byte[]> action)
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
            tcpClient.EndConnect(result);

            if (!tcpClient.Connected)
            {
                return;
            }

            stream = tcpClient.GetStream();
            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (tcpClient != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
                var shouldReset = HandleData(data);
                receivedData.Reset(shouldReset);
                stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            _receivePackageAction(data);
            return true;
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
