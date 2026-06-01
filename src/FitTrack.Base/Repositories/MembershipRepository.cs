using FitTrack.Base.Queries;
using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Data;
using FitTrack.Core.Interfaces.Repositories;
using Microsoft.Data.SqlClient;

namespace FitTrack.Base.Repositories;

public class MembershipRepository : IMembershipRepository
{
    private static readonly SemaphoreSlim SchemaLock = new(1, 1);
    private static bool _schemaEnsured;

    private readonly IDbConnectionFactory _connectionFactory;

    public MembershipRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyCollection<MembershipPlan>> GetPlansAsync()
    {
        await EnsureSchemaAsync();

        var plans = new List<MembershipPlan>();

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = MembershipSqlQueries.GetPlans;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            plans.Add(new MembershipPlan
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                DurationInDays = reader.GetInt32(reader.GetOrdinal("duration_days"))
            });
        }

        return plans;
    }

    public async Task<Membership?> GetActiveByUserIdAsync(Guid userId)
    {
        await EnsureSchemaAsync();

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = MembershipSqlQueries.GetActiveByUserId;
        command.Parameters.Add(new SqlParameter("@UserId", userId));

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Membership
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
            PlanId = reader.GetInt32(reader.GetOrdinal("plan_id")),
            PlanName = reader.GetString(reader.GetOrdinal("plan_name")),
            StartsAtUtc = reader.GetDateTime(reader.GetOrdinal("starts_at_utc")),
            EndsAtUtc = reader.GetDateTime(reader.GetOrdinal("ends_at_utc"))
        };
    }

    public async Task<Membership> CreateAsync(Membership membership)
    {
        await EnsureSchemaAsync();

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = MembershipSqlQueries.Create;
        command.Parameters.Add(new SqlParameter("@Id", membership.Id));
        command.Parameters.Add(new SqlParameter("@UserId", membership.UserId));
        command.Parameters.Add(new SqlParameter("@PlanId", membership.PlanId));
        command.Parameters.Add(new SqlParameter("@StartsAtUtc", membership.StartsAtUtc));
        command.Parameters.Add(new SqlParameter("@EndsAtUtc", membership.EndsAtUtc));

        await command.ExecuteNonQueryAsync();

        return membership;
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

            command.CommandText = MembershipSqlQueries.EnsureSchema;
            await command.ExecuteNonQueryAsync();

            _schemaEnsured = true;
        }
        finally
        {
            SchemaLock.Release();
        }
    }
}
