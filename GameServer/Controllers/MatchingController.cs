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
            var response = new MatchingResponse
            {
                RoomNumber = await _memoryDB.MatchRoomId(),
                ServerIP = _configuration["ServerIP"]
            };
            _logger.ZLogDebug($"Matched room number {response.RoomNumber}");
            return response;
        }
    }
}
