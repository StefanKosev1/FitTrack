using FitTrack.Core.Dtos;

namespace FitTrack.Core.Interfaces.Services;

public interface IMembershipService
{
    Task<IReadOnlyCollection<MembershipPlanDto>> GetPlansAsync();

    Task<MembershipDto?> GetActiveMembershipAsync(Guid userId);

    Task<MembershipDto> StartMembershipAsync(Guid userId, int planId);
}
