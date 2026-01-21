GO

CREATE TABLE [school_books].[PersonMedicalNotice] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [PersonId]                  INT             NOT NULL,
    [HisMedicalNoticeId]        INT             NOT NULL,

    [CreateDate]                DATETIME        NOT NULL,
    [ModifyDate]                DATETIME        NOT NULL,
    [Version]                   ROWVERSION      NOT NULL,

    CONSTRAINT [PK_PersonMedicalNotice] PRIMARY KEY ([SchoolYear], [PersonId], [HisMedicalNoticeId]),

    CONSTRAINT [FK_PersonMedicalNotice_HisMedicalNoticeSchoolYear]
        FOREIGN KEY ([HisMedicalNoticeId], [SchoolYear])
        REFERENCES [school_books].[HisMedicalNoticeSchoolYear] ([HisMedicalNoticeId], [SchoolYear]),

    -- external references
    CONSTRAINT [FK_PersonMedicalNotice_Person]
        FOREIGN KEY ([PersonId])
        REFERENCES [core].[Person] ([PersonID]),
);
GO

ALTER TABLE [school_books].[Absence]
ADD [HisMedicalNoticeId] INT NULL,
    CONSTRAINT [FK_Absence_PersonMedicalNotice] FOREIGN KEY ([SchoolYear], [PersonId], [HisMedicalNoticeId])
        REFERENCES [school_books].[PersonMedicalNotice] ([SchoolYear], [PersonId], [HisMedicalNoticeId]);
GO
