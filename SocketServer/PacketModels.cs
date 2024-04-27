using MemoryPack;

namespace SocketServer;

[MemoryPackable]
public partial class PacketHeader
{
    public int Id { get; set; }
    public int Size { get; set; }
    // public int Type { get; set; }
}
/* ----------------------------------- 로그인 ---------------------------------- */
[MemoryPackable]
public partial class LoginRequest : PacketHeader
{
    public string? UserId { get; set; }
    public string? Token { get; set; }
}

[MemoryPackable]
public partial class LoginResponse : PacketHeader
{
    public bool Result { get; set; }
}

/* ---------------------------------- 방 입장 ---------------------------------- */
[MemoryPackable]
public partial class EnterRoomRequest : PacketHeader
{
    public string? RoomName { get; set; }
}

[MemoryPackable]
public partial class EnterRoomResponse : PacketHeader
{
    public bool Result { get; set; }
}

/* ---------------------------------- 방 나가기 --------------------------------- */
[MemoryPackable]
public partial class LeaveRoomRequest : PacketHeader
{
    public string? RoomName { get; set; }
}

[MemoryPackable]
public partial class LeaveRoomResponse : PacketHeader
{
    public bool Result { get; set; }
}

/* ---------------------------------- 방 채팅 ---------------------------------- */
[MemoryPackable]
public partial class ChatRequest : PacketHeader
{
    public string? Message { get; set; }
}

[MemoryPackable]
public partial class ChatResponse : PacketHeader
{
    public bool Result { get; set; }
}

/* ---------------------------------- 게임 시작 --------------------------------- */
[MemoryPackable]
public partial class StartGameRequest : PacketHeader
{
    public string? RoomName { get; set; }
}

[MemoryPackable]
public partial class StartGameResponse : PacketHeader
{
    public bool Result { get; set; }
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
    public bool Result { get; set; }
}

/* ---------------------------------- 게임 종료 --------------------------------- */
[MemoryPackable]
public partial class GameEndResponse : PacketHeader
{
    public bool Result { get; set; }
}

[MemoryPackable]
public partial class GameEndRequest : PacketHeader
{
    public string? RoomName { get; set; }
}
