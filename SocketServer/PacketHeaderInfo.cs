namespace SocketServer;

public struct PacketHeaderInfo
{
    public UInt16 TotalSize;
    public UInt16 Id;
    public PacketType Type;

    public static UInt16 GetTotalSize(byte[] data, int startPos)
    {
        return FastBinaryRead.ReadUInt16FromByteArray(data, startPos + (UInt16)PacketDefine.MemoryPackOffset);
    }

    public static void WritePacketType(byte[] data, UInt16 packetType)
    {
        FastBinaryWrite.WriteUInt16ToByteArray(data, (UInt16)PacketDefine.MemoryPackOffset + 2, packetType);
    }

    public void Read(byte[] headerData)
    {
        var pos = (UInt16)PacketDefine.MemoryPackOffset;

        TotalSize = FastBinaryRead.ReadUInt16FromByteArray(headerData, pos);
        pos += 2;

        Id = FastBinaryRead.ReadUInt16FromByteArray(headerData, pos);
        pos += 2;

        Type = (PacketType)headerData[pos];
    }

    public static void Write(byte[] packetData, PacketType packetType, byte type = 0)
    {
        var pos = (UInt16)PacketDefine.MemoryPackOffset;

        FastBinaryWrite.WriteUInt16ToByteArray(packetData, pos, (UInt16)packetData.Length);
        pos += 2;

        FastBinaryWrite.WriteUInt16ToByteArray(packetData, pos, (UInt16)packetType);
        pos += 2;

        packetData[pos] = type;
    }
}

