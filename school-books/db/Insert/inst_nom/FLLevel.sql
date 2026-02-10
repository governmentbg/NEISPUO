GO
PRINT 'Insert [inst_nom].[FLLevel]'
GO

insert [inst_nom].[FLLevel] ([FLLevelID],[Name],[Description],[IsValid],[SortOrd],[ValidFrom],[ValidTo])
select 1,'A1',NULL,1,1,'2020-11-05 00:00:00.000',NULL UNION ALL
select 2,'A2',NULL,1,2,'2020-11-05 00:00:00.000',NULL UNION ALL
select 3,'B1',NULL,1,3,'2020-11-05 00:00:00.000',NULL UNION ALL
select 4,'B2',NULL,1,6,'2020-11-05 00:00:00.000',NULL UNION ALL
select 5,'C1',NULL,1,7,'2020-11-05 00:00:00.000',NULL UNION ALL
select 6,'C2',NULL,1,8,'2020-11-05 00:00:00.000',NULL UNION ALL
select 7,'В1.1',NULL,1,4,'2020-11-05 00:00:00.000',NULL UNION ALL
select 8,'B2.1',NULL,1,5,'2022-06-02 00:00:00.000',NULL;

GO
