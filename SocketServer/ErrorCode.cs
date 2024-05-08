namespace SocketServer;

public enum ErrorCode : short
{
    Success = 0,

    LoginFailed,
    RoomEnterFailed,
    UserFull,
    UserNotFound,
    UserSessionAlreadyExist,
    UserSessionNotFound,
    UserForcedClose,
    InvalidUser,
    InvalidRoomNumber,
    PingTimeout,
}
