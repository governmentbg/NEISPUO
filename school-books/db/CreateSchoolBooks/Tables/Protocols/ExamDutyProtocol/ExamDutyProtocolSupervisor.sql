PRINT 'Create ExamDutyProtocolSupervisor table'
GO

CREATE TABLE [school_books].[ExamDutyProtocolSupervisor] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ExamDutyProtocolId]    INT              NOT NULL,
    [PersonId]              INT              NOT NULL,

    CONSTRAINT [PK_ExamDutyProtocolSupervisor] PRIMARY KEY ([SchoolYear], [ExamDutyProtocolId], [PersonId]),
    CONSTRAINT [FK_ExamDutyProtocolSupervisor_ExamDutyProtocol] FOREIGN KEY ([SchoolYear], [ExamDutyProtocolId]) REFERENCES [school_books].[ExamDutyProtocol] ([SchoolYear], [ExamDutyProtocolId]),

    CONSTRAINT [FK_ExamDutyProtocolSupervisor_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO

exec school_books.spDescTable  N'ExamDutyProtocolSupervisor', N'Протокол за дежурство при провеждане на изпит - квестор.'

exec school_books.spDescColumn N'ExamDutyProtocolSupervisor', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ExamDutyProtocolSupervisor', N'ExamDutyProtocolId'        , N'Идентификатор на протокол за дежурство при провеждане на изпит.'
exec school_books.spDescColumn N'ExamDutyProtocolSupervisor', N'PersonId'                  , N'Идентификатор на квестор.'
