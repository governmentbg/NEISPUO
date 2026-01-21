PRINT 'Create GradeChangeExamsAdmProtocolStudentSubject table'
GO

CREATE TABLE [school_books].[GradeChangeExamsAdmProtocolStudentSubject] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [GradeChangeExamsAdmProtocolId]     INT              NOT NULL,
    [ClassId]                           INT              NOT NULL,
    [PersonId]                          INT              NOT NULL,
    [SubjectId]                         INT              NOT NULL,
    [SubjectTypeId]                     INT              NOT NULL,

    CONSTRAINT [PK_GradeChangeExamsAdmProtocolStudentSubject] PRIMARY KEY ([SchoolYear], [GradeChangeExamsAdmProtocolId], [ClassId], [PersonId], [SubjectId], [SubjectTypeId]),
    CONSTRAINT [FK_GradeChangeExamsAdmProtocolStudentSubject_AdmProtocolStudent] FOREIGN KEY ([SchoolYear], [GradeChangeExamsAdmProtocolId], [ClassId], [PersonId])
        REFERENCES [school_books].[GradeChangeExamsAdmProtocolStudent] ([SchoolYear], [GradeChangeExamsAdmProtocolId], [ClassId], [PersonId]),

    -- external references
    CONSTRAINT [FK_GradeChangeExamsAdmProtocolStudentSubject_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID]),
    CONSTRAINT [FK_GradeChangeExamsAdmProtocolStudentSubject_SubjectType] FOREIGN KEY ([SubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_GradeChangeExamsAdmProtocolStudentSubject_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'GradeChangeExamsAdmProtocolStudentSubject', N'Протокол за допускане до изпити за промяна на оценката - заявен изпит по предмет.'

exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolStudentSubject', N'SchoolYear'                         , N'Учебна година.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolStudentSubject', N'GradeChangeExamsAdmProtocolId'      , N'Идентификатор на протокол за допускане до изпити за промяна на оценката.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolStudentSubject', N'ClassId'                            , N'Идентификатор на група/паралелка, към която принадлежи ученикът.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolStudentSubject', N'PersonId'                           , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolStudentSubject', N'SubjectId'                          , N'Уч. предмет. Номенклатура inst_nom.Subject.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocolStudentSubject', N'SubjectTypeId'                      , N'Начин на изучаване. Номенклатура inst_nom.SubjectType.'
