using FitTrack.Core.Entities;

namespace FitTrack.Core.Interfaces.Repositories;

public interface IMembershipRepository
{
    Task<Membership?> GetActiveByUserIdAsync(int userId);
}
