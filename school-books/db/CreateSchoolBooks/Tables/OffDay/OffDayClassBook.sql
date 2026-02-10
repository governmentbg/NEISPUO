PRINT 'Create OffDayClassBook table'
GO

CREATE TABLE [school_books].[OffDayClassBook] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [OffDayId]              INT              NOT NULL,
    [ClassBookId]           INT              NOT NULL,

    CONSTRAINT [PK_OffDayClassBook] PRIMARY KEY ([SchoolYear], [OffDayId], [ClassBookId]),

    CONSTRAINT [FK_OffDayClassBook_OffDay] FOREIGN KEY ([SchoolYear], [OffDayId]) REFERENCES [school_books].[OffDay] ([SchoolYear], [OffDayId]),
    CONSTRAINT [FK_OffDayClassBook_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId]) REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
);
GO

exec school_books.spDescTable  N'OffDayClassBook', N'Неучебен период - дневник.'

exec school_books.spDescColumn N'OffDayClassBook', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'OffDayClassBook', N'OffDayId'                  , N'Идентификатор на неучебен период.'
exec school_books.spDescColumn N'OffDayClassBook', N'ClassBookId'               , N'Идентификатор на дневник.'
