using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class UdpDatagramm
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public UdpDatagramm(int id,byte[] data)
        {
            Id = id;
            Data = data;
        }
    }
}
