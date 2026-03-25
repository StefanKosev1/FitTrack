using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;

namespace FitTrack.Base.Repositories;

public class QRCodeRepository : IQRCodeRepository
{
    public async Task<QRCode?> GetByUserIdAsync(int userId)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
