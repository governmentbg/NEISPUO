PRINT 'Create StateExamDutyProtocolSupervisor table'
GO

CREATE TABLE [school_books].[StateExamDutyProtocolSupervisor] (
    [SchoolYear]              SMALLINT         NOT NULL,
    [StateExamDutyProtocolId] INT              NOT NULL,
    [PersonId]                INT              NOT NULL,

    CONSTRAINT [PK_StateExamDutyProtocolSupervisor] PRIMARY KEY ([SchoolYear], [StateExamDutyProtocolId], [PersonId]),
    CONSTRAINT [FK_StateExamDutyProtocolSupervisor_StateExamDutyProtocol] FOREIGN KEY ([SchoolYear], [StateExamDutyProtocolId]) REFERENCES [school_books].[StateExamDutyProtocol] ([SchoolYear], [StateExamDutyProtocolId]),

    -- external references
    CONSTRAINT [FK_StateExamDutyProtocolSupervisor_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO

exec school_books.spDescTable  N'StateExamDutyProtocolSupervisor', N'Протокол за дежурство при провеждане на писмен ДЗИ - квестор.'

exec school_books.spDescColumn N'StateExamDutyProtocolSupervisor', N'SchoolYear'                    , N'Учебна година.'
exec school_books.spDescColumn N'StateExamDutyProtocolSupervisor', N'StateExamDutyProtocolId'       , N'Идентификатор на протокол за дежурство при провеждане на писмен ДЗИ.'
exec school_books.spDescColumn N'StateExamDutyProtocolSupervisor', N'PersonId'                      , N'Идентификатор на квестор.'
