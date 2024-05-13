namespace SocketServer;

public class ServerOption
{
    public string Ip { get; set; }
    public int Port { get; set; }
    public string Name { get; set; }
    public int MaxConnectionNumber { get; set; }
    public int MaxRequestLength { get; set; }
    public int ReceiveBufferSize { get; set; }
    public int SendBufferSize { get; set; }
    public int MaxRoom { get; set; }
    public int MaxUserPerRoom { get; set; }
    public int MaxThread { get; set; }
}
