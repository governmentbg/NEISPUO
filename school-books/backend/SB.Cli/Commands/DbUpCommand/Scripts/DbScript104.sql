ALTER TABLE [school_books].[ScheduleLesson]
ADD
    [IsVerified]            BIT              NOT NULL CONSTRAINT DEFAULT_IsVerified DEFAULT 0,
    [VerifyDate]            DATETIME2        NULL,
    [VerifiedBySysUserId]   INT              NULL,

    CONSTRAINT [FK_ScheduleLesson_VerifiedBySysUserId] FOREIGN KEY ([VerifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID])
GO

ALTER TABLE [school_books].[ScheduleLesson]
DROP
    CONSTRAINT DEFAULT_IsVerified
GO

CREATE NONCLUSTERED INDEX [IX_TopicTeacher_PersonId] ON [school_books].[TopicTeacher] ([SchoolYear], [PersonId])
GO
