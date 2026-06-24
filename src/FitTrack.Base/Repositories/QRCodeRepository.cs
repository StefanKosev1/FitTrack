using FitTrack.Base.Queries;
using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Data;
using FitTrack.Core.Interfaces.Repositories;
using Microsoft.Data.SqlClient;

namespace FitTrack.Base.Repositories;

public class QRCodeRepository : IQRCodeRepository
{
    private static readonly SemaphoreSlim SchemaLock = new(1, 1);
    private static bool _schemaEnsured;

    private readonly IDbConnectionFactory _connectionFactory;

    public QRCodeRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<QRCode?> GetByUserIdAsync(Guid userId)
    {
        await EnsureSchemaAsync();

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = QRCodeSqlQueries.GetByUserId;
        command.Parameters.Add(new SqlParameter("@UserId", userId));

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return QRCode.Restore(
            reader.GetGuid(reader.GetOrdinal("id")),
            reader.GetGuid(reader.GetOrdinal("user_id")),
            reader.GetString(reader.GetOrdinal("code")),
            reader.GetDateTime(reader.GetOrdinal("created_at_utc")));
    }

    public async Task<QRCode> CreateAsync(QRCode qrCode)
    {
        await EnsureSchemaAsync();

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = QRCodeSqlQueries.Create;
        command.Parameters.Add(new SqlParameter("@Id", qrCode.Id));
        command.Parameters.Add(new SqlParameter("@UserId", qrCode.UserId));
        command.Parameters.Add(new SqlParameter("@Code", qrCode.Code));
        command.Parameters.Add(new SqlParameter("@CreatedAtUtc", qrCode.CreatedAtUtc));

        await command.ExecuteNonQueryAsync();

        return qrCode;
    }

    private async Task EnsureSchemaAsync()
    {
        if (_schemaEnsured)
        {
            return;
        }

        await SchemaLock.WaitAsync();
        try
        {
            if (_schemaEnsured)
            {
                return;
            }

            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            await using var command = connection.CreateCommand();

            command.CommandText = QRCodeSqlQueries.EnsureSchema;
            await command.ExecuteNonQueryAsync();

            _schemaEnsured = true;
        }
        finally
        {
            SchemaLock.Release();
        }
    }
}
