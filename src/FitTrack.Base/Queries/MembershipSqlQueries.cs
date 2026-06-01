namespace FitTrack.Base.Queries;

public static class MembershipSqlQueries
{
    public const string EnsureSchema = """
        IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'fittrack')
        BEGIN
            EXEC('CREATE SCHEMA fittrack');
        END;

        IF OBJECT_ID('fittrack.MembershipPlans', 'U') IS NULL
        BEGIN
            CREATE TABLE fittrack.MembershipPlans
            (
                id int NOT NULL PRIMARY KEY,
                name nvarchar(100) NOT NULL,
                description nvarchar(500) NOT NULL,
                price decimal(10, 2) NOT NULL,
                duration_days int NOT NULL
            );
        END;

        IF OBJECT_ID('fittrack.Memberships', 'U') IS NULL
        BEGIN
            CREATE TABLE fittrack.Memberships
            (
                id uniqueidentifier NOT NULL PRIMARY KEY,
                user_id uniqueidentifier NOT NULL,
                plan_id int NOT NULL,
                starts_at_utc datetime2 NOT NULL,
                ends_at_utc datetime2 NOT NULL,
                CONSTRAINT FK_Memberships_Users
                    FOREIGN KEY (user_id) REFERENCES fittrack.Users(id),
                CONSTRAINT FK_Memberships_MembershipPlans
                    FOREIGN KEY (plan_id) REFERENCES fittrack.MembershipPlans(id)
            );
        END;

        MERGE fittrack.MembershipPlans AS target
        USING
        (
            VALUES
                (1, 'Weekly', 'Seven days of gym floor access with standard member check-in.', 9.99, 7),
                (2, 'Monthly', 'Thirty days of gym floor access, group classes, and conditioning sessions.', 29.99, 30),
                (3, 'Yearly', 'A full year of gym access, classes, recovery area, and priority booking.', 299.99, 365)
        ) AS source (id, name, description, price, duration_days)
        ON target.id = source.id
        WHEN MATCHED THEN
            UPDATE SET
                name = source.name,
                description = source.description,
                price = source.price,
                duration_days = source.duration_days
        WHEN NOT MATCHED THEN
            INSERT (id, name, description, price, duration_days)
            VALUES (source.id, source.name, source.description, source.price, source.duration_days);
        """;

    public const string GetPlans = """
        SELECT
            id,
            name,
            description,
            price,
            duration_days
        FROM fittrack.MembershipPlans
        ORDER BY price;
        """;

    public const string GetActiveByUserId = """
        SELECT TOP 1
            memberships.id,
            memberships.user_id,
            memberships.plan_id,
            plans.name AS plan_name,
            memberships.starts_at_utc,
            memberships.ends_at_utc
        FROM fittrack.Memberships AS memberships
        INNER JOIN fittrack.MembershipPlans AS plans
            ON plans.id = memberships.plan_id
        WHERE memberships.user_id = @UserId
            AND memberships.starts_at_utc <= SYSUTCDATETIME()
            AND memberships.ends_at_utc > SYSUTCDATETIME()
        ORDER BY memberships.ends_at_utc DESC;
        """;

    public const string Create = """
        INSERT INTO fittrack.Memberships (
            id,
            user_id,
            plan_id,
            starts_at_utc,
            ends_at_utc
        )
        VALUES (
            @Id,
            @UserId,
            @PlanId,
            @StartsAtUtc,
            @EndsAtUtc
        );
        """;
}
