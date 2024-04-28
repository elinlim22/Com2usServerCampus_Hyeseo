using SuperSocket.SocketBase.Protocol;

namespace SocketServer;

public class RequestInfo(byte[] bytes) : BinaryRequestInfo(null, bytes)
{
    public string? _sessionId;
    public byte[]? _bytes = bytes;
}
