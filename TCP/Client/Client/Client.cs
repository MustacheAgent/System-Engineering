using AsyncTcpLib;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Client : Form
    {
        AsyncTcpClient client;

        public Client()
        {
            InitializeComponent();
            TxtAddress.Text = GetLocalIP();
            TxtPort.Text = "8080";
            BarServer.Items.Add(new ToolStripStatusLabel());
            BarServer.Items[0].Text = "Нет подключения";

            client = new AsyncTcpClient();
            client.OnConnected += Client_OnConnected;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnRefused += Client_OnRefused;
            client.OnConnectionLost += Client_OnConnectionLost;
            client.OnReconnectAttempt += Client_OnReconnectAttempt;
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
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }

        private void Log(string log)
        {
            StringBuilder logBuilder = new StringBuilder();
            logBuilder.Append(DateTime.Now.ToString() + ": " + log + "\n");
            Action<string> update = logs =>
            {
                TxtRichLog.AppendText(logBuilder.ToString());
            };

            try
            {
                Invoke(update, logBuilder.ToString());
            }
            catch(Exception)
            {
                return;
            }
        }

        /*
        private bool Ping(string address)
        {
            PingReply pingReply;
            using (var ping = new Ping())
                pingReply = ping.Send(address);
            return pingReply.Status == IPStatus.Success;
        }
        */

        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            if (client.IsConnected)
                BarServer.Items[0].Text = "Есть подключение";
            else
                BarServer.Items[0].Text = "Нет подключения";
        }

        private void Client_OnConnected(Socket server)
        {
            Log("Подключен к серверу " + server.RemoteEndPoint.ToString());
        }

        private void Client_OnMessageReceived(Socket server, string message)
        {
            Log("От сервера " + server.RemoteEndPoint.ToString() + " получено сообщение: " + message);
        }

        private void Client_OnRefused(IPEndPoint endPoint)
        {
            Log("Не удалось подключиться к серверу " + endPoint.ToString());
            Invoke((MethodInvoker)delegate ()
            {
                BtnConnect.Text = "Подключиться";
                TxtAddress.ReadOnly = TxtPort.ReadOnly = false;
            });
        }

        private void Client_OnConnectionLost(Socket server)
        {
            Log("Соединение с сервером потеряно");

            Invoke((MethodInvoker)delegate ()
            {
                BtnConnect.Text = "Подключиться";
                TxtAddress.ReadOnly = TxtPort.ReadOnly = false;
            });

            client.Connect(TxtAddress.Text, int.Parse(TxtPort.Text));
        }

        private void Client_OnReconnectAttempt(IPEndPoint serverAddr)
        {
            Log("Попытка переподключения к серверу " + serverAddr.ToString());
        }
    }
}
