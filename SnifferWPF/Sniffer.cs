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
        private static MainWindow window;
        private static bool stopPressed;
        private static IPAddress currentInterface;

        public static void Init(MainWindow window)
        {
            Sniffer.window = window;
        }

        public static void Start(IPAddress ipAddress)
        {
            currentInterface = ipAddress;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                socket.Bind(new IPEndPoint(currentInterface, 0));
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

                byte[] bytesIn = new byte[] { 1, 0, 0, 0 };
                byte[] bytesOut = new byte[] { 1, 0, 0, 0 };
                socket.IOControl(IOControlCode.ReceiveAll, bytesIn, bytesOut);

                stopPressed = false;
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), window);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.GetType()}\n{e.Message}\n{e.StackTrace}");
            }
        }

        public static void Stop()
        {
            stopPressed = true;
        }

        private static void Receive(IAsyncResult result)
        {
            try
            {
                MainWindow window = (MainWindow)result.AsyncState;
                int length = socket.EndReceive(result);

                if (!stopPressed)
                {
                    IPHeader ipHeader = new IPHeader(buffer, length);
                    window.Dispatcher.Invoke(new Action(() => window.AddPacket(ipHeader)));
                    Start(currentInterface);
                }
            }
            catch (ArgumentException)
            { }
            catch (Exception e)
            {
                MessageBox.Show($"{e.GetType()}\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
