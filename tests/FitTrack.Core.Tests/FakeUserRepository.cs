using System.Collections.Concurrent;
using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;

namespace FitTrack.Core.Tests;

internal class FakeUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _users = new(StringComparer.OrdinalIgnoreCase);

    public Task<User?> GetByEmailAsync(string email)
    {
        _users.TryGetValue(email.Trim(), out var user);

        return Task.FromResult(user);
    }

    public Task<User> CreateAsync(User user)
    {
        user.Email = user.Email.Trim();

        if (!_users.TryAdd(user.Email, user))
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        return Task.FromResult(user);
    }
}
