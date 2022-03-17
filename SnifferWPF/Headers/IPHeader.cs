using SnifferWPF.Headers;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;

namespace SnifferWPF
{
    public enum TransportProtocol { ICMP = 1, IGMP = 2, TCP = 6, UDP = 17, Other = 0 }

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

        public byte RawProtocol => rawProtocol;
        public string IPVersion => GetIPVersion();
        public ushort IdentificationNumber => rawIdentification;
        public ushort FragmentOffset => rawFragmentOffset;
        public string Flags => FlagsToString();
        public byte TTL => rawTTL;
        public TransportProtocol Protocol => (TransportProtocol)rawProtocol;
        public ushort Checksum => rawChecksum;
        public string SourceIP => UIntToIPv4(rawSourceAddress);
        public string DestinationIP => UIntToIPv4(rawDestinationAddress);
        public byte[] Data { get; set; }
        public ITransportLevelHeader UnderlyingPacket { get; set; }

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
                rawFragmentOffset = (ushort)(flagsAndOffset & 0b_0001_1111_1111_1111);

                rawTTL = br.ReadByte();

                rawProtocol = br.ReadByte();

                rawChecksum = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawSourceAddress = br.ReadUInt32();

                rawDestinationAddress = br.ReadUInt32();

                Data = new byte[rawTotalLength - rawHeaderLength];
                Array.Copy(buffer, rawHeaderLength, Data, 0, Data.Length);

                GetLevel4Packet();

                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", "\n" + data.Length + "\n");
                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", Encoding.Default.GetString(data));
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n{e.StackTrace}");
            }
        }

        private void GetLevel4Packet()
        {
            switch (Protocol)
            {
                case TransportProtocol.TCP:
                    UnderlyingPacket = new TCPHeader(Data);
                    break;

                case TransportProtocol.UDP:
                    UnderlyingPacket = new UDPHeader(Data);
                    break;

                case TransportProtocol.ICMP:
                    UnderlyingPacket = new ICMPHeader(Data);
                    break;

                case TransportProtocol.Other:
                    //MessageBox.Show("Other");
                    break;
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

        private string GetIPVersion()
        {
            if (rawVersion == 4) return "4";
            if (rawVersion == 6) return "6";
            return "Unknown";
        }

        private string FlagsToString()
        {
            string flags = "";
            if ((rawFlags & 0b_0000_0010) == 0b_0000_0010)
            {
                flags += "D";
            }
            if ((rawFlags & 0b_0000_0001) == 0b_0000_0001)
            {
                flags += " M";
            }
            return flags;
        }
    }
}
