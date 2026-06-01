namespace FitTrack.Core.Dtos;

public class MembershipDto
{
    public string PlanName { get; set; } = string.Empty;

    public DateTime StartsAtUtc { get; set; }

    public DateTime EndsAtUtc { get; set; }
}
