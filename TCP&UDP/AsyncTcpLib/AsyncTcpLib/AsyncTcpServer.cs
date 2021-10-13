using System;
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

        private bool _shutdown;
        private bool _listening;

        /// <summary>
        /// Возвращает статус прослушивания сервером входящих подключений.
        /// true - прослушивает, иначе false.
        /// </summary>
        public bool IsListening
        {
            get
            {
                return _listening;
            }
            private set
            {
                _listening = value;
            }
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

        public AsyncTcpServer() { }

        /// <summary>
        /// Запускает сервер и начинает принимать входящие запросы на подключение.
        /// </summary>
        /// <param name="address">IP-адрес.</param>
        /// <param name="port">Порт.</param>
        public void StartServer(string address, int port)
        {
            try
            {
                _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _server.Bind(new IPEndPoint(IPAddress.Parse(address), port));
                _server.Listen(0);
                IsListening = true;
                _shutdown = false;
                // ВЫЗВАТЬ СОБЫТИЕ ЗАПУСКА СЕРВЕРА
                _server.BeginAccept(new AsyncCallback(AcceptCallback), null);
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
            try
            {
                _shutdown = true;
                _server.Close();
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                if(!_shutdown)
                {
                    _client = _server.EndAccept(ar);
                    _buffer = new byte[_client.ReceiveBufferSize];
                    _client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                    _server.BeginAccept(new AsyncCallback(AcceptCallback), null);
                }
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
    }
}
