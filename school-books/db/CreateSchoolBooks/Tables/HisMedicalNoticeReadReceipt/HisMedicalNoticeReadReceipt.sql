PRINT 'Create HisMedicalNoticeReadReceipt table'
GO

CREATE TABLE [school_books].[HisMedicalNoticeReadReceipt] (
    [ExtSystemId]           INT             NOT NULL,
    [HisMedicalNoticeId]    INT             NOT NULL,

    [IsAcknowledged]        BIT             NOT NULL,

    [CreateDate]            DATETIME2       NOT NULL,
    [ModifyDate]            DATETIME2       NOT NULL,
    [Version]               ROWVERSION      NOT NULL,

    CONSTRAINT [PK_HisMedicalNoticeReadReceipt] PRIMARY KEY ([ExtSystemId], [HisMedicalNoticeId]),

    CONSTRAINT [FK_HisMedicalNoticeReadReceipt_HisMedicalNoticeId]
        FOREIGN KEY ([HisMedicalNoticeId])
        REFERENCES [school_books].[HisMedicalNotice] ([HisMedicalNoticeId]),

    -- external references
    CONSTRAINT [FK_HisMedicalNoticeReadReceipt_ExtSystemId]
        FOREIGN KEY ([ExtSystemId])
        REFERENCES [core].[ExtSystem] ([ExtSystemID]),
);
GO

exec school_books.spDescTable  N'HisMedicalNoticeReadReceipt', N'Маркер за прочетена медицинска бележка от доставчик на електронен дневник.'

exec school_books.spDescColumn N'HisMedicalNoticeReadReceipt', N'ExtSystemId'           , N'Идентификатор на външна система.'
exec school_books.spDescColumn N'HisMedicalNoticeReadReceipt', N'HisMedicalNoticeId'    , N'Идентификатор на медицинска бележка.'
exec school_books.spDescColumn N'HisMedicalNoticeReadReceipt', N'IsAcknowledged'        , N'Потвърдено получаване – Да/Не.'

exec school_books.spDescColumn N'HisMedicalNoticeReadReceipt', N'CreateDate'            , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'HisMedicalNoticeReadReceipt', N'ModifyDate'            , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'HisMedicalNoticeReadReceipt', N'Version'               , N'Версия.'
