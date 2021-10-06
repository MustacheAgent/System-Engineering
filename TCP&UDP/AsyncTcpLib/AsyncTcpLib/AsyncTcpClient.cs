using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace AsyncTcpLib
{
    public class AsyncTcpClient
    {
        private Socket _client;
        private bool _connected;

        public bool IsConnected
        {
            get
            {
                return _connected;
            }
            private set
            {
                _connected = value;
            }
        }

        public Socket Socket
        {
            get
            {
                return _client;
            }
            private set
            {
                _client = value;
            }
        }

        public AsyncTcpClient() { }

        public void Connect(string address, int port)
        {
            _client.BeginConnect(address, port, new AsyncCallback(ConnectCallback), _client);
        }

        public event ConnectedToServerEventHandler ConnectedToServerEvent;
        protected void OnConnected(ConnectedToServerEventArgs e)
        {
            ConnectedToServerEvent(this, e);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _client = (Socket)ar;
                _client.EndConnect(ar);
                IsConnected = true;
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
    }
}
