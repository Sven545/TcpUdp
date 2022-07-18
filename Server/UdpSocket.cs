using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class UdpSocket
    {
        private UdpClient _receiver;
        private IPEndPoint _ip;
        private int _port;
        private bool _isRecieve;

        public event EventHandler<byte[]> PackageIsRecieved; 

        public UdpSocket(int port)
        {
            _port = port;
           
        }
       
        public void StartRecieve()
        {

            try
            {
                _receiver = new UdpClient(_port);
                _isRecieve = true;
                while (_isRecieve)
                {
                  
                    if (_receiver.Available > 0)
                    {
                        byte[] recievedData = _receiver.Receive(ref _ip); // получаем данные
                       //Console.WriteLine(Encoding.Unicode.GetString(recievedData, 0, recievedData.Length));
                        OnPackageIsRecieved(recievedData);

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
