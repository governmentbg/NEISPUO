GO
PRINT 'Insert [inst_nom].[BasicSubjectType]'
GO

insert [inst_nom].[BasicSubjectType] ([BasicSubjectTypeID],[Name],[Abrev],[IsValid])
select 1,'Задължителна подготовка','ЗП',1 UNION ALL
select 2,'Задължителноизбираема подготовка','ЗИП',1 UNION ALL
select 3,'Свободноизбираема подготовка','СИП',1 UNION ALL
select 4,'Възпитателни дейности','ВД',0 UNION ALL
select 5,'Часове извън учебния план','...',1 UNION ALL
select 11,'Задължителни учебни часове','ЗУЧ',1 UNION ALL
select 12,'Избираеми учебни часове','ИУЧ',1 UNION ALL
select 13,'Факултативни учебни часове','ФУЧ',1 UNION ALL
select 14,'Целодневна организация/ Личностно развитие','ЦДО/ ПЛР',1 UNION ALL
select 15,'Специални предмети (за спец.училища със СУ)','СУП',1;

GO
