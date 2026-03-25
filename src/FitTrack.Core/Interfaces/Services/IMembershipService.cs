using FitTrack.Core.Entities;

namespace FitTrack.Core.Interfaces.Services;

public interface IMembershipService
{
    Task<Membership?> GetActiveMembershipAsync(int userId);
}
