PRINT 'Create NoteStudent table'
GO

CREATE TABLE [school_books].[NoteStudent] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [NoteId]                INT              NOT NULL,
    [PersonId]              INT              NOT NULL,

    CONSTRAINT [PK_NoteStudent] PRIMARY KEY ([SchoolYear], [NoteId], [PersonId]),
    CONSTRAINT [FK_NoteStudent_Note] FOREIGN KEY ([SchoolYear], [NoteId]) REFERENCES [school_books].[Note] ([SchoolYear], [NoteId]),

    -- external references
    CONSTRAINT [FK_NoteStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
);
GO

exec school_books.spDescTable  N'NoteStudent', N'Бележка - ученик.'

exec school_books.spDescColumn N'NoteStudent', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'NoteStudent', N'NoteId'                    , N'Идентификатор на бележка.'
exec school_books.spDescColumn N'NoteStudent', N'PersonId'                  , N'Идентификатор на ученик.'
