using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

        public AsyncTcpClient() 
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string address, int port)
        {
            try
            {
                _client.BeginConnect(new IPEndPoint(IPAddress.Parse(address), port), new AsyncCallback(ConnectCallback), null);
            }
            catch(SocketException)
            {

            }
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] byteMessage = Encoding.ASCII.GetBytes(message);
                _client.BeginSend(byteMessage, 0, byteMessage.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }
            catch(Exception)
            {

            }
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
                _client.EndConnect(ar);
                IsConnected = true;
                // ВЫЗВАТЬ СОБЫТИЕ ПРИСОЕДИНЕНИЯ
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
        private void SendCallback(IAsyncResult ar)
        {
            int bytesSent = _client.EndSend(ar);
        }
    }
}
