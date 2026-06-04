using System.Security.Cryptography;
using FitTrack.Core.Dtos;
using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;
using FitTrack.Core.Interfaces.Services;

namespace FitTrack.Core.Services;

public class QRCodeService : IQRCodeService
{
    private readonly IQRCodeRepository _qrCodeRepository;
    private readonly IMembershipRepository _membershipRepository;
    
    public QRCodeService(
        IQRCodeRepository qrCodeRepository,
        IMembershipRepository membershipRepository)
    {
        _qrCodeRepository = qrCodeRepository;
        _membershipRepository = membershipRepository;
    }

    // Returns an existing QR code or creates one for an active member.
    public async Task<QRCodeDto> GetOrCreateForUserAsync(Guid userId)
    {
        var activeMembership = await _membershipRepository.GetActiveByUserIdAsync(userId);
        if (activeMembership is null)
        {
            throw new InvalidOperationException("A member needs an active membership before a QR code can be issued.");
        }

        var existingCode = await _qrCodeRepository.GetByUserIdAsync(userId);
        if (existingCode is not null)
        {
            return ToDto(existingCode);
        }

        var tokenBytes = RandomNumberGenerator.GetBytes(24);
        var qrCode = new QRCode
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Code = $"FITTRACK-{Convert.ToHexString(tokenBytes)}",
            CreatedAtUtc = DateTime.UtcNow
        };

        var createdCode = await _qrCodeRepository.CreateAsync(qrCode);

        return ToDto(createdCode);
    }

    // Maps the internal QR code entity to a safe DTO for the web layer.
    private static QRCodeDto ToDto(QRCode qrCode)
    {
        return new QRCodeDto
        {
            Code = qrCode.Code,
            CreatedAtUtc = qrCode.CreatedAtUtc
        };
    }
}
