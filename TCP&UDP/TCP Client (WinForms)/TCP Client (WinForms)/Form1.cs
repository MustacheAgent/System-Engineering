﻿using System;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace TCP_Client__WinForms_
{
    public partial class TCPClient : Form
    {
        TcpClient client;
        public TCPClient()
        {
            InitializeComponent();
            client = new();
            TxtAddress.Text = GetLocalIP();
            TxtPort.Text = "8080";
            TimerStatus.Start();
            BarServer.Items.Add(new ToolStripStatusLabel());
            BarServer.Items[0].Text = "Нет подключения";
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (!client.Connected)
            {
                try
                {
                    client.Connect(TxtAddress.Text, int.Parse(TxtPort.Text));
                    BtnConnect.Text = "Отключиться";
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
            else
            {
                try
                {
                    client.Close();
                    BtnConnect.Text = "Подключиться";
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

        private string GetLocalIP()
        {
            using (Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }

        private bool Ping(string address)
        {
            PingReply pingReply;
            using (var ping = new Ping())
                pingReply = ping.Send(address);
            return pingReply.Status == IPStatus.Success;
        }

        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            if (Ping(TxtAddress.Text))
                BarServer.Items[0].Text = "Есть подключение";
            else
                BarServer.Items[0].Text = "Нет подключения";
        }
    }
}