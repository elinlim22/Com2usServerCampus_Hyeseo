namespace SocketServer;

class RoomManager
{
    List<Room> _roomsList = [];


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
}
