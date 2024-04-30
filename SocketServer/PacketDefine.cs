namespace SocketServer;

public enum PacketDefine : UInt16
{
    MemoryPackOffset = 1,
    HeaderSize = 5,
}

public enum PacketType : UInt16
{
    /* ----------------------------------- 세션 ----------------------------------- */
    InSessionConnected = 0,
    InSessionDisconnected,
    /* ----------------------------------- 로그인 ---------------------------------- */
    LoginRequest = 1000,
    LoginResponse,
    /* ---------------------------------- 방 입장 ---------------------------------- */
    EnterRoomRequest = 2000,
    EnterRoomResponse,
    NotifyRoomUserList,
    NotifyRoomNewUser,
    /* ---------------------------------- 방 나가기 --------------------------------- */
    NotifyRoomUserLeft = 3000,
    LeaveRoomRequest,
    LeaveRoomResponse,
    NotifyUserMustClose,
    /* ---------------------------------- 방 채팅 ---------------------------------- */
    ChatRequest = 4000,
    ChatResponse,
    NotifyRoomChat,
    /* ---------------------------------- 게임 시작 --------------------------------- */
    StartGameRequest = 5000,
    StartGameResponse,
    /* ---------------------------------- 돌 두기 ---------------------------------- */
    PutStoneRequest = 6000,
    PutStoneResponse,
    /* ---------------------------------- 게임 종료 --------------------------------- */
    EndGameRequest = 7000,
    EndGameResponse,
}
