using System.Net.Sockets;

namespace AsyncTcpLib
{
    public class ConnectedToServerEventArgs
    {
        public TcpListener Server { set; get; }

        public ConnectedToServerEventArgs()
        {
            
        }
    }

    public delegate void ConnectedToServerEventHandler(object sender, ConnectedToServerEventArgs e);
}
