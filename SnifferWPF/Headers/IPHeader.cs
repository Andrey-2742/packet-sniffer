using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;

namespace SnifferWPF
{
    public enum TransportProtocol { ICMP = 1, TCP = 6, UDP = 17 }

    public class IPHeader
    {
        private readonly byte rawVersion;
        private readonly byte rawHeaderLength;
        private readonly byte rawTOS;
        private readonly ushort rawTotalLength;
        private readonly ushort rawIdentification;
        private readonly byte rawFlags;
        private readonly ushort rawFragmentOffset;
        private readonly byte rawTTL;
        private readonly byte rawProtocol;
        private readonly ushort rawChecksum;
        private readonly uint rawSourceAddress;
        private readonly uint rawDestinationAddress;
        private readonly byte[] data;

        public byte[] Data => data;
        public TransportProtocol Protocol => (TransportProtocol)rawProtocol;
        public string SourceIP => UIntToIPv4(rawSourceAddress);
        public string DestinationIP => UIntToIPv4(rawDestinationAddress);

        public IPHeader(byte[] buffer, int length)
        {
            try
            {
                BinaryReader br = new BinaryReader(new MemoryStream(buffer, 0, length));

                byte versionAndLength = br.ReadByte();
                rawVersion = (byte)(versionAndLength >> 4);
                rawHeaderLength = (byte)(versionAndLength & 0x0F);

                rawTOS = br.ReadByte();

                rawTotalLength = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawIdentification = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                ushort flagsAndOffset = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                rawFlags = (byte)(flagsAndOffset >> 13);
                rawFragmentOffset = (byte)(flagsAndOffset & 0b_0001_1111_1111_1111);

                rawTTL = br.ReadByte();

                rawProtocol = br.ReadByte();

                rawChecksum = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawSourceAddress = br.ReadUInt32();

                rawDestinationAddress = br.ReadUInt32();

                data = new byte[rawTotalLength - rawHeaderLength];
                Array.Copy(buffer, rawHeaderLength, data, 0, data.Length);

                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", "\n" + data.Length + "\n");
                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", Encoding.Default.GetString(data));
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n{e.StackTrace}");
            }
        }

        public static string UIntToIPv4(uint ip)
        {
            byte[] bytes = BitConverter.GetBytes(ip);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return new IPAddress(bytes).ToString();
        }
    }
}
