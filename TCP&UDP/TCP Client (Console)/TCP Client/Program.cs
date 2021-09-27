using System;
using System.Net.Sockets;
using System.Text;

namespace TCP_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("IP адрес: ");
            string ip = Console.ReadLine();

            Console.WriteLine("Порт: ");
            int port = int.Parse(Console.ReadLine());

            try
            {
                TcpClient client = new();
                client.Connect(ip, port);

                byte[] data = new byte[256];
                StringBuilder response = new();
                NetworkStream stream = client.GetStream();

                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable); // пока данные есть в потоке

                Console.WriteLine(response.ToString());

                // Закрываем потоки
                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            Console.WriteLine("Запрос завершен...");
            Console.Read();
        }
    }
}
