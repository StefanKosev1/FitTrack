namespace FitTrack.Core.Entities;

public sealed class Membership
{
    private Membership(
        Guid id,
        Guid userId,
        int planId,
        string planName,
        DateTime startsAtUtc,
        DateTime endsAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Membership ID is required.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (planId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(planId), "Plan ID must be positive.");
        }

        if (string.IsNullOrWhiteSpace(planName))
        {
            throw new ArgumentException("Plan name is required.", nameof(planName));
        }

        if (startsAtUtc == default)
        {
            throw new ArgumentException("Membership start date is required.", nameof(startsAtUtc));
        }

        if (endsAtUtc <= startsAtUtc)
        {
            throw new ArgumentException("Membership end date must be after its start date.", nameof(endsAtUtc));
        }

        Id = id;
        UserId = userId;
        PlanId = planId;
        PlanName = planName.Trim();
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public int PlanId { get; private set; }

    public string PlanName { get; private set; }

    public DateTime StartsAtUtc { get; private set; }

    public DateTime EndsAtUtc { get; private set; }

    public bool IsActive => IsActiveAt(DateTime.UtcNow);

    public static Membership Start(Guid userId, MembershipPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        var startsAtUtc = DateTime.UtcNow;
        return new Membership(
            Guid.NewGuid(),
            userId,
            plan.Id,
            plan.Name,
            startsAtUtc,
            startsAtUtc.AddDays(plan.DurationInDays));
    }

    public static Membership Restore(
        Guid id,
        Guid userId,
        int planId,
        string planName,
        DateTime startsAtUtc,
        DateTime endsAtUtc)
    {
        return new Membership(id, userId, planId, planName, startsAtUtc, endsAtUtc);
    }

    public bool IsActiveAt(DateTime utcNow)
    {
        return StartsAtUtc <= utcNow && EndsAtUtc > utcNow;
    }
}
