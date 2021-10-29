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
        UdpClient SendClient;
        UdpClient FeedbackClient;

        IPEndPoint SendAddress, ReceiveAddress;

        bool isWorking;

        Thread SendThread, FeedbackThread;

        public Sender()
        {
            InitializeComponent();
            TxtAddress.Text = GetLocalIP();
            TxtSendPort.Text = "8080";
            TxtReceivePort.Text = "8081";

            isWorking = false;

            SendClient = new UdpClient();
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
                ReceiveAddress = new IPEndPoint(IPAddress.Parse(TxtAddress.Text), int.Parse(TxtReceivePort.Text));
                SendThread = new Thread(SendMessage)
                {
                    IsBackground = true
                };

                SendThread.Start();
                BtnConnect.Text = "Отключиться";
                isWorking = true;
            }
            else
            {
                SendThread.Abort();
                BtnConnect.Text = "Подключиться";
                isWorking = false;
            }
        }
    }
}
