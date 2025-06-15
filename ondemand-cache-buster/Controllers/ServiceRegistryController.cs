using Microsoft.AspNetCore.Mvc;
using ServiceRegistry;

[ApiController]
[Route("[controller]")]
public class ServiceRegistryController(ServiceRegistryService serviceRegistryService) : ControllerBase
{
    private readonly ServiceRegistryService _serviceRegistryService = serviceRegistryService;

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveInstances()
    {
        var appName = Environment.GetEnvironmentVariable("AppName");
        var region = Environment.GetEnvironmentVariable("Region");
        var since = DateTime.UtcNow.AddMinutes(-5);

        var instances = await _serviceRegistryService.GetActiveInstancesAsync(appName, since, region);

        return Ok(instances);
    }
}