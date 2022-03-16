using SnifferWPF.Headers;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;

namespace SnifferWPF
{
    class UDPHeader : ITransportLevelHeader
    {
        private const int HeaderLength = 8;
        private readonly ushort rawSourcePort;
        private readonly ushort rawDestinationPort;
        private readonly ushort rawMessageLength;
        private readonly ushort rawChecksum;

        public byte[] Data { get; set; }

        public UDPHeader(byte[] buffer)
        {
            try
            {
                BinaryReader br = new BinaryReader(new MemoryStream(buffer, 0, buffer.Length));

                rawSourcePort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawDestinationPort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawMessageLength = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawChecksum = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                Data = new byte[buffer.Length - HeaderLength];
                Array.Copy(buffer, HeaderLength, Data, 0, Data.Length);

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
