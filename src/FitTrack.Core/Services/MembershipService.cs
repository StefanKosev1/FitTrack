using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Services;

namespace FitTrack.Core.Services;

public class MembershipService : IMembershipService
{
    public async Task<Membership?> GetActiveMembershipAsync(int userId)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
