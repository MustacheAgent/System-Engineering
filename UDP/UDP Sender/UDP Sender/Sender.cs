using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;

namespace UDP_Sender
{
    public partial class Sender : Form
    {
        UdpClient SendClient, FeedbackClient;
        DateTime lastFeedback;
        bool isWorking;

        Thread SendThread, FeedbackThread, AwaitThread;

        public Sender()
        {
            InitializeComponent();
            TxtAddress.Text = GetLocalIP();
            TxtSendPort.Text = "8080";
            TxtReceivePort.Text = "8081";

            isWorking = false;
        }

        private void SendMessage()
        {
            IPEndPoint send = new IPEndPoint(IPAddress.Parse(TxtAddress.Text), int.Parse(TxtSendPort.Text));
            while(true)
            {
                byte[] dgram = Encoding.UTF8.GetBytes("check");
                SendClient.Send(dgram, dgram.Length, send);
                Thread.Sleep(1000);
            }
        }

        private void Feedback()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(TxtAddress.Text), int.Parse(TxtReceivePort.Text));
            while(true)
            {
                byte[] dgram = FeedbackClient.Receive(ref endpoint);
                string msg = Encoding.UTF8.GetString(dgram);
                Log(msg);
                lastFeedback = DateTime.Now;
            }
        }

        private void AwaitMessage()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(TxtAddress.Text), int.Parse(TxtReceivePort.Text));
            while (true)
            {
                Thread.Sleep(2000);
                if (DateTime.Now - lastFeedback > TimeSpan.FromSeconds(2))
                    Log("Нет ответа от клиента " + endpoint.ToString());
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
            catch (Exception)
            {
                return;
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

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (!isWorking)
            {
                SendClient = new UdpClient(int.Parse(TxtSendPort.Text));
                SendThread = new Thread(SendMessage)
                {
                    IsBackground = true
                };
                SendThread.Start();

                FeedbackClient = new UdpClient(int.Parse(TxtReceivePort.Text));
                FeedbackThread = new Thread(Feedback)
                {
                    IsBackground = true
                };
                FeedbackThread.Start();

                AwaitThread = new Thread(AwaitMessage)
                {
                    IsBackground = true
                };
                AwaitThread.Start();

                BtnConnect.Text = "Отключиться";
                TxtAddress.ReadOnly = TxtReceivePort.ReadOnly = TxtSendPort.ReadOnly = true;
                isWorking = true;
            }
            else
            {
                SendThread.Abort();
                SendClient.Close();
                FeedbackThread.Abort();
                FeedbackClient.Close();

                AwaitThread.Abort();

                BtnConnect.Text = "Подключиться";
                TxtAddress.ReadOnly = TxtReceivePort.ReadOnly = TxtSendPort.ReadOnly = false;
                isWorking = false;
            }
        }
    }
}
