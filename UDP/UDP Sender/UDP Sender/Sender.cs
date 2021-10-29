using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace UDP_Sender
{
    public partial class Sender : Form
    {
        public Sender()
        {
            InitializeComponent();
            TxtAddress.Text = GetLocalIP();
            TxtPort.Text = "8080";
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
    }
}
