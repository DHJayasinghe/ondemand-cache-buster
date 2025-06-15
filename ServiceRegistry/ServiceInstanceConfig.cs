namespace ServiceRegistry;

public class ServiceInstanceConfig
{
    public string AppName { get; set; } = "demo-application";
    public string HostName { get; set; }
    public int Port { get; set; }
    public string? Region { get; set; }
}