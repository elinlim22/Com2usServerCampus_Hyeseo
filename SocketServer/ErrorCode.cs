namespace SocketServer;

public enum ErrorCode : short
{
    Success = 0,

    LoginFailed,
    TokenNotMatched,
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
