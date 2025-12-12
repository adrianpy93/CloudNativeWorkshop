#region

using System.Data;
using Npgsql;

#endregion

namespace Dometrain.Monolith.Api.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}

public class NpgsqlConnectionFactory(NpgsqlDataSource dataSource) : IDbConnectionFactory
{
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        return await dataSource.OpenConnectionAsync(token);
    }
}