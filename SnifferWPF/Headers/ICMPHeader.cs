using System;
using System.IO;
using System.Net;
using SnifferWPF.Headers;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SnifferWPF
{
    class ICMPHeader : ITransportLevelHeader
    {
        private const int HeaderLength = 4;
        private readonly byte rawType;
        private readonly byte rawCode;
        private readonly short rawChecksum;
        private readonly byte[] data;

        public byte Type => rawType;
        public byte Code => rawCode;
        public string Checksum => "0x" + Convert.ToString(rawChecksum, 16).ToUpper().PadLeft(4, '0');
        public string Data => BytesToString(data);
        public ushort Length => (ushort)data.Length;
        public ushort MessageLength => (ushort)(data.Length - HeaderLength);

        public ICMPHeader(byte[] buffer)
        {
            try
            {
                BinaryReader br = new BinaryReader(new MemoryStream(buffer, 0, buffer.Length));

                rawType = br.ReadByte();

                rawCode = br.ReadByte();

                rawChecksum = IPAddress.NetworkToHostOrder(br.ReadInt16());

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

        public static string BytesToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 3);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2} ", b);
            }
            return hex.ToString();
        }
    }
}
