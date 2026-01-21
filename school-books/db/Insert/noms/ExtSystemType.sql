GO
PRINT 'Insert [noms].[ExtSystemType]'
GO

insert [noms].[ExtSystemType] ([ExtSystemTypeID],[Name],[IsValid],[InstitutionCheck])
select 1,'Електронни дневници',1,1 UNION ALL
select 2,'Списък-образец',1,1 UNION ALL
select 3,'Външна интеграция',1,0 UNION ALL
select 4,'aSc - Седмично разписание',1,1 UNION ALL
select 5,'Оценяване на персонала',1,1 UNION ALL
select 6,'Модул Документация',1,1;

GO
