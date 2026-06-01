using System.Collections.Concurrent;
using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;

namespace FitTrack.Base.Repositories;

public class InMemoryMembershipRepository : IMembershipRepository
{
    private static readonly IReadOnlyCollection<MembershipPlan> Plans =
    [
        new MembershipPlan
        {
            Id = 1,
            Name = "Weekly",
            Description = "Seven days of gym floor access with standard member check-in.",
            Price = 9.99m,
            DurationInDays = 7
        },
        new MembershipPlan
        {
            Id = 2,
            Name = "Monthly",
            Description = "Thirty days of gym floor access, group classes, and conditioning sessions.",
            Price = 29.99m,
            DurationInDays = 30
        },
        new MembershipPlan
        {
            Id = 3,
            Name = "Yearly",
            Description = "A full year of gym access, classes, recovery area, and priority booking.",
            Price = 299.99m,
            DurationInDays = 365
        }
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
