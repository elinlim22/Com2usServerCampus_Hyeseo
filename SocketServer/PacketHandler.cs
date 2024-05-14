namespace SocketServer;

public class PacketHandler
{
    public static Func<string, byte[], bool> SendData;
    public static Action<RequestInfo> DistributeInnerPacket;
    public static Action<string> CloseSession;
    protected UserManager _userMgr = null;

    public void Init(UserManager userMgr)
    {
        this._userMgr = userMgr;
        UserManager.SendData = SendData;
        UserManager.DistributeInnerPacket = DistributeInnerPacket;
    }
}
