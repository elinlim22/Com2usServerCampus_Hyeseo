namespace SocketServer;

public enum PacketDefine : Int32
{
    MemoryPackOffset = 1,
    HeaderSize = 5,
}

public enum PakcetType : Int32
{
    LoginRequest = 1,
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
    GameEndRequest,
    GameEndResponse,
}
