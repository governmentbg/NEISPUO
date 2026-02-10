GO

ALTER TABLE [school_books].[ClassBookExtProvider]
ADD [ScheduleExtSystemId] INT NULL,
    CONSTRAINT [CHK_ExtSystemId_ScheduleExtSystemId] CHECK (
        NOT ([ExtSystemId] IS NOT NULL AND [ScheduleExtSystemId] IS NOT NULL)
    );
GO
