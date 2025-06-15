using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;

namespace ServiceRegistry;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceRegistry(this IServiceCollection services, string connectionString, ServiceHeartbeatConfig heartbeatConfig = null)
    {
        services.AddSingleton(new ServiceRegistryService(connectionString));
        if (heartbeatConfig?.Enabled ?? false)
        {
            services.AddSingleton(heartbeatConfig);
            services.AddHostedService<ServiceHeartbeat>();
        }

        return services;
    }

    public static void UseServiceRegistry(this IApplicationBuilder app, IServiceProvider serviceProvider, ServiceInstanceConfig serviceInstanceConfig)
    {
        var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(async () =>
        {
            var repository = serviceProvider.GetRequiredService<ServiceRegistryService>();

            var serviceInstance = new ServiceInstance
            {
                AppName = serviceInstanceConfig.AppName,
                IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? "NA",
                Port = serviceInstanceConfig.Port,
                HostName = serviceInstanceConfig.HostName ?? Environment.GetEnvironmentVariable("HOSTNAME") ?? "NA",
                Region = serviceInstanceConfig.Region,
            };
            await repository.RegisterAsync(serviceInstance);
        });
        lifetime.ApplicationStopping.Register(async () =>
        {
            var repository = serviceProvider.GetRequiredService<ServiceRegistryService>();
            await repository.DeregisterAsync(Environment.GetEnvironmentVariable("HOSTNAME") ?? "NA");
        });
    }
}
