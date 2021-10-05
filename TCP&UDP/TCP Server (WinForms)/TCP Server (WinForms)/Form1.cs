using System;
using System.Net;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TCP_Server__WinForms_
{
    public partial class TCPServer : Form
    {
        TcpListener server;
        //string message = "курлык";

        List<TcpClient> clients;

        //public ManualResetEvent tcpClientConnected;

        public TCPServer()
        {
            InitializeComponent();

            clients = new();

            TxtAddress.Text = GetLocalIP();
            TxtPort.Text = "8080";
            
            BarServer.Items.Add(new ToolStripStatusLabel());
            BarServer.Items[0].Text = "недоступен";

            //tcpClientConnected = new(false);
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
                    server.BeginAcceptTcpClient(AcceptClientCallback, server);

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
                    server.Stop();
                    server = null;
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

        private void AcceptClientCallback(IAsyncResult ar)
        {
            TcpListener listener = ar.AsyncState as TcpListener;

            try
            {
                TcpClient client = listener.EndAcceptTcpClient(ar);
                clients.Add(client);

                Log("Принят запрос от на подключение: " + ((IPEndPoint)client.Client.RemoteEndPoint).Address);

                //tcpClientConnected.Set();
            }
            catch(Exception)
            {
                return;
            }
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

        private void SendDataToClient(TcpClient client, string message)
        {
            // получаем сетевой поток для чтения и записи
            NetworkStream stream = client.GetStream();

            // преобразуем сообщение в массив байтов
            byte[] data = Encoding.UTF8.GetBytes(message);

            // отправка сообщения
            stream.Write(data, 0, data.Length);

            //stream.Close();
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            string message = TxtRichMessage.Text;
            foreach(var client in clients)
            {
                SendDataToClient(client, message);
                Log("Клиенту: " + ((IPEndPoint)client.Client.RemoteEndPoint).Address + " отправлено сообщение: " + message);
            }
        }
    }
}
