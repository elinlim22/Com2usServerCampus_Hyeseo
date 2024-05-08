using System;
using System.Collections.Generic;
using System.Linq;


namespace SocketServer;

public class UserManager
{
    int _totalUserMaximum; // TODO : Config���� MaxUserCount �ҷ��ͼ� �����ϱ�
    int currentGroupIndex = 0;
    UInt64 _userSequenceNumber = 0;
    
    int _maxUserCount;
    List<Tuple<string, User>> UserStatusCheckList;
    Timer UserStatusCheckTimer;
    TimerCallback UserStatusCheckTimerCallback;
    TimeSpan StatusStandardTime = TimeSpan.FromHours(1); // TODO : Config�� ����

    Dictionary<string, User> Users = [];
    public static Action<RequestInfo> DistributeInnerPacket;

    public void Init(int totalUserMaximum)
    {
        _totalUserMaximum = totalUserMaximum;
        UserStatusCheckList = new List<Tuple<string, User>>(_totalUserMaximum);

        // ���� �������� Ÿ�̸� ����
        SetTimer();
    }

    public ErrorCode AddSession(string sessionID) // Session�� ����ǰ� �α������� ���� ���, ������ �߰���(ù ����)
    {
        var findUserSession = UserStatusCheckList.Find(x => x.Item1 == sessionID);
        if (findUserSession != null)
        {
            return ErrorCode.UserSessionAlreadyExist;
        }
        else
        {
            var newUserSession = new Tuple<string, User>(sessionID, new User());
            newUserSession.Item2.Set(0, sessionID, null); // ���Ǿ��̵� ����
            newUserSession.Item2.UpdateLastConnection();
            var insertIndex = UserStatusCheckList.FindIndex(x => x.Item1 == null);
            if(insertIndex == -1)
            {
                return ErrorCode.UserFull;
            }
            else
            {
                UserStatusCheckList[insertIndex] = newUserSession;
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
        foreach (var userSession in UserStatusCheckList.GetRange(startIndex, batchSize))
        {
            if (userSession == null || userSession.Item2.RoomNumber == -1)
            {
                continue;
            }
            if (DateTime.Now.TimeOfDay - userSession.Item2.LastConnection > StatusStandardTime)
            {
                // ���� �������� ��Ŷ �����Ͽ� Ŭ���̾�Ʈ���� ������ �����Ų��.
                var sendPacket = PacketMaker.MakeNotifyUserMustClose(ErrorCode.UserForcedClose, userSession.Item1);
                DistributeInnerPacket(sendPacket);
                MainServer.MainLogger.Error($"User Forced Close due to Inactivity. SessionID:{userSession.Item1}");
                RemoveUser(userSession.Item1);
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
        var userSession = UserStatusCheckList.Find(x => x.Item1 == sessionID);
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
        if (!Users.ContainsKey(sessionID))
        {
            return ErrorCode.UserSessionNotFound;
        }
        // ���� ��ųʸ����� ����
        Users.Remove(sessionID);
        // ���� ��������� ����Ʈ���� ����
        var userSessionIndex = UserStatusCheckList.FindIndex(x => x.Item1 == sessionID);
        UserStatusCheckList[userSessionIndex] = null;
        return ErrorCode.Success;
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
