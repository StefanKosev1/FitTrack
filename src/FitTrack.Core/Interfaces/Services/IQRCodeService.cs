using FitTrack.Core.Dtos;

namespace FitTrack.Core.Interfaces.Services;

public interface IQRCodeService
{
    Task<QRCodeDto> GetOrCreateForUserAsync(Guid userId);
}
