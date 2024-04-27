namespace SocketServer;

public class Handlers
{
    public static void HandleLoginRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Login request received!");
    }

    public static void HandleEnterRoomRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Enter room request received!");
    }

    public static void HandleLeaveRoomRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Leave room request received!");
    }

    public static void HandleChatRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Chat request received!");
    }

    public static void HandleStartGameRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Start game request received!");
    }

    public static void HandlePutStoneRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Put stone request received!");
    }

    public static void HandleEndGameRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("End game request received!");
    }
}
