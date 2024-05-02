using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSCommon
{
    [MemoryPackObject]
    public class PKTResponse : PacketHeader
    {
        [Key(1)]
        public short Result;
    }



    // 로그인 요청
    [MemoryPackObject]
    public class LoginRequest : PacketHeader
    {
        [Key(1)]
        public string UserId;
        [Key(2)]
        public string Token;
    }

    [MemoryPackObject]
    public class LoginResult : PKTResponse
    {
    }



    [MemoryPackObject]
    public class PKTNtfMustClose : PKTResponse
    {
    }



    // 방 입장
    [MemoryPackObject]
    public class EnterRoomRequest : PacketHeader
    {
        [Key(1)]
        public int RoomNum;
    }

    [MemoryPackObject]
    public class EnterRoomResult : PKTResponse
    {
    }

    [MemoryPackObject]
    public class NotifyRoomUserList : PacketHeader
    {
        [Key(1)]
        public List<string> UserIdList = new List<string>();
    }

    [MemoryPackObject]
    public class NotifyRoomNewUser : PacketHeader
    {
        [Key(1)]
        public string UserId;
    }


    // 방 나가기(보디가 없다)
    [MemoryPackObject]
    public class LeaveRoomRequest : PacketHeader
    {
    }

    [MemoryPackObject]
    public class LeaveRoomResponse : PKTResponse
    {
    }

    [MemoryPackObject]
    public class NotifyRoomUserLeft : PacketHeader
    {
        [Key(1)]
        public string UserId;
    }


    [MemoryPackObject]
    public class ChatRequest : PacketHeader
    {
        [Key(1)]
        public string Message;
    }

    [MemoryPackObject]
    public class PKTResRoomChat : PKTResponse
    {
    }

    [MemoryPackObject]
    public class NotifyRoomChat : PacketHeader
    {
        [Key(1)]
        public string UserId;

        [Key(2)]
        public string Message;
    }

    [MemoryPackObject]
    public class PKTInternalReqRoomEnter : PacketHeader
    {
        [Key(1)]
        public int RoomNumber;

        [Key(2)]
        public string UserId;
    }


    // 오목 플레이 준비 완료 요청
    [MemoryPackObject]
    public class PKTReqReadyOmok : PKTResponse
    {
    }

    [MemoryPackObject]
    public class PKTResReadyOmok : PKTResponse
    {
    }

    [MemoryPackObject]
    public class PKTNtfReadyOmok : PacketHeader
    {
        [Key(1)]
        public string UserId;
        [Key(2)]
        public bool IsReady;
    }


    // 오목 시작 통보(서버에서 클라이언트들에게)
    [MemoryPackObject]
    public class PKTNtfStartOmok : PacketHeader
    {
        [Key(1)]
        public string FirstUserId; // 선턴 유저 ID
    }


    // 돌 두기
    [MemoryPackObject]
    public class PKTReqPutMok : PacketHeader
    {
        [Key(1)]
        public int PosX;
        [Key(2)]
        public int PosY;
    }

    [MemoryPackObject]
    public class PKTResPutMok : PKTResponse
    {
    }

    [MemoryPackObject]
    public class PKTNtfPutMok : PacketHeader
    {
        [Key(1)]
        public int PosX;
        [Key(2)]
        public int PosY;
        [Key(3)]
        public int Mok;
    }

    // 오목 게임 종료 통보
    [MemoryPackObject]
    public class PKTNtfEndOmok : PacketHeader
    {
        [Key(1)]
        public string WinUserId;
    }


}
