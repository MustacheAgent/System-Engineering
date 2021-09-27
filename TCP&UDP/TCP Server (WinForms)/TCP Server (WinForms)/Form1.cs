using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace TCP_Server__WinForms_
{
    public partial class TCPServer : Form
    {
        TcpListener server;

        public TCPServer()
        {
            InitializeComponent();
            TxtAddress.Text = GetLocalIP();
            TxtPort.Text = "8080";
            TimerStatus.Start();
            BarServer.Items.Add(new ToolStripStatusLabel());
            BarServer.Items[0].Text = "недоступен";
            //server = new();
        }

        private void BtnConnect_Click(object sender, System.EventArgs e)
        {
            if (!PortInUse(int.Parse(TxtPort.Text)))
            {
                try
                {
                    server = new(IPAddress.Parse(TxtAddress.Text), int.Parse(TxtPort.Text));
                    server.Start();
                    BtnConnect.Text = "Остановить";
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
                    server.Stop();
                    server = null;
                    BtnConnect.Text = "Запустить";
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
            if (PortInUse(int.Parse(TxtPort.Text)))
                BarServer.Items[0].Text = "Доступен";
            else
                BarServer.Items[0].Text = "Недоступен";
        }
    }
}
