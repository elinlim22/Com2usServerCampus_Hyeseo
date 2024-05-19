using CloudStructures.Structures;
using CloudStructures;
using GameServer.Repository;
using GameServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace GameServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MatchingController : ControllerBase
    {
        readonly IGameDB _gameDB;
        readonly IMemoryDB _memoryDB;
        readonly IConfiguration _configuration;
        readonly ILogger<MatchingController> _logger;

        public MatchingController(IGameDB gameDB, IMemoryDB memoryDB, IConfiguration configuration, ILogger<MatchingController> logger)
        {
            _gameDB = gameDB;
            _memoryDB = memoryDB;
            _configuration = configuration;
            _logger = logger;
        }
        [HttpGet]
        public async Task<MatchingResponse> Get()
        {
            try
            {
                var roomNumber = await _memoryDB.MatchRoomId();

                var response = new MatchingResponse
                {
                    StatusCode = (short)ErrorCode.Success,
                    RoomNumber = roomNumber,
                    ServerIP = _configuration["ServerAddress"].Replace("{listenAddr}", Environment.GetEnvironmentVariable("LISTEN_ADDR"))
                };
                _logger.ZLogDebug($"Matched room number {response.RoomNumber}");
                return response;
            }
            catch (Exception e)
            {
                _logger.ZLogError($"Error matching room: {e.Message}");
                return new MatchingResponse
                {
                    StatusCode = (short)ErrorCode.RoomMatchingFailed,
                    RoomNumber = -1,
                    ServerIP = ""
                };
            }
        }
    }
}
