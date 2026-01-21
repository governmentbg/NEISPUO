PRINT 'Create HisMedicalNoticeSchoolYear table'
GO

CREATE TABLE [school_books].[HisMedicalNoticeSchoolYear] (
    [HisMedicalNoticeId]    INT             NOT NULL,
    [SchoolYear]            SMALLINT        NOT NULL,

    CONSTRAINT [PK_HisMedicalNoticeSchoolYear] PRIMARY KEY ([HisMedicalNoticeId], [SchoolYear]),

    CONSTRAINT [FK_HisMedicalNoticeSchoolYear_HisMedicalNoticeId]
        FOREIGN KEY ([HisMedicalNoticeId])
        REFERENCES [school_books].[HisMedicalNotice] ([HisMedicalNoticeId]),
);
GO

exec school_books.spDescTable  N'HisMedicalNoticeSchoolYear', N'Учебна година на медицинска бележка.'

exec school_books.spDescColumn N'HisMedicalNoticeSchoolYear', N'HisMedicalNoticeId'         , N'Идентификатор на медицинска бележка.'
exec school_books.spDescColumn N'HisMedicalNoticeSchoolYear', N'SchoolYear'                 , N'Учебна година.'
