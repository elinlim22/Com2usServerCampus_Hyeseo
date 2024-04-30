using SuperSocket.SocketBase.Protocol;

namespace SocketServer;

public class RequestInfo : BinaryRequestInfo
{
    public string SessionID;
    public byte[] Data;

    public RequestInfo(byte[] body)
        : base(null, body)
    {
        Data = body;
    }
}
