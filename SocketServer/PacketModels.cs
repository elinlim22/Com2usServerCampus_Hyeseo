using MemoryPack;

namespace SocketServer;

[MemoryPackable]
public partial class PacketHeader
{
    public UInt16 Size { get; set; }
    public UInt16 Id { get; set; }
    public byte Type { get; set; }
}

[MemoryPackable]
public partial class HeartBeatPing : PacketHeader
{
}

[MemoryPackable]
public partial class HeartBeatPong : PacketHeader
{
    public short Result { get; set; }
}
/* ----------------------------------- 세션 ----------------------------------- */
[MemoryPackable]
public partial class InSessionConnected : PacketHeader
{
    public string SessionID { get; set; }
}

[MemoryPackable]
public partial class InSessionDisconnected : PacketHeader
{
    public string SessionID { get; set; }
}
/* ----------------------------------- 로그인 ---------------------------------- */
[MemoryPackable]
public partial class LoginRequest : PacketHeader
{
    public string UserId { get; set; }
    public string Token { get; set; }
}

[MemoryPackable]
public partial class LoginResponse : PacketHeader
{
    public short Result { get; set; }
}

/* ---------------------------------- 방 입장 ---------------------------------- */
[MemoryPackable]
public partial class EnterRoomRequest : PacketHeader
{
    public int RoomNumber { get; set; }
}

[MemoryPackable]
public partial class EnterRoomResponse : PacketHeader
{
    public short Result { get; set; }
}

[MemoryPackable]
public partial class NotifyRoomUserList : PacketHeader
{
    public List<string> UserIdList { get; set; } = [];
}

[MemoryPackable]
public partial class NotifyRoomNewUser : PacketHeader
{
    public string UserId { get; set; }
}

/* ---------------------------------- 방 나가기 --------------------------------- */
[MemoryPackable]
public partial class LeaveRoomRequest : PacketHeader
{
    public string RoomName { get; set; }
}

[MemoryPackable]
public partial class LeaveRoomResponse : PacketHeader
{
    public short Result { get; set; }
}

[MemoryPackable]
public partial class NotifyRoomUserLeft : PacketHeader
{
    public int RoomNumber { get; set; }
    public string UserId { get; set; }
}

[MemoryPackable]
public partial class NotifyUserMustClose : PacketHeader
{
    public short ErrorCode { get; set; }
}
/* ---------------------------------- 방 채팅 ---------------------------------- */
[MemoryPackable]
public partial class ChatRequest : PacketHeader
{
    public string Message { get; set; }
}

[MemoryPackable]
public partial class ChatResponse : PacketHeader
{
    public short Result { get; set; }
}

[MemoryPackable]
public partial class NotifyRoomChat : PacketHeader
{
    public string UserId { get; set; }
    public string Message { get; set; }
}

/* ---------------------------------- 게임 시작 --------------------------------- */
// [MemoryPackable]
// public partial class StartGameRequest : PacketHeader
// {
//     public string RoomName { get; set; }
// }

// [MemoryPackable]
// public partial class StartGameResponse : PacketHeader
// {
//     public short Result { get; set; }
// }
    // 오목 플레이 준비 완료 요청
    [MemoryPackable]
    public partial class PKTReqReadyOmok : PacketHeader
    {
        public short Result { get; set; }
    }

    [MemoryPackable]
    public partial class PKTResReadyOmok : PacketHeader
    {
        public short Result { get; set; }
    }

    [MemoryPackable]
    public partial class PKTNtfReadyOmok : PacketHeader
    {
        public string UserId { get; set; }
        public bool IsReady { get; set; }
    }


    // 오목 시작 통보(서버에서 클라이언트들에게)
    [MemoryPackable]
    public partial class PKTNtfStartOmok : PacketHeader
    {
        public string FirstUserId { get; set; } // 선턴 유저 ID
    }


/* ---------------------------------- 돌 두기 ---------------------------------- */
[MemoryPackable]
public partial class PutStoneRequest : PacketHeader
{
    public int X { get; set; }
    public int Y { get; set; }
}

[MemoryPackable]
public partial class PutStoneResponse : PacketHeader
{
    public short Result { get; set; }
}

[MemoryPackable]
public partial class NotifyPutStone : PacketHeader
{
    public int X { get; set;}
    public int Y { get; set;}
    // public int Mok { get; set;}
}
/* ---------------------------------- 게임 종료 --------------------------------- */
// [MemoryPackable]
// public partial class EndGameResponse : PacketHeader
// {
//     public short Result { get; set; }
// }

// [MemoryPackable]
// public partial class EndGameRequest : PacketHeader
// {
//     public string RoomName { get; set; }
// }
    // 오목 게임 종료 통보
[MemoryPackable]
public partial class PKTNtfEndOmok : PacketHeader
{
    public string WinUserId { get; set; }
}
