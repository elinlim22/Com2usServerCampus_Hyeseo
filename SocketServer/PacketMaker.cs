using MemoryPack;
using System.Dynamic;

namespace SocketServer;

public class PacketMaker
{
    public static RequestInfo MakeNotifyRoomUserLeftPacket(string sessionID, int roomNumber, string UserId)
    {
        var packet = new NotifyRoomUserLeft()
        {
            RoomNumber = roomNumber,
            UserId = UserId,
        };

        var sendData = MemoryPackSerializer.Serialize(packet);
        PacketHeaderInfo.Write(sendData, PacketType.NotifyRoomUserLeft);

        var newPacket = new RequestInfo(null)
        {
            Data = sendData,
            SessionID = sessionID
        };
        return newPacket;
    }

    public static RequestInfo MakeSessionConnectionPacket(bool isConnect, string sessionID)
    {
        var newPacket = new RequestInfo(null)
        {
            Data = new byte[(UInt16)PacketDefine.HeaderSize]
        };

        if (isConnect)
        {
            PacketHeaderInfo.WritePacketType(newPacket.Data, (UInt16)PacketType.InSessionConnected);
        }
        else
        {
            PacketHeaderInfo.WritePacketType(newPacket.Data, (UInt16)PacketType.InSessionDisconnected);
        }

        newPacket.SessionID = sessionID;
        return newPacket;
    }

    public static byte[] MakeNotifyUserMustClose(ErrorCode errorCode, string sessionID)
    {
        var notifyPacket = new NotifyUserMustClose()
        {
            ErrorCode = (short)errorCode
        };

        var sendData = MemoryPackSerializer.Serialize(notifyPacket);
        PacketHeaderInfo.Write(sendData, PacketType.NotifyUserMustClose);

        return sendData;
    }

    public static RequestInfo MakeInnerUserLeaveRoom(string sessionId)
    {
        var innerPacket = new LeaveRoomRequest();

        var sendData = MemoryPackSerializer.Serialize(innerPacket);
        PacketHeaderInfo.Write(sendData, PacketType.LeaveRoomRequest);
        var newInnerPacket = new RequestInfo(sendData)
        {
            SessionID = sessionId
        };
        return newInnerPacket;
    }

    public static RequestInfo MakeCloseSessionRequest(string sessionId)
    {
        var innerPacket = new CloseSessionRequest();

        var sendData = MemoryPackSerializer.Serialize(innerPacket);
        PacketHeaderInfo.Write(sendData, PacketType.CloseSessionRequest);
        var newInnerPacket = new RequestInfo(sendData)
        {
            SessionID = sessionId
        };
        return newInnerPacket;
    }

    public static RequestInfo MakeForfeitureRequest(string sessionId)
    {
        var innerPacket = new ForfeitureRequest();

        var sendData = MemoryPackSerializer.Serialize(innerPacket);
        PacketHeaderInfo.Write(sendData, PacketType.ForfeitureRequest);
        var newInnerPacket = new RequestInfo(sendData)
        {
            SessionID = sessionId
        };
        return newInnerPacket;
    }

    public static RequestInfo MakeWinnerDBUpdate(string userId, string sessionId)
    {
        var innerPacket = new UpdateUserGameDataRequest()
        {
            UserId = userId,
            IsWinner = true
        };

        var sendData = MemoryPackSerializer.Serialize(innerPacket);
        PacketHeaderInfo.Write(sendData, PacketType.UpdateUserGameDataRequest);
        var newInnerPacket = new RequestInfo(sendData)
        {
            SessionID = sessionId
        };
        return newInnerPacket;
    }

    public static RequestInfo MakeLoserDBUpdate(string userId, string sessionId)
    {
        var innerPacket = new UpdateUserGameDataRequest()
        {
            UserId = userId,
            IsWinner = false
        };

        var sendData = MemoryPackSerializer.Serialize(innerPacket);
        PacketHeaderInfo.Write(sendData, PacketType.UpdateUserGameDataRequest);
        var newInnerPacket = new RequestInfo(sendData)
        {
            SessionID = sessionId
        };
        return newInnerPacket;
    }

    public static RequestInfo MakeValidateUserTokenRequest(string userId, string sessionId, string token)
    {
        var innerPacket = new ValidateUserTokenRequest()
        {
            UserId = userId,
            Token = token
        };

        var sendData = MemoryPackSerializer.Serialize(innerPacket);
        PacketHeaderInfo.Write(sendData, PacketType.ValidateUserTokenRequest);
        var newInnerPacket = new RequestInfo(sendData)
        {
            SessionID = sessionId
        };
        return newInnerPacket;
    }

    public static RequestInfo MakeValidateUserTokenResponse(short result, string sessionId)
    {
        var innerPacket = new ValidateUserTokenResponse()
        {
            Result = result
        };

        var sendData = MemoryPackSerializer.Serialize(innerPacket);
        PacketHeaderInfo.Write(sendData, PacketType.ValidateUserTokenResponse);
        var newInnerPacket = new RequestInfo(sendData)
        {
            SessionID = sessionId
        };
        return newInnerPacket;
    }

    public static RequestInfo MakeUserStatusCheckRequest(int i)
    {
        var innerPacket = new UserStatusCheckRequest()
        {
            Index = i
        };

        var sendData = MemoryPackSerializer.Serialize(innerPacket);
        PacketHeaderInfo.Write(sendData, PacketType.UserStatusCheckRequest);
        var newInnerPacket = new RequestInfo(sendData);
        return newInnerPacket;
    }

    public static RequestInfo MakeRoomStatusCheckRequest(int i)
    {
        var innerPacket = new RoomStatusCheckRequest()
        {
            Index = i
        };

        var sendData = MemoryPackSerializer.Serialize(innerPacket);
        PacketHeaderInfo.Write(sendData, PacketType.RoomStatusCheckRequest);
        var newInnerPacket = new RequestInfo(sendData);
        return newInnerPacket;
    }
}
