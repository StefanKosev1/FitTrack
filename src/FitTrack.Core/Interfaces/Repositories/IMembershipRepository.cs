using FitTrack.Core.Entities;

namespace FitTrack.Core.Interfaces.Repositories;

public interface IMembershipRepository
{
    Task<IReadOnlyCollection<MembershipPlan>> GetPlansAsync();

    Task<Membership?> GetActiveByUserIdAsync(Guid userId);

    Task<Membership> CreateAsync(Membership membership);
}
