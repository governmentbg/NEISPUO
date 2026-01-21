PRINT 'Create GradeChangeExamsAdmProtocolStudent table'
GO

CREATE TABLE [school_books].[GradeChangeExamsAdmProtocolStudent] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [GradeChangeExamsAdmProtocolId]     INT              NOT NULL,
    [ClassId]                           INT              NOT NULL,
    [PersonId]                          INT              NOT NULL,

    CONSTRAINT [PK_GradeChangeExamsAdmProtocolStudent] PRIMARY KEY ([SchoolYear], [GradeChangeExamsAdmProtocolId], [ClassId], [PersonId]),
    CONSTRAINT [FK_GradeChangeExamsAdmProtocolStudent_GradeChangeExamsAdmProtocol] FOREIGN KEY ([SchoolYear], [GradeChangeExamsAdmProtocolId])
        REFERENCES [school_books].[GradeChangeExamsAdmProtocol] ([SchoolYear], [GradeChangeExamsAdmProtocolId]),

    -- external references
    CONSTRAINT [FK_GradeChangeExamsAdmProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_GradeChangeExamsAdmProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_GradeChangeExamsAdmProtocolStudent_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'GradeChangeExamsAdmProtocolStudent', N'Протокол за допускане до изпити за промяна на оценката - ученик.'

exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolStudent', N'SchoolYear'                         , N'Учебна година.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolStudent', N'GradeChangeExamsAdmProtocolId'      , N'Идентификатор на протокол за допускане до изпити за промяна на оценката.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolStudent', N'ClassId'                            , N'Идентификатор на група/паралелка, към която принадлежи ученикът.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolStudent', N'PersonId'                           , N'Идентификатор на ученик.'
