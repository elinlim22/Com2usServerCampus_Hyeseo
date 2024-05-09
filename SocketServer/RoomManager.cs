using MemoryPack;

namespace SocketServer;

class RoomManager
{
    int currentGroupIndex = 0;
    List<Room> _roomsList = [];

    Timer RoomStatusCheckTimer;
    TimerCallback RoomStatusCheckTimerCallback;
    public static Func<string, byte[], bool> SendData;

    public void CreateRooms(ServerOption serverOpt)
    {
        var maxRoomCount = serverOpt.MaxRoom;
        var startNumber = 0;
        var maxUserCount = serverOpt.MaxUserPerRoom;

        for(int i = 0; i < maxRoomCount; ++i)
        {
            var roomNumber = (startNumber + i);
            var room = new Room();
            room.Init(i, roomNumber, maxUserCount);

            _roomsList.Add(room);
        }
        SetTimer();
    }


    public List<Room> GetRoomsList()
    {
        return _roomsList;
    }
    public void SetTimer()
    {
        RoomStatusCheckTimerCallback = new TimerCallback(RoomStatusCheck);
        RoomStatusCheckTimer = new System.Threading.Timer(RoomStatusCheckTimerCallback, null, 0, 250); // TODO : Config로 빼기
    }

    public void RoomStatusCheck(object state)
    {
        int batchSize = _roomsList.Count / 2;

        int startIndex = currentGroupIndex * batchSize;
        // 방을 순회하면서...
        foreach(var room in _roomsList.GetRange(startIndex, batchSize))
        {
            if (room == null || room.CurrentUserCount() == 0)
            {
                continue;
            }
            if (DateTime.Now.TimeOfDay - room.LastActivity > room.StatusStandardTime)
            {
                //if (room.StatusStandardTime == TimeSpan.FromMinutes(5)) // 게임 시작한거면
                if (room.StatusStandardTime == TimeSpan.FromSeconds(3)) // Debug용
                {
                    // 몰수패 처리:
                    room.EndRoomGame(room.GetCurrentUser(), room.GetNextUser());
                    ClientKickUser(room.GetNextUser(), room);
                    room.RemoveUser(room.GetNextUser().NetSessionID);
                }
                else // 게임 시작 안한거면
                {
                    var users = room.GetUserList();
                    for (int i = 0; i < room.CurrentUserCount(); ++i)
                    {
                        ClientKickUser(users[i], room);
                        room.RemoveUser(users[i].NetSessionID);
                    }
                }
            }
        }
        currentGroupIndex = (currentGroupIndex + 1) % 2;
    }

    void ClientKickUser(RoomUser user, Room room)
    {
        // 클라이언트에 내보내기 처리 요청
        var sendPacket = new NotifyUserMustLeave()
        {
            RoomNumber = room.Number,
            UserId = user.UserId
        };
        var packet = MemoryPackSerializer.Serialize(sendPacket);
        PacketHeaderInfo.Write(packet, PacketType.NotifyUserMustLeave);
        SendData(user.NetSessionID, packet);
    }
}
