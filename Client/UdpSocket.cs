using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public class UdpSocket
    {
        private UdpClient _traanciever;
        private IPAddress _ipAddress;

        public UdpSocket(IPAddress ipAddress,int port)
        {

            _ipAddress = ipAddress;
            _traanciever = new UdpClient();
            try
            {
                _traanciever.Connect(ipAddress, port);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           

        }
       
       public void SendMessage(byte[] message)
        {
            try
            {
                _traanciever.Send(message, message.Length);
            }
            catch(Exception ex)
            {
                _traanciever.Close();
                Console.WriteLine(ex.Message);
            }
            
        }
    }
    
}
