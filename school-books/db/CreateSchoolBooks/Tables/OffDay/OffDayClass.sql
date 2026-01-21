PRINT 'Create OffDayClass table'
GO

CREATE TABLE [school_books].[OffDayClass] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [OffDayId]              INT              NOT NULL,
    [BasicClassId]          INT              NOT NULL,

    CONSTRAINT [PK_OffDayClass] PRIMARY KEY ([SchoolYear], [OffDayId], [BasicClassId]),

    CONSTRAINT [FK_OffDayClass_OffDay] FOREIGN KEY ([SchoolYear], [OffDayId]) REFERENCES [school_books].[OffDay] ([SchoolYear], [OffDayId]),
    CONSTRAINT [FK_OffDayClass_BasicClass] FOREIGN KEY ([BasicClassId]) REFERENCES [inst_nom].[BasicClass] ([BasicClassId]),
);
GO

exec school_books.spDescTable  N'OffDayClass', N'Неучебен период - клас.'

exec school_books.spDescColumn N'OffDayClass', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'OffDayClass', N'OffDayId'                  , N'Идентификатор на неучебен период.'
exec school_books.spDescColumn N'OffDayClass', N'BasicClassId'              , N'Идентификатор на випуска. Номенклатура inst_nom.BasicClass.'
