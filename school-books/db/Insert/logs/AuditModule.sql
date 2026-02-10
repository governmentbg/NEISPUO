PRINT 'Insert AuditModule'
GO

INSERT INTO [logs].[AuditModule]
    ([AuditModuleId], [Name])
VALUES
    (301, 'Документи за дейността'),
    (302, 'Документи за дейността - външни доставчици');

GO
