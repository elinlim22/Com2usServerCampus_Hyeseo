namespace SocketServer;

public class HandlerDictionary
{
    public static Dictionary<int, Action<RequestInfo>> PacketHandlers = [];

    public void RegistPacketHandler(int packetType, Action<RequestInfo> handler)
    {
        PacketHandlers[packetType] = handler;
    }

    public void HandlePacket(byte packetType, RequestInfo receivedPacket)
    {
        if (PacketHandlers.TryGetValue(packetType, out Action<RequestInfo>? value))
        {
            value(receivedPacket);
        }
        else
        {
            // Unknown packet exception
        }
    }
}
