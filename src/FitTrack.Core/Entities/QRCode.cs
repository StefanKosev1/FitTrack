namespace FitTrack.Core.Entities;

public class QRCode
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Code { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
