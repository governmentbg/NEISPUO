GO
PRINT 'Insert [noms].[BaseSchoolType]'
GO

insert [noms].[BaseSchoolType] ([BaseSchoolTypeID],[Name],[Description],[IsValid],[ValidFrom],[ValidTo])
select 1,'общообразователно',NULL,0,'2020-10-26 11:18:53.006',NULL UNION ALL
select 2,'професионално',NULL,0,'2020-10-26 11:18:53.006',NULL UNION ALL
select 3,'специално',NULL,0,'2020-10-26 11:18:53.006',NULL UNION ALL
select 4,'обслужващо звено',NULL,0,'2020-10-26 11:18:53.006',NULL UNION ALL
select 7,'по изкуствата',NULL,0,'2020-10-26 11:18:53.006',NULL UNION ALL
select 8,'ДОВДЛРГ',NULL,0,'2020-10-26 11:18:53.006',NULL UNION ALL
select 9,'детска градина',NULL,0,'2020-10-26 11:18:53.006',NULL UNION ALL
select 11,'специализирано ',NULL,1,'2020-10-26 11:18:53.006',NULL UNION ALL
select 12,'неспециализирано',NULL,1,'2020-10-26 11:18:53.023',NULL UNION ALL
select 13,'специално',NULL,1,'2020-10-26 11:18:53.023',NULL UNION ALL
select 14,'училище към местата за лишаване от свобода',NULL,1,'2020-10-26 11:18:53.023',NULL UNION ALL
select 15,'детска градина',NULL,1,'2020-10-26 11:18:53.023',NULL UNION ALL
select 16,'център за подкрепа за личностно развитие',NULL,1,'2020-10-26 11:18:53.023',NULL UNION ALL
select 17,'специализирано обслужващо звено',NULL,1,'2020-10-26 11:18:53.023',NULL UNION ALL
select 18,'училище, функциониращо по силата на международен договор',NULL,1,'2020-11-23 16:25:54.406',NULL;

GO
