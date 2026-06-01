namespace FitTrack.Base.Queries;

public static class UserSqlQueries
{
    public const string GetByEmail = """
        SELECT
            id,
            full_name,
            email,
            password_hash,
            password_salt,
            created_at_utc
        FROM fittrack.Users
        WHERE email = @Email;
        """;

    public const string Create = """
        INSERT INTO fittrack.Users (
            id,
            full_name,
            email,
            password_hash,
            password_salt,
            created_at_utc
        )
        VALUES (
            @Id,
            @FullName,
            @Email,
            @PasswordHash,
            @PasswordSalt,
            @CreatedAtUtc
        );
        """;
}
