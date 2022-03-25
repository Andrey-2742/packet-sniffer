using System;
using System.IO;
using System.Net;
using SnifferWPF.Headers;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SnifferWPF.Headers
{
    class IGMPHeader : ITransportLevelHeader
    {
        private const int HeaderLength = 8;
        private readonly byte rawType;
        private readonly byte rawMaxResponseTime;
        private readonly short rawChecksum;
        private readonly uint rawGroupAddress;

        public byte Type => rawType;
        public byte MaxResponseTime => rawMaxResponseTime;
        public string Checksum => "0x" + Convert.ToString(rawChecksum, 16).ToUpper().PadLeft(4, '0');
        public string GroupAddress => IPHeader.UIntToIPv4(rawGroupAddress);
        public ushort Length { get; private set; }
        public string Data => string.Empty;

        public IGMPHeader(byte[] buffer)
        {
            try
            {
                BinaryReader br = new BinaryReader(new MemoryStream(buffer, 0, buffer.Length));

                rawType = br.ReadByte();

                rawMaxResponseTime = br.ReadByte();

                rawChecksum = IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawGroupAddress = (uint)IPAddress.NetworkToHostOrder(br.ReadInt32());

                Length = (ushort)buffer.Length;
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
