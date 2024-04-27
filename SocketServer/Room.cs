using MemoryPack;
using System;
using System.Collections.Generic;

namespace SocketServer;
// 방 관련된 로직 처리하기.
// 방에 들어가기, 방에서 나가기, 방에 있는 클라이언트에게 메시지 보내기
// 방 정보: 방 이름, 방에 있는 클라이언트 리스트
public class Room(string name)
{
    string _name = name;
    List<Client> _clients = [];
    /* ------------------------------ Room Handlers ----------------------------- */
    public void EnterRoom(byte[] bytes)
    {
        var request = MemoryPackSerializer.Deserialize<EnterRoomRequest>(bytes);
        if (request == null)
        {
            // exception?
            return;
        }
        var response = new EnterRoomResponse();
        response.Result = false;

        if (request.RoomName == null)
        {
            // return MemoryPackSerializer.Serialize(response);
            // response를 보내야 한다?
            // Client를 어떻게 알것인가?
            return;
        }
        response.Result = true;
        // _clients.Add(client);

    }

    public void LeaveRoom(byte[] bytes)
    {
        // _clients.Remove(client);
    }
}
