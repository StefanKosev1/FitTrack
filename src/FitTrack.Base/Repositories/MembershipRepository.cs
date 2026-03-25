using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;

namespace FitTrack.Base.Repositories;

public class MembershipRepository : IMembershipRepository
{
    public async Task<Membership?> GetActiveByUserIdAsync(int userId)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
