GO
PRINT 'Insert [student].[CommuterType]'
GO

set identity_insert [student].[CommuterType] on;

insert [student].[CommuterType] ([Id],[Name],[Description],[IsValid],[ValidFrom],[ValidTo])
select 1,'непътуващ','непътуващ',1,'2021-01-27 18:14:10.863',NULL UNION ALL
select 2,'с друг междуселищен транспорт, без заплащане от училището/ДГ','с друг междуселищен транспорт, без заплащане от училището/ДГ',1,'2021-01-27 18:14:10.863',NULL UNION ALL
select 3,'с училищен автобус','с училищен автобус',1,'2021-01-27 18:14:10.863',NULL UNION ALL
select 4,'с обществен транспорт, заплатен от училището/ДГ','с обществен транспорт, заплатен от училището/ДГ',1,'2021-01-27 18:14:10.863',NULL UNION ALL
select 5,'пътуващ','пътуващ',0,'2021-08-30 00:00:00.000',NULL;

set identity_insert [student].[CommuterType] off;

GO
