using System;
using System.Net;
using System.Net.Sockets;

namespace SnifferWPF
{
    enum TransportProtocol { ICMP = 1, TCP = 6, UDP = 17, Other = 0 }
    class Program
    {
        private static byte[] buffer = new byte[8192];
        //private static Socket socket;

        public static void Start()
        {
            IPAddress[] IPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                socket.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.7"), 0));
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

                byte[] bytesIn = new byte[] { 1, 0, 0, 0 };
                byte[] bytesOut = new byte[] { 1, 0, 0, 0 };
                socket.IOControl(IOControlCode.ReceiveAll, bytesIn, bytesOut);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), socket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            while (true) { }
        }

        static void Receive(IAsyncResult result)
        {
            Socket socket = result.AsyncState as Socket;
            //Console.WriteLine(result.AsyncState);
            int length = socket.EndReceive(result);
            IPHeader ipHeader = new IPHeader(buffer, length);
            switch ((TransportProtocol)ipHeader.Protocol)
            {
                case TransportProtocol.TCP:
                    TCPHeader tcpHeader = new TCPHeader(ipHeader.Data);
                    break;

                case TransportProtocol.UDP:
                    UDPHeader udpHeader = new UDPHeader(ipHeader.Data);
                    break;

                case TransportProtocol.ICMP:
                    ICMPHeader icmpHeader = new ICMPHeader(ipHeader.Data);
                    break;

                case TransportProtocol.Other:
                    Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                    break;
            }
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), socket);
        }
    }
}
