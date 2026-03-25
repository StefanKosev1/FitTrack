using FitTrack.Core.Interfaces.Services;
using FitTrack.Core.Models;

namespace FitTrack.Core.Services;

public class AuthService : IAuthService
{
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
