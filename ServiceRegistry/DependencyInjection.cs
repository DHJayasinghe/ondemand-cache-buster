using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;

namespace ServiceRegistry;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceRegistry(this IServiceCollection services, string connectionString, bool healthCheckEnabled = false)
    {
        services.AddSingleton(new ServiceRegistryRepository(connectionString));
        if (healthCheckEnabled)
            services.AddHostedService<ServiceHeartbeat>();

        return services;
    }

    public static void UseServiceRegistry(this IApplicationBuilder app, IServiceProvider serviceProvider, ServiceInstance serviceInstance)
    {
        var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(async () =>
        {
            var repository = serviceProvider.GetRequiredService<ServiceRegistryRepository>();

            serviceInstance.AppName ??= "demo-application";
            serviceInstance.IP ??= Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? "NA";
            serviceInstance.HostName ??= Environment.GetEnvironmentVariable("HOSTNAME") ?? "NA";

            await repository.RegisterAsync(serviceInstance);
        });
        lifetime.ApplicationStopping.Register(async () =>
        {
            var repository = serviceProvider.GetRequiredService<ServiceRegistryRepository>();
            await repository.DeregisterAsync(Environment.GetEnvironmentVariable("HOSTNAME") ?? "NA");
        });
    }
}
