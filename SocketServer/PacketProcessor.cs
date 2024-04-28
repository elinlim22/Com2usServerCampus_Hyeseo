using System.Threading.Tasks.Dataflow;

namespace SocketServer;

public class PacketProcessor(HandlerDictionary handlerDictionary)
{
    bool _isRunning = false;
    Thread? _thread = null;
    readonly BufferBlock<RequestInfo> _bufferBlock = new();
    public Int32 _id;
    public Int32 _size;
    public byte _type;
    public HandlerDictionary _handlerDictionary = handlerDictionary;
    public Users _users = new();

    public void Start()
    {
        _thread = new Thread(Process);
        _isRunning = true;
        _thread.Start();
    }

    public void ReadHeader(byte[] bytes, int offset)
    {
        _id = BitConverter.ToInt32(bytes, offset);
        _size = BitConverter.ToInt32(bytes, offset + sizeof(Int32));
        _type = bytes[offset + sizeof(Int32) * 2];
    }

    public RequestInfo MakePacket(string sessionId)
    {
        // TODO : 세션이 New Connect이거나 Closed인 경우를 나눠야 하는 이유가 뭘까?
        var newBytes = new byte[(Int32)PacketDefine.HeaderSize];
        BitConverter.GetBytes((Int32)PacketType.IN_SessionConnectedOrClosed).CopyTo(newBytes, (Int32)PacketDefine.MemoryPackOffset);

        var newPacket = new RequestInfo(newBytes)
        {
            _sessionId = sessionId
        };
        return newPacket;
    }

    public void Process()
    {
        while (_isRunning)
        {
            var receivedPacket = _bufferBlock.Receive(); // 받은 패킷은 RequestInfo형태이다.
            if (receivedPacket == null)
            {
                break;
            }

            ReadHeader(receivedPacket._bytes ?? [], (Int32)PacketDefine.MemoryPackOffset);
            // var response?
            _handlerDictionary.HandlePacket(_type, receivedPacket);
        }
    }

    public void Enqueue(RequestInfo requestInfo) // 버퍼에 넣기
    {
        _bufferBlock.Post(requestInfo);
    }

    public void Stop()
    {
        _isRunning = false;
        _thread?.Join();
    }
}
