using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class UdpPackagesReciever
    {
        private UdpClient _receiver;
        private IPEndPoint _ip;
        private int _port;
        private Socket _tcpCallBackSocket;
        public List<UdpDatagramm> RecievedPackages { get; private set; } = new List<UdpDatagramm>();
      
        private bool _isRecieve;
        public UdpPackagesReciever(int port, Socket tcpCallBackSocket)
        {
            _port = port;
            _tcpCallBackSocket = tcpCallBackSocket;
            
        }
        
        public void StopRecieve()
        {
            _isRecieve = false;
        }
        public void StartRecieve()
        {
           
            try
            {
                _receiver = new UdpClient(_port);
                _isRecieve = true;
                int packageNumber = 1;
                while (_isRecieve)
                {
                    
                    if (_receiver.Available > 0)
                    {
                        byte[] recievedData = _receiver.Receive(ref _ip); // получаем данные
                        RecievedPackages.Add(new UdpDatagramm(packageNumber, recievedData));
                        packageNumber++;

                        Console.WriteLine(Encoding.Unicode.GetString(recievedData, 0, recievedData.Length));

                        byte[] callBackData = Encoding.Unicode.GetBytes(packageNumber.ToString());
                        _tcpCallBackSocket.Send(callBackData);                       
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _receiver.Close();
            }
        }
    }
}
