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

    public static RequestInfo MakeNotifyUserMustClose(ErrorCode errorCode, string sessionID)
    {
        var notifyPacket = new NotifyUserMustClose()
        {
            ErrorCode = (short)errorCode
        };

        var sendData = MemoryPackSerializer.Serialize(notifyPacket);
        PacketHeaderInfo.Write(sendData, PacketType.NotifyUserMustClose);

        var newPacket = new RequestInfo(null)
        {
            Data = sendData,
            SessionID = sessionID
        };
        return newPacket;
    }

    public static RequestInfo MakeNotifyUserMustLeave(string userId, string sessionId, int roomNumber)
    {
        var notifyPacket = new NotifyUserMustLeave()
        {
            RoomNumber = roomNumber,
            UserId = userId
        };

        var sendData = MemoryPackSerializer.Serialize(notifyPacket);
        PacketHeaderInfo.Write(sendData, PacketType.NotifyUserMustLeave);

        var newPacket = new RequestInfo(null)
        {
            Data = sendData,
            SessionID = sessionId
        };
        return newPacket;
    }

}
