using System.Security.Claims;
using FitTrack.Core.Dtos;
using FitTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitTrack.Web.Pages.Memberships;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IMembershipService _membershipService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IMembershipService membershipService, ILogger<IndexModel> logger)
    {
        _membershipService = membershipService;
        _logger = logger;
    }

    public IReadOnlyCollection<MembershipPlanDto> Plans { get; private set; } = [];

    public MembershipDto? ActiveMembership { get; private set; }

    [BindProperty]
    public int SelectedPlanId { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Challenge();
        }

        await LoadMembershipDataAsync(userId.Value);

        return Page();
    }

    public async Task<IActionResult> OnPostStartAsync()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Challenge();
        }

        try
        {
            var membership = await _membershipService.StartMembershipAsync(userId.Value, SelectedPlanId);
            StatusMessage = $"Your {membership.PlanName} membership is active.";
            return RedirectToPage();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not start membership for user {UserId}.", userId);
            ModelState.AddModelError(string.Empty, "Membership could not be started. Check the selected plan and try again.");
            await LoadMembershipDataAsync(userId.Value);
            return Page();
        }
    }

    private async Task LoadMembershipDataAsync(Guid userId)
    {
        Plans = await _membershipService.GetPlansAsync();
        ActiveMembership = await _membershipService.GetActiveMembershipAsync(userId);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim, out var userId)
            ? userId
            : null;
    }
}
