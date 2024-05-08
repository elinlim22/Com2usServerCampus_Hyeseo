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
        RoomStatusCheckTimer = new System.Threading.Timer(RoomStatusCheckTimerCallback, null, 0, 250); // TODO : Config�� ����
    }

    public void RoomStatusCheck(object state)
    {
        int batchSize = _roomsList.Count / 2;

        int startIndex = currentGroupIndex * batchSize;
        // ���� ��ȸ�ϸ鼭...
        foreach(var room in _roomsList.GetRange(startIndex, batchSize))
        {
            if (room == null || room.CurrentUserCount() == 0)
            {
                continue;
            }
            if (DateTime.Now.TimeOfDay - room.LastActivity > room.StatusStandardTime)
            {
                if (room.StatusStandardTime == TimeSpan.FromMinutes(5)) // �� �ȵаŸ�
                {
                    // ������ ó��:
                    room.EndRoomGame(room.GetNextUser(), room.GetCurrentUser());
                    // ���� �� ������ ��������.
                    ClientKickUser(room.GetCurrentUser(), room);
                }
                else // ���� ���� ���ѰŸ�
                {
                    // �ο� ��� ��������
                    foreach (var user in room.GetUserList())
                    {
                        ClientKickUser(user, room);
                        // �濡�� ���� ����
                        room.RemoveUser(user.NetSessionID);
                    }
                }
            }
        }
        currentGroupIndex = (currentGroupIndex + 1) % 2;
    }

    void ClientKickUser(RoomUser user, Room room)
    {
        // Ŭ���̾�Ʈ�� �������� ó�� ��û
        var sendPacket = PacketMaker.MakeNotifyUserMustLeave(user.UserId, user.NetSessionID, room.Number);
        var packet = MemoryPackSerializer.Serialize(sendPacket);
        PacketHeaderInfo.Write(packet, PacketType.NotifyUserMustLeave);
        SendData(user.NetSessionID, packet);
    }
}
