using System;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using AsyncTcpLib;
using System.Text;

namespace TCP_Client__WinForms_
{
    public partial class TCPClient : Form
    {
        AsyncTcpClient client;

        public TCPClient()
        {
            InitializeComponent();
            TxtAddress.Text = GetLocalIP();
            TxtPort.Text = "8080";
            BarServer.Items.Add(new ToolStripStatusLabel());
            BarServer.Items[0].Text = "Нет подключения";

            client = new();
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnRefused += Client_OnRefused;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (!client.IsConnected)
            {
                try
                {
                    client.Connect(TxtAddress.Text, int.Parse(TxtPort.Text));
                    BtnConnect.Text = "Отключиться";
                    TxtAddress.ReadOnly = TxtPort.ReadOnly = true;
                    TimerStatus.Start();
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
                    client.Disconnect();
                    BtnConnect.Text = "Подключиться";
                    TxtAddress.ReadOnly = TxtPort.ReadOnly = false;
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

        private void Log(string log)
        {
            StringBuilder logBuilder = new();
            logBuilder.Append(DateTime.Now.ToString() + ": " + log + "\n");
            Action<string> update = logs =>
            {
                TxtRichMessage.AppendText(logBuilder.ToString());
            };
            Invoke(update, logBuilder.ToString());
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
            if (client.IsConnected)
                BarServer.Items[0].Text = "Есть подключение";
            else
                BarServer.Items[0].Text = "Нет подключения";
        }

        private void Client_OnMessageReceived(Socket server, string message)
        {
            Log("От сервера " + server.RemoteEndPoint.ToString() + " получено сообщение: " + message);
        }

        private void Client_OnRefused(IPEndPoint endPoint)
        {
            Log("Не удалось подключиться к серверу " + endPoint.ToString());
            Action<string> update = text =>
            {
                BtnConnect.Text = "Подключиться";
                TxtAddress.ReadOnly = TxtPort.ReadOnly = false;
            };
            Invoke(update, "0");
        }
    }
}
