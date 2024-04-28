namespace SocketServer;

public class Users
{
    public static Dictionary<string, User> _users = [];

    public static void AddUser(User user)
    {
        _users.Add(user._sessionId, user);
    }

    public static void RemoveUser(string sessionId)
    {
        _users.Remove(sessionId);
    }

    public static User GetUser(string sessionId)
    {
        return _users[sessionId];
    }
}

public class User(string sessionId)
{
    public string? _userId;
    public string _sessionId = sessionId;
    public Int32 _roomId = -1;
}
