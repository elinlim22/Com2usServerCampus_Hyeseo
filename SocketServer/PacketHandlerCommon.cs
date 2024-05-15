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
            var errorCodeAddUser = _userMgr.AddUser(reqData.UserId, sessionID);
            // var errorCodeAddSession = _userMgr.AddSession(sessionID);
            if (errorCodeAddUser != ErrorCode.Success)
            {
                MakeLoginResponse(errorCodeAddUser, packetData.SessionID);

                if (errorCodeAddUser == ErrorCode.UserFull)
                {
                    MakeNotifyUserMustClose(ErrorCode.UserFull, packetData.SessionID);
                }
                return;
            }

            MakeLoginResponse(errorCodeAddUser, packetData.SessionID);
            MainServer.MainLogger.Debug($"로그인 결과. UserId:{reqData.UserId}, {errorCodeAddUser}");
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
        // TimeSpan now = DateTime.Now.TimeOfDay;

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


}
