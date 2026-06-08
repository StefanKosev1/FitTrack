namespace FitTrack.Core.Entities;

public sealed class QRCode
{
    private QRCode(Guid id, Guid userId, string code, DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("QR code ID is required.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("QR code value is required.", nameof(code));
        }

        if (createdAtUtc == default)
        {
            throw new ArgumentException("Creation date is required.", nameof(createdAtUtc));
        }

        Id = id;
        UserId = userId;
        Code = code.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Code { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public static QRCode Create(Guid userId, string code)
    {
        return new QRCode(Guid.NewGuid(), userId, code, DateTime.UtcNow);
    }

    public static QRCode Restore(Guid id, Guid userId, string code, DateTime createdAtUtc)
    {
        return new QRCode(id, userId, code, createdAtUtc);
    }
}
