namespace FitTrack.Core.Entities;

public sealed class MembershipPlan
{
    private MembershipPlan(int id, string name, string description, decimal price, int durationInDays)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Plan ID must be positive.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Plan name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Plan description is required.", nameof(description));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Plan price cannot be negative.");
        }

        if (durationInDays <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(durationInDays), "Plan duration must be positive.");
        }

        Id = id;
        Name = name.Trim();
        Description = description.Trim();
        Price = price;
        DurationInDays = durationInDays;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public decimal Price { get; private set; }

    public int DurationInDays { get; private set; }

    public static MembershipPlan Restore(int id, string name, string description, decimal price, int durationInDays)
    {
        return new MembershipPlan(id, name, description, price, durationInDays);
    }
}
