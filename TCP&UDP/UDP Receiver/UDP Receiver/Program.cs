using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDP_Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Порт: ");
            int port = int.Parse(Console.ReadLine());
            UdpClient receiver = new(port);

            try
            {
                IPEndPoint ipAddr = null;
                byte[] data = receiver.Receive(ref ipAddr);
                string message = Encoding.UTF8.GetString(data);
                Console.WriteLine("Принято сообщение: {0}", message);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            receiver.Close();
            Console.WriteLine("Обмен завершен");
        }
    }
}
