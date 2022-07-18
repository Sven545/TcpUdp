using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        private static IPAddress ipAddress;
        private static int _tcpPort;
        private static int _udpPort;
        private static string fileDirectory;
        static TcpSocket tcpSocket;
        private static string _fileName;


        static void Main(string[] args)
        {
            ipAddress = IPAddress.Parse("127.0.0.1");
            _tcpPort = 8006;
            fileDirectory = Environment.CurrentDirectory;

            tcpSocket = new TcpSocket(ipAddress, _tcpPort);
            
            tcpSocket.StartRecieveAsync();
            tcpSocket.PackageIsRecieved += TcpSocket_PackageIsRecieved;

           
            Console.ReadLine();
            /*
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(ipAddress, port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen();

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                    // отправляем ответ если пришли параметры
                    string message = "ваше сообщение доставлено";
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);
                    UdpSocket udpSocket = new UdpSocket(8007);
                    udpSocket.PackageIsRecieved += UdpSocket_PackageIsRecieved;
                    udpSocket.StartRecieve();
                    //Начинаем прием udp пакетов
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            */
        }

        private static void TcpSocket_PackageIsRecieved(object sender, byte[] e)
        {
            string clientData = Encoding.Unicode.GetString(e, 0, e.Length);
            Console.WriteLine(clientData);
            var clientDataArray = clientData.Split(' ');
            if(clientData.Length==2)
            {
                _fileName = clientDataArray[0];
                //_udpPort = int.Parse(udpDataArray[1]);
                if (int.TryParse(clientDataArray[1], out _udpPort))
                {
                    tcpSocket.SendMessage(Encoding.Unicode.GetBytes("Udp data recieved"));
                    UdpSocket udpSocket = new UdpSocket(_udpPort);
                    udpSocket.PackageIsRecieved += UdpSocket_PackageIsRecieved;
                    udpSocket.StartRecieve();
                }
            }
            else
            {
                if(clientData == "End")
                {
                    //Логика записи в файл
                }
            }
           
            // отправляем ответ если пришли параметры
            /*
            tcpSocket.SendMessage(Encoding.Unicode.GetBytes("Udp data delivered"));

            UdpSocket udpSocket = new UdpSocket(8007);
            udpSocket.PackageIsRecieved += UdpSocket_PackageIsRecieved;
            udpSocket.StartRecieve();
            */
            Console.WriteLine();
        }

        private static void UdpSocket_PackageIsRecieved(object sender, byte[] e)
        {
            Console.WriteLine(Encoding.Unicode.GetString(e, 0, e.Length));
            tcpSocket.SendMessage(Encoding.Unicode.GetBytes("Response"));
        }
    }
}
