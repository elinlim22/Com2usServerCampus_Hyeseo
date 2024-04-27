using System.Threading.Tasks.Dataflow;

namespace SocketServer;

public class PacketProcessor
{
    bool _isRunning = false;
    System.Threading.Thread? _thread = null;
    BufferBlock<RequestInfo> _bufferBlock = new();
    public Int32 _id;
    public Int32 _size;
    public byte[]? _data;

    public void Start()
    {
        _isRunning = true;
        _thread = new System.Threading.Thread(Process);
        _thread.Start();
    }

    public void ReadHeader(byte[] buffer, int offset, int length)
    {
        _id = BitConverter.ToInt32(buffer, offset);
        _size = BitConverter.ToInt32(buffer, offset + 4);
        // 데이터를 언제 복사하지?
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
            ReadHeader(receivedPacket._bytes, 0, receivedPacket._bytes.Length);
            // 헤더에 따른 핸들러를 호출한다.
            HandlerDictionary.HandlePacket(_id, _data ?? []);


            // 패킷 처리
            // ProcessPacket(requestInfo);
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
