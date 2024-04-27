namespace SocketServer;

public class HandlerDictionary
{
    public static Dictionary<int, Action<RequestInfo>> PacketHandlers = [];

    public void RegistPacketHandler(int packetType, Action<RequestInfo> handler)
    {
        PacketHandlers[packetType] = handler;
    }

    public void HandlePacket(int packetId, RequestInfo receivedPacket)
    {
        if (PacketHandlers.TryGetValue(packetId, out Action<RequestInfo>? value))
        {
            value(receivedPacket);
        }
        else
        {
            // Unknown packet exception
        }
    }
}
