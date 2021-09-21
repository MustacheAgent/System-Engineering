using System;
using System.Net.Sockets;
using System.Text;

namespace UDP_Transmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("IP адрес: ");
            string ip = Console.ReadLine();

            Console.WriteLine("Порт: ");
            int port = int.Parse(Console.ReadLine());
            UdpClient transmitter = new(ip, port); ;

            try
            {
                //transmitter = new(ip, port);
                string message = "Привет мир!";
                byte[] data = Encoding.UTF8.GetBytes(message);
                int bytesSent = transmitter.Send(data, data.Length);
            }
            catch(SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            transmitter.Close();
            Console.WriteLine("Обмен завершен");
        }
    }
}
