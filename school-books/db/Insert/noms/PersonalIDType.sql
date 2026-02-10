GO
PRINT 'Insert [noms].[PersonalIDType]'
GO

insert [noms].[PersonalIDType] ([PersonalIDTypeID],[Name],[Description],[IsValid],[ValidFrom],[ValidTo])
select -1,'Без идентификатор','Без идентификатор',1,'2021-03-16 14:28:22.763',NULL UNION ALL
select 0,'ЕГН',NULL,1,'2020-10-26 11:15:40.163',NULL UNION ALL
select 1,'ЛНЧ',NULL,1,'2020-10-26 11:15:40.163',NULL UNION ALL
select 2,'ИДН',NULL,1,'2020-10-26 11:15:40.163',NULL;

GO
