PRINT 'Create UserPushSubscription table'
GO

CREATE TABLE [school_books].[UserPushSubscription](
    [UserPushSubscriptionId] INT             NOT NULL IDENTITY(1,1),
    [SysUserId]              INT             NOT NULL,
    [Endpoint]               NVARCHAR(500)   NOT NULL,
    [P256dh]                 NVARCHAR(100)   NOT NULL,
    [Auth]                   NVARCHAR(100)   NOT NULL,
    [CreateDate]             DATETIME2       NOT NULL,
    [ModifyDate]             DATETIME2       NOT NULL,
    [Version]                ROWVERSION      NOT NULL,

    CONSTRAINT [PK_UserPushSubscription] PRIMARY KEY ([UserPushSubscriptionId]),

    -- external references
    CONSTRAINT [FK_UserPushSubscription_SysUserId] FOREIGN KEY ([SysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
)
GO
