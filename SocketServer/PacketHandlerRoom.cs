using MemoryPack;
using System.ComponentModel;

namespace SocketServer;

public class PacketHandlerRoom : PacketHandler
{
    List<Room> _roomList = null;
    int _startRoomNumber;

    public void SetRooomList(List<Room> roomList)
    {
        _roomList = roomList;
        _startRoomNumber = roomList[0].Number;
    }

    public void RegistPacketHandler(Dictionary<int, Action<RequestInfo>> packetHandlerMap)
    {
        packetHandlerMap[(int)PacketType.EnterRoomRequest] = RequestRoomEnter;
        packetHandlerMap[(int)PacketType.LeaveRoomRequest] = RequestLeave;
        packetHandlerMap[(int)PacketType.NotifyRoomUserLeft] = NotifyLeaveInternal;
        packetHandlerMap[(int)PacketType.ChatRequest] = RequestChat;
        packetHandlerMap[(int)PacketType.PKTReqReadyOmok] = RequestReadyOmok;
        packetHandlerMap[(int)PacketType.PKTNtfStartOmok] = RequestStartOmok;
        packetHandlerMap[(int)PacketType.PutStoneRequest] = RequestPutOmok;
    }


    Room GetRoom(int roomNumber)
    {
        var index = roomNumber - _startRoomNumber;

        if( index < 0 || index >= _roomList.Count())
        {
            return null;
        }

        return _roomList[index];
    }

    (bool, Room, RoomUser) CheckRoomAndRoomUser(string userNetSessionID)
    {
        var user = _userMgr.GetUser(userNetSessionID);
        if (user == null)
        {
            return (false, null, null);
        }

        var roomNumber = user.RoomNumber;
        var room = GetRoom(roomNumber);

        if(room == null)
        {
            return (false, null, null);
        }

        var roomUser = room.GetUserByNetSessionId(userNetSessionID);

        if (roomUser == null)
        {
            return (false, room, null);
        }

        return (true, room, roomUser);
    }

