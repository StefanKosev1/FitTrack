using FitTrack.Core.Entities;

namespace FitTrack.Core.Tests.Entities;

[TestClass]
public sealed class MembershipTests
{
    [TestMethod]
    public void Start_WithValidUserAndPlan_CreatesMembershipForPlanDuration()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plan = MembershipPlan.Restore(
            2,
            "Monthly",
            "Thirty days of gym access.",
            29.99m,
            30);

        // Act
        var membership = Membership.Start(userId, plan);

        // Assert
        Assert.AreNotEqual(Guid.Empty, membership.Id);
        Assert.AreEqual(userId, membership.UserId);
        Assert.AreEqual(plan.Id, membership.PlanId);
        Assert.AreEqual(plan.Name, membership.PlanName);
        Assert.AreEqual(30d, (membership.EndsAtUtc - membership.StartsAtUtc).TotalDays);
    }

    [TestMethod]
    public void Restore_WithEndDateBeforeStartDate_ThrowsArgumentException()
    {
        // Arrange
        var startsAtUtc = DateTime.UtcNow;
        var endsAtUtc = startsAtUtc.AddDays(-1);

        // Act and Assert
        Assert.ThrowsExactly<ArgumentException>(() =>
            Membership.Restore(
                Guid.NewGuid(),
                Guid.NewGuid(),
                1,
                "Weekly",
                startsAtUtc,
                endsAtUtc));
    }
}
