GO

ALTER TABLE [school_books].[PersonMedicalNotice]
ADD
    [CreateDate] DATETIME NOT NULL CONSTRAINT DEFAULT_CreateDate DEFAULT '2023-09-15';
GO

ALTER TABLE [school_books].[PersonMedicalNotice]
DROP
    CONSTRAINT DEFAULT_CreateDate
GO
