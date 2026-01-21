GO
PRINT 'Insert [student].[Ministry]'
GO

set identity_insert [student].[Ministry] on;

insert [student].[Ministry] ([Id],[Name],[Description],[IsValid],[ValidFrom],[ValidTo],[NameEN],[NameDE],[NameFR])
select 1,'Министерство на образованието и науката',NULL,1,'2021-09-13 15:47:11.230',NULL,'Ministry of Education and Science','Ministerium für Bildung und Wissenschaft','Ministère de l''éducation et des sciences' UNION ALL
select 2,'Министерство на културата',NULL,1,'2021-09-13 15:47:11.230',NULL,'Ministry of Culture','Ministerium für Kultur','Ministère de la culture' UNION ALL
select 3,'Министерство на младежта и спорта',NULL,1,'2021-09-13 15:47:11.230',NULL,'Ministry of Youth and Sports','Ministerium für Jugend und Sport','Ministère de la jeunesse et des sports';

set identity_insert [student].[Ministry] off;

GO
