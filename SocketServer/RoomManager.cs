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
                if (room.StatusStandardTime == TimeSpan.FromMinutes(5)) // 돌 안둔거면
                {
                    // 몰수패 처리:
                    room.EndRoomGame(room.GetNextUser(), room.GetCurrentUser());
                    // 현재 턴 유저를 내보낸다.
                    ClientKickUser(room.GetCurrentUser(), room);
                }
                else // 게임 시작 안한거면
                {
                    // 인원 모두 내보내기
                    foreach (var user in room.GetUserList())
                    {
                        ClientKickUser(user, room);
                        // 방에서 유저 제거
                        room.RemoveUser(user.NetSessionID);
                    }
                }
            }
        }
        currentGroupIndex = (currentGroupIndex + 1) % 2;
    }

    void ClientKickUser(RoomUser user, Room room)
    {
        // 클라이언트에 내보내기 처리 요청
        var sendPacket = PacketMaker.MakeNotifyUserMustLeave(user.UserId, user.NetSessionID, room.Number);
        var packet = MemoryPackSerializer.Serialize(sendPacket);
        PacketHeaderInfo.Write(packet, PacketType.NotifyUserMustLeave);
        SendData(user.NetSessionID, packet);
    }
}
