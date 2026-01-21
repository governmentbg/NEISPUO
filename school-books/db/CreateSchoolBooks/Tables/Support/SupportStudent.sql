PRINT 'Create SupportStudent table'
GO

CREATE TABLE [school_books].[SupportStudent] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [SupportId]             INT              NOT NULL,
    [PersonId]              INT              NOT NULL,

    CONSTRAINT [PK_SupportStudent] PRIMARY KEY ([SchoolYear], [SupportId], [PersonId]),
    CONSTRAINT [FK_SupportStudent_Support] FOREIGN KEY ([SchoolYear], [SupportId]) REFERENCES [school_books].[Support] ([SchoolYear], [SupportId]),

    -- external references
    CONSTRAINT [FK_SupportStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
);
GO

exec school_books.spDescTable  N'SupportStudent', N'Подкрепа - ученик.'

exec school_books.spDescColumn N'SupportStudent', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'SupportStudent', N'SupportId'                 , N'Идентификатор на подкрепа.'
exec school_books.spDescColumn N'SupportStudent', N'PersonId'                  , N'Идентификатор на ученик.'