    public void RequestRoomEnter(RequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("RequestRoomEnter");

        try
        {
            var user = _userMgr.GetUser(sessionID);

            if (user == null || user.IsConfirm(sessionID) == false)
            {
                ResponseEnterRoomToClient(ErrorCode.InvalidUser, sessionID);
                return;
            }

            var reqData = MemoryPackSerializer.Deserialize<EnterRoomRequest>(packetData.Data);
            var room = GetRoom(reqData.RoomNumber);
            if (room == null)
            {
                ResponseEnterRoomToClient(ErrorCode.InvalidRoomNumber, sessionID);
                return;
            }

            if (room.AddUser(user.ID(), sessionID) == false)
            {
                ResponseEnterRoomToClient(ErrorCode.RoomEnterFailed, sessionID);
                return;
            }


            user.EnteredRoom(reqData.RoomNumber);

            room.NotifyPacketUserList(sessionID);
            room.NofifyPacketNewUser(sessionID, user.ID());

            ResponseEnterRoomToClient(ErrorCode.Success, sessionID);

            MainServer.MainLogger.Debug("RequestEnterInternal - Success");
        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

    void ResponseEnterRoomToClient(ErrorCode errorCode, string sessionID)
    {
        var resRoomEnter = new EnterRoomResponse()
        {
            Result = (short)errorCode
        };

        var sendPacket = MemoryPackSerializer.Serialize(resRoomEnter);
        PacketHeaderInfo.Write(sendPacket, PacketType.EnterRoomResponse);

        SendData(sessionID, sendPacket);
    }

    public void RequestLeave(RequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("방나가기 요청 받음");

        try
        {
            var user = _userMgr.GetUser(sessionID);
            if(user == null)
            {
                return;
            }

            if(LeaveRoomUser(sessionID, user.RoomNumber) == false)
            {
                return;
            }

            user.LeaveRoom();

            ResponseLeaveRoomToClient(sessionID);

            MainServer.MainLogger.Debug("Room RequestLeave - Success");
        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

    bool LeaveRoomUser(string sessionID, int roomNumber)
    {
        MainServer.MainLogger.Debug($"LeaveRoomUser. SessionID:{sessionID}");

        var room = GetRoom(roomNumber);
        if (room == null)
        {
            return false;
        }

        var roomUser = room.GetUserByNetSessionId(sessionID);
        if (roomUser == null)
        {
            return false;
        }

        var UserId = roomUser.UserId;
        room.RemoveUser(roomUser);

        room.NotifyPacketLeaveUser(UserId);
        return true;
    }

    void ResponseLeaveRoomToClient(string sessionID)
    {
        var resRoomLeave = new LeaveRoomResponse()
        {
            Result = (short)ErrorCode.Success
        };

        var sendPacket = MemoryPackSerializer.Serialize(resRoomLeave);
        PacketHeaderInfo.Write(sendPacket, PacketType.LeaveRoomResponse);

        SendData(sessionID, sendPacket);
    }

    public void NotifyLeaveInternal(RequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug($"NotifyLeaveInternal. SessionID: {sessionID}");

        var reqData = MemoryPackSerializer.Deserialize<NotifyRoomUserLeft>(packetData.Data);
        LeaveRoomUser(sessionID, reqData.RoomNumber);
    }

    public void RequestChat(RequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("Room RequestChat");

        try
        {
            var roomObject = CheckRoomAndRoomUser(sessionID);

            if(roomObject.Item1 == false)
            {
                return;
            }


            var reqData = MemoryPackSerializer.Deserialize<ChatRequest>(packetData.Data);

            var notifyPacket = new NotifyRoomChat()
            {
                UserId = roomObject.Item3.UserId,
                Message = reqData.Message
            };

            var sendPacket = MemoryPackSerializer.Serialize(notifyPacket);
            PacketHeaderInfo.Write(sendPacket, PacketType.NotifyRoomChat);

            roomObject.Item2.Broadcast("", sendPacket);

            MainServer.MainLogger.Debug("Room RequestChat - Success");
        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

    public void RequestReadyOmok(RequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("RequestReadyOmok");

        try
        {
            var roomObject = CheckRoomAndRoomUser(sessionID);

            if (roomObject.Item1 == false)
            {
                return;
            }

            var room = roomObject.Item2;
            var roomUser = roomObject.Item3;

            roomUser.ReadyOmok();

            if (room.IsAllUserReadyOmok())
            {
                room.StartOmok();
            }
            else
            {
                room.NotifyPacketReadyOmok(roomUser.UserId);
            }

            MainServer.MainLogger.Debug("RequestReadyOmok - Success");
        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

    public void RequestStartOmok(RequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("RequestStartOmok");

        try
        {
            var roomObject = CheckRoomAndRoomUser(sessionID);

            if (roomObject.Item1 == false)
            {
                return;
            }

            var room = roomObject.Item2;
            room.StartOmok();

            MainServer.MainLogger.Debug("RequestStartOmok - Success");
        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

    public void RequestPutOmok(RequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("ReqeustPutOmok");

        try
        {
            var roomObject = CheckRoomAndRoomUser(sessionID);

            if (roomObject.Item1 == false)
            {
                return;
            }

            var room = roomObject.Item2;
            var roomUser = roomObject.Item3;
            var roomOtherUser = room.GetOtherUser(roomUser.UserId);

            var reqData = MemoryPackSerializer.Deserialize<PutStoneRequest>(packetData.Data);

            // 돌 두기 요청 처리(오목 규칙 체크, 게임 종료 체크), 돌 두기 응답
            room.PutStoneRequest(roomUser.UserId, reqData.X, reqData.Y); // 요청을 보낸 플레이어에게는 PutStoneResponse가 전송된다.
            // 요청을 보내지 않은 플레이어에게는 NotifyPutStone이 전송된다.
            room.NotifyPutStone(roomOtherUser.UserId, reqData.X, reqData.Y); // 이 패킷을 받은 클라이언트는 돌을 그린다.
            MainServer.MainLogger.Debug("ReqeustPutOmok - Success");
            
            // 게임 종료 시 게임 종료 응답
            if (room.omokRule.게임종료 == true)
            {
                var endPacket = new PKTNtfEndOmok
                {
                    // 승자 정보 저장
                    WinUserId = roomUser.UserId
                };
                room.omokRule.EndGame();
                // 플레이어 준비 상태 초기화
                roomUser.CancelReadyOmok();
                roomOtherUser.CancelReadyOmok();
                // 유저 타이머 업데이트
                _userMgr.UpdateUserLastConnection(roomUser.UserId);
                _userMgr.UpdateUserLastConnection(roomOtherUser.UserId);

                var sendPacket = MemoryPackSerializer.Serialize(endPacket);
                PacketHeaderInfo.Write(sendPacket, PacketType.PKTNtfEndOmok);
                room.Broadcast("", sendPacket);
                MainServer.MainLogger.Debug("게임 종료");
            }

        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

}
