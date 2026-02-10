GO
PRINT 'Insert [noms].[BudgetingInstitution]'
GO

insert [noms].[BudgetingInstitution] ([BudgetingInstitutionID],[Name],[Description],[IsValid],[ValidFrom],[ValidTo])
select 1,'Министерство на образованието и науката',NULL,1,'2020-10-26 11:19:03.570',NULL UNION ALL
select 2,'Министерство на културата',NULL,1,'2020-10-26 11:19:03.570',NULL UNION ALL
select 3,'Министерство на земеделието и храните',NULL,0,'2020-10-26 11:19:03.570',NULL UNION ALL
select 4,'Министерство на младежта и спорта',NULL,1,'2020-10-26 11:19:03.570',NULL UNION ALL
select 5,'община',NULL,1,'2020-10-26 11:19:03.570',NULL UNION ALL
select 6,'частно финансиране',NULL,1,'2020-10-26 11:19:03.570',NULL UNION ALL
select 7,'Министерство на правосъдието',NULL,1,'2020-10-26 11:19:03.570',NULL UNION ALL
select 8,'Министерство на отбраната',NULL,1,'2020-10-26 11:19:03.570',NULL UNION ALL
select 11,'Технически университет',NULL,1,'2020-10-26 11:19:03.570',NULL UNION ALL
select 12,'МОН',NULL,0,'2020-10-26 11:19:03.570',NULL UNION ALL
select 13,'Религиозна институция',NULL,1,'2020-10-26 11:19:03.570',NULL UNION ALL
select 14,'по международен договор',NULL,1,'2020-11-23 16:29:28.953',NULL;

GO
