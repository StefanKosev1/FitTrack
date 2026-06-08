using System.Collections.Concurrent;
using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;

namespace FitTrack.Base.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _users = new(StringComparer.OrdinalIgnoreCase);

    public Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim();
        _users.TryGetValue(normalizedEmail, out var user);

        return Task.FromResult(user);
    }

    public Task<User> CreateAsync(User user)
    {
        if (!_users.TryAdd(user.Email, user))
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        return Task.FromResult(user);
    }
}
