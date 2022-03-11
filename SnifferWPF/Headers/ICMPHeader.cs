using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SnifferWPF
{
    class ICMPHeader
    {
        private const int HeaderLength = 8;
        private readonly byte rawType;
        private readonly byte rawCode;
        private readonly ushort rawChecksum;
        private readonly uint rawOtherInfo;
        private readonly byte[] data;

        public byte[] Data { get => data; }

        public ICMPHeader(byte[] buffer)
        {
            try
            {
                BinaryReader br = new BinaryReader(new MemoryStream(buffer, 0, buffer.Length));

                rawType = br.ReadByte();

                rawCode = br.ReadByte();

                rawChecksum = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());

                rawOtherInfo = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt32());

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
