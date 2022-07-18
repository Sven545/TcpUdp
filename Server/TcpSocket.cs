using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class TcpSocket
    {
        private IPAddress _ipAddress;
        IPEndPoint _ipPoint;
        Socket _socket;
        Socket _handler;
        private int _port;
        private bool _isRecieveNewConnection;
        private bool _isRecieveNewData;

        public event EventHandler<byte[]> PackageIsRecieved;
        public TcpSocket(IPAddress ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;

            try
            {
                // получаем адреса для запуска сокета
                _ipPoint = new IPEndPoint(_ipAddress, _port);

                // создаем сокет
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // связываем сокет с локальной точкой, по которой будем принимать данные
                _socket.Bind(_ipPoint);
                Console.WriteLine("Tcp сокет создан");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
        public void SendMessage(byte[] sendBytes)
        {
            try
            {
                _handler.Send(sendBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public async Task StartRecieveAsync()
        {
            await Task.Run(() => StartRecieve());
        }
        public void StartRecieve()
        {
            try
            {

                // начинаем прослушивание
                _socket.Listen();
                _isRecieveNewConnection = true;
                Console.WriteLine("Ожидание подключения...");
                while (_isRecieveNewConnection)
                {
                    _handler = _socket.Accept();
                    Console.WriteLine("Клиент подключился");
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data; // буфер для получаемых данных


                    Console.WriteLine("Сообщение по Tcp получено");
                    _isRecieveNewData = true;
                    while (_isRecieveNewData)
                    {
                        if (_handler.Available > 0)
                        {
                            data = new byte[_handler.Available];
                            do
                            {
                                bytes = _handler.Receive(data);
                                OnPackageIsRecieved(data);
                            }
                            while (_handler.Available > 0);
                        }
                        
                    }








                    //Начинаем прием udp пакетов
                    // закрываем сокет
                    //_handler.Shutdown(SocketShutdown.Both);
                    //_handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void StopRecieveConnections()
        {
            _isRecieveNewConnection = false;
        }
        public void StopRecieveData()
        {
            _isRecieveNewData = false;
        }

        public void OnPackageIsRecieved(byte[] recievedBytes)
        {
            if (PackageIsRecieved != null)
            {
                PackageIsRecieved(this, recievedBytes);
            }
        }
    }
}
