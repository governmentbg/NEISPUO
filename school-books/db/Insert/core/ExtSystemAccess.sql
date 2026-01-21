GO
PRINT 'Insert ExtSystemAccess'
GO

SET IDENTITY_INSERT core.ExtSystemAccess ON

insert core.ExtSystemAccess ([ExtSystemAccessID], [ExtSystemID], [ExtSystemType], [IsValid])
select 1, 1, 1, 1 UNION ALL
select 2, 2, 4, 1 UNION ALL
select 3, 3, 3, 1;

SET IDENTITY_INSERT core.ExtSystemAccess OFF
