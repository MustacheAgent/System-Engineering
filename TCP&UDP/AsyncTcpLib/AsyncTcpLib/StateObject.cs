using System.Net.Sockets;
using System.Text;

namespace AsyncTcpLib
{
    internal class StateObject
    {     
        public int bufferSize;

        // Receive buffer.
        public byte[] buffer;

        // Received data string.
        public StringBuilder received;

        // Client socket.
        public Socket workSocket;

        public StateObject(Socket socket, int bufferSize = 1024)
        {
            workSocket = socket;
            this.bufferSize = bufferSize;
            buffer = new byte[this.bufferSize];
            received = new StringBuilder();
        }
    }
}
