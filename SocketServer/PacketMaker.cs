using MemoryPack;

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

}
