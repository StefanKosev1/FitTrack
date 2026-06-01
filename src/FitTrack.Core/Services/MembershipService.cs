using FitTrack.Core.Dtos;
using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;
using FitTrack.Core.Interfaces.Services;

namespace FitTrack.Core.Services;

public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _membershipRepository;

    public MembershipService(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public async Task<IReadOnlyCollection<MembershipPlanDto>> GetPlansAsync()
    {
        var plans = await _membershipRepository.GetPlansAsync();

        return plans
            .Select(ToDto)
            .ToArray();
    }

    public async Task<MembershipDto?> GetActiveMembershipAsync(Guid userId)
    {
        var membership = await _membershipRepository.GetActiveByUserIdAsync(userId);

        return membership is null
            ? null
            : ToDto(membership);
    }

    public async Task<MembershipDto> StartMembershipAsync(Guid userId, int planId)
    {
        var activeMembership = await _membershipRepository.GetActiveByUserIdAsync(userId);
        if (activeMembership is not null)
        {
            return ToDto(activeMembership);
        }

        var plans = await _membershipRepository.GetPlansAsync();
        var selectedPlan = plans.FirstOrDefault(plan => plan.Id == planId);

        if (selectedPlan is null)
        {
            throw new InvalidOperationException("The selected membership plan does not exist.");
        }

        var startsAtUtc = DateTime.UtcNow;
        var membership = new Membership
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanId = selectedPlan.Id,
            PlanName = selectedPlan.Name,
            StartsAtUtc = startsAtUtc,
            EndsAtUtc = startsAtUtc.AddDays(selectedPlan.DurationInDays)
        };

        var createdMembership = await _membershipRepository.CreateAsync(membership);

        return ToDto(createdMembership);
    }

    private static MembershipPlanDto ToDto(MembershipPlan plan)
    {
        return new MembershipPlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            Price = plan.Price,
            DurationInDays = plan.DurationInDays
        };
    }

    private static MembershipDto ToDto(Membership membership)
    {
        return new MembershipDto
        {
            PlanName = membership.PlanName,
            StartsAtUtc = membership.StartsAtUtc,
            EndsAtUtc = membership.EndsAtUtc
        };
    }
}
