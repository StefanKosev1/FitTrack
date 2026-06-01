using System.Collections.Concurrent;
using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;

namespace FitTrack.Base.Repositories;

public class InMemoryQRCodeRepository : IQRCodeRepository
{
    private readonly ConcurrentDictionary<Guid, QRCode> _qrCodes = new();

    public Task<QRCode?> GetByUserIdAsync(Guid userId)
    {
        _qrCodes.TryGetValue(userId, out var qrCode);

        return Task.FromResult(qrCode);
    }

    public Task<QRCode> CreateAsync(QRCode qrCode)
    {
        _qrCodes[qrCode.UserId] = qrCode;

        return Task.FromResult(qrCode);
    }
}
