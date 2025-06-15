using ServiceRegistry;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddServiceRegistry(
    connectionString: builder.Configuration.GetConnectionString("SQLServer"),
    new ServiceHeartbeatConfig { Enabled = true, Interval = TimeSpan.FromSeconds(10) });

var app = builder.Build();

app.UseServiceRegistry(app.Services, new ServiceInstanceConfig
{
    AppName = Environment.GetEnvironmentVariable("AppName"),
    Port = int.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS") ?? "8080"),
    HostName = Environment.GetEnvironmentVariable("HOSTNAME") ?? "NA",
    Region = Environment.GetEnvironmentVariable("RegionCode") ?? "NA",
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();