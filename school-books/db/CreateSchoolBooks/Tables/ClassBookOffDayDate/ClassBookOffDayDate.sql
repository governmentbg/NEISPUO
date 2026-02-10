PRINT 'Create ClassBookOffDayDate table'
GO

CREATE TABLE [school_books].[ClassBookOffDayDate] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ClassBookId]           INT              NOT NULL,
    [Date]                  DATE             NOT NULL,

    [OffDayId]              INT              NOT NULL,
    [IsPgOffProgramDay]     BIT              NOT NULL,

    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_ClassBookOffDayDate] PRIMARY KEY ([SchoolYear], [ClassBookId], [Date]),

    CONSTRAINT [FK_ClassBookOffDayDate_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId]) REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [FK_ClassBookOffDayDate_OffDay] FOREIGN KEY ([SchoolYear], [OffDayId]) REFERENCES [school_books].[OffDay] ([SchoolYear], [OffDayId]),

    INDEX [IX_ClassBookOffDayDate] UNIQUE ([SchoolYear], [OffDayId], [ClassBookId], [Date]),
);
GO

exec school_books.spDescTable  N'ClassBookOffDayDate', N'Неучебен ден за дневник.'

exec school_books.spDescColumn N'ClassBookOffDayDate', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookOffDayDate', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ClassBookOffDayDate', N'Date'                      , N'Дата.'

exec school_books.spDescColumn N'ClassBookOffDayDate', N'OffDayId'                  , N'Идентификатор на неучебни дни.'
exec school_books.spDescColumn N'ClassBookOffDayDate', N'IsPgOffProgramDay'         , N'Позволено е въвеждане на присъствия в ПГ за дните от понеделник до петък – да/не.'

exec school_books.spDescColumn N'ClassBookOffDayDate', N'Version'                   , N'Версия.'
