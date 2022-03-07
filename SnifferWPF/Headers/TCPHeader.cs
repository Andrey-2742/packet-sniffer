using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SnifferWPF
{
    class TCPHeader
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
        private readonly byte[] data;

        public byte[] Data { get => data; }

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

                data = new byte[buffer.Length - rawOffset];
                Array.Copy(buffer, rawOffset, data, 0, data.Length);

                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", "\n" + data.Length + "\n");
                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", Encoding.Default.GetString(data));
                //Console.WriteLine(Encoding.Default.GetString(data));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
