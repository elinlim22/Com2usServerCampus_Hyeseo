namespace SocketServer;

public class User
{
    UInt64 SequenceNumber = 0;
    string SessionID;

    public int RoomNumber { get; private set; } = -1;
    string UserId;

    public void Set(UInt64 sequence, string sessionID, string userId)
    {
        SequenceNumber = sequence;
        SessionID = sessionID;
        UserId = userId;
    }

    public bool IsConfirm(string netSessionID)
    {
        return SessionID == netSessionID;
    }

    public string ID()
    {
        return UserId;
    }

    public void EnteredRoom(int roomNumber)
    {
        RoomNumber = roomNumber;
    }

    public void LeaveRoom()
    {
        RoomNumber = -1;
    }
}
