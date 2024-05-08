namespace SocketServer;

public class PacketHandler
{
    public static Func<string, byte[], bool> SendData;
    public static Action<RequestInfo> DistributeInnerPacket;
    protected UserManager _userMgr = null;

    public void Init(UserManager userMgr)
    {
        this._userMgr = userMgr;
        UserManager.DistributeInnerPacket = DistributeInnerPacket;
    }
}
