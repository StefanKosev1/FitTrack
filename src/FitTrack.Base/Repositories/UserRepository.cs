using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;

namespace FitTrack.Base.Repositories;

public class UserRepository : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
