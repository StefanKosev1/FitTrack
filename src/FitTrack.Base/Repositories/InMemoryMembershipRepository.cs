using System.Collections.Concurrent;
using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;

namespace FitTrack.Base.Repositories;

public class InMemoryMembershipRepository : IMembershipRepository
{
    private static readonly IReadOnlyCollection<MembershipPlan> Plans =
    [
        MembershipPlan.Restore(
            1,
            "Weekly",
            "Seven days of gym floor access with standard member check-in.",
            9.99m,
            7),
        MembershipPlan.Restore(
            2,
            "Monthly",
            "Thirty days of gym floor access, group classes, and conditioning sessions.",
            29.99m,
            30),
        MembershipPlan.Restore(
            3,
            "Yearly",
            "A full year of gym access, classes, recovery area, and priority booking.",
            299.99m,
            365)
    ];

    private readonly ConcurrentDictionary<Guid, Membership> _memberships = new();

    public Task<IReadOnlyCollection<MembershipPlan>> GetPlansAsync()
    {
        return Task.FromResult(Plans);
    }

    public Task<Membership?> GetActiveByUserIdAsync(Guid userId)
    {
        if (_memberships.TryGetValue(userId, out var membership) && membership.IsActive)
        {
            return Task.FromResult<Membership?>(membership);
        }

        return Task.FromResult<Membership?>(null);
    }

    public Task<Membership> CreateAsync(Membership membership)
    {
        _memberships[membership.UserId] = membership;

        return Task.FromResult(membership);
    }
}
