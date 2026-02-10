GO
PRINT 'Insert [noms].[FinancialSchoolType]'
GO

insert [noms].[FinancialSchoolType] ([FinancialSchoolTypeID],[Name],[Description],[IsValid],[ValidFrom],[ValidTo])
select 1,'Държавно',NULL,1,'2020-10-26 11:18:59.803',NULL UNION ALL
select 2,'Общинско',NULL,1,'2020-10-26 11:18:59.803',NULL UNION ALL
select 3,'Частно',NULL,1,'2020-10-26 11:18:59.803',NULL UNION ALL
select 4,'Държавно с чуждестр. участие',NULL,0,'2020-10-26 11:18:59.803',NULL UNION ALL
select 5,'Чуждестранно',NULL,0,'2020-10-26 11:18:59.803',NULL UNION ALL
select 11,'Духовно',NULL,1,'2020-10-26 11:18:59.803',NULL UNION ALL
select 12,'По силата на международен договор',NULL,1,'2020-11-23 16:29:11.510',NULL;

GO
