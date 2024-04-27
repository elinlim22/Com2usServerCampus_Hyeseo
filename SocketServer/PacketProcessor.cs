using System.Threading.Tasks.Dataflow;

namespace SocketServer;

public class PacketProcessor(HandlerDictionary handlerDictionary)
{
    bool _isRunning = false;
    Thread? _thread = null;
    BufferBlock<RequestInfo> _bufferBlock = new();
    public Int32 _id;
    public Int32 _size;
    public HandlerDictionary _handlerDictionary = handlerDictionary;

    public void Start()
    {
        _thread = new Thread(Process);
        _isRunning = true;
        _thread.Start();
    }

    public void ReadHeader(byte[] buffer, int offset, int length)
    {
        _id = BitConverter.ToInt32(buffer, offset);
        _size = BitConverter.ToInt32(buffer, offset + sizeof(Int32));
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

            // 패킷의 헤더를 읽는다.
            ReadHeader(receivedPacket._bytes, (Int32)PacketDefine.MemoryPackOffset, (Int32)PacketDefine.HeaderSize);
            // 헤더에 따른 핸들러를 호출한다.
            _handlerDictionary.HandlePacket(_id, receivedPacket);
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
