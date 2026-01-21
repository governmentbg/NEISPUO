GO
PRINT 'Insert [noms].[Gender]'
GO

insert [noms].[Gender] ([GenderID],[Name],[Description],[IsValid],[ValidFrom],[ValidTo])
select 1,'жена',NULL,1,'2020-10-26 11:15:17.380',NULL UNION ALL
select 2,'мъж',NULL,1,'2020-10-26 11:15:17.380',NULL;

GO
