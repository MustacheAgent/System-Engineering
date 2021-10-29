using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System;
using System.Text;

namespace UDP_Receiver
{
    public partial class Receiver : Form
    {
        UdpClient ReceiveClient, FeedbackClient;

        bool isWorking;

        Thread ReceiveThread, FeedbackThread;

        public Receiver()
        {
            InitializeComponent();
            TxtAddress.Text = GetLocalIP();
            TxtSendPort.Text = "8081";
            TxtReceivePort.Text = "8080";

            isWorking = false;
        }

        private void Receive()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, int.Parse(TxtReceivePort.Text));
            while(true)
            {
                byte[] dgram = ReceiveClient.Receive(ref endpoint);
                string msg = Encoding.UTF8.GetString(dgram);
                Log(msg);
                if (msg.Equals("check"))
                    SendFeedback();
            }
        }

        private void SendFeedback()
        {
            IPEndPoint feedback = new IPEndPoint(IPAddress.Parse(TxtAddress.Text), int.Parse(TxtSendPort.Text));
            byte[] dgram = Encoding.UTF8.GetBytes("ok");
            FeedbackClient.Send(dgram, dgram.Length, feedback);
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
                ReceiveClient = new UdpClient(int.Parse(TxtReceivePort.Text));

                ReceiveThread = new Thread(Receive)
                {
                    IsBackground = true
                };

                FeedbackClient = new UdpClient(int.Parse(TxtSendPort.Text));

                ReceiveThread.Start();
                BtnConnect.Text = "Отключиться";
                isWorking = true;
            }
            else
            {
                ReceiveThread.Abort();
                ReceiveClient.Close();
                BtnConnect.Text = "Подключиться";
                isWorking = false;
            }
        }
    }
}
