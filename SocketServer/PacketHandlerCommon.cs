using MemoryPack;

namespace SocketServer;
public class PacketHandlerCommon(ServerOption serverOption) : PacketHandler
{
    ServerOption _serverOption = serverOption;
    public void RegistPacketHandler(Dictionary<int, Action<RequestInfo>> packetHandlerMap)
    {
        packetHandlerMap[(int)PacketType.InSessionConnected] = NotifyInConnectClient;
        packetHandlerMap[(int)PacketType.InSessionDisconnected] = NotifyInDisConnectClient;
        packetHandlerMap[(int)PacketType.LoginRequest] = HandleLoginRequest;
        packetHandlerMap[(int)PacketType.ReqHeartBeat] = HandleHeartBeatRequest;
        packetHandlerMap[(int)PacketType.CloseSessionRequest] = HandleCloseSessionRequest;
        packetHandlerMap[(int)PacketType.ValidateUserTokenResponse] = HandleValidateUserTokenResponse;
    }

    public void NotifyInConnectClient(RequestInfo requestData)
    {
        var sessionID = requestData.SessionID;
        MainServer.MainLogger.Debug($"세션 연결됨. SessionID:{sessionID}");

        // 유저상태조사 리스트에 추가
        var errorCode = _userMgr.AddSession(sessionID);
        if (errorCode != ErrorCode.Success)
        {
            MainServer.MainLogger.Error($"세션 연결 실패. ErrorCode:{errorCode}");
            return;
        }
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
            // 로그인 요청 시, Redis에서 Validation을 체크하도록 패킷을 보낸다.
            var innerPacket = PacketMaker.MakeValidateUserTokenRequest(reqData.UserId, sessionID, reqData.Token);
            DistributeRedisPacket(innerPacket);
            // Redis로부터 ValidationUserTokenResponse 패킷을 받으면, LoginResponse 함수가 호출된다.
            // 그 때 유저를 추가하는걸로 바꿔야 함.
        }
        catch(Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

    public void MakeLoginResponse(ErrorCode errorCode, string userId, string sessionID)
    {
        var resLogin = new LoginResponse();
        // 성공 시, 유저를 추가한다.
        if (errorCode == ErrorCode.Success)
        {
            var errorCodeAddUser = _userMgr.AddUser(userId, sessionID);
            if (errorCodeAddUser != ErrorCode.Success)
            {
                resLogin.Result = (short)errorCodeAddUser;
                if (errorCodeAddUser == ErrorCode.UserFull)
                {
                    MakeNotifyUserMustClose(ErrorCode.UserFull, sessionID);
                }
            }
            MainServer.MainLogger.Debug($"로그인 결과. UserId:{userId}, {errorCodeAddUser}");
        }
        else
        {
            resLogin.Result = (short)errorCode;
        }
        var sendData = MemoryPackSerializer.Serialize(resLogin);
        PacketHeaderInfo.Write(sendData, PacketType.LoginResponse);

        SendData(sessionID, sendData);
    }

    public void MakeNotifyUserMustClose(ErrorCode errorCode, string sessionID)
    {
        var resLogin = new NotifyUserMustClose()
        {
            ErrorCode = (short)errorCode
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
        if (user.ID() == "")
        {
            return;
        }

        if (DateTime.Now.TimeOfDay - user.LastPing > TimeSpan.FromSeconds(_serverOption.HeartBeatInSeconds))
        {
            resHeartBeat.Result = (short)ErrorCode.PingTimeout;
            MainServer.MainLogger.Error($"Ping Timeout. SessionID:{sessionID}");
        }
        else
        {
            resHeartBeat.Result = (short)ErrorCode.Success;
            user.UpdateLastPing();
            // MainServer.MainLogger.Debug($"SessionID:{sessionID}, Pong..oO");
        }

        var sendData = MemoryPackSerializer.Serialize(resHeartBeat);
        PacketHeaderInfo.Write(sendData, PacketType.ResHeartBeat);

        SendData(sessionID, sendData);
    }

    public void HandleCloseSessionRequest(RequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        CloseSession(sessionID);
    }

    public void HandleValidateUserTokenResponse(RequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        var resData = MemoryPackSerializer.Deserialize<ValidateUserTokenResponse>(packetData.Data);

        if (resData.Result != (short)ErrorCode.Success)
        {
            MainServer.MainLogger.Error($"ValidateUserTokenResponse - token not matched: {resData.Result}");
        }
        else
        {
            MainServer.MainLogger.Info($"ValidateUserTokenResponse - token matched: {resData.Result}");
        }
        MakeLoginResponse((ErrorCode)resData.Result, resData.UserId, sessionID);
    }
}
