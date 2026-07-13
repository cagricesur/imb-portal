IF NOT EXISTS (
    SELECT 1
    FROM sys.tables
    WHERE name = 'UserSessions'
)
BEGIN
    CREATE TABLE [dbo].[UserSessions] (
        [SessionId] UNIQUEIDENTIFIER NOT NULL,
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [RefreshTokenHash] VARCHAR(64) NOT NULL,
        [UserAgentHash] VARCHAR(64) NOT NULL,
        [CreatedAt] DATETIME NOT NULL,
        [ExpiresAt] DATETIME NOT NULL,
        [LastRefreshedAt] DATETIME NOT NULL,
        [IsRevoked] BIT NOT NULL CONSTRAINT [DF_UserSessions_IsRevoked] DEFAULT (0),
        CONSTRAINT [PK_UserSessions] PRIMARY KEY ([SessionId]),
        CONSTRAINT [FK_UserSessions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UID]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_UserSessions_UserId] ON [dbo].[UserSessions] ([UserId]);
    CREATE INDEX [IX_UserSessions_RefreshTokenHash] ON [dbo].[UserSessions] ([RefreshTokenHash]);
END;
