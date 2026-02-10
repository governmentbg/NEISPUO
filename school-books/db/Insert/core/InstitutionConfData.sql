GO
PRINT 'Insert [core].[InstitutionConfData]'
GO

set identity_insert [core].[InstitutionConfData] on;

insert [core].[InstitutionConfData] ([InstitutionConfDataID],[InstitutionID],[SchoolYear],[SOVersion],[CBVersion],[SysUserID],[SOExtProviderID],[CBExtProviderID])
select 3,100006,2025,7,7,1,NULL,NULL UNION ALL
select 286,200277,2025,251,251,1,NULL,NULL UNION ALL
select 462,300110,2025,5,5,1,NULL,NULL UNION ALL
select 474,300125,2025,45,45,1,NULL,NULL UNION ALL
select 990,607055,2025,44,44,1,NULL,NULL UNION ALL
select 2344,1690180,2025,4,4,1,NULL,NULL UNION ALL
select 2794,2000218,2025,4,4,1,NULL,NULL UNION ALL
select 3141,2206409,2025,4,4,1,NULL,NULL;

set identity_insert [core].[InstitutionConfData] off;

GO
