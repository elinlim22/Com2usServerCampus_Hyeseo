using MemoryPack;

namespace SocketServer;
public class PacketHandlerCommon : PacketHandler
{
    public void RegistPacketHandler(Dictionary<int, Action<RequestInfo>> packetHandlerMap)
    {
        packetHandlerMap[(int)PacketType.InSessionConnected] = NotifyInConnectClient;
        packetHandlerMap[(int)PacketType.InSessionDisconnected] = NotifyInDisConnectClient;
        packetHandlerMap[(int)PacketType.LoginRequest] = HandleLoginRequest;
        packetHandlerMap[(int)PacketType.ReqHeartBeat] = HandleHeartBeatRequest;
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

    public void HandleHeartBeatRequest(RequestInfo packetData)
    {
        var resHeartBeat = new HeartBeatPong();
        var sessionID = packetData.SessionID;
        var user = _userMgr.GetUser(sessionID);
        TimeSpan now = DateTime.Now.TimeOfDay;

        if (now - user.LastPing > TimeSpan.FromSeconds(10))
        {
            resHeartBeat.Result = (short)ErrorCode.PingTimeout;
            MainServer.MainLogger.Error($"Ping Timeout. SessionID:{sessionID}");
        }
        else
        {
            resHeartBeat.Result = (short)ErrorCode.Success;
            user.UpdatePing();
            MainServer.MainLogger.Debug($"SessionID:{sessionID}, Pong..oO");
        }

        var sendData = MemoryPackSerializer.Serialize(resHeartBeat);
        PacketHeaderInfo.Write(sendData, PacketType.ResHeartBeat);

        SendData(sessionID, sendData);
    }
}
