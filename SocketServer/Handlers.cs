using MemoryPack;

namespace SocketServer;

public class Handlers
{
    public static void HandleLoginRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Login request received!");
        if (requestInfo._sessionId == null)
        {
            Console.WriteLine("SessionId is null!");
            return;
        }
        // requestInfo._sessionId를 이용하여 해당 유저의 정보를 가져온다.
        var user = new User(requestInfo._sessionId);
        // requestInfo._bytes를 Deserialize하고, UserId를 추출한다.
        var loginRequest = MemoryPackSerializer.Deserialize<LoginRequest>(requestInfo._bytes);
        if (loginRequest == null)
        {
            Console.WriteLine("UserId is null!");
            return;
        }
        user._userId = loginRequest.UserId;
        // 유저를 유저목록에 추가한다.
        Users.AddUser(user);
        // 해당 유저에게 보낼 응답을 생성한다. (LoginResponse패킷을 만들고, Serialize한다.)
        var loginResponse = new LoginResponse
        {
            Result = true
        };
        var response = MemoryPackSerializer.Serialize(loginResponse);
        // 해당 유저에게 응답을 보낸다. (SendAsync를 사용한다? SendData?)
    }

    public static void HandleEnterRoomRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Enter room request received!");
        // requestInfo._sessionId를 이용하여 해당 유저의 정보를 가져온다.
        // requestInfo._bytes를 Deserialize하고, RoomId를 추출한다.
        // 해당 유저를 해당 룸에 추가한다.
        // 해당 룸에 있는 모든 유저에게 해당 유저가 입장했다는 메시지를 보낸다. (ChatRequest 패킷인가?)
        // 해당 유저에게 해당 룸에 있는 모든 유저의 정보를 보낸다. << 이건 어떻게?
        // 해당 유저에게 보낼 응답을 생성한다. (EnterRoomResponse패킷을 만들고, Serialize한다.)
        // 해당 유저에게 응답을 보낸다. (SendData?)
    }

    public static void HandleLeaveRoomRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Leave room request received!");
        // requestInfo._sessionId를 이용하여 해당 유저의 정보를 가져온다.
        // requestInfo._bytes를 Deserialize하고, RoomId를 추출한다.
        // 해당 유저를 해당 룸에서 제거한다.
        // 해당 룸에 있는 모든 유저에게 해당 유저가 퇴장했다는 메시지를 보낸다. (ChatRequest 패킷인가?)
        // 해당 유저에게 보낼 응답을 생성한다. (LeaveRoomResponse패킷을 만들고, Serialize한다.)
        // 해당 유저에게 응답을 보낸다. (SendData?)
    }

    public static void HandleChatRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Chat request received!");
        // requestInfo._sessionId를 이용하여 해당 유저의 정보를 가져온다.
        // requestInfo._bytes를 Deserialize하고, ChatMessage를 추출한다.
        // 해당 유저가 속한 룸에 있는 모든 유저에게 해당 메시지를 보낸다.
        // 해당 유저에게 보낼 응답을 생성한다. (ChatResponse패킷을 만들고, Serialize한다.)
        // 해당 유저에게 응답을 보낸다. (SendData?)
    }

    public static void HandleStartGameRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Start game request received!");
        // requestInfo._sessionId를 이용하여 해당 유저의 정보를 가져온다.
        // requestInfo._bytes를 Deserialize하고, RoomId를 추출한다.
        // 해당 룸에 있는 모든 유저에게 게임을 시작한다는 메시지를 보낸다.
        //
    }

    public static void HandlePutStoneRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("Put stone request received!");
        // requestInfo._sessionId를 이용하여 해당 유저의 정보를 가져온다.
        // requestInfo._bytes를 Deserialize하고, StonePosition을 추출한다.

    }

    public static void HandleEndGameRequest(RequestInfo requestInfo)
    {
        Console.WriteLine("End game request received!");
        // requestInfo._sessionId를 이용하여 해당 유저의 정보를 가져온다.
        // requestInfo._bytes를 Deserialize하고, RoomId를 추출한다.
        // 해당 룸에 있는 모든 유저에게 게임이 종료되었다는 메시지를 보낸다.
        // 해당 룸에 있는 모든 유저의 점수를 계산한다.
        // 해당 룸에 있는 모든 유저에게 점수를 보낸다.
    }
}
