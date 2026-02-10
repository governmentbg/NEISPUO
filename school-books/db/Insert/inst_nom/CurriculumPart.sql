GO
PRINT 'Insert [inst_nom].[CurriculumPart]'
GO

insert [inst_nom].[CurriculumPart] ([CurriculumPartID],[Name],[Description],[IsValid],[ValidFrom],[ValidTo])
select 1,'ЗУЧ','Раздел А на учебния план',1,'2021-03-02 00:00:00.000',NULL UNION ALL
select 2,'ИУЧ','Раздел Б на учебния план',1,'2021-03-02 00:00:00.000',NULL UNION ALL
select 3,'ФУЧ','Раздел В на учебния план',1,'2021-03-02 00:00:00.000',NULL UNION ALL
select 4,'Специални предмети','Раздел Г на учебния план',1,'2021-03-02 00:00:00.000',NULL UNION ALL
select 5,'Часове извън учебния план','Часове извън учебния план',1,'2021-03-02 00:00:00.000',NULL UNION ALL
select 6,'Дейности в целодневна организация','Часове извън учебния план',1,'2021-03-02 00:00:00.000',NULL UNION ALL
select 7,'Дейности за ПЛР','Дейности за ПЛР',1,'2021-03-02 00:00:00.000',NULL;

GO
