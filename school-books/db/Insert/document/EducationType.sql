GO
PRINT 'Insert [document].[EducationType]'
GO

set identity_insert [document].[EducationType] on;

insert [document].[EducationType] ([Id],[Name],[Description],[IsValid],[ValidFrom],[ValidTo])
select 0,'-','няма',1,'2021-10-19 08:59:16.256',NULL UNION ALL
select 1,'средно образование','средно образование',1,'2021-10-19 08:59:16.256',NULL UNION ALL
select 2,'средно специално образование','средно специално образование',1,'2021-10-19 08:59:16.256',NULL UNION ALL
select 3,'начален етап','начален етап',1,'2021-10-19 08:59:16.256',NULL UNION ALL
select 4,'основна степен на образование','основна степен на образование',1,'2021-10-19 08:59:16.260',NULL UNION ALL
select 5,'първи гимназиален етап','първи гимназиален етап',1,'2021-10-19 08:59:16.260',NULL;

set identity_insert [document].[EducationType] off;

GO
