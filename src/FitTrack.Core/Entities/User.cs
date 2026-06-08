namespace FitTrack.Core.Entities;

public sealed class User
{
    private User(
        Guid id,
        string fullName,
        string email,
        string passwordHash,
        string passwordSalt,
        DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(id));
        }

        if (createdAtUtc == default)
        {
            throw new ArgumentException("Creation date is required.", nameof(createdAtUtc));
        }

        Id = id;
        FullName = RequireText(fullName, nameof(fullName)).Trim();
        Email = RequireText(email, nameof(email)).Trim();
        PasswordHash = RequireText(passwordHash, nameof(passwordHash));
        PasswordSalt = RequireText(passwordSalt, nameof(passwordSalt));
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public string FullName { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public string PasswordSalt { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public static User Create(string fullName, string email, string passwordHash, string passwordSalt)
    {
        return new User(Guid.NewGuid(), fullName, email, passwordHash, passwordSalt, DateTime.UtcNow);
    }

    public static User Restore(
        Guid id,
        string fullName,
        string email,
        string passwordHash,
        string passwordSalt,
        DateTime createdAtUtc)
    {
        return new User(id, fullName, email, passwordHash, passwordSalt, createdAtUtc);
    }

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value;
    }
}
