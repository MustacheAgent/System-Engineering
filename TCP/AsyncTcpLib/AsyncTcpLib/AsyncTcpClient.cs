using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AsyncTcpLib
{
    public class AsyncTcpClient
    {
        private Socket _server;
        private byte[] _buffer;

        Thread checkThread;

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
                IsConnected = false;
                _server.Close();
                _server.Dispose();
                _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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

        private void CheckConnection()
        {
            while(true)
            {
                try
                {
                    //byte[] tmp = new byte[] { 0 };
                    byte[] tmp = Encoding.UTF8.GetBytes("check");
                    int sent = _server.Send(tmp);
                }
                catch (SocketException)
                {
                    IsConnected = false;
                    OnConnectionLost?.Invoke(_server);
                    checkThread.Abort();
                }

                Thread.Sleep(1000);
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
                _server.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessageCallback), null);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10061)
                    OnRefused?.Invoke(ServerAddress);
                else
                    MessageBox.Show(ex.Message, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(ex.Message, "Ошибка отправки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveMessageCallback(IAsyncResult ar)
        {
            try
            {
                if(IsConnected)
                {
                    int receivedBytes = _server.EndReceive(ar);
                    if (receivedBytes == 0) return;

                    Array.Resize(ref _buffer, receivedBytes);
                    string text = Encoding.UTF8.GetString(_buffer);

                    if (!text.Equals("check"))
                        OnMessageReceived?.Invoke(_server, text);

                    Array.Resize(ref _buffer, _server.SendBufferSize);

                    if (IsConnected)
                        _server.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessageCallback), null);
                }
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка приема", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка приема", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public delegate void MessageReceivedHandler(Socket server, string message);
        public event MessageReceivedHandler OnMessageReceived;

        public delegate void OnConnectedHandler(Socket server);
        public event OnConnectedHandler OnConnected;

        public delegate void OnRefusedHandler(IPEndPoint endPoint);
        public event OnRefusedHandler OnRefused;

        public delegate void OnLostConnectionHandler(Socket server);
        public event OnLostConnectionHandler OnConnectionLost;
    }
}
