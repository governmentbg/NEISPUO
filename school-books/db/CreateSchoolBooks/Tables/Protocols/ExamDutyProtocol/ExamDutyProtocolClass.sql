PRINT 'Create ExamDutyProtocolClass table'
GO

CREATE TABLE [school_books].[ExamDutyProtocolClass] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ExamDutyProtocolId]    INT              NOT NULL,
    [ClassId]               INT              NOT NULL,

    CONSTRAINT [PK_ExamDutyProtocolClass] PRIMARY KEY ([SchoolYear], [ExamDutyProtocolId], [ClassId]),
    CONSTRAINT [FK_ExamDutyProtocolClass_ExamDutyProtocol] FOREIGN KEY ([SchoolYear], [ExamDutyProtocolId]) REFERENCES [school_books].[ExamDutyProtocol] ([SchoolYear], [ExamDutyProtocolId]),

    -- external references
    CONSTRAINT [FK_ExamDutyProtocolClass_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_ExamDutyProtocolClass_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'ExamDutyProtocolClass', N'Протокол за дежурство при провеждане на изпит - клас.'

exec school_books.spDescColumn N'ExamDutyProtocolClass', N'SchoolYear'                     , N'Учебна година.'
exec school_books.spDescColumn N'ExamDutyProtocolClass', N'ExamDutyProtocolId'             , N'Идентификатор на протокол за дежурство при провеждане на изпит.'
exec school_books.spDescColumn N'ExamDutyProtocolClass', N'ClassId'                        , N'Идентификатор на група/паралелка.'

