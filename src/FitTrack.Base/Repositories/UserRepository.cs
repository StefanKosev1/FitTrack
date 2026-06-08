using FitTrack.Base.Queries;
using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Data;
using FitTrack.Core.Interfaces.Repositories;
using Microsoft.Data.SqlClient;

namespace FitTrack.Base.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = UserSqlQueries.GetByEmail;

        var emailParameter = new SqlParameter("@Email", email.Trim());
        command.Parameters.Add(emailParameter);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return User.Restore(
            reader.GetGuid(reader.GetOrdinal("id")),
            reader.GetString(reader.GetOrdinal("full_name")),
            reader.GetString(reader.GetOrdinal("email")),
            reader.GetString(reader.GetOrdinal("password_hash")),
            reader.GetString(reader.GetOrdinal("password_salt")),
            reader.GetDateTime(reader.GetOrdinal("created_at_utc")));
    }

    public async Task<User> CreateAsync(User user)
    {
        var existingUser = await GetByEmailAsync(user.Email);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = UserSqlQueries.Create;
        command.Parameters.Add(new SqlParameter("@Id", user.Id));
        command.Parameters.Add(new SqlParameter("@FullName", user.FullName));
        command.Parameters.Add(new SqlParameter("@Email", user.Email));
        command.Parameters.Add(new SqlParameter("@PasswordHash", user.PasswordHash));
        command.Parameters.Add(new SqlParameter("@PasswordSalt", user.PasswordSalt));
        command.Parameters.Add(new SqlParameter("@CreatedAtUtc", user.CreatedAtUtc));

        await command.ExecuteNonQueryAsync();

        return user;
    }
}
