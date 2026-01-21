GO
PRINT 'Insert SysUser'
GO

SET IDENTITY_INSERT core.SysUser ON

insert core.SysUser ([SysUserID],[Username],[Password],[IsAzureUser],[PersonID])
select 1, 'migration', '', 0, 1 UNION ALL
select 3, 'schoolbooks', NULL, 0, 3 UNION ALL
select 4, 'SchoolBooksProvider', '', 0, 2 UNION ALL
select 5, 'ScheduleProvider', '', 0, 2 UNION ALL
select 1001, 'school@mon.bg', '', 0, 84364660 UNION ALL
select 1002, 'school2@mon.bg', '', 0, 84431053 UNION ALL
select 1003, 'teacher@mon.bg', '', 0, 84368694 UNION ALL
select 1004, 'teacher2@mon.bg', '', 0, 84613549 UNION ALL
-- select 1005, 'teacher3@mon.bg', '', 0, 84416511 UNION ALL
select 1006, 'parent1@mon.bg', '', 0, 83059455 UNION ALL -- TODO this is a randomly selected PersonId
select 1007, 'student1@mon.bg', '', 0, 83398005 UNION ALL
select 1008, 'teacher4@mon.bg', '', 0, 85753100 UNION ALL
select 1009, 'teacher5@mon.bg', '', 0, 85366174 UNION ALL
select 1010, 'teacher6@mon.bg', '', 0, 85774665 UNION ALL
select 1011, 'teacher7@mon.bg', '', 0, 84414911 UNION ALL
select 1012, 'mon@mon.bg', '', 0, 1012 UNION ALL
select 1014, 'dg@mon.bg', '', 0, 1014 UNION ALL
select 1015, 'csop@mon.bg', '', 0, 1015 UNION ALL
select 1016, 'rcpppo@mon.bg', '', 0, 1016 UNION ALL
select 1017, 'dg2@mon.bg', '', 0, 1017 UNION ALL
select 1018, 'school3@mon.bg', '', 0, 1018 UNION ALL
select 1019, 'cplr@mon.bg', '', 0, 1019;

SET IDENTITY_INSERT core.SysUser OFF
