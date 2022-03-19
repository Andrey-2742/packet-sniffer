using System;
using System.IO;
using System.Collections.Generic;
using SnifferWPF.Headers;
using System.Net;
using System.Text;
using System.Windows;

namespace SnifferWPF
{
    enum TCPFlag { NS, CWR, ECE, URG, ACK, PSH, PST, SYN, FIN }
    class TCPHeader : ITransportLevelHeader
    {
        private readonly ushort rawSourcePort;
        private readonly ushort rawDestinationPort;
        private readonly uint rawSequenceNumber;
        private readonly uint rawAcknowledgementNumber;
        private readonly ushort rawOffset;
        private readonly ushort rawFlags;
        private readonly ushort rawWindow;
        private readonly short rawChecksum;
        private readonly ushort rawUrgentPointer;
        private readonly byte[] data;

        public ushort SourcePort => rawSourcePort;
        public ushort DestinationPort => rawDestinationPort;
        public uint SequenceNumber => rawSequenceNumber;
        public uint AcknowledgementNumber => rawAcknowledgementNumber;
        public string Flags => GetFlags();
        public ushort Window => rawWindow;
        public string Checksum => "0x" + Convert.ToString(rawChecksum, 16).ToUpper().PadLeft(4, '0');
        public ushort UrgentPointer => rawUrgentPointer;
        public string Data => Encoding.Default.GetString(data);
        public ushort Length { get; private set; }
        public ushort MessageLength { get; private set; }

        public TCPHeader(byte[] buffer)
        {
            try
            {
                BinaryReader br = new BinaryReader(new MemoryStream(buffer, 0, buffer.Length));

                rawSourcePort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawDestinationPort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawSequenceNumber = (uint)IPAddress.NetworkToHostOrder(br.ReadInt32());

                rawAcknowledgementNumber = (uint)IPAddress.NetworkToHostOrder(br.ReadInt32());

                ushort offsetAndFlags = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                rawOffset = (byte)((offsetAndFlags >> 12) * 4);
                rawFlags = (ushort)(offsetAndFlags & 0b_0000_0001_1111_1111);

                rawWindow = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawChecksum = IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawUrgentPointer = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                data = new byte[buffer.Length - rawOffset];
                Array.Copy(buffer, rawOffset, data, 0, Data.Length);

                Length = (ushort)buffer.Length;
                MessageLength = (ushort)(buffer.Length - rawOffset);
                //MessageBox.Show($"TCP\n{buffer.Length}\n{(ushort)(buffer.Length - rawOffset)}");

                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", "\n" + data.Length + "\n");
                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", Encoding.Default.GetString(data));
                //Console.WriteLine(Encoding.Default.GetString(data));
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n{e.StackTrace}");
            }
        }

        private string GetFlags()
        {
            List<string> flags = new List<string>();

            for (int i = 0; i < Enum.GetValues(typeof(TCPFlag)).Length; i++)
            {
                if ((rawFlags >> (8 - i) & 1) == 1)
                {
                    flags.Add(((TCPFlag)i).ToString());
                }
            }
            return new StringBuilder().AppendJoin(", ", flags).ToString();
        }
    }
}
