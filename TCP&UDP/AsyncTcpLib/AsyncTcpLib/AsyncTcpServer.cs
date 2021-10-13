using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AsyncTcpLib
{
    public class AsyncTcpServer
    {
        Socket _server;
        Socket _client;

        byte[] _buffer;

        public AsyncTcpServer()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartServer(string address, int port)
        {
            try
            {
                _server.Bind(new IPEndPoint(IPAddress.Parse(address), port));
                _server.Listen(0);
                _server.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
            catch(SocketException ex)
            {

            }
        }


        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                _client = _server.EndAccept(ar);
                _buffer = new byte[_client.ReceiveBufferSize];
                _client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                _server.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
            catch(SocketException ex)
            {

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
            catch
            {

            }
        }
    }
}
