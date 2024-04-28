namespace SocketServer;

public enum PacketDefine : Int32
{
    MemoryPackOffset = 1,
    HeaderSize = 5,
}

public enum PacketType : byte
{
    IN_SessionConnectedOrClosed = 0,
    LoginRequest,
    LoginResponse,
    EnterRoomRequest,
    EnterRoomResponse,
    LeaveRoomRequest,
    LeaveRoomResponse,
    ChatRequest,
    ChatResponse,
    StartGameRequest,
    StartGameResponse,
    PutStoneRequest,
    PutStoneResponse,
    EndGameRequest,
    EndGameResponse,
}
