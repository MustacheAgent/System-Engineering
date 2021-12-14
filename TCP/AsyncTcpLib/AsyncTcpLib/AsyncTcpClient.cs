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

        Thread checkThread, waitThread;

        public bool IsConnected
        {
            get
            {
                return _server.Connected;
            }
        }

        public IPEndPoint ServerAddress
        {
            get; private set;
        }

        public AsyncTcpClient() 
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            checkThread = new Thread(CheckConnection)
            {
                IsBackground = true
            };
        }

        public AsyncTcpClient(Socket server)
        {
            _server = server;
        }

        public void Connect(string address, int port)
        {
            try
            {
                _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerAddress = new IPEndPoint(IPAddress.Parse(address), port);
                _server.BeginConnect(ServerAddress, new AsyncCallback(ConnectCallback), null);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения к серверу", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Connect(IPEndPoint address)
        {
            try
            {
                _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerAddress = address;
                _server.BeginConnect(ServerAddress, new AsyncCallback(ConnectCallback), null);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения к серверу", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Disconnect()
        {
            try
            {
                checkThread.Abort();
                _server.Close();
                _server.Dispose();
                _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка отключения от сервера", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(ex.Message, "Ошибка отправки сообщения клиентом", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка отправки сообщения клиентом", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckConnection()
        {
            while(true)
            {
                try
                {
                    byte[] tmp = Encoding.UTF8.GetBytes("check");
                    int sent = _server.Send(tmp);
                }
                catch (SocketException ex)
                {
                    if(ex.ErrorCode.Equals(10054))
                    {
                        if(OnConnectionLost != null) OnConnectionLost.Invoke(_server);
                        checkThread.Abort();
                    }
                    else
                    {
                        MessageBox.Show(ex.Message, "Ошибка проверки подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _server.EndConnect(ar);
                if(OnConnected != null) OnConnected.Invoke(_server);

                checkThread = new Thread(CheckConnection)
                {
                    IsBackground = true
                };
                checkThread.Start();

                _buffer = new byte[_server.SendBufferSize];
                _server.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessageCallback), null);
            }
            catch (SocketException ex)
            {
                if(OnReconnectAttempt != null) OnReconnectAttempt.Invoke(ServerAddress);
                _server.Close();
                Connect(ServerAddress);
                /*
                if (ex.ErrorCode == 10061)
                {
                    OnRefused?.Invoke(ServerAddress);
                }   
                else
                {
                    OnReconnectAttempt?.Invoke(ServerAddress);
                    _server.Close();
                    _server.BeginConnect(ServerAddress, new AsyncCallback(ConnectCallback), null);
                }
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения к серверу", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(ex.Message, "Ошибка отправки сообщения клиентом", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        if(OnMessageReceived != null) OnMessageReceived.Invoke(_server, text);

                    Array.Resize(ref _buffer, _server.SendBufferSize);

                    if (IsConnected)
                        _server.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessageCallback), null);
                }
            }
            catch(SocketException ex)
            {
                if(ex.ErrorCode.Equals(10054))
                    return;
                else
                {
                    if (OnReconnectAttempt != null) OnReconnectAttempt.Invoke(ServerAddress);
                    try
                    {
                        _server.Close();
                    }
                    catch(Exception)
                    {

                    }
                    Connect(ServerAddress);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка приема сообщения клиентом", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public delegate void OnReconnectAttemptHandler(IPEndPoint serverAddr);
        public event OnReconnectAttemptHandler OnReconnectAttempt;
    }
}
