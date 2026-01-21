PRINT 'Create NvoExamDutyProtocolSupervisor table'
GO

CREATE TABLE [school_books].[NvoExamDutyProtocolSupervisor] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [NvoExamDutyProtocolId] INT              NOT NULL,
    [PersonId]              INT              NOT NULL,

    CONSTRAINT [PK_NvoExamDutyProtocolSupervisor] PRIMARY KEY ([SchoolYear], [NvoExamDutyProtocolId], [PersonId]),
    CONSTRAINT [FK_NvoExamDutyProtocolSupervisor_NvoExamDutyProtocol] FOREIGN KEY ([SchoolYear], [NvoExamDutyProtocolId]) REFERENCES [school_books].[NvoExamDutyProtocol] ([SchoolYear], [NvoExamDutyProtocolId]),

    -- external references
    CONSTRAINT [FK_NvoExamDutyProtocolSupervisor_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO

exec school_books.spDescTable  N'NvoExamDutyProtocolSupervisor', N'Протокол за дежурство при провеждане на писмен изпит от НВО - квестор.'

exec school_books.spDescColumn N'NvoExamDutyProtocolSupervisor', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'NvoExamDutyProtocolSupervisor', N'NvoExamDutyProtocolId'     , N'Идентификатор на протокол за дежурство при провеждане на писмен изпит от НВО.'
exec school_books.spDescColumn N'NvoExamDutyProtocolSupervisor', N'PersonId'                  , N'Идентификатор на квестор.'
