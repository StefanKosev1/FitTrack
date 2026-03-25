namespace FitTrack.Core.Interfaces.Services;

public interface IAccessService
{
    Task<bool> CanUserAccessGymAsync(int userId);
}
