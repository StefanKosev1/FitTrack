/*
    Complete Microsoft SQL Server setup script for FitTrack.

    When using the school database, remove the CREATE DATABASE and USE
    sections and run the rest of the script inside the assigned database.
*/

IF DB_ID(N'FitTrackDb') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE [FitTrackDb]');
END;
GO

USE [FitTrackDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'fittrack')
BEGIN
    EXEC(N'CREATE SCHEMA fittrack');
END;
GO

IF OBJECT_ID(N'fittrack.Users', N'U') IS NULL
BEGIN
    CREATE TABLE fittrack.Users
    (
        id uniqueidentifier NOT NULL,
        full_name nvarchar(200) NOT NULL,
        email nvarchar(320) NOT NULL,
        password_hash nvarchar(500) NOT NULL,
        password_salt nvarchar(500) NOT NULL,
        created_at_utc datetime2 NOT NULL,

        CONSTRAINT PK_Users PRIMARY KEY (id),
        CONSTRAINT UQ_Users_Email UNIQUE (email)
    );
END;
GO

IF OBJECT_ID(N'fittrack.MembershipPlans', N'U') IS NULL
BEGIN
    CREATE TABLE fittrack.MembershipPlans
    (
        id int NOT NULL,
        name nvarchar(100) NOT NULL,
        description nvarchar(500) NOT NULL,
        price decimal(10, 2) NOT NULL,
        duration_days int NOT NULL,

        CONSTRAINT PK_MembershipPlans PRIMARY KEY (id),
        CONSTRAINT CK_MembershipPlans_Price CHECK (price >= 0),
        CONSTRAINT CK_MembershipPlans_DurationDays CHECK (duration_days > 0)
    );
END;
GO

IF OBJECT_ID(N'fittrack.Memberships', N'U') IS NULL
BEGIN
    CREATE TABLE fittrack.Memberships
    (
        id uniqueidentifier NOT NULL,
        user_id uniqueidentifier NOT NULL,
        plan_id int NOT NULL,
        starts_at_utc datetime2 NOT NULL,
        ends_at_utc datetime2 NOT NULL,

        CONSTRAINT PK_Memberships PRIMARY KEY (id),
        CONSTRAINT FK_Memberships_Users
            FOREIGN KEY (user_id) REFERENCES fittrack.Users(id),
        CONSTRAINT FK_Memberships_MembershipPlans
            FOREIGN KEY (plan_id) REFERENCES fittrack.MembershipPlans(id),
        CONSTRAINT CK_Memberships_DateRange CHECK (ends_at_utc > starts_at_utc)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'fittrack.Memberships')
        AND name = N'IX_Memberships_UserId_Dates'
)
BEGIN
    CREATE INDEX IX_Memberships_UserId_Dates
        ON fittrack.Memberships (user_id, starts_at_utc, ends_at_utc);
END;
GO

IF OBJECT_ID(N'fittrack.QRCodes', N'U') IS NULL
BEGIN
    CREATE TABLE fittrack.QRCodes
    (
        id uniqueidentifier NOT NULL,
        user_id uniqueidentifier NOT NULL,
        code nvarchar(100) NOT NULL,
        created_at_utc datetime2 NOT NULL,

        CONSTRAINT PK_QRCodes PRIMARY KEY (id),
        CONSTRAINT UQ_QRCodes_UserId UNIQUE (user_id),
        CONSTRAINT UQ_QRCodes_Code UNIQUE (code),
        CONSTRAINT FK_QRCodes_Users
            FOREIGN KEY (user_id) REFERENCES fittrack.Users(id)
    );
END;
GO

MERGE fittrack.MembershipPlans AS target
USING
(
    VALUES
        (1, N'Weekly', N'Seven days of gym floor access with standard member check-in.', CAST(9.99 AS decimal(10, 2)), 7),
        (2, N'Monthly', N'Thirty days of gym floor access, group classes, and conditioning sessions.', CAST(29.99 AS decimal(10, 2)), 30),
        (3, N'Yearly', N'A full year of gym access, classes, recovery area, and priority booking.', CAST(299.99 AS decimal(10, 2)), 365)
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
GO
