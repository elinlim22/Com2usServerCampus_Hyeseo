using MemoryPack;

namespace SocketServer;

class RoomManager(ServerOption serverOption)
{
    int currentGroupIndex = 0;
    int batchSize = serverOption.RoomStatusCheckSize;
    List<Room> _roomsList = [];

    Timer RoomStatusCheckTimer;
    TimerCallback RoomStatusCheckTimerCallback;
    public Func<string, byte[], bool> SendData;
    public Action<RequestInfo> DistributeInnerPacket;
    public Action<RequestInfo> DistributeMySQLPacket;

    public void CreateRooms()
    {
        var startNumber = 0;

        for(int i = 0; i < serverOption.MaxRoom; ++i)
        {
            var roomNumber = (startNumber + i);
            var room = new Room(serverOption);
            room.Init(i, roomNumber, serverOption.MaxUserPerRoom);

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
        RoomStatusCheckTimer = new System.Threading.Timer(RoomStatusCheckTimerCallback, null, 0, serverOption.RoomStatusCheckSize);
    }

    /*public void RoomStatusCheck(object state)
    {
        int startIndex = currentGroupIndex++ * batchSize;
        int endIndex = startIndex + batchSize;
        if (endIndex >= serverOption.MaxRoom)
        {
            endIndex = serverOption.MaxRoom;
            currentGroupIndex = 0;
        }
        for (int i = startIndex; i < endIndex; i++)
        {
            if (i >= serverOption.MaxRoom)
            {
                break;
            }
            var room = _roomsList[i];
            if (room.Status == RoomStatus.Empty)
            {
                continue;
            }
            if (DateTime.Now.TimeOfDay - room.LastActivity > room.TimeoutThreshold)
            {
                if (room.Status == RoomStatus.OneUser)
                {
                    ClientKickUser(room.GetUserList()[0], room);

                    MainServer.MainLogger.Error($"User Forced Leave due to Inactivity. RoomNumber:{room.Number}");
                    var innerPacket = PacketMaker.MakeInnerUserLeaveRoom(room.GetUserList()[0].NetSessionID);
                    DistributeInnerPacket(innerPacket);
                }
                else if (room.Status == RoomStatus.Playing)
                {
                    var roomWinner = room.GetCurrentUser();
                    var roomLoser = room.GetNextUser();

                    var innerForfeiturePacket = PacketMaker.MakeForfeitureRequest(roomWinner.NetSessionID);
                    DistributeInnerPacket(innerForfeiturePacket);
                    ClientKickUser(roomLoser, room);
                    var innerLeavePacket = PacketMaker.MakeInnerUserLeaveRoom(roomLoser.NetSessionID);
                    DistributeInnerPacket(innerLeavePacket);

                }
            }
        }
    }*/

    public void RoomStatusCheck(object state)
    {
        int startIndex = currentGroupIndex++ * batchSize;
        int endIndex = startIndex + batchSize;
        if (endIndex >= serverOption.MaxRoom)
        {
            endIndex = serverOption.MaxRoom;
            currentGroupIndex = 0;
        }
        for (int i = startIndex; i < endIndex; i++)
        {
            if (i >= serverOption.MaxRoom)
            {
                break;
            }
            var innerPacket = PacketMaker.MakeRoomStatusCheckRequest(i);
            DistributeInnerPacket(innerPacket);
        }
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
