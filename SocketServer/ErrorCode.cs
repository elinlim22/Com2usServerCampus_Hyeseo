namespace SocketServer;

public enum ErrorCode : int
{
    Success = 0,

    LoginFailed,
    RoomEnterFailed,
    UserFull,
    UserNotFound,
    InvalidUser,
    InvalidRoomNumber,
}
