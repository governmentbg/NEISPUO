PRINT 'Create NvoExamDutyProtocolStudent table'
GO

CREATE TABLE [school_books].[NvoExamDutyProtocolStudent] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [NvoExamDutyProtocolId]     INT              NOT NULL,
    [ClassId]                   INT              NOT NULL,
    [PersonId]                  INT              NOT NULL,

    CONSTRAINT [PK_NvoExamDutyProtocolStudent] PRIMARY KEY ([SchoolYear], [NvoExamDutyProtocolId], [ClassId], [PersonId]),
    CONSTRAINT [FK_NvoExamDutyProtocolStudent_NvoExamDutyProtocol] FOREIGN KEY ([SchoolYear], [NvoExamDutyProtocolId]) REFERENCES [school_books].[NvoExamDutyProtocol] ([SchoolYear], [NvoExamDutyProtocolId]),

    -- external references
    CONSTRAINT [FK_NvoExamDutyProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_NvoExamDutyProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_NvoExamDutyProtocolStudent_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'NvoExamDutyProtocolStudent', N'Протокол за дежурство при провеждане на писмен изпит от НВО - ученик.'

exec school_books.spDescColumn N'NvoExamDutyProtocolStudent', N'SchoolYear'                         , N'Учебна година.'
exec school_books.spDescColumn N'NvoExamDutyProtocolStudent', N'NvoExamDutyProtocolId'              , N'Идентификатор на протокол за дежурство при провеждане на писмен изпит от НВО.'
exec school_books.spDescColumn N'NvoExamDutyProtocolStudent', N'ClassId'                            , N'Идентификатор на група/паралелка, към която принадлежи ученикът.'
exec school_books.spDescColumn N'NvoExamDutyProtocolStudent', N'PersonId'                           , N'Идентификатор на ученик.'
