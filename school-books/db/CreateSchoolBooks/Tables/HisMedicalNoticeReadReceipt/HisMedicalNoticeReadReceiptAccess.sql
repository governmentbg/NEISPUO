PRINT 'Create HisMedicalNoticeReadReceiptAccess table'
GO

CREATE TABLE [school_books].[HisMedicalNoticeReadReceiptAccess](
    [ExtSystemId]                       INT         NOT NULL,
    [HisMedicalNoticeId]                INT         NOT NULL,
    [SchoolYear]                        SMALLINT    NOT NULL,
    [InstId]                            INT         NOT NULL,

    CONSTRAINT [PK_HisMedicalNoticeReadReceiptAccess] PRIMARY KEY ([ExtSystemId], [HisMedicalNoticeId], [SchoolYear], [InstId]),
    CONSTRAINT [FK_HisMedicalNoticeReadReceiptAccess_HisMedicalNoticeReadReceipt]
        FOREIGN KEY ([ExtSystemId], [HisMedicalNoticeId])
        REFERENCES [school_books].[HisMedicalNoticeReadReceipt] ([ExtSystemId], [HisMedicalNoticeId]),
    CONSTRAINT [FK_HisMedicalNoticeReadReceiptAccess_ClassBookExtProvider]
        FOREIGN KEY ([SchoolYear], [InstId]) REFERENCES [school_books].[ClassBookExtProvider] ([SchoolYear], [InstId]),
)
GO

exec school_books.spDescTable  N'HisMedicalNoticeReadReceiptAccess', N'Учебна година/Институция на база на които е прочетена медицинска бележка от доставчик на електронен дневник.'

exec school_books.spDescColumn N'HisMedicalNoticeReadReceiptAccess', N'ExtSystemId'             , N'Идентификатор на външна система.'
exec school_books.spDescColumn N'HisMedicalNoticeReadReceiptAccess', N'HisMedicalNoticeId'      , N'Идентификатор на медицинска бележка.'
exec school_books.spDescColumn N'HisMedicalNoticeReadReceiptAccess', N'SchoolYear'              , N'Учебна година.'
exec school_books.spDescColumn N'HisMedicalNoticeReadReceiptAccess', N'InstId'                  , N'Идентификатор на институцията.'
