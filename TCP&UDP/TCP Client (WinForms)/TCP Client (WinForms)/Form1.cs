using System;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace TCP_Client__WinForms_
{
    public partial class TCPClient : Form
    {
        TcpClient client;

        Socket TcpClient;

        public TCPClient()
        {
            InitializeComponent();
            TxtAddress.Text = GetLocalIP();
            TxtPort.Text = "8080";
            BarServer.Items.Add(new ToolStripStatusLabel());
            BarServer.Items[0].Text = "Нет подключения";
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (client == null)
                client = new();

            if (!client.Connected)
            {
                try
                {
                    client.Connect(TxtAddress.Text, int.Parse(TxtPort.Text));

                    NetworkStream stream = client.GetStream();

                    byte[] buffer = new byte[256];

                    stream.BeginRead(buffer, 0, buffer.Length, )

                    BtnConnect.Text = "Отключиться";

                    TxtAddress.ReadOnly = TxtPort.ReadOnly = false;

                    //TimerStatus.Start();
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
                    //client.Close();
                    BtnConnect.Text = "Подключиться";

                    TxtAddress.ReadOnly = TxtPort.ReadOnly = true;

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
