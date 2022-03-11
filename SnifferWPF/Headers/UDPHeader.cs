using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;

namespace SnifferWPF
{
    class UDPHeader
    {
        private const int HeaderLength = 8;
        private readonly ushort rawSourcePort;
        private readonly ushort rawDestinationPort;
        private readonly ushort rawMessageLength;
        private readonly ushort rawChecksum;
        private readonly byte[] data;

        public byte[] Data { get => data; }

        public UDPHeader(byte[] buffer)
        {
            try
            {
                BinaryReader br = new BinaryReader(new MemoryStream(buffer, 0, buffer.Length));

                rawSourcePort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawDestinationPort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawMessageLength = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawChecksum = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                data = new byte[buffer.Length - HeaderLength];
                Array.Copy(buffer, HeaderLength, data, 0, data.Length);

                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", "\n" + data.Length + "\n");
                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", Encoding.Default.GetString(data));
                //Console.WriteLine(Encoding.Default.GetString(data));
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
