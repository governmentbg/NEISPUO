PRINT 'Create SpbsBookRecordAbsence table'
GO

CREATE TABLE [school_books].[SpbsBookRecordAbsence] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [SpbsBookRecordId]          INT              NOT NULL,
    [OrderNum]                  SMALLINT         NOT NULL,

    [AbsenceDate]               DATE             NOT NULL,
    [AbsenceReason]             NVARCHAR (1000)  NOT NULL,

    CONSTRAINT [PK_SpbsBookRecordAbsence] PRIMARY KEY ([SchoolYear], [SpbsBookRecordId], [OrderNum]),

    CONSTRAINT [FK_SpbsBookRecordAbsence_SpbsBookRecord] FOREIGN KEY ([SchoolYear], [SpbsBookRecordId]) REFERENCES [school_books].[SpbsBookRecord] ([SchoolYear], [SpbsBookRecordId]),
);
GO

exec school_books.spDescTable  N'SpbsBookRecordAbsence', N'Запис от книга за движението на учениците от СПИ/ВУИ - отсъствие.'

exec school_books.spDescColumn N'SpbsBookRecordAbsence', N'SchoolYear'            , N'Учебна година.'
exec school_books.spDescColumn N'SpbsBookRecordAbsence', N'SpbsBookRecordId'      , N'Идентификатор на запис от книга за движението на учениците от СПИ/ВУИ.'
exec school_books.spDescColumn N'SpbsBookRecordAbsence', N'OrderNum'              , N'Пореден номер.'

exec school_books.spDescColumn N'SpbsBookRecordAbsence', N'AbsenceDate'           , N'Дата на отсъствие.'
exec school_books.spDescColumn N'SpbsBookRecordAbsence', N'AbsenceReason'         , N'Причини.'
