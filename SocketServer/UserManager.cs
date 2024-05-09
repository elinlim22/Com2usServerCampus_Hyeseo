using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;


namespace SocketServer;

public class UserManager
{
    int _totalUserMaximum; // TODO : Config���� MaxUserCount �ҷ��ͼ� �����ϱ�
    int currentGroupIndex = 0;
    UInt64 _userSequenceNumber = 0;
    
    List<Tuple<string, User>> UserStatusCheckList;
    Timer UserStatusCheckTimer;
    TimerCallback UserStatusCheckTimerCallback;
    // TimeSpan StatusStandardTime = TimeSpan.FromHours(1); // TODO : Config�� ����
    TimeSpan StatusStandardTime = TimeSpan.FromSeconds(5); // Debug��
    Dictionary<string, User> Users = [];
    public static Func<string, byte[], bool> SendData;
    public static Action<string> CloseSession;

    public void Init(int totalUserMaximum)
    {
        _totalUserMaximum = totalUserMaximum;
        UserStatusCheckList = new List<Tuple<string, User>>(_totalUserMaximum);
        // UserStatusCheckList �ʱ�ȭ
        for (int i = 0; i < _totalUserMaximum; i++)
        {
            UserStatusCheckList.Add(null);
        }

        // ���� �������� Ÿ�̸� ����
        SetTimer();
    }

    public ErrorCode AddSession(string sessionID) // Session�� ����ǰ� �α������� ���� ���, ������ �߰���(ù ����)
    {
        var findUserSession = UserStatusCheckList.Find(x => x?.Item1 == sessionID);
        if (findUserSession != null)
        {
            return ErrorCode.UserSessionAlreadyExist;
        }
        else
        {
            var newUserSession = new Tuple<string, User>(sessionID, new User());
            newUserSession.Item2.Set(0, sessionID, null); // ���Ǿ��̵� ����
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
        } // �ش� ����(����)�� �α����� ��� AddUser���� UserStatusCheckList�� �߰���
    }

    public void SetTimer()
    {
        UserStatusCheckTimerCallback = new TimerCallback(UserStatusCheck);
        UserStatusCheckTimer = new System.Threading.Timer(UserStatusCheckTimerCallback, null, 0, 250); // TODO : Config�� ����
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
                // ���� �������� ��Ŷ �����Ͽ� Ŭ���̾�Ʈ���� ������ �����Ų��. << TODO : ���ʿ�?
                var sendPacket = PacketMaker.MakeNotifyUserMustClose(ErrorCode.UserForcedClose, userSession.Item1);
                SendData(userSession.Item1, sendPacket);

                // ������ �ݱ�
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

    public ErrorCode AddUser(string UserId, string sessionID) // ������ �α����� ��
    {
        if(IsFull())
        {
            return ErrorCode.UserFull;
        }
        ++_userSequenceNumber; // << ?

        // �ش� ������ ������ ã�Ƽ� ��������Ʈ�� �߰�
        var userSession = UserStatusCheckList.Find(x => x?.Item1 == sessionID);
        if(userSession == null)
        {
            MainServer.MainLogger.Error($"UserSession Not Found. SessionID:{sessionID}");
            return ErrorCode.UserSessionNotFound; // �������� ���� ������ �α��� �õ�
        }
        userSession.Item2.Set(_userSequenceNumber, sessionID, UserId);
        Users.Add(sessionID, userSession.Item2);
        
        userSession.Item2.UpdateLastConnection();
        return ErrorCode.Success;
    }

    public ErrorCode RemoveUser(string sessionID) // ���� ���� ��
    {
        // ���� ��������� ����Ʈ���� ����
        var userSessionIndex = UserStatusCheckList.FindIndex(x => x?.Item1 == sessionID);
        if(userSessionIndex == -1)
        {
            MainServer.MainLogger.Error($"UserSession Not Found. SessionID:{sessionID}");
            return ErrorCode.UserSessionNotFound;
        }
        UserStatusCheckList[userSessionIndex] = null;
        // ���� ��ųʸ����� ����
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
