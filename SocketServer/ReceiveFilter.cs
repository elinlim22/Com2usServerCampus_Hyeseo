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
        return BitConverter.ToInt32(header, offset);
    }

    protected override RequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
    {
        return new RequestInfo(bodyBuffer.CloneRange(offset, length));
    }
}
