using MemoryPack;

namespace SocketServer;

public class Room
{
    public const int InvalidRoomNumber = -1;


    public int Index { get; private set; }
    public int Number { get; private set; }

    int _maxUserCount = 0;

    List<RoomUser> _userList = [];

    public static Func<string, byte[], bool> SendData;


    public void Init(int index, int number, int maxUserCount)
    {
        Index = index;
        Number = number;
        _maxUserCount = maxUserCount;
    }

    public bool AddUser(string UserId, string netSessionID)
    {
        if(GetUser(UserId) != null)
        {
            return false;
        }

        var roomUser = new RoomUser();
        roomUser.Set(UserId, netSessionID);
        _userList.Add(roomUser);

        return true;
    }

    public void RemoveUser(string netSessionID)
    {
        var index = _userList.FindIndex(x => x.NetSessionID == netSessionID);
        _userList.RemoveAt(index);
    }

    public bool RemoveUser(RoomUser user)
    {
        return _userList.Remove(user);
    }

    public RoomUser GetUser(string UserId)
    {
        return _userList.Find(x => x.UserId == UserId);
    }

    public RoomUser GetUserByNetSessionId(string netSessionID)
    {
        return _userList.Find(x => x.NetSessionID == netSessionID);
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
        foreach(var user in _userList)
        {
            if(user.NetSessionID == excludeNetSessionID)
            {
                continue;
            }

            SendData(user.NetSessionID, sendPacket);
        }
    }

    public bool IsAllUserReadyOmok()
    {
        foreach(var user in _userList)
        {
            if(user.IsReady == false)
            {
                return false;
            }
        }

        return true;
    }

    public void StartOmok()
    {
        var packet = new PKTNtfStartOmok();
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
}

public class RoomUser
{
    public string UserId { get; private set; }
    public string NetSessionID { get; private set; }
    public bool IsReady { get; set; }

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
