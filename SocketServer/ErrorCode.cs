namespace SocketServer;

public enum ErrorCode : short
{
    Success = 0,

    LoginFailed,
    RoomEnterFailed,
    UserFull,
    UserNotFound,
    InvalidUser,
    InvalidRoomNumber,
}
