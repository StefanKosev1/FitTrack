using FitTrack.Core.Models;

namespace FitTrack.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
}
