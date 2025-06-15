using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServiceRegistry;

public class ServiceHeartbeat(IServiceProvider serviceProvider, ILogger<ServiceHeartbeat> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<ServiceHeartbeat> _logger = logger;
    private readonly TimeSpan _heartbeatInterval = TimeSpan.FromSeconds(10); 

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var instanceId = Environment.GetEnvironmentVariable("HOSTNAME") ?? "NA";
        _logger.LogInformation("Heartbeat started for instance {Instance}", instanceId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ServiceRegistryRepository>();

                repository.UpdateLastActiveDateTimeAsync(instanceId).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Heartbeat update failed");
            }

            await Task.Delay(_heartbeatInterval, stoppingToken);
        }

        _logger.LogInformation("Heartbeat stopping for instance {Instance}", instanceId);
    }
}
