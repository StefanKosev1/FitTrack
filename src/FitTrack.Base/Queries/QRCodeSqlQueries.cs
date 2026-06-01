namespace FitTrack.Base.Queries;

public static class QRCodeSqlQueries
{
    public const string EnsureSchema = """
        IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'fittrack')
        BEGIN
            EXEC('CREATE SCHEMA fittrack');
        END;

        IF OBJECT_ID('fittrack.QRCodes', 'U') IS NULL
        BEGIN
            CREATE TABLE fittrack.QRCodes
            (
                id uniqueidentifier NOT NULL PRIMARY KEY,
                user_id uniqueidentifier NOT NULL,
                code nvarchar(100) NOT NULL,
                created_at_utc datetime2 NOT NULL,
                CONSTRAINT UQ_QRCodes_UserId UNIQUE (user_id),
                CONSTRAINT UQ_QRCodes_Code UNIQUE (code),
                CONSTRAINT FK_QRCodes_Users
                    FOREIGN KEY (user_id) REFERENCES fittrack.Users(id)
            );
        END;

        IF OBJECT_ID('fittrack.QRCodes', 'U') IS NOT NULL
            AND NOT EXISTS
            (
                SELECT 1
                FROM sys.indexes
                WHERE object_id = OBJECT_ID('fittrack.QRCodes')
                    AND name = 'UQ_QRCodes_Code'
            )
        BEGIN
            ALTER TABLE fittrack.QRCodes
            ADD CONSTRAINT UQ_QRCodes_Code UNIQUE (code);
        END;
        """;

    public const string GetByUserId = """
        SELECT
            id,
            user_id,
            code,
            created_at_utc
        FROM fittrack.QRCodes
        WHERE user_id = @UserId;
        """;

    public const string Create = """
        INSERT INTO fittrack.QRCodes (
            id,
            user_id,
            code,
            created_at_utc
        )
        VALUES (
            @Id,
            @UserId,
            @Code,
            @CreatedAtUtc
        );
        """;
}
