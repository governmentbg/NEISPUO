GO

CREATE TABLE [school_books].[SchoolYearDateInfoClassBook] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [SchoolYearDateInfoId]  INT              NOT NULL,
    [ClassBookId]           INT              NOT NULL,

    CONSTRAINT [PK_SchoolYearDateInfoClassBook] PRIMARY KEY ([SchoolYear], [SchoolYearDateInfoId], [ClassBookId]),

    CONSTRAINT [FK_SchoolYearDateInfoClassBook_SchoolYearDateInfo] FOREIGN KEY ([SchoolYear], [SchoolYearDateInfoId]) REFERENCES [school_books].[SchoolYearDateInfo] ([SchoolYear], [SchoolYearDateInfoId]),
    CONSTRAINT [FK_SchoolYearDateInfoClassBook_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId]) REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
);
GO
