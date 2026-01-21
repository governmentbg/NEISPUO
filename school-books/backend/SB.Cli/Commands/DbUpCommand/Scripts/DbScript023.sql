CREATE TABLE [school_books].[NoteStudent] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [NoteId]                INT              NOT NULL,
    [ClassId]               INT              NOT NULL,
    [PersonId]              INT              NOT NULL,

    CONSTRAINT [PK_NoteStudent] PRIMARY KEY ([SchoolYear], [NoteId], [ClassId], [PersonId]),
    CONSTRAINT [FK_NoteStudent_Note] FOREIGN KEY ([SchoolYear], [NoteId]) REFERENCES [school_books].[Note] ([SchoolYear], [NoteId]),

 -- external references
    CONSTRAINT [FK_NoteStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_NoteStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
);
GO

INSERT INTO [school_books].[NoteStudent]
        ([SchoolYear], [NoteId], [ClassId], [PersonId])
    SELECT
        n.[SchoolYear],
        n.[NoteId],
        n.[ClassId],
        n.[PersonId]
    FROM [school_books].[Note] n
GO

ALTER TABLE [school_books].[Note]
DROP
    CONSTRAINT [FK_Note_ClassGroup],
    CONSTRAINT [FK_Note_Person],
    COLUMN [ClassId],
    COLUMN [PersonId];
GO

ALTER TABLE [school_books].[Note]
ADD
    [IsForAllStudents] BIT NOT NULL CONSTRAINT DEFAULT_IsForAllStudents DEFAULT 0;
GO

ALTER TABLE [school_books].[Note]
DROP
    CONSTRAINT DEFAULT_IsForAllStudents
GO
