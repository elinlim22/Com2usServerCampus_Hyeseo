using System;
using System.Collections.Generic;
using System.Text;

namespace SocketServer
{
    public class FastBinaryWrite
    {
        public static void WriteUInt16ToByteArray(byte[] data, int pos, UInt16 value)
        {
            data[pos] = (byte)(value & 0xff);
            data[pos + 1] = (byte)(value >> 8);
        }
    }
}
