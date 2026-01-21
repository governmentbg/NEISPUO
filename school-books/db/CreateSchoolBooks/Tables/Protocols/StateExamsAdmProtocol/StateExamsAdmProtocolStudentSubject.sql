PRINT 'Create StateExamsAdmProtocolStudentSubject table'
GO

CREATE TABLE [school_books].[StateExamsAdmProtocolStudentSubject] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [StateExamsAdmProtocolId]           INT              NOT NULL,
    [ClassId]                           INT              NOT NULL,
    [PersonId]                          INT              NOT NULL,
    [SubjectId]                         INT              NOT NULL,
    [SubjectTypeId]                     INT              NOT NULL,
    [IsAdditional]                      BIT              NOT NULL,

    CONSTRAINT [PK_StateExamsAdmProtocolStudentSubject] PRIMARY KEY ([SchoolYear], [StateExamsAdmProtocolId], [ClassId], [PersonId], [SubjectId], [SubjectTypeId], [IsAdditional]),
    CONSTRAINT [FK_StateExamsAdmProtocolStudentSubject_StateExamsAdmProtocolStudent] FOREIGN KEY ([SchoolYear], [StateExamsAdmProtocolId], [ClassId], [PersonId])
        REFERENCES [school_books].[StateExamsAdmProtocolStudent] ([SchoolYear], [StateExamsAdmProtocolId], [ClassId], [PersonId]),

    -- external references
    CONSTRAINT [FK_StateExamsAdmProtocolStudentSubject_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID]),
    CONSTRAINT [FK_StateExamsAdmProtocolStudentSubject_SubjectType] FOREIGN KEY ([SubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_StateExamsAdmProtocolStudentSubject_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'StateExamsAdmProtocolStudentSubject', N'Протокол за допускане до държавни изпити - допълнителни предмети.'

exec school_books.spDescColumn N'StateExamsAdmProtocolStudentSubject', N'SchoolYear'                         , N'Учебна година.'
exec school_books.spDescColumn N'StateExamsAdmProtocolStudentSubject', N'StateExamsAdmProtocolId'            , N'Идентификатор на протокол за допускане до изпити за промяна на оценката.'
exec school_books.spDescColumn N'StateExamsAdmProtocolStudentSubject', N'ClassId'                            , N'Идентификатор на група/паралелка, към която принадлежи ученикът.'
exec school_books.spDescColumn N'StateExamsAdmProtocolStudentSubject', N'PersonId'                           , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'StateExamsAdmProtocolStudentSubject', N'SubjectId'                          , N'Уч. предмет. Номенклатура inst_nom.Subject.'
exec school_books.spDescColumn N'StateExamsAdmProtocolStudentSubject', N'SubjectTypeId'                      , N'Начин на изучаване. Номенклатура inst_nom.SubjectType.'
exec school_books.spDescColumn N'StateExamsAdmProtocolStudentSubject', N'IsAdditional'                       , N'Допълнителен предмет за придобиване на проф. квалификация – Да/Не.'
