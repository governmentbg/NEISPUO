GO

ALTER TABLE [school_books].[HisMedicalNotice]
DROP CONSTRAINT [UK_HisMedicalNotice_NrnMedicalNotice];
GO

ALTER TABLE [school_books].[HisMedicalNotice]
ADD CONSTRAINT [UK_HisMedicalNotice] UNIQUE CLUSTERED ([AuthoredOn], [HisMedicalNoticeId]);
GO

ALTER TABLE [school_books].[HisMedicalNotice]
ADD CONSTRAINT [UK_HisMedicalNotice_NrnMedicalNotice] UNIQUE ([NrnMedicalNotice]);
GO
