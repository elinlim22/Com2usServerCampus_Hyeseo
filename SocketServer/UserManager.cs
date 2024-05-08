using System;
using System.Collections.Generic;
using System.Linq;


namespace SocketServer;

public class UserManager
{
    int _totalUserMaximum;
    UInt64 _userSequenceNumber = 0;
    
    // HeartBeat
    int _maxUserCount;
    List<User> _usersHeartBeat = [];
    //Timer _timer;

    Dictionary<string, User> Users = [];


    public void Init(int totalUserMaximum)
    {
        _totalUserMaximum = totalUserMaximum;
       // _timer.Inverval = 250; // TODO : 상수로 넣지 말고 Config로 빼자
       // _timer.Run(OnHeartBeat);
    }

    public ErrorCode AddUser(string UserId, string sessionID)
    {
        if(IsFull())
        {
            return ErrorCode.UserFull;
        }
        ++_userSequenceNumber;

        var user = new User();
        user.Set(_userSequenceNumber, sessionID, UserId);
        Users.Add(sessionID, user);

        return ErrorCode.Success;
    }

    public ErrorCode RemoveUser(string sessionID)
    {
        if(Users.Remove(sessionID) == false)
        {
            return ErrorCode.UserNotFound;
        }
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
