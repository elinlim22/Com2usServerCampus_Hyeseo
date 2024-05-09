using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;


namespace SocketServer;

public class UserManager
{
    int _totalUserMaximum; // TODO : Config에서 MaxUserCount 불러와서 설정하기
    int currentGroupIndex = 0;
    UInt64 _userSequenceNumber = 0;
    
    List<Tuple<string, User>> UserStatusCheckList;
    Timer UserStatusCheckTimer;
    TimerCallback UserStatusCheckTimerCallback;
    // TimeSpan StatusStandardTime = TimeSpan.FromHours(1); // TODO : Config로 빼기
    TimeSpan StatusStandardTime = TimeSpan.FromSeconds(5); // Debug용
    Dictionary<string, User> Users = [];
    public static Func<string, byte[], bool> SendData;
    public static Action<string> CloseSession;

    public void Init(int totalUserMaximum)
    {
        _totalUserMaximum = totalUserMaximum;
        UserStatusCheckList = new List<Tuple<string, User>>(_totalUserMaximum);
        // UserStatusCheckList 초기화
        for (int i = 0; i < _totalUserMaximum; i++)
        {
            UserStatusCheckList.Add(null);
        }

        // 유저 상태조사 타이머 세팅
        SetTimer();
    }

    public ErrorCode AddSession(string sessionID) // Session만 연결되고 로그인하지 않은 경우, 세션을 추가함(첫 접속)
    {
        var findUserSession = UserStatusCheckList.Find(x => x?.Item1 == sessionID);
        if (findUserSession != null)
        {
            return ErrorCode.UserSessionAlreadyExist;
        }
        else
        {
            var newUserSession = new Tuple<string, User>(sessionID, new User());
            newUserSession.Item2.Set(0, sessionID, null); // 세션아이디만 저장
            newUserSession.Item2.UpdateLastConnection();
            var insertIndex = UserStatusCheckList.FindIndex(x => x?.Item1 == null);
            if(insertIndex == -1)
            {
                return ErrorCode.UserFull;
            }
            else
            {
                UserStatusCheckList[insertIndex] = newUserSession;
                if (UserStatusCheckList.FindIndex(x => x?.Item1 == sessionID) == -1)
                {
                    MainServer.MainLogger.Error($"AddSession Fail : SessionID : {sessionID} Not Added.");
                }
                else
                {
                    MainServer.MainLogger.Debug($"AddSession Success : SessionID : {sessionID}");
                }
            }
            return ErrorCode.Success;
        } // 해당 유저(세션)이 로그인한 경우 AddUser에서 UserStatusCheckList에 추가됨
    }

    public void SetTimer()
    {
        UserStatusCheckTimerCallback = new TimerCallback(UserStatusCheck);
        UserStatusCheckTimer = new System.Threading.Timer(UserStatusCheckTimerCallback, null, 0, 250); // TODO : Config로 빼기
        MainServer.MainLogger.Debug("UserStatusCheckTimer Start");
    }

    public void UserStatusCheck(object state)
    {
        int batchSize = UserStatusCheckList.Count / 4;

        int startIndex = currentGroupIndex * batchSize;
        for (int i = startIndex; i < startIndex + batchSize; i++)
        {
            var userSession = UserStatusCheckList[i];

            if (userSession == null || userSession.Item2.RoomNumber != -1)
            {
                continue;
            }
            if (DateTime.Now.TimeOfDay - userSession.Item2.LastConnection > StatusStandardTime)
            {
                // 유저 강제종료 패킷 전송하여 클라이언트에서 세션을 종료시킨다. << TODO : 불필요?
                var sendPacket = PacketMaker.MakeNotifyUserMustClose(ErrorCode.UserForcedClose, userSession.Item1);
                SendData(userSession.Item1, sendPacket);

                // 세션을 닫기
                CloseSession(userSession.Item1);
                MainServer.MainLogger.Error($"User Forced Close due to Inactivity. SessionID:{userSession.Item1}");
                RemoveUser(userSession.Item1);
                UserStatusCheckList[i] = null;
            }
        }
        currentGroupIndex++;
        if (currentGroupIndex >= 4)
        {
            currentGroupIndex = 0;
        }
    }

    public ErrorCode AddUser(string UserId, string sessionID) // 유저가 로그인할 때
    {
        if(IsFull())
        {
            return ErrorCode.UserFull;
        }
        ++_userSequenceNumber; // << ?

        // 해당 세션의 유저를 찾아서 유저리스트에 추가
        var userSession = UserStatusCheckList.Find(x => x?.Item1 == sessionID);
        if(userSession == null)
        {
            MainServer.MainLogger.Error($"UserSession Not Found. SessionID:{sessionID}");
            return ErrorCode.UserSessionNotFound; // 접속하지 않은 유저가 로그인 시도
        }
        userSession.Item2.Set(_userSequenceNumber, sessionID, UserId);
        Users.Add(sessionID, userSession.Item2);
        
        userSession.Item2.UpdateLastConnection();
        return ErrorCode.Success;
    }

    public ErrorCode RemoveUser(string sessionID) // 세션 종료 시
    {
        // 유저 상태조사용 리스트에서 삭제
        var userSessionIndex = UserStatusCheckList.FindIndex(x => x?.Item1 == sessionID);
        if(userSessionIndex == -1)
        {
            MainServer.MainLogger.Error($"UserSession Not Found. SessionID:{sessionID}");
            return ErrorCode.UserSessionNotFound;
        }
        UserStatusCheckList[userSessionIndex] = null;
        // 유저 딕셔너리에서 삭제
        if (!Users.ContainsKey(sessionID))
        {
            return ErrorCode.UserSessionNotFound;
        }
        Users.Remove(sessionID);
        return ErrorCode.Success;
    }

    public void UpdateUserLastConnection(string sessionID)
    {
        var user = GetUser(sessionID);
        if(user != null)
        {
            user.UpdateLastConnection();
        }
        else
        {
            MainServer.MainLogger.Error($"UpdateUserLastConnection: User Not Found. SessionID:{sessionID}");
        }
    }

    public User GetUser(string sessionID)
    {
        Users.TryGetValue(sessionID, out User user);
        return user;
    }

    bool IsFull()
    {
        return _totalUserMaximum <= Users.Count;
    }
}
