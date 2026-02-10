GO
PRINT 'Insert ExtSystem'
GO

SET IDENTITY_INSERT core.ExtSystem ON

insert core.ExtSystem ([ExtSystemID], [Name], [IsValid], [SysUserId])
select 1, 'SchoolBooksProvider', 1, 4 union all
select 2, 'ScheduleProvider', 1, 5 union all
select 3, 'HIS', 1, null;

SET IDENTITY_INSERT core.ExtSystem OFF
