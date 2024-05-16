using Microsoft.Extensions.Configuration;
using System.Threading.Tasks.Dataflow;

namespace SocketServer;

class PacketProcessor(ServerOption serverOption, ConnectionStrings connStr)
{
    bool _isThreadRunning = false;
    Thread _processThread = null;

    public Func<string, byte[], bool> SendData;
    public Action<string> CloseSession;

    BufferBlock<RequestInfo> _msgBuffer = new();

    UserManager _userMgr = new(serverOption);

    List<Room> _roomList = [];

    Dictionary<int, Action<RequestInfo>> PacketHandlers = [];
    PacketHandlerCommon _commonPacketHandler = new(serverOption);
    PacketHandlerRoom _roomPacketHandler = new();

    public DBMySQLConnection _mySQLConnection = new(serverOption, connStr);
    public DBRedisConnection _redisConnection = new(serverOption, connStr);

    public void CreateAndStart(List<Room> roomList, ServerOption serverOpt)
    {
        var totalUserMaximum = serverOpt.MaxRoom * serverOpt.MaxUserPerRoom;
        _userMgr.Init(totalUserMaximum);

        _roomList = roomList;
        var minRoomNum = _roomList[0].Number;
        var maxRoomNum = _roomList[0].Number + _roomList.Count() - 1;

        RegistPacketHandler();

        _isThreadRunning = true;
        _processThread = new System.Threading.Thread(this.Process);
        _processThread.Start();
    }

    public void Destory()
    {
        MainServer.MainLogger.Info("PacketProcessor::Destory - begin");

        _isThreadRunning = false;
        _msgBuffer.Complete();

        _processThread.Join();

        MainServer.MainLogger.Info("PacketProcessor::Destory - end");
    }

    public void InsertPacket(RequestInfo data)
    {
        _msgBuffer.Post(data);
    }


    void RegistPacketHandler()
    {
        PacketHandler.SendData = SendData;
        PacketHandler.DistributeInnerPacket = InsertPacket;
        PacketHandler.CloseSession = CloseSession;
        PacketHandler.DistributeMySQLPacket = _mySQLConnection.InsertPacket;
        PacketHandler.DistributeRedisPacket = _redisConnection.InsertPacket;

        _commonPacketHandler.Init(_userMgr);
        _commonPacketHandler.RegistPacketHandler(PacketHandlers);

        _redisConnection.DistributeInnerPacket = InsertPacket;

        _roomPacketHandler.Init(_userMgr);
        _roomPacketHandler.SetRooomList(_roomList);
        _roomPacketHandler.RegistPacketHandler(PacketHandlers);
    }

    void Process()
    {
        while (_isThreadRunning)
        {
            try
            {
                var packet = _msgBuffer.Receive();

                var header = new PacketHeaderInfo();
                header.Read(packet.Data);

                if (PacketHandlers.ContainsKey(header.Id))
                {
                    PacketHandlers[header.Id](packet);
                }
                else
                {
                    MainServer.MainLogger.Error($"PacketProcessor::Process - invalid packet id : {header.Id}");
                }
            }
            catch (Exception ex)
            {
                if (_isThreadRunning)
                {
                    MainServer.MainLogger.Error(ex.ToString());
                }
            }
        }
    }
}
