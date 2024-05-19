namespace GameServer.Models
{
    public class MatchingResponse
    {
        public short StatusCode { get; set; }
        public int RoomNumber { get; set; }
        public string ServerIP { get; set; }
    }
}
