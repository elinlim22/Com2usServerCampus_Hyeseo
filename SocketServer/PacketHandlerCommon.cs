using MemoryPack;

namespace SocketServer;
public class PacketHandlerCommon : PacketHandler
{
    public void RegistPacketHandler(Dictionary<int, Action<RequestInfo>> packetHandlerMap)
    {
        packetHandlerMap[(int)PacketType.InSessionConnected] = NotifyInConnectClient;
        packetHandlerMap[(int)PacketType.InSessionDisconnected] = NotifyInDisConnectClient;
        packetHandlerMap[(int)PacketType.LoginRequest] = HandleLoginRequest;
    }

    public void NotifyInConnectClient(RequestInfo requestData)
    {
    }

    public void NotifyInDisConnectClient(RequestInfo requestData)
    {
        var sessionID = requestData.SessionID;
        var user = _userMgr.GetUser(sessionID);

        if (user != null)
        {
            var roomNum = user.RoomNumber;

            if (roomNum != Room.InvalidRoomNumber)
            {
                var internalPacket = PacketMaker.MakeNotifyRoomUserLeftPacket(sessionID, roomNum, user.ID());
                DistributeInnerPacket(internalPacket);
            }

            _userMgr.RemoveUser(sessionID);
        }
    }


    public void HandleLoginRequest(RequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("로그인 요청 받음");

        try
        {
            var reqData = MemoryPackSerializer.Deserialize<LoginRequest>(packetData.Data);
            var errorCode = _userMgr.AddUser(reqData.UserId, sessionID);
            if (errorCode != ErrorCode.Success)
            {
                MakeLoginResponse(errorCode, packetData.SessionID);

                if (errorCode == ErrorCode.UserFull)
                {
                    MakeNotifyUserMustClose(ErrorCode.UserFull, packetData.SessionID);
                }
                return;
            }

            MakeLoginResponse(errorCode, packetData.SessionID);
            MainServer.MainLogger.Debug($"로그인 결과. UserId:{reqData.UserId}, {errorCode}");
        }
        catch(Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

    public void MakeLoginResponse(ErrorCode errorCode, string sessionID)
    {
        var resLogin = new LoginResponse()
        {
            Result = (short)errorCode
        };

        var sendData = MemoryPackSerializer.Serialize(resLogin);
        PacketHeaderInfo.Write(sendData, PacketType.LoginResponse);

        SendData(sessionID, sendData);
    }

    public void MakeNotifyUserMustClose(ErrorCode errorCode, string sessionID)
    {
        var resLogin = new NotifyUserMustClose()
        {
            Result = (short)errorCode
        };

        var sendData = MemoryPackSerializer.Serialize(resLogin);
        PacketHeaderInfo.Write(sendData, PacketType.NotifyUserMustClose);

        SendData(sessionID, sendData);
    }
}
