PRINT 'Create SupportDifficulty table'
GO

EXEC [school_books].[spCreateIdSequence] N'SupportDifficulty'
GO

CREATE TABLE [school_books].[SupportDifficulty] (
    [SchoolYear]                    SMALLINT         NOT NULL,
    [SupportId]                     INT              NOT NULL,
    [SupportDifficultyTypeId]       INT              NOT NULL

    CONSTRAINT [PK_SupportDifficulty] PRIMARY KEY ([SchoolYear], [SupportId], [SupportDifficultyTypeId]),
    CONSTRAINT [FK_SupportDifficulty_Support] FOREIGN KEY ([SchoolYear], [SupportId]) REFERENCES [school_books].[Support] ([SchoolYear], [SupportId]),
    CONSTRAINT [FK_SupportDifficulty_SupportDifficultyType] FOREIGN KEY ([SupportDifficultyTypeId]) REFERENCES [school_books].[SupportDifficultyType] ([Id])
);
GO

exec school_books.spDescTable  N'SupportDifficulty', N'Подкрепа - затруднение.'

exec school_books.spDescColumn N'SupportDifficulty', N'SchoolYear'                    , N'Учебна година.'
exec school_books.spDescColumn N'SupportDifficulty', N'SupportId'                     , N'Идентификатор на подкрепа.'
exec school_books.spDescColumn N'SupportDifficulty', N'SupportDifficultyTypeId'       , N'Вид затруднение. Номенклатура SupportActivityType.'
