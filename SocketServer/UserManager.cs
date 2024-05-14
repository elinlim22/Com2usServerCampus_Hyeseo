﻿using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;


namespace SocketServer;

public class UserManager(ServerOption serverOption)
{
    int _totalUserMaximum; // TODO : Config에서 MaxUserCount 불러와서 설정하기
    int currentGroupIndex = 0;
    UInt64 _userSequenceNumber = 0;
    int batchSize = serverOption.UserStatusCheckSize;

    List<User> Users = [];
    Timer UserStatusCheckTimer;
    TimerCallback UserStatusCheckTimerCallback;
    // TimeSpan StatusStandardTime = TimeSpan.FromHours(1); // TODO : Config로 빼기
    TimeSpan StatusStandardTime = TimeSpan.FromSeconds(10); // Debug용

    public static Func<string, byte[], bool> SendData;
    public static Action<RequestInfo> DistributeInnerPacket;

    public void Init(int totalUserMaximum)
    {
        _totalUserMaximum = totalUserMaximum;
        for (int i = 0; i < _totalUserMaximum; i++)
        {
            Users.Add(new User());
        }
        SetTimer();
    }

    public void SetTimer()
    {
        UserStatusCheckTimerCallback = new TimerCallback(UserStatusCheck);
        UserStatusCheckTimer = new System.Threading.Timer(UserStatusCheckTimerCallback, null, 0, 250); // TODO : Config로 빼기
        MainServer.MainLogger.Debug("UserStatusCheckTimer Start");
    }

    public void UserStatusCheck(object state)
    {
        int startIndex = currentGroupIndex * batchSize;
        int endIndex = startIndex + batchSize;
        if (endIndex > _totalUserMaximum)
        {
            endIndex = _totalUserMaximum;
        }
        for (int i = startIndex; i < endIndex; i++)
        {
            if (i >= _totalUserMaximum)
            {
                break;
            }
            var user = Users[i];
            var sessionID = user.GetSessionID();
            if (DateTime.Now.TimeOfDay - user.LastConnection > StatusStandardTime)
            {
                // 유저 강제종료 패킷 전송하여 클라이언트에서 세션을 종료시킨다. << TODO : 불필요?
                var sendPacket = PacketMaker.MakeNotifyUserMustClose(ErrorCode.UserForcedClose, sessionID);
                SendData(sessionID, sendPacket);

                // 세션을 닫기 이너패킷 전송
                MainServer.MainLogger.Error($"User Forced Close due to Inactivity. SessionID:{sessionID}");
                var innerPacket = PacketMaker.MakeCloseSessionRequest(sessionID);
                DistributeInnerPacket(innerPacket);
            }
        }
        currentGroupIndex++;
        if (endIndex == _totalUserMaximum)
        {
            currentGroupIndex = 0;
        }
    }

    public ErrorCode AddSession(string sessionID)
    {
        if (IsFull())
        {
            MainServer.MainLogger.Error($"AddSession failed: User Full: SessionID:{sessionID}");
            return ErrorCode.UserFull;
        }
        var user = GetUser(sessionID) ?? Users.Find(x => x.SequenceNumber == 0);
        ++_userSequenceNumber;
        user.Set(_userSequenceNumber, sessionID, "");
        user.UpdateLastConnection();
        return ErrorCode.Success;
    }

    public ErrorCode AddUser(string UserId, string sessionID)
    {
        if(IsFull())
        {
            MainServer.MainLogger.Error($"AddUser failed: User Full: SessionID:{sessionID}");
            return ErrorCode.UserFull;
        }
        var user = GetUser(sessionID);
        if(user == null)
        {
            MainServer.MainLogger.Error($"AddUser: UserSession Not Found. SessionID:{sessionID}");
            return ErrorCode.UserSessionNotFound; // 접속하지 않은 유저가 로그인 시도
        }
        user.Set(_userSequenceNumber, sessionID, UserId);
        
        user.UpdateLastConnection();
        return ErrorCode.Success;
    }

    public ErrorCode RemoveUser(string sessionID) // 세션 종료 시
    {
        var user = GetUser(sessionID);
        if (user == null)
        {
            MainServer.MainLogger.Error($"RemoveUser: User Not Found. SessionID:{sessionID}");
            return ErrorCode.UserNotFound;
        }
        Users.Remove(user);
        return ErrorCode.Success;
    }

    public void UpdateUserLastConnection(string sessionID)
    {
        var user = GetUser(sessionID);
        if(user == null)
        {
            MainServer.MainLogger.Error($"UpdateUserLastConnection: User Not Found. SessionID:{sessionID}");
            return;
        }
        user.UpdateLastConnection();
    }

    public User GetUser(string sessionID)
    {
        var user = Users.Find(x => x.GetSessionID() == sessionID);
        return user;
    }

    bool IsFull()
    {
        var userSpace = Users.Find(x => x.SequenceNumber == 0);
        return userSpace == null;
    }
}
