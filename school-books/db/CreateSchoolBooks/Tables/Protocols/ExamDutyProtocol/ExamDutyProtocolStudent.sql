PRINT 'Create ExamDutyProtocolStudent table'
GO

CREATE TABLE [school_books].[ExamDutyProtocolStudent] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ExamDutyProtocolId]    INT              NOT NULL,
    [ClassId]               INT              NOT NULL,
    [PersonId]              INT              NOT NULL,

    CONSTRAINT [PK_ExamDutyProtocolStudent] PRIMARY KEY ([SchoolYear], [ExamDutyProtocolId], [ClassId], [PersonId]),
    CONSTRAINT [FK_ExamDutyProtocolStudent_ExamDutyProtocol] FOREIGN KEY ([SchoolYear], [ExamDutyProtocolId]) REFERENCES [school_books].[ExamDutyProtocol] ([SchoolYear], [ExamDutyProtocolId]),

    -- external references
    CONSTRAINT [FK_ExamDutyProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_ExamDutyProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_ExamDutyProtocolStudent_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'ExamDutyProtocolStudent', N'Протокол за дежурство при провеждане на изпит - ученик.'

exec school_books.spDescColumn N'ExamDutyProtocolStudent', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ExamDutyProtocolStudent', N'ExamDutyProtocolId'        , N'Идентификатор на протокол за дежурство при провеждане на изпит.'
exec school_books.spDescColumn N'ExamDutyProtocolStudent', N'ClassId'                   , N'Идентификатор на група/паралелка, към която принадлежи ученикът.'
exec school_books.spDescColumn N'ExamDutyProtocolStudent', N'PersonId'                  , N'Идентификатор на ученик.'
