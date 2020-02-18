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
        public TCP(int id, IClient client, Action<Packet> action)
        {
            _parentClient = client;
            _receivePackageAction = action;
        }

        public void Connect(TcpClient tcpClient = null)
        {
            if (tcpClient == null)
                tcpClient = new TcpClient();
                
            tcpClient.ReceiveBufferSize = Constants.PACKET_DATA_BUFFER_SIZE;
            tcpClient.SendBufferSize = Constants.PACKET_DATA_BUFFER_SIZE;
            this.tcpClient = tcpClient;

            receiveBuffer = new byte[Constants.PACKET_DATA_BUFFER_SIZE];
            if(!tcpClient.Connected)
            {
                tcpClient.BeginConnect(_parentClient.ServerIP, _parentClient.ServerPort, ConnectCallback, tcpClient);
            }
            else
            {
                NetworkStream stream = this.tcpClient.GetStream();
                stream.BeginRead(receiveBuffer, 0, Constants.PACKET_DATA_BUFFER_SIZE, ReceiveCallback, null);
            }
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
                Console.WriteLine("TPC Connected");
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
