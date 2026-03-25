using FitTrack.Core.Entities;

namespace FitTrack.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
}
