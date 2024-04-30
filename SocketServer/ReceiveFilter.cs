using SuperSocket.Common;
using SuperSocket.SocketEngine.Protocol;

namespace SocketServer;

public class ReceiveFilter : FixedHeaderReceiveFilter<RequestInfo>
{
    public ReceiveFilter()
        : base((int)PacketDefine.HeaderSize)
    {
    }

    protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
    {
        var totalSize = BitConverter.ToUInt16(header, offset + (int)PacketDefine.MemoryPackOffset);
        return totalSize - (int)PacketDefine.HeaderSize;
    }

    protected override RequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] readBuffer, int offset, int length)
    {
        // 패킷 데이터 생성
        byte[] packetData;
        if (length > 0)
        {
            // 헤더와 바디를 합치는 경우
            int packetStartPos = offset >= (UInt16)PacketDefine.HeaderSize ? offset - (UInt16)PacketDefine.HeaderSize : 0;
            int packetSize = length + (UInt16)PacketDefine.HeaderSize;
            packetData = new byte[packetSize];

            Array.Copy(header.Array, header.Offset, packetData, 0, (UInt16)PacketDefine.HeaderSize);
            Array.Copy(readBuffer, offset, packetData, (UInt16)PacketDefine.HeaderSize, length);
        }
        else
        {
            // 바디 데이터가 없는 경우 헤더만 사용
            packetData = new byte[(UInt16)PacketDefine.HeaderSize];
            Array.Copy(header.Array, header.Offset, packetData, 0, (UInt16)PacketDefine.HeaderSize);
        }

        return new RequestInfo(packetData);
    }
}
