using System;
using System.Collections.Generic;
using System.Text;

namespace SnifferWPF.Headers
{
    public interface ITransportLevelHeader
    {
        public byte[] Data { get; }
    }
}
