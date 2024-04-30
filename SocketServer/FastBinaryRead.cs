using System;
using System.Collections.Generic;
using System.Text;

namespace SocketServer
{
    public class FastBinaryRead
    {
        public static UInt16 ReadUInt16FromByteArray(byte[] data, int pos)
        {
            return (UInt16)(data[pos] | data[pos + 1] << 8);
        }
    }
}
