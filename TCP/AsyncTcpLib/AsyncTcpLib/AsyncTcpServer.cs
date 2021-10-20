using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AsyncTcpLib
{
    public class AsyncTcpServer
    {
        private Socket _server;
        private Socket _client;

        Thread checkConnectionThread;

        /// <summary>
        /// Возвращает статус прослушивания сервером входящих подключений.
        /// true - прослушивает, иначе false.
        /// </summary>
        public bool IsListening
        {
            get;
            private set;
        }

        /// <summary>
        /// Порт для прослушивания входящих запросов.
        /// </summary>
        public int Port
        {
            get; set;
        }

        /// <summary>
        /// Возвращает статус подключения сервера к указанному порту.
        /// true - подключен, иначе false.
        /// </summary>
        public bool IsBound
        {
            get
            {
                return _server.IsBound;
            }
        }

        byte[] _buffer;

        public AsyncTcpServer(int port)
        {
            Port = port;
            IsListening = false;
        }

        /// <summary>
        /// Запускает сервер и начинает принимать входящие запросы на подключение.
        /// </summary>
        public void StartServer()
        {
            if (IsListening) return;
            try
            {
                _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _server.NoDelay = true;
                _server.Bind(new IPEndPoint(0, Port));
                _server.Listen(0);
                _server.BeginAccept(new AsyncCallback(AcceptCallback), null);
                IsListening = true;
                OnServerStart?.Invoke(_server);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка запуска", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Останавливает сервер.
        /// </summary>
        public void StopServer()
        {
            if (!IsListening) return;

            try
            {
                //_server.Disconnect(false);
                //_server.Shutdown(SocketShutdown.Both);
                _server.Close();
                IsListening = false;
                OnServerStop?.Invoke(_server);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка остановки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Отправляет сообщение клиенту.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public void SendMessage(string message)
        {
            try
            {   
                byte[] byteMessage = Encoding.UTF8.GetBytes(message);
                _client.BeginSend(byteMessage, 0, byteMessage.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка отправки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка отправки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckConnection()
        {
            while (true)
            {
                try
                {
                    //byte[] tmp = new byte[] { 0 };
                    byte[] tmp = Encoding.UTF8.GetBytes("check");
                    int sent = _client.Send(tmp);
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка проверки подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Thread.Sleep(1000);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                _client = _server.EndAccept(ar);
                _buffer = new byte[_server.ReceiveBufferSize];
                _client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                OnClientConnected?.Invoke(_client);

                checkConnectionThread = new Thread(new ThreadStart(CheckConnection))
                {
                    IsBackground = true
                };

                checkConnectionThread.Start();

                if (IsListening)
                    _server.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(ObjectDisposedException)
            {
                return;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                _client.EndSend(ar);
                OnMessageSent?.Invoke(_client);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка отправки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (_client.Connected)
                {
                    int receivedBytes = _client.EndReceive(ar);

                    if (receivedBytes == 0) return;

                    Array.Resize(ref _buffer, receivedBytes);
                    string text = Encoding.ASCII.GetString(_buffer);

                    if (!string.IsNullOrEmpty(text) /*&& !text.Equals("check")*/)
                    {
                        OnMessageReceived?.Invoke(_client, text);
                    }

                    Array.Resize(ref _buffer, _client.ReceiveBufferSize);

                    if (IsListening)
                        _client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                }
            }
            catch(SocketException ex)
            {
                if (ex.ErrorCode == 10054)
                    OnClientDisconnected?.Invoke(_client);
                else
                    MessageBox.Show(ex.Message, "Ошибка приема", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public delegate void ServerStartHandler(Socket server);
        public event ServerStartHandler OnServerStart;

        public delegate void ServerStopHandler(Socket server);
        public event ServerStopHandler OnServerStop;

        public delegate void ClientConnectedHandler(Socket client);
        public event ClientConnectedHandler OnClientConnected;

        public delegate void ClientDisconnectedHandler(Socket client);
        public event ClientDisconnectedHandler OnClientDisconnected;

        public delegate void MessageSentHandler(Socket client);
        public event MessageSentHandler OnMessageSent;

        public delegate void MessageReceiverHandler(Socket client, string message);
        public event MessageReceiverHandler OnMessageReceived;
    }
}
