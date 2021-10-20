using AsyncTcpLib;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {
        AsyncTcpServer server;
        string message;

        public Server()
        {
            InitializeComponent();

            TxtAddress.Text = GetLocalIP();
            TxtPort.Text = "8080";

            BarServer.Items.Add(new ToolStripStatusLabel());
            BarServer.Items[0].Text = "Недоступен";

            server = new AsyncTcpServer(int.Parse(TxtPort.Text));
            server.OnClientConnected += Server_OnClientConnected;
            server.OnMessageSent += Server_OnMessageSent;
            server.OnMessageReceived += Server_OnMessageReceived;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (!server.IsListening)
            {
                try
                {
                    server.StartServer();
                    BtnConnect.Text = "Остановить";
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
                    server.StopServer();
                    BtnConnect.Text = "Запустить";
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

        private void BtnSend_Click(object sender, EventArgs e)
        {
            message = TxtRichMessage.Text;
            server.SendMessage(message);
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

        private void Server_OnClientConnected(Socket client)
        {
            Invoke((Action<string>)Log, "Присоединился клиент: " + client.RemoteEndPoint.ToString());
            //Log("Присоединился клиент: " + client.RemoteEndPoint.ToString());
        }

        private void Server_OnMessageSent(Socket client)
        {
            Invoke((Action<string>)Log, "Клиенту " + client.RemoteEndPoint.ToString() + " отправлено сообщение: " + message);
            //Log("Клиенту " + client.RemoteEndPoint.ToString() + " отправлено сообщение: " + TxtRichMessage.Text);
        }

        private void Server_OnMessageReceived(Socket client, string message)
        {
            Invoke((Action<string>)Log, "От клиента " + client.RemoteEndPoint.ToString() + " получено сообщение: " + message);
            //Log("От клиента " + client.RemoteEndPoint.ToString() + " получено сообщение: " + message);
        }

        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            if (server.IsListening)
                BarServer.Items[0].Text = "Доступен";
            else
                BarServer.Items[0].Text = "Недоступен";
        }

        private void Log(string log)
        {
            StringBuilder logBuilder = new StringBuilder();
            logBuilder.Append(DateTime.Now.ToString() + ": " + log + "\n");
            TxtRichLog.AppendText(logBuilder.ToString());
            /*
            Action<string> update = logs =>
            {
                TxtRichLog.AppendText(logBuilder.ToString());
            };
            Invoke(update, logBuilder.ToString());
            */
        }
    }
}
