namespace ServiceRegistry;

public class ServiceInstance
{
    public int Id { get; set; }
    public string AppName { get; set; }
    public string HostName { get; set; }
    public int Port { get; set; }
    public string IP { get; set; }
    public string? Region { get; set; }
    public DateTime RegisteredDateTime { get; set; } = DateTime.UtcNow;
    public DateTime? LastActiveDateTime { get; set; } = DateTime.UtcNow;
}