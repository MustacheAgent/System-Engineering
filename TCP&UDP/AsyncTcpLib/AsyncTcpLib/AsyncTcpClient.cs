﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace AsyncTcpLib
{
    public class AsyncTcpClient
    {
        private Socket _server;
        private byte[] _buffer;

        public bool IsConnected
        {
            get; private set;
        }

        public IPEndPoint EndPoint
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
                EndPoint = new IPEndPoint(IPAddress.Parse(address), port);
                _server.BeginConnect(EndPoint, new AsyncCallback(ConnectCallback), null);
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
                _server.Close();
                _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IsConnected = false;
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

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _server.EndConnect(ar);
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
            try
            {
                int bytesSent = _server.EndSend(ar);
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
                int receivedBytes = _server.EndReceive(ar);

                if (receivedBytes == 0) return;

                Array.Resize(ref _buffer, receivedBytes);
                string text = Encoding.ASCII.GetString(_buffer);
                if (OnMessageReceived != null)
                {
                    OnMessageReceived(_server, text);
                }
                Array.Resize(ref _buffer, _server.ReceiveBufferSize);

                _server.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public delegate void MessageReceivedHandler(Socket server, string message);
        public event MessageReceivedHandler OnMessageReceived;
    }
}
