﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public class TcpSocket
    {
        private IPAddress _ipAddress;
        IPEndPoint _ipPoint;
        Socket _socket;
        Socket _handler;
        private int _port;
        private bool _isRecieve;

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
                Console.WriteLine("Tcp сокет создан");
                // связываем сокет с локальной точкой, по которой будем принимать данные
                _socket.Connect(_ipPoint);
                Console.WriteLine($"Tcp подключение создано - {ipAddress}:{port} ");
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
                _socket.Send(sendBytes);
                Console.WriteLine("Сообщение отправлено по TCP: " + Encoding.Unicode.GetString(sendBytes, 0, sendBytes.Length));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public string ReadMessage()
        {
            // получаем ответ
            byte[] data = new byte[256]; // буфер для ответа
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байт
            //if (_socket.Available > 0)
            //{
                do
                {
                
                    bytes = _socket.Receive(data, data.Length, 0);
               
                    
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (_socket.Available > 0);
           // }
            Console.WriteLine("Сообщение из Tcp сокета: " + builder.ToString());
            return builder.ToString();
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
                // _socket.Listen();
                _isRecieve = true;
                while (_isRecieve)
                {
                    _handler = _socket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    if (_handler.Available > 0)
                    {

                        while (_handler.Available > 0)
                        {
                            bytes = _handler.Receive(data);
                            OnPackageIsRecieved(data);
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
        public void StopRecieve()
        {
            _isRecieve = false;
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
