PRINT 'Create SupportActivity table'
GO

EXEC [school_books].[spCreateIdSequence] N'SupportActivity'
GO

CREATE TABLE [school_books].[SupportActivity] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [SupportId]             INT              NOT NULL,
    [SupportActivityId]     INT              NOT NULL,

    [SupportActivityTypeId] INT              NOT NULL,
    [Target]                NVARCHAR(MAX)    NULL,
    [Result]                NVARCHAR(MAX)    NULL,
    [Date]                  DATE             NULL,

    CONSTRAINT [PK_SupportActivity] PRIMARY KEY ([SchoolYear], [SupportId], [SupportActivityId]),
    CONSTRAINT [FK_SupportActivity_Support] FOREIGN KEY ([SchoolYear], [SupportId]) REFERENCES [school_books].[Support] ([SchoolYear], [SupportId]),
    CONSTRAINT [FK_SupportActivity_SupportActivityType] FOREIGN KEY ([SupportActivityTypeId]) REFERENCES [school_books].[SupportActivityType] ([Id])
);
GO

exec school_books.spDescTable  N'SupportActivity', N'Подкрепа - дейност.'

exec school_books.spDescColumn N'SupportActivity', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'SupportActivity', N'SupportId'                 , N'Идентификатор на подкрепа.'
exec school_books.spDescColumn N'SupportActivity', N'SupportActivityId'         , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'SupportActivity', N'SupportActivityTypeId'     , N'Вид дейност. Номенклатура SupportActivityType.'
exec school_books.spDescColumn N'SupportActivity', N'Target'                    , N'Цел.'
exec school_books.spDescColumn N'SupportActivity', N'Result'                    , N'Резултат.'
exec school_books.spDescColumn N'SupportActivity', N'Date'                      , N'Дата.'
