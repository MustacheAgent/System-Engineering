using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AsyncUdpLib
{
    public class Client
    {
        Socket _client;
        private string sentMessage;
        private string receivedMessage;
        private byte[] buffer;

        public IPEndPoint ClientAddress
        {
            get;
            private set;
        }

        public Client(IPEndPoint address)
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ClientAddress = address;
            EndPoint tmp = ClientAddress;
            _client.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref tmp, new AsyncCallback(ReceiveFromClientCallback), null);
        }

        public void SendMessage(IPEndPoint clientAddress, string msg)
        {
            try
            {
                sentMessage = msg;
                byte[] dgram = Encoding.UTF8.GetBytes(sentMessage);
                _client.BeginSendTo(dgram, 0, dgram.Length, SocketFlags.None, clientAddress, new AsyncCallback(SendToClientCallback), null);
            }
            catch(SocketException ex)
            {

            }
        }

        private void SendToClientCallback(IAsyncResult ar)
        {
            try
            {
                _client.EndSendTo(ar);
                OnMessageSent?.Invoke(ClientAddress, sentMessage);
            }
            catch(SocketException ex)
            {

            }
        }

        private void ReceiveFromClientCallback(IAsyncResult ar)
        {
            try
            {

            }
            catch(SocketException ex)
            {

            }
        }

        public delegate void MessageSentHandler(IPEndPoint client, string message);
        public event MessageSentHandler OnMessageSent;

        public delegate void MessageReceivedHandler(IPEndPoint client, string message);
        public event MessageReceivedHandler OnMessageReceived;
    }
}
