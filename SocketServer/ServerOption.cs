namespace SocketServer;

public class ServerOption
{
    public string Ip { get; set; } = "Any";
    public int Port { get; set; } = 9000;
    public int MaxConnectionNumber { get; set; } = 100;
    public int MaxRequestLength { get; set; } = 1024;
    public int ReceiveBufferSize { get; set; } = 1024;
    public int SendBufferSize { get; set; } = 1024;
}
