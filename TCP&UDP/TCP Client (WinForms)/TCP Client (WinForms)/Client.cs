using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace TCP_Client__WinForms_
{
    public class Client
    {
        private TcpClient _client;
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

        public Client()
        {
            _client = new();
            IsConnected = false;
        }

        public void Connect(string address, int port)
        {
            _client.BeginConnect(address, port, ConnectCallback, _client);
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
                _client = ar as TcpClient;
                _client.EndConnect(ar);
                IsConnected = true;
                OnConnected(new ConnectedToServerEventArgs());
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
    }
}
