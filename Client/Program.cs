using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    internal class Program
    {

        private static UdpSocket udpSocket;
        private static TcpSocket tcpSocket;

        private static IPAddress ipAddress;
        private static int _tcpPort;
        private static int udpPort;
        private static string filePath;
        private static string fileName;
        private static string udpDataForServer;
        private static byte[] fileBytes;
        private static int timeOutUdpResponse=500;
        private static bool _udpPackageIsTransferred;

        static void Main(string[] args)
        {
            ipAddress = IPAddress.Parse("127.0.0.1");
            _tcpPort = 8006;
            udpPort = 8007;
            filePath = string.Concat(Environment.CurrentDirectory, @"\", "testFile.txt");
            try
            {
                if (File.Exists(filePath))
                {
                    fileBytes = File.ReadAllBytes(filePath);
                    fileName = Path.GetFileName(filePath);
                    udpDataForServer = string.Concat(fileName, " ", udpPort);

                    tcpSocket = new TcpSocket(ipAddress, _tcpPort);
                   // tcpSocket.StartRecieveAsync();
                    tcpSocket.SendMessage(Encoding.Unicode.GetBytes(udpDataForServer));
                    string tcpResponce = tcpSocket.ReadMessage();
                    Console.WriteLine(tcpResponce);
                    if (tcpResponce == "Udp data recieved")
                    {
                        udpSocket = new UdpSocket(ipAddress, udpPort);
                        SendPackages(GetUdpPackages(512, fileBytes));
                    }
                        // tcpSocket.PackageIsRecieved += TcpSocket_PackageIsRecieved;

                        /*
                        IPEndPoint ipPoint = new IPEndPoint(ipAddress, tcpPort);

                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        // подключаемся к удаленному хосту
                        socket.Connect(ipPoint);
                        // Console.Write(udpDataForServer);
                        //string message = Console.ReadLine();
                        byte[] data = Encoding.Unicode.GetBytes(udpDataForServer);
                        // Console.WriteLine(GetUdpPackages(300,data).Count);
                        socket.Send(data);
                        */
                        /*
                        UdpSocket udpSocket = new UdpSocket(ipAddress, udpPort);
                        foreach(var package in GetUdpPackages(256,fileBytes))
                        {
                            udpSocket.SendMessage(package.Data);

                            // получаем ответ
                            data = new byte[256]; // буфер для ответа
                            StringBuilder builder = new StringBuilder();
                            int bytes = 0; // количество полученных байт
                            Thread.Sleep(timeOutUdpResponse);

                            while (socket.Available > 0)
                            {
                                bytes = socket.Receive(data, data.Length, 0);
                                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                            }
                            Console.WriteLine("ответ сервера: " + builder.ToString());
                        }
                        */

                    }
                
               

                

                // закрываем сокет
               // socket.Shutdown(SocketShutdown.Both);
                //socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();

        }

        private static void TcpSocket_PackageIsRecieved(object sender, byte[] e)
        {
            string serverData = Encoding.Unicode.GetString(e, 0, e.Length);
            if(serverData== "Udp data recieved")
            {
                 udpSocket = new UdpSocket(ipAddress, udpPort);
                SendPackages(GetUdpPackages(512, fileBytes));
                /*
                foreach (var package in GetUdpPackages(256, fileBytes))
                {
                    udpSocket.SendMessage(package.Data);

                    // получаем ответ
                    data = new byte[256]; // буфер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байт
                    Thread.Sleep(timeOutUdpResponse);

                    while (socket.Available > 0)
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    Console.WriteLine("ответ сервера: " + builder.ToString());
                }
                */
            }
        }
        private static void SendPackages(List<UdpDatagramm> packages)
        {
            int commonCountPackages = packages.Count;
            int currentPackageNumber = 1;
            udpSocket = new UdpSocket(ipAddress, udpPort);
            while (currentPackageNumber<=commonCountPackages)
            {
                _udpPackageIsTransferred = false;
                udpSocket.SendMessage(packages[currentPackageNumber-1].Data);
                Thread.Sleep(timeOutUdpResponse);
                string tcpResponce = tcpSocket.ReadMessage();
                if (tcpResponce== "Response")
                {
                    currentPackageNumber++;
                    Console.WriteLine(currentPackageNumber-1+" "+tcpResponce);
                }
            }
            tcpSocket.SendMessage(Encoding.Unicode.GetBytes("End"));
            
        }
        private static List<UdpDatagramm> GetUdpPackages(int bytesPerPackage,byte[] fileBytes)
        {
            if(bytesPerPackage<fileBytes.Length)
            {
                int countPackages = fileBytes.Length / bytesPerPackage;
                List<UdpDatagramm> packages = new List<UdpDatagramm>();
                int packageNumber = 1;
                byte[] packageBytes = new byte[bytesPerPackage+1];
                int currentByteNum = 0;
                while(currentByteNum<fileBytes.Length-bytesPerPackage)
                {
                    packageBytes[0] = Convert.ToByte(packageNumber);
                    Array.Copy(fileBytes, currentByteNum, packageBytes, 1, bytesPerPackage);
                    packages.Add(new UdpDatagramm(packageNumber, packageBytes));
                    packageNumber++;
                    currentByteNum = currentByteNum + bytesPerPackage;
                }
              
                if (packages.Count * bytesPerPackage < fileBytes.Length)
                {
                    int div = fileBytes.Length % bytesPerPackage;
                    packageBytes = new byte[div];
                    Array.Copy(fileBytes, packages.Count, packageBytes, 0, div);
                    packages.Add(new UdpDatagramm(packageNumber, packageBytes));
                }
                return packages;
            }
            else
            {
                return new List<UdpDatagramm>() { new UdpDatagramm(1, fileBytes) };
            }
           
        }
    }
}
