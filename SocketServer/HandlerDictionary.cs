namespace SocketServer;

public class HandlerDictionary
{
    public static Dictionary<int, Action<byte[]>> PacketHandlers = [];

    public void RegisterPacketHandler(int packetType, Action<byte[]> handler)
    {
        PacketHandlers[packetType] = handler;
    }

    public static void HandlePacket(int packetId, byte[] packet)
    {
        if (PacketHandlers.TryGetValue(packetId, out var handler))
        {
            handler(packet);
        }
        else
        {
            // Unknown packet exception
        }
    }

    // public void SendPacket<T>(ClientSession session, T packet) where T : PacketHeader
    // {
    //     var packetSize = PacketHeader.Size + packet.Size;
    //     var buffer = new byte[packetSize];
    //     var packetType = packet.Type;

    //     Buffer.BlockCopy(BitConverter.GetBytes(packet.Id), 0, buffer, 0, 4);
    //     Buffer.BlockCopy(BitConverter.GetBytes(packetSize), 0, buffer, 4, 4);
    //     Buffer.BlockCopy(BitConverter.GetBytes(packetType), 0, buffer, 8, 4);

    //     var packetBuffer = packet.ToBytes();
    //     Buffer.BlockCopy(packetBuffer, 0, buffer, PacketHeader.Size, packetBuffer.Length);

    //     session.Send(buffer, 0, packetSize);
    // }

    // public void ReadPacket(byte[] packet)
    // {
    //     var header = new PacketHeader();
    //     header.Id = BitConverter.ToInt32(packet, 0);
    //     header.Size = BitConverter.ToInt32(packet, 4);
    //     header.Type = BitConverter.ToInt32(packet, 8);
    // }
}
