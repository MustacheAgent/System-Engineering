using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace AsyncTcpLib
{
    public class AsyncTcpServer
    {
        private Socket _server;
        private Socket _client;
        private List<Socket> _clients;

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
                _server.Bind(new IPEndPoint(0, Port));
                _server.Listen(0);
                _server.BeginAccept(new AsyncCallback(AcceptCallback), null);
                // ВЫЗВАТЬ СОБЫТИЕ ЗАПУСКА СЕРВЕРА
                IsListening = true;
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                _server.Close();
                IsListening = false;
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] byteMessage = Encoding.ASCII.GetBytes(message);
                _client.BeginSend(byteMessage, 0, byteMessage.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                _client = _server.EndAccept(ar);

                if (OnClientConnected != null)
                {
                    OnClientConnected(_client);
                }

                //_client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                _server.BeginAccept(new AsyncCallback(AcceptCallback), null);
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

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                int bytesSent = _server.EndSend(ar);
                if (OnMessageSent != null)
                    OnMessageSent(_client);
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
                int receivedBytes = _client.EndReceive(ar);

                if (receivedBytes == 0) return;

                Array.Resize(ref _buffer, receivedBytes);
                string text = Encoding.ASCII.GetString(_buffer);
                // ВЫЗВАТЬ СОБЫТИЕ ПОЛУЧЕНИЯ ДАННЫХ
                Array.Resize(ref _buffer, _client.ReceiveBufferSize);

                _client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public delegate void ClientConnectedHandler(Socket client);
        public event ClientConnectedHandler OnClientConnected;

        public delegate void MessageSentHandler(Socket client);
        public event MessageSentHandler OnMessageSent;
    }
}
