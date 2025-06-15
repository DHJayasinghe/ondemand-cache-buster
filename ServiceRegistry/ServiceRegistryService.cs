using Dapper;
using Microsoft.Data.SqlClient;

namespace ServiceRegistry;

public class ServiceRegistryService(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public async Task<int> RegisterAsync(ServiceInstance registry)
    {
        const string sql = @"
            INSERT INTO [dbo].[ServiceRegistry]([AppName], [HostName], [Port], [IP], [Region], [RegisteredDateTime], [LastActiveDateTime])
            VALUES(@AppName, @HostName, @Port, @IP, @Region, @RegisteredDateTime, @LastActiveDateTime);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var id = await connection.QuerySingleAsync<int>(sql, registry);
        return id;
    }

    public async Task<int> UpdateLastActiveDateTimeAsync(string hostName)
    {
        const string sql = @"UPDATE [dbo].[ServiceRegistry]
            SET [LastActiveDateTime] = @LastActiveDateTime
            WHERE [HostName] = @HostName;";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var affectedRows = await connection.ExecuteAsync(sql, new { HostName = hostName, LastActiveDateTime = DateTime.UtcNow });
        return affectedRows;
    }

    public async Task<int> DeregisterAsync(string hostName)
    {
        const string sql = @"
            DELETE FROM [dbo].[ServiceRegistry]
            WHERE [HostName] = @HostName;";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var affectedRows = await connection.ExecuteAsync(sql, new { HostName = hostName });
        return affectedRows;
    }

    public async Task<IEnumerable<ServiceInstance>> GetActiveInstancesAsync(string appName, DateTime lastActiveSince, string? region = null)
    {
        var sql = @"
            SELECT [Id], [AppName], [HostName], [Port], [IP], [Region], [RegisteredDateTime], [LastActiveDateTime]
            FROM [dbo].[ServiceRegistry]
            WHERE [AppName] = @AppName
              AND ([Region] = @Region OR @Region IS NULL)
              AND [LastActiveDateTime] >= @LastActiveSince";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var result = await connection.QueryAsync<ServiceInstance>(
            sql,
            new { AppName = appName, Region = region, LastActiveSince = lastActiveSince }
        );
        return result;
    }
}