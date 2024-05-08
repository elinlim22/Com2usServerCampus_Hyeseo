namespace SocketServer;

public enum PacketDefine : UInt16
{
    MemoryPackOffset = 1,
    HeaderSize = 6,
}

public enum PacketType : UInt16
{
    /* ----------------------------------- 세션 ----------------------------------- */
    InSessionConnected = 0,
    InSessionDisconnected,
    ReqHeartBeat,
    ResHeartBeat,
    /* ----------------------------------- 로그인 ---------------------------------- */
    LoginRequest = 1002,
    LoginResponse,
    /* ---------------------------------- 방 입장 ---------------------------------- */
    EnterRoomRequest = 1015,
    EnterRoomResponse,
    NotifyRoomUserList,
    NotifyRoomNewUser,
    /* ---------------------------------- 방 나가기 --------------------------------- */
    NotifyRoomUserLeft = 1023,
    LeaveRoomRequest = 1021,
    LeaveRoomResponse,
    NotifyUserMustClose = 1005,
    /* ---------------------------------- 방 채팅 ---------------------------------- */
    ChatRequest = 1026,
    ChatResponse,
    NotifyRoomChat,
    /* ---------------------------------- 게임 시작 --------------------------------- */
    // StartGameRequest = 5000,
    // StartGameResponse,
    PKTReqReadyOmok = 1031,
    PKTResReadyOmok,
    PKTNtfReadyOmok,
    PKTNtfStartOmok,
    /* ---------------------------------- 돌 두기 ---------------------------------- */
    PutStoneRequest = 1035,
    PutStoneResponse,
    NotifyPutStone,
    /* ---------------------------------- 게임 종료 --------------------------------- */
    // EndGameRequest = 7000,
    // EndGameResponse,
    PKTNtfEndOmok = 1038,
}
