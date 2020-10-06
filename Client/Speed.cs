using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Speed
    {
        NetworkInterface[] nicArr = null;
        IPv4InterfaceStatistics interfaceStats = null;
        private long _lastBytesRecevied;
        private long _lastBytesSent;
        private DateTime _lastReceivedMesurement;
        private DateTime _lastSentMesurement;
        private void Init()
        {
            _lastReceivedMesurement = DateTime.UtcNow;
            _lastBytesRecevied = interfaceStats.BytesReceived;

            _lastSentMesurement = DateTime.UtcNow;
            _lastBytesSent = interfaceStats.BytesSent;
        }
     
       /* private double getKBDownloadSpeed()
        {
            double result = (interfaceStats.BytesReceived - _lastBytesRecevied) / (DateTime.UtcNow - _lastMesurement).TotalSeconds;

            _lastReceivedMesurement = DateTime.UtcNow;
            _lastBytesRecevied = interfaceStats.BytesReceived;

            return result / 1024d;
        }*/

        private double getKBUploadSpeed()
        {
            double result = (interfaceStats.BytesSent - _lastBytesSent) / (DateTime.UtcNow - _lastSentMesurement).TotalSeconds;

            _lastSentMesurement = DateTime.UtcNow;
            _lastBytesSent = interfaceStats.BytesSent;

            return result / 1024d;
        }
        void getUpload()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPGlobalStatistics stats = null;
            stats = properties.GetIPv4GlobalStatistics();
            Console.WriteLine(stats.ForwardingEnabled);
            Console.WriteLine(stats.ReceivedPacketsDelivered);
           
        }
    }
}
