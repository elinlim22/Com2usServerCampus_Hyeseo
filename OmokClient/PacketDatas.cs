using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSCommon
{
    [MemoryPackable]
    public partial class PacketHeader
    {
        public Byte[] Head = new Byte[PacketHeaderInfo.HeadSize];
    }


    [MemoryPackable]
    public partial class NotifyUserMustClose : PacketHeader
    {
        public short Result;
    }


    // 로그인 요청
    [MemoryPackable]
    public partial class LoginRequest : PacketHeader
    {
        public string UserId;
        public string Token;
    }

    [MemoryPackable]
    public partial class LoginResult : PacketHeader
    {
        public short Result;
    }



    [MemoryPackable]
    public partial class PKTReqLobbyEnter : PacketHeader
    {
        public Int16 LobbyNumber;
    }

    [MemoryPackable]
    public partial class PKTResLobbyEnter : PacketHeader
    {
        public short Result;
        public Int16 LobbyNumber;
    }

    [MemoryPackable]
    public partial class PKTNtfLobbyEnterNewUser : PacketHeader
    {
        public string UserId;
    }


    [MemoryPackable]
    public partial class PKTResLobbyLeave : PacketHeader
    {
        public short Result;
    }

    [MemoryPackable]
    public partial class PKTNtfLobbyLeaveUser : PacketHeader
    {
        public string UserId;
    }


    [MemoryPackable]
    public partial class PKTReqLobbyChat : PacketHeader
    {
        public string Message;
    }


    [MemoryPackable]
    public partial class PKTNtfLobbyChat : PacketHeader
    {
        public string UserId;

        public string Message;
    }




    [MemoryPackable]
    public partial class EnterRoomRequest : PacketHeader
    {
        public int RoomNumber;
    }

    [MemoryPackable]
    public partial class EnterRoomResult : PacketHeader
    {
        public short Result;
    }

    [MemoryPackable]
    public partial class NotifyRoomUserList : PacketHeader
    {
        public List<string> UserIdList = new List<string>();
    }

    [MemoryPackable]
    public partial class NotifyRoomNewUser : PacketHeader
    {
        public string UserId;
    }


    [MemoryPackable]
    public partial class LeaveRoomRequest : PacketHeader
    {
        public string RoomName;
    }

    [MemoryPackable]
    public partial class LeaveRoomResponse : PacketHeader
    {
        public short Result;
    }

    [MemoryPackable]
    public partial class NotifyRoomUserLeft : PacketHeader
    {
        public int RoomNumber;
        public string UserId;
    }


    [MemoryPackable]
    public partial class ChatRequest : PacketHeader
    {
        public string Message;
    }


    [MemoryPackable]
    public partial class NotifyRoomChat : PacketHeader
    {
        public string UserId;

        public string Message;
    }
}
