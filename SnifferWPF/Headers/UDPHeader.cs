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
        private readonly ushort rawSegmentLength;
        private readonly short rawChecksum;
        private readonly byte[] data;

        public ushort SourcePort => rawSourcePort;
        public ushort DestinationPort => rawDestinationPort;
        public string Checksum => "0x" + Convert.ToString(rawChecksum, 16).ToUpper().PadLeft(4, '0');
        public string Data => Encoding.Default.GetString(data);
        public ushort Length => rawSegmentLength;
        public ushort MessageLength => (ushort)(rawSegmentLength - HeaderLength);

        public UDPHeader(byte[] buffer)
        {
            try
            {
                BinaryReader br = new BinaryReader(new MemoryStream(buffer, 0, buffer.Length));

                rawSourcePort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawDestinationPort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawSegmentLength = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawChecksum = IPAddress.NetworkToHostOrder(br.ReadInt16());

                data = new byte[buffer.Length - HeaderLength];
                Array.Copy(buffer, HeaderLength, data, 0, Data.Length);

                //if (rawSegmentLength != Data.Length)
                //    MessageBox.Show($"UDP\n{buffer.Length}\n{rawSegmentLength}\n{Data.Length}");
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
