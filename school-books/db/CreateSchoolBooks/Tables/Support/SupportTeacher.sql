PRINT 'Create SupportTeacher table'
GO

CREATE TABLE [school_books].[SupportTeacher] (
    [SchoolYear]          SMALLINT        NOT NULL,
    [SupportId]           INT             NOT NULL,
    [PersonId]            INT             NOT NULL,

    CONSTRAINT [PK_SupportTeacher] PRIMARY KEY ([SchoolYear], [SupportId], [PersonId]),
    CONSTRAINT [FK_SupportTeacher_Support] FOREIGN KEY ([SchoolYear], [SupportId]) REFERENCES [school_books].[Support] ([SchoolYear], [SupportId]),

    INDEX [IX_SupportTeacher_PersonId] ([SchoolYear], [PersonId]),

    -- external references
    CONSTRAINT [FK_SupportTeacher_PersonId] FOREIGN KEY ([PersonId]) REFERENCES  [core].[Person] ([PersonID]),
);
GO

exec school_books.spDescTable  N'SupportTeacher', N'Подкрепа - учител от екипа.'

exec school_books.spDescColumn N'SupportTeacher', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'SupportTeacher', N'SupportId'                 , N'Идентификатор на подкрепа.'
exec school_books.spDescColumn N'SupportTeacher', N'PersonId'                  , N'Идентификатор на учител.'
