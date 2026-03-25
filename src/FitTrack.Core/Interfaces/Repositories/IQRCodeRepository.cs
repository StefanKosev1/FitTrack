using FitTrack.Core.Entities;

namespace FitTrack.Core.Interfaces.Repositories;

public interface IQRCodeRepository
{
    Task<QRCode?> GetByUserIdAsync(int userId);
}
