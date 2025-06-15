namespace ServiceRegistry;

public class ServiceHeartbeatConfig
{
    public bool Enabled { get; set; } = true;
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(10);
}