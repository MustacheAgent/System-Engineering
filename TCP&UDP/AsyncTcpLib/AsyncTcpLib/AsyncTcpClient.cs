using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace AsyncTcpLib
{
    public class AsyncTcpClient
    {
        private Socket _server;
        private byte[] _buffer;

        public bool IsConnected
        {
            get; private set;
        }

        public IPEndPoint ServerAddress
        {
            get; private set;
        }

        public AsyncTcpClient() 
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public AsyncTcpClient(Socket server)
        {
            _server = server;
        }

        public void Connect(string address, int port)
        {
            if (IsConnected) return;
            try
            {
                ServerAddress = new IPEndPoint(IPAddress.Parse(address), port);
                _server.BeginConnect(ServerAddress, new AsyncCallback(ConnectCallback), null);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Disconnect()
        {
            if (!IsConnected) return;

            try
            {
                _server.Close();
                _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IsConnected = false;
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(ObjectDisposedException)
            {
                return;
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] byteMessage = Encoding.ASCII.GetBytes(message);
                _server.BeginSend(byteMessage, 0, byteMessage.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _server.EndConnect(ar);
                IsConnected = true;
                OnConnected?.Invoke(_server);

                _buffer = new byte[_server.SendBufferSize];
                _server.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10061)
                    OnRefused?.Invoke(ServerAddress);
                else
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                int bytesSent = _server.EndSend(ar);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = _server.EndReceive(ar);

                if (receivedBytes == 0) return;

                Array.Resize(ref _buffer, receivedBytes);
                string text = Encoding.UTF8.GetString(_buffer);

                OnMessageReceived?.Invoke(_server, text);

                Array.Resize(ref _buffer, _server.SendBufferSize);

                _server.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public delegate void MessageReceivedHandler(Socket server, string message);
        public event MessageReceivedHandler OnMessageReceived;

        public delegate void OnConnectedHandler(Socket server);
        public event OnConnectedHandler OnConnected;

        public delegate void OnRefusedHandler(IPEndPoint endPoint);
        public event OnRefusedHandler OnRefused;
    }
}
