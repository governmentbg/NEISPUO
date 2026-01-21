GO
PRINT 'Insert [document].[ITLevel]'
GO

set identity_insert [document].[ITLevel] on;

insert [document].[ITLevel] ([Id],[Name],[Description],[IsValid],[ValidFrom],[ValidTo])
select 0,'0','няма',1,'2021-10-19 08:59:08.583',NULL UNION ALL
select 1,'1','1. Основно',1,'2021-10-19 08:59:08.586',NULL UNION ALL
select 2,'2','2. Основно',1,'2021-10-19 08:59:08.586',NULL UNION ALL
select 3,'3','3. Средно',1,'2021-10-19 08:59:08.586',NULL UNION ALL
select 4,'4','самостоятелно ниво на владеене',1,'2021-10-19 08:59:08.586',NULL UNION ALL
select 5,'5','5. Напреднали',1,'2021-10-19 08:59:08.586',NULL UNION ALL
select 6,'6','6. Напреднали',1,'2021-10-19 08:59:08.590',NULL UNION ALL
select 7,'7','7. Високо специализирано',1,'2021-10-19 08:59:08.590',NULL UNION ALL
select 8,'8','8. Високо специализирано',1,'2021-10-19 08:59:08.590',NULL;

set identity_insert [document].[ITLevel] off;

GO
