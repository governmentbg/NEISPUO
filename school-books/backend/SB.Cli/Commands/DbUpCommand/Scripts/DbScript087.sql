ALTER TABLE [school_books].[ExamDutyProtocol]
ADD
    [ProtocolNumber] NVARCHAR(100) NULL,
    [ProtocolDate] DATE NULL
GO

ALTER TABLE [school_books].[SkillsCheckExamResultProtocol]
ADD
    [ProtocolNumber] NVARCHAR(100) NULL
GO

ALTER TABLE [school_books].[SkillsCheckExamDutyProtocol]
ADD
    [ProtocolNumber] NVARCHAR(100) NULL,
    [ProtocolDate] DATE NULL
GO

ALTER TABLE [school_books].[NvoExamDutyProtocol]
ADD
    [ProtocolNumber] NVARCHAR(100) NULL,
    [ProtocolDate] DATE NULL
GO

ALTER TABLE [school_books].[StateExamDutyProtocol]
ADD
    [ProtocolNumber] NVARCHAR(100) NULL,
    [ProtocolDate] DATE NULL
GO
