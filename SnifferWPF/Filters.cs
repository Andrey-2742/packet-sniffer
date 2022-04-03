using SnifferWPF.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SnifferWPF
{
    public class Filters
    {
        public string SourceIP { get; set; } = string.Empty;
        public string DestinationIP { get; set; } = string.Empty;
        public List<string> Protocol { get; set; } = new List<string>();
        public List<string> SourcePort { get; set; } = new List<string>();
        public List<string> DestinationPort { get; set; } = new List<string>();
        public string Data { get; set; } = string.Empty;

        public bool MatchPacket(IPHeader packet)
        {
            bool flag = true;

            //System.Windows.MessageBox.Show("0");
            if (!new Regex(SourceIP).IsMatch(packet.SourceIP))
                return false;

            //System.Windows.MessageBox.Show("1");

            if (!new Regex(DestinationIP).IsMatch(packet.DestinationIP))
                return false;
            //System.Windows.MessageBox.Show("2");

            foreach (string value in Protocol)
            {
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
                byte valueByte;
                flag = false;
                if (Enum.TryParse(value, out TransportProtocol valueEnum))
                {
                    valueByte = (byte)valueEnum;
                    if (valueByte == packet.ProtocolNumber)
                    {
                        flag = true;
                        break;
                    }
                }
                else if (byte.TryParse(value, out valueByte) && valueByte == packet.ProtocolNumber)
                {
                    flag = true;
                    break;
                }
            }
            if (flag == false)
            {
                return false;
            }
            //System.Windows.MessageBox.Show($"3");

            if (packet.UnderlyingPacket == null)
            {
                return SourcePort.Union(DestinationPort).All(x => string.IsNullOrEmpty(x))
                    && string.IsNullOrEmpty(Data);
            }

            //System.Windows.MessageBox.Show("3.5");

            if (!new Regex(Data).IsMatch(packet.UnderlyingPacket.Data))
                return false;
            //System.Windows.MessageBox.Show("4");

            if (!typeof(IHasPortInformation).IsAssignableFrom(packet.UnderlyingPacket.GetType()))
            {
                return SourcePort.Union(DestinationPort).All(x => !string.IsNullOrEmpty(x));
            }

            //if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.S))
            //    System.Windows.MessageBox.Show("4.5");

            foreach (string value in SourcePort)
            {
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
                flag = false;
                if (ushort.TryParse(value, out ushort valueByte)
                    && valueByte == (packet.UnderlyingPacket as IHasPortInformation).SourcePort)
                {
                    flag = true;
                    break;
                }
            }
            if (flag == false)
            {
                return false;
            }

            foreach (string value in DestinationPort)
            {
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
                flag = false;
                if (ushort.TryParse(value, out ushort valueByte)
                    && valueByte == (packet.UnderlyingPacket as IHasPortInformation).DestinationPort)
                {
                    flag = true;
                    break;
                }
            }
            if (flag == false)
            {
                return false;
            }
            //System.Windows.MessageBox.Show("6");

            return true;
        }
    }
}
