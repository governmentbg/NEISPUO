GO
PRINT 'Insert [student].[RepeaterReason]'
GO

set identity_insert [student].[RepeaterReason] on;

insert [student].[RepeaterReason] ([Id],[Name],[Description],[IsValid],[ValidFrom],[ValidTo])
select -1,'Не се посочва','Не се посочва',0,'2021-11-22 12:23:35.933',NULL UNION ALL
select 1,'не','не',1,'2021-01-27 18:14:10.880',NULL UNION ALL
select 2,'да, поради слаб успех','да, поради слаб успех',1,'2021-01-27 18:14:10.880',NULL UNION ALL
select 3,'да, повторно записване след отпадане','да, повторно записване след отпадане',1,'2021-01-27 18:14:10.880',NULL UNION ALL
select 4,'да, повторно записване след обучение в чужбина','да, повторно записване след обучение в чужбина',1,'2021-01-27 18:14:10.880',NULL UNION ALL
select 5,'да, поради здравословни причини','да, поради здравословни причини',1,'2021-01-27 18:14:10.880',NULL UNION ALL
select 6,'да, поради други причини','да, поради други причини',1,'2021-01-27 18:14:10.880',NULL;

set identity_insert [student].[RepeaterReason] off;

GO
