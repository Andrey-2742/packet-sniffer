using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace SnifferWPF
{
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
            int length = socket.EndReceive(result);
            IPHeader ipHeader = new IPHeader(buffer, length);
            window.Dispatcher.Invoke(new Action(() => window.CapturedPackets.Add(ipHeader)));

            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), window);
        }
    }
}
