PRINT 'Create ClassBookStudentCarriedAbsence table'
GO

CREATE TABLE [school_books].[ClassBookStudentCarriedAbsence] (
    [SchoolYear]               SMALLINT         NOT NULL,
    [ClassBookId]              INT              NOT NULL,
    [PersonId]                 INT              NOT NULL,

    [FirstTermExcusedCount]    INT              NOT NULL,
    [FirstTermUnexcusedCount]  INT              NOT NULL,
    [FirstTermLateCount]       INT              NOT NULL,
    [SecondTermExcusedCount]   INT              NOT NULL,
    [SecondTermUnexcusedCount] INT              NOT NULL,
    [SecondTermLateCount]      INT              NOT NULL,

    CONSTRAINT [PK_ClassBookStudentCarriedAbsence] PRIMARY KEY ([SchoolYear], [ClassBookId], [PersonId]),
    CONSTRAINT [FK_ClassBookStudentCarriedAbsence_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_ClassBookStudentCarriedAbsence_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO

exec school_books.spDescTable  N'ClassBookStudentCarriedAbsence', N'Дневник - пренесени отсъствия.'

exec school_books.spDescColumn N'ClassBookStudentCarriedAbsence', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookStudentCarriedAbsence', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ClassBookStudentCarriedAbsence', N'PersonId'                  , N'Идентификатор на ученик.'

exec school_books.spDescColumn N'ClassBookStudentCarriedAbsence', N'FirstTermExcusedCount'     , N'Пренесени уважителни отсъствия за I срок.'
exec school_books.spDescColumn N'ClassBookStudentCarriedAbsence', N'FirstTermUnexcusedCount'   , N'Пренесени неуважителни отсъствия за I срок.'
exec school_books.spDescColumn N'ClassBookStudentCarriedAbsence', N'FirstTermLateCount'        , N'Пренесени закъснения за I срок.'
exec school_books.spDescColumn N'ClassBookStudentCarriedAbsence', N'SecondTermExcusedCount'    , N'Пренесени уважителни отсъствия за II срок.'
exec school_books.spDescColumn N'ClassBookStudentCarriedAbsence', N'SecondTermUnexcusedCount'  , N'Пренесени неуважителни отсъствия за II срок.'
exec school_books.spDescColumn N'ClassBookStudentCarriedAbsence', N'SecondTermLateCount'       , N'Пренесени закъснения за II срок.'
