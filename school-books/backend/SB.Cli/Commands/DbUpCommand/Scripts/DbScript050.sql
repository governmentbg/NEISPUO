GO

CREATE TABLE [school_books].[OffDayClassBook] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [OffDayId]              INT              NOT NULL,
    [ClassBookId]           INT              NOT NULL,

    CONSTRAINT [PK_OffDayClassBook] PRIMARY KEY ([SchoolYear], [OffDayId], [ClassBookId]),

    CONSTRAINT [FK_OffDayClassBook_OffDay] FOREIGN KEY ([SchoolYear], [OffDayId]) REFERENCES [school_books].[OffDay] ([SchoolYear], [OffDayId]),
    CONSTRAINT [FK_OffDayClassBook_BasicClass] FOREIGN KEY ([SchoolYear], [ClassBookId]) REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
);
GO
