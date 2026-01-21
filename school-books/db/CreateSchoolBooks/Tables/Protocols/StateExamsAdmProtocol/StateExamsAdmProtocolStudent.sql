PRINT 'Create StateExamsAdmProtocolStudent table'
GO

CREATE TABLE [school_books].[StateExamsAdmProtocolStudent] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [StateExamsAdmProtocolId]           INT              NOT NULL,
    [ClassId]                           INT              NOT NULL,
    [PersonId]                          INT              NOT NULL,

    [HasFirstMandatorySubject]          BIT              NOT NULL,
    [SecondMandatorySubjectId]          INT              NULL,
    [SecondMandatorySubjectTypeId]      INT              NULL,

    CONSTRAINT [PK_StateExamsAdmProtocolStudent] PRIMARY KEY ([SchoolYear], [StateExamsAdmProtocolId], [ClassId], [PersonId]),
    CONSTRAINT [FK_StateExamsAdmProtocolStudent_StateExamsAdmProtocol] FOREIGN KEY ([SchoolYear], [StateExamsAdmProtocolId])
        REFERENCES [school_books].[StateExamsAdmProtocol] ([SchoolYear], [StateExamsAdmProtocolId]),

    -- external references
    CONSTRAINT [FK_StateExamsAdmProtocol_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_StateExamsAdmProtocol_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_StateExamsAdmProtocolStudent_Subject] FOREIGN KEY ([SecondMandatorySubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID]),
    CONSTRAINT [FK_StateExamsAdmProtocolStudent_SubjectType] FOREIGN KEY ([SecondMandatorySubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_StateExamsAdmProtocolStudent_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'StateExamsAdmProtocolStudent', N'Протокол за допускане до държавни изпити - ученик.'

exec school_books.spDescColumn N'StateExamsAdmProtocolStudent', N'SchoolYear'                             , N'Учебна година.'
exec school_books.spDescColumn N'StateExamsAdmProtocolStudent', N'StateExamsAdmProtocolId'                , N'Идентификатор на протокол за допускане до държавни изпити.'
exec school_books.spDescColumn N'StateExamsAdmProtocolStudent', N'ClassId'                                , N'Идентификатор на група/паралелка, към която принадлежи ученикът.'
exec school_books.spDescColumn N'StateExamsAdmProtocolStudent', N'PersonId'                               , N'Идентификатор на ученик.'

exec school_books.spDescColumn N'StateExamsAdmProtocolStudent', N'HasFirstMandatorySubject'               , N'БЕЛ – Да/Не.'
exec school_books.spDescColumn N'StateExamsAdmProtocolStudent', N'SecondMandatorySubjectId'               , N'Идентификатор на уч. предмет на втори задължителен изпит. Номенклатура inst_nom.Subject.'
exec school_books.spDescColumn N'StateExamsAdmProtocolStudent', N'SecondMandatorySubjectTypeId'           , N'Начин на изучаване на втори задължителен изпит. Номенклатура inst_nom.SubjectType.'
