using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace SnifferWPF
{
    enum TransportProtocol { ICMP = 1, TCP = 6, UDP = 17, Other = 0 }

    static class Sniffer
    {
        private static readonly byte[] buffer = new byte[4096];
        private static Socket socket;

        public static void Start(MainWindow window)
        {
            IPAddress[] IPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                socket.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.7"), 0));
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

                byte[] bytesIn = new byte[] { 1, 0, 0, 0 };
                byte[] bytesOut = new byte[] { 1, 0, 0, 0 };
                socket.IOControl(IOControlCode.ReceiveAll, bytesIn, bytesOut);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), window);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n{e.StackTrace}");
            }
        }

        private static void Receive(IAsyncResult result)
        {
            MainWindow window = (MainWindow)result.AsyncState;
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
                    MessageBox.Show("TransportProtocol.Other");
                    break;
            }
            window.PacketList.Dispatcher.Invoke(new Action(() => window.AddPacketToList(ipHeader.Protocol.ToString())));

            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), window);
        }
    }
}
