using MemoryPack;

namespace SocketServer;

public class Room
{
    public const int InvalidRoomNumber = -1;
    // public TimeSpan StatusStandardTime = TimeSpan.FromMinutes(30); // TODO : Config로 빼기
    public TimeSpan StatusStandardTime = TimeSpan.FromSeconds(30); // Debug용
    public TimeSpan LastActivity;

    public int Index { get; private set; }
    public int Number { get; private set; }

    int _maxUserCount = 0;

    List<RoomUser> _userList = [];

    public static Func<string, byte[], bool> SendData;
    public static Action<RequestInfo> DistributeInnerPacket;
    
    public OmokRule omokRule = new OmokRule();


    public void Init(int index, int number, int maxUserCount)
    {
        Index = index;
        Number = number;
        _maxUserCount = maxUserCount;
        UpdateLastActivity();
    }

    public void UpdateLastActivity()
    {
        LastActivity = DateTime.Now.TimeOfDay;
    }

    public bool AddUser(string UserId, string netSessionID)
    {
        if (GetUser(UserId) != null)
        {
            return false;
        }

        var roomUser = new RoomUser();
        roomUser.Set(UserId, netSessionID);
        _userList.Add(roomUser);
        UpdateLastActivity();

        return true;
    }

    public void RemoveUser(string netSessionID)
    {
        var innerPacket = PacketMaker.MakeInnerUserLeaveRoom(netSessionID);
        DistributeInnerPacket(innerPacket);
    }

    public bool RemoveUser(RoomUser user)
    {
        return _userList.Remove(user);
    }

    public RoomUser GetUser(string UserId)
    {
        return _userList.Find(x => x.UserId == UserId);
    }

    public RoomUser GetOtherUser(string UserId)
    {
        return _userList.Find(x => x.UserId != UserId);
    }

    public RoomUser GetUserByNetSessionId(string netSessionID)
    {
        return _userList.Find(x => x.NetSessionID == netSessionID);
    }

    public List<RoomUser> GetUserList()
    {
        return _userList;
    }

    public RoomUser GetNextUser()
    {
        return _userList.Find(x => x.IsMyTurn == false);
    }

    public RoomUser GetCurrentUser()
    {
        return _userList.Find(x => x.IsMyTurn == true);
    }

    public int CurrentUserCount()
    {
        return _userList.Count();
    }

    public void NotifyPacketUserList(string userNetSessionID)
    {
        var packet = new NotifyRoomUserList();
        foreach (var user in _userList)
        {
            packet.UserIdList.Add(user.UserId);
        }

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeaderInfo.Write(sendPacket, PacketType.NotifyRoomUserList);

        SendData(userNetSessionID, sendPacket);
    }

    public void NofifyPacketNewUser(string newUserNetSessionID, string newUserId)
    {
        var packet = new NotifyRoomNewUser
        {
            UserId = newUserId
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeaderInfo.Write(sendPacket, PacketType.NotifyRoomNewUser);

        Broadcast(newUserNetSessionID, sendPacket);
    }

    public void NotifyPacketLeaveUser(string UserId)
    {
        //if(CurrentUserCount() == 0)
        //{
        //  return;
        //}

        var packet = new NotifyRoomUserLeft
        {
            UserId = UserId
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeaderInfo.Write(sendPacket, PacketType.NotifyRoomUserLeft);

        Broadcast("", sendPacket);
    }

    public void Broadcast(string excludeNetSessionID, byte[] sendPacket)
    {
        foreach (var user in _userList)
        {
            if (user.NetSessionID == excludeNetSessionID)
            {
                continue;
            }

            SendData(user.NetSessionID, sendPacket);
        }
    }

    public bool IsAllUserReadyOmok()
    {
        if (_userList.Count() != _maxUserCount)
        {
            return false;
        }
        foreach (var user in _userList)
        {
            if (user.IsReady == false)
            {
                return false;
            }
        }

        return true;
    }

    public void StartOmok()
    {
        omokRule.StartGame();
        // StatusStandardTime = TimeSpan.FromMinutes(5); // TODO : Config로 빼기
        StatusStandardTime = TimeSpan.FromSeconds(10); // Debug용
        UpdateLastActivity();
        var packet = new PKTNtfStartOmok
        {
            FirstUserId = _userList[0].UserId
        };
        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeaderInfo.Write(sendPacket, PacketType.PKTNtfStartOmok);

        Broadcast("", sendPacket);
    }

    public void NotifyPacketReadyOmok(string UserId)
    {
        var packet = new PKTNtfReadyOmok
        {
            UserId = UserId,
            IsReady = true
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeaderInfo.Write(sendPacket, PacketType.PKTNtfReadyOmok);

        Broadcast("", sendPacket);
    }

    public void PutStoneRequest(string UserId, int x, int y)
    {
        RoomUser user = GetUser(UserId);
        var packet = new PutStoneResponse();
        돌두기_결과 result = omokRule.돌두기(x, y); // TODO : 삼삼 체크가 안됨..
        omokRule.오목확인(x, y);
        packet.Result = (short)result;

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeaderInfo.Write(sendPacket, PacketType.PutStoneResponse);

        SendData(user.NetSessionID, sendPacket);

        UpdateLastActivity();
    }

    public void NotifyPutStone(string UserId, int x, int y)
    {
        RoomUser user = GetUser(UserId);
        var packet = new NotifyPutStone
        {
            X = x,
            Y = y,
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeaderInfo.Write(sendPacket, PacketType.NotifyPutStone);

        SendData(user.NetSessionID, sendPacket);
    }

    public void EndRoomGame(RoomUser roomUser, RoomUser roomOtherUser)
    {
        var endPacket = new PKTNtfEndOmok
        {
            WinUserId = roomUser.UserId
        };
        omokRule.EndGame();
        // 플레이어 준비 상태 초기화
        roomUser.CancelReadyOmok();
        roomOtherUser.CancelReadyOmok();
        // 방 타이머 업데이트
        UpdateLastActivity();
        // StatusStandardTime = TimeSpan.FromMinutes(30); // TODO : Config로 빼기
        StatusStandardTime = TimeSpan.FromSeconds(30); // Debug용

        var sendPacket = MemoryPackSerializer.Serialize(endPacket);
        PacketHeaderInfo.Write(sendPacket, PacketType.PKTNtfEndOmok);

        Broadcast("", sendPacket);
    }
}

public class RoomUser
{
    public string UserId { get; private set; }
    public string NetSessionID { get; private set; }
    public bool IsReady { get; set; }
    public bool IsMyTurn { get; set; }

    public void Set(string userId, string netSessionID)
    {
        UserId = userId;
        NetSessionID = netSessionID;
    }

    public void ReadyOmok()
    {
        IsReady = true;
    }

    public void CancelReadyOmok()
    {
        IsReady = false;
    }

}
