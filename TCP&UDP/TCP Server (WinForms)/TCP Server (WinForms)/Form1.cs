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
            BarServer.Items[0].Text = "недоступен";

            server = new();
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (!PortInUse(int.Parse(TxtPort.Text)))
            {
                try
                {
                    server.StartServer(TxtAddress.Text, int.Parse(TxtPort.Text));
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
                    TimerStatus.Stop();
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

        public bool PortInUse(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    return true;
                }
            }

            return false;
        }

        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            if (server.IsBound)
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
            logBuilder.Append("\n" + DateTime.Now.ToString() + ": " + log);
            Invoke(new Action<string>(UpdateLog), logBuilder.ToString());
        }
    }
}
