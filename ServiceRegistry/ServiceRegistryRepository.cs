using Dapper;
using Microsoft.Data.SqlClient;

namespace ServiceRegistry;

public class ServiceRegistryRepository(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public async Task<int> RegisterAsync(ServiceInstance registry)
    {
        const string sql = @"
            INSERT INTO [dbo].[ServiceRegistry]([AppName], [HostName], [Port], [IP], [Region], [RegisteredDateTime])
            VALUES(@AppName, @HostName, @Port, @IP, @Region, @RegisteredDateTime);
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
}