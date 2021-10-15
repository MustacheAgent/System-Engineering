using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using AsyncTcpLib;

namespace TCP_Server__WinForms_
{
    public partial class TCPServer : Form
    {
        AsyncTcpServer server;

        public TCPServer()
        {
            InitializeComponent();

            TxtAddress.Text = GetLocalIP();
            TxtPort.Text = "8080";
            
            BarServer.Items.Add(new ToolStripStatusLabel());
            BarServer.Items[0].Text = "Недоступен";

            server = new(int.Parse(TxtPort.Text));
            server.OnClientConnected += Server_OnClientConnected;
            server.OnMessageSent += Server_OnMessageSent;
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
            server.SendMessage(TxtRichMessage.Text);
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

        private void Server_OnClientConnected(Socket client)
        {
            Log("Присоединился клиент: " + client.RemoteEndPoint.ToString());
        }

        private void Server_OnMessageSent(Socket client)
        {
            Log("Клиенту " + client.RemoteEndPoint.ToString() + " отправлено сообщение: " + TxtRichMessage.Text);
        }

        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            if (server.IsListening)
                BarServer.Items[0].Text = "Доступен";
            else
                BarServer.Items[0].Text = "Недоступен";
        }

        private void UpdateLog(string update)
        {
            TxtRichLog.AppendText(update);
        }

        private void Log(string log)
        {
            StringBuilder logBuilder = new();
            logBuilder.Append(DateTime.Now.ToString() + ": " + log + "\n");
            Invoke(new Action<string>(UpdateLog), logBuilder.ToString());
        }
    }
}
