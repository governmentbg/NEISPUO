PRINT 'Create PersonMedicalNotice table'
GO

CREATE TABLE [school_books].[PersonMedicalNotice] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [PersonId]                  INT             NOT NULL,
    [HisMedicalNoticeId]        INT             NOT NULL,

    [CreateDate]                DATETIME        NOT NULL,

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

exec school_books.spDescTable  N'PersonMedicalNotice', N'Медицинска бележка за ученик.'

exec school_books.spDescColumn N'PersonMedicalNotice', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'PersonMedicalNotice', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'PersonMedicalNotice', N'HisMedicalNoticeId'        , N'Идентификатор на медицинска бележка.'

exec school_books.spDescColumn N'PersonMedicalNotice', N'CreateDate'                , N'Дата на създаване на записа.'
