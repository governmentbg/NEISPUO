GO

ALTER TABLE [school_books].[HisMedicalNoticeSchoolYear]
ADD CONSTRAINT [FK_HisMedicalNoticeSchoolYear_HisMedicalNoticeId]
    FOREIGN KEY ([HisMedicalNoticeId])
    REFERENCES [school_books].[HisMedicalNotice] ([HisMedicalNoticeId]);
GO
