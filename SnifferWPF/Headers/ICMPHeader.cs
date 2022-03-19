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
        private const int HeaderLength = 8;
        private readonly byte rawType;
        private readonly byte rawCode;
        private readonly short rawChecksum;
        private readonly uint rawOtherInfo;
        private readonly byte[] data;

        public byte Type => rawType;
        public byte Code => rawCode;
        public string Checksum => "0x" + Convert.ToString(rawChecksum, 16).ToUpper().PadLeft(4, '0');
        public string OtherInfo => GetOtherInfo();
        public string Data => Encoding.Default.GetString(data);
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

                rawOtherInfo = (uint)IPAddress.NetworkToHostOrder(br.ReadInt32());

                data = new byte[buffer.Length - HeaderLength];
                Array.Copy(buffer, HeaderLength, data, 0, Data.Length);

                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", "\n" + data.Length + "\n");
                //File.AppendAllText("C:\\Users\\johncji\\Desktop\\text.txt", Encoding.Default.GetString(data));
                //Console.WriteLine(Encoding.Default.GetString(data));
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n{e.StackTrace}");
            }
        }

        private string GetOtherInfo()
        {
            string binary = Convert.ToString(rawOtherInfo, 2).PadLeft(32, '0');
            List<string> stringBytes = new List<string>();

            for (int i = 0; i < 32; i += 8)
            {
                stringBytes.Add(binary.Substring(i, 8));
            }

            return string.Join('_', stringBytes);
        }
    }
}
