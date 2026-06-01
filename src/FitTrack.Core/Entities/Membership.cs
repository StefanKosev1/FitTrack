namespace FitTrack.Core.Entities;

public class Membership
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int PlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public DateTime StartsAtUtc { get; set; }

    public DateTime EndsAtUtc { get; set; }

    public bool IsActive => StartsAtUtc <= DateTime.UtcNow && EndsAtUtc > DateTime.UtcNow;
}
