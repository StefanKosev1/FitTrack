using FitTrack.Core.Interfaces.Repositories;
using FitTrack.Core.Interfaces.Services;

namespace FitTrack.Core.Services;

public class AccessService : IAccessService
{
    public async Task<bool> CanUserAccessGymAsync(int userId)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
