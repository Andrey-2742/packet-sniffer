using System;
using System.IO;
using System.Collections.Generic;
using SnifferWPF.Headers;
using System.Net;
using System.Text;
using System.Windows;

namespace SnifferWPF
{
    class TCPHeader : ITransportLevelHeader
    {
        private readonly ushort rawSourcePort;
        private readonly ushort rawDestinationPort;
        private readonly uint rawSequenceNumber;
        private readonly uint rawAcknowledgementNumber;
        private readonly byte rawOffset;
        private readonly byte rawReserved;
        private readonly byte rawFlags;
        private readonly uint rawWindow;
        private readonly uint rawChecksum;
        private readonly uint rawUrgentPointer;

        public byte[] Data { get; set; }

        public TCPHeader(byte[] buffer)
        {
            try
            {
                BinaryReader br = new BinaryReader(new MemoryStream(buffer, 0, buffer.Length));

                rawSourcePort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawDestinationPort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawSequenceNumber = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt32());

                rawAcknowledgementNumber = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt32());

                byte offsetAndReversed = br.ReadByte();
                rawOffset = (byte)(offsetAndReversed >> 4);
                rawReserved = (byte)(offsetAndReversed & 0x0F);

                rawFlags = br.ReadByte();

                rawWindow = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawChecksum = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawUrgentPointer = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                Data = new byte[buffer.Length - rawOffset];
                Array.Copy(buffer, rawOffset, Data, 0, Data.Length);

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
