using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSCommon
{
    [MemoryPackable]
    public class PacketHeader
    {
        [Key(0)]
        public Byte[] Head = new Byte[PacketHeaderInfo.HeadSize];
    }


    [MemoryPackable]
    public class NotifyUserMustClose : PacketHeader
    {
        [Key(1)]
        public short Result;
    }


    // 로그인 요청
    [MemoryPackable]
    public class LoginRequest : PacketHeader
    {
        [Key(1)]
        public string UserId;
        [Key(2)]
        public string Token;
    }

    [MemoryPackable]
    public class LoginResult : PacketHeader
    {
        [Key(1)]
        public short Result;
    }



    [MemoryPackable]
    public class PKTReqLobbyEnter : PacketHeader
    {
        [Key(1)]
        public Int16 LobbyNumber;
    }

    [MemoryPackable]
    public class PKTResLobbyEnter : PacketHeader
    {
        [Key(1)]
        public short Result;
        [Key(2)]
        public Int16 LobbyNumber;
    }

    [MemoryPackable]
    public class PKTNtfLobbyEnterNewUser : PacketHeader
    {
        [Key(1)]
        public string UserId;
    }


    [MemoryPackable]
    public class PKTResLobbyLeave : PacketHeader
    {
        [Key(1)]
        public short Result;
    }

    [MemoryPackable]
    public class PKTNtfLobbyLeaveUser : PacketHeader
    {
        [Key(1)]
        public string UserId;
    }


    [MemoryPackable]
    public class PKTReqLobbyChat : PacketHeader
    {
        [Key(1)]
        public string Message;
    }


    [MemoryPackable]
    public class PKTNtfLobbyChat : PacketHeader
    {
        [Key(1)]
        public string UserId;

        [Key(2)]
        public string Message;
    }




    [MemoryPackable]
    public class EnterRoomRequest : PacketHeader
    {
        [Key(1)]
        public int RoomNumber;
    }

    [MemoryPackable]
    public class EnterRoomResult : PacketHeader
    {
        [Key(1)]
        public short Result;
    }

    [MemoryPackable]
    public class NotifyRoomUserList : PacketHeader
    {
        [Key(1)]
        public List<string> UserIdList = new List<string>();
    }

    [MemoryPackable]
    public class NotifyRoomNewUser : PacketHeader
    {
        [Key(1)]
        public string UserId;
    }


    [MemoryPackable]
    public class LeaveRoomRequest : PacketHeader
    {
        public string RoomName;
    }

    [MemoryPackable]
    public class LeaveRoomResponse : PacketHeader
    {
        [Key(1)]
        public short Result;
    }

    [MemoryPackable]
    public class NotifyRoomUserLeft : PacketHeader
    {
        [Key(1)]
        public int RoomNumber;
        public string UserId;
    }


    [MemoryPackable]
    public class ChatRequest : PacketHeader
    {
        [Key(1)]
        public string Message;
    }


    [MemoryPackable]
    public class NotifyRoomChat : PacketHeader
    {
        [Key(1)]
        public string UserId;

        [Key(2)]
        public string Message;
    }
}
