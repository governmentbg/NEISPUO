PRINT 'Create SpbsBookRecordEscape table'
GO

CREATE TABLE [school_books].[SpbsBookRecordEscape] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [SpbsBookRecordId]          INT              NOT NULL,
    [OrderNum]                  SMALLINT         NOT NULL,

    [EscapeDate]                DATE             NOT NULL,
    [EscapeTime]                TIME             NOT NULL,
    [PoliceNotificationDate]    DATE             NOT NULL,
    [PoliceNotificationTime]    TIME             NOT NULL,
    [PoliceLetterNumber]        NVARCHAR (100)   NOT NULL,
    [PoliceLetterDate]          DATE             NOT NULL,
    [ReturnDate]                DATE             NULL,

    CONSTRAINT [PK_SpbsBookRecordEscape] PRIMARY KEY ([SchoolYear], [SpbsBookRecordId], [OrderNum]),

    CONSTRAINT [FK_SpbsBookRecordEscape_SpbsBookRecord] FOREIGN KEY ([SchoolYear], [SpbsBookRecordId]) REFERENCES [school_books].[SpbsBookRecord] ([SchoolYear], [SpbsBookRecordId]),
);
GO

exec school_books.spDescTable  N'SpbsBookRecordEscape', N'Запис от книга за движението на учениците от СПИ/ВУИ - бягство.'

exec school_books.spDescColumn N'SpbsBookRecordEscape', N'SchoolYear'               , N'Учебна година.'
exec school_books.spDescColumn N'SpbsBookRecordEscape', N'SpbsBookRecordId'         , N'Идентификатор на запис от книга за движението на учениците от СПИ/ВУИ.'
exec school_books.spDescColumn N'SpbsBookRecordEscape', N'OrderNum'                 , N'Пореден номер.'

exec school_books.spDescColumn N'SpbsBookRecordEscape', N'EscapeDate'               , N'Дата на регистритане на бягството.'
exec school_books.spDescColumn N'SpbsBookRecordEscape', N'EscapeTime'               , N'Час на регистритане на бягството.'
exec school_books.spDescColumn N'SpbsBookRecordEscape', N'PoliceNotificationDate'   , N'Дата на уведомяване на РПУ.'
exec school_books.spDescColumn N'SpbsBookRecordEscape', N'PoliceNotificationTime'   , N'Час на уведомяване на РПУ.'
exec school_books.spDescColumn N'SpbsBookRecordEscape', N'PoliceLetterNumber'       , N'Номер на писмо до РПУ/НС Полиция.'
exec school_books.spDescColumn N'SpbsBookRecordEscape', N'PoliceLetterDate'         , N'Дата на писмо до РПУ/НС Полиция.'
exec school_books.spDescColumn N'SpbsBookRecordEscape', N'ReturnDate'               , N'Дата на връщане.'
