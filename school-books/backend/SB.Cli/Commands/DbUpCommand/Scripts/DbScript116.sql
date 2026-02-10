GO

ALTER TABLE [school_books].[Attendance]
ADD [HisMedicalNoticeId] INT NULL,
    CONSTRAINT [FK_Attendance_PersonMedicalNotice] FOREIGN KEY ([SchoolYear], [PersonId], [HisMedicalNoticeId])
        REFERENCES [school_books].[PersonMedicalNotice] ([SchoolYear], [PersonId], [HisMedicalNoticeId])
GO
