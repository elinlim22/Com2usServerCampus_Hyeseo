﻿using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSCommon
{
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
    public partial class NotifyUserMustClose : PacketHeader
    {
        public short Result;
    }



    // 방 입장
    [MemoryPackable]
    public partial class EnterRoomRequest : PacketHeader
    {
        public int RoomNum;
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


    // 방 나가기(보디가 없다)
    [MemoryPackable]
    public partial class LeaveRoomRequest : PacketHeader
    {
    }

    [MemoryPackable]
    public partial class LeaveRoomResponse : PacketHeader
    {
        public short Result;
    }

    [MemoryPackable]
    public partial class NotifyRoomUserLeft : PacketHeader
    {
        public string UserId;
    }


    [MemoryPackable]
    public partial class ChatRequest : PacketHeader
    {
        public string Message;
    }

    [MemoryPackable]
    public partial class ChatResponse : PacketHeader
    {
        public int Result;
    }

    [MemoryPackable]
    public partial class NotifyRoomChat : PacketHeader
    {
        public string UserId;

        public string Message;
    }

    // [MemoryPackable]
    // public partial class PKTInternalReqRoomEnter : PacketHeader
    // {
    //     public int RoomNumber;

    //     public string UserId;
    // }


    // 오목 플레이 준비 완료 요청
    [MemoryPackable]
    public partial class PKTReqReadyOmok : PacketHeader
    {
        public short Result;
    }

    [MemoryPackable]
    public partial class PKTResReadyOmok : PacketHeader
    {
        public short Result;
    }

    [MemoryPackable]
    public partial class PKTNtfReadyOmok : PacketHeader
    {
        public string UserId;
        public bool IsReady;
    }


    // 오목 시작 통보(서버에서 클라이언트들에게)
    [MemoryPackable]
    public partial class PKTNtfStartOmok : PacketHeader
    {
        public string FirstUserId; // 선턴 유저 ID
    }


    // 돌 두기
    [MemoryPackable]
    public partial class PutStoneRequest : PacketHeader
    {
        public int X;
        public int Y;
    }

    [MemoryPackable]
    public partial class PutStoneResponse : PacketHeader
    {
        public short Result;
    }

    [MemoryPackable]
    public partial class NotifyPutStone : PacketHeader
    {
        public int X;
        public int Y;
        public int Mok;
    }

    // 오목 게임 종료 통보
    [MemoryPackable]
    public partial class PKTNtfEndOmok : PacketHeader
    {
        public string WinUserId;
    }


}
