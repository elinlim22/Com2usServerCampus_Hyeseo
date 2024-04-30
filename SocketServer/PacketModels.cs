using MemoryPack;

namespace SocketServer;

[MemoryPackable]
public partial class PacketHeader
{
    public int Id { get; set; }
    public int Size { get; set; }
    public int Type { get; set; }
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
    public int Result { get; set; }
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
    public int Result { get; set; }
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
    public int Result { get; set; }
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
    public int Result { get; set; }
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
    public int Result { get; set; }
}

[MemoryPackable]
public partial class NotifyRoomChat : PacketHeader
{
    public string UserId { get; set; }
    public string Message { get; set; }
}

/* ---------------------------------- 게임 시작 --------------------------------- */
[MemoryPackable]
public partial class StartGameRequest : PacketHeader
{
    public string RoomName { get; set; }
}

[MemoryPackable]
public partial class StartGameResponse : PacketHeader
{
    public int Result { get; set; }
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
    public int Result { get; set; }
}

/* ---------------------------------- 게임 종료 --------------------------------- */
[MemoryPackable]
public partial class EndGameResponse : PacketHeader
{
    public int Result { get; set; }
}

[MemoryPackable]
public partial class EndGameRequest : PacketHeader
{
    public string RoomName { get; set; }
}
