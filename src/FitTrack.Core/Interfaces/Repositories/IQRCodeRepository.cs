using FitTrack.Core.Entities;

namespace FitTrack.Core.Interfaces.Repositories;

public interface IQRCodeRepository
{
    Task<QRCode?> GetByUserIdAsync(Guid userId);

    Task<QRCode> CreateAsync(QRCode qrCode);
}
