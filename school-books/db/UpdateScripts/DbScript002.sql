ALTER TABLE [school_books].[ExamResultProtocol]
DROP
    CONSTRAINT [FK_ExamResultProtocol_ProtocolExamType],
    CONSTRAINT [FK_ExamResultProtocol_ProtocolExamSubType];
GO

EXEC sp_rename 'school_books.ExamResultProtocol.ProtocolExamTypeId', 'TempProtocolExamSubTypeId', 'COLUMN';
EXEC sp_rename 'school_books.ExamResultProtocol.ProtocolExamSubTypeId', 'ProtocolExamTypeId', 'COLUMN';
EXEC sp_rename 'school_books.ExamResultProtocol.TempProtocolExamSubTypeId', 'ProtocolExamSubTypeId', 'COLUMN';
GO

ALTER TABLE [school_books].[ExamResultProtocol] ADD CONSTRAINT [FK_ExamResultProtocol_ProtocolExamType]
    FOREIGN KEY ([ProtocolExamTypeId]) REFERENCES [school_books].[ProtocolExamType] ([Id])
GO

ALTER TABLE [school_books].[ExamResultProtocol] ADD CONSTRAINT [FK_ExamResultProtocol_ProtocolExamSubType]
    FOREIGN KEY ([ProtocolExamSubTypeId]) REFERENCES [school_books].[ProtocolExamSubType] ([Id])
GO
