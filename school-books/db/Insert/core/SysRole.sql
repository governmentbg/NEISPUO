GO
PRINT 'Insert [core].[SysRole]'
GO

set identity_insert [core].[SysRole] on;

insert [core].[SysRole] ([SysRoleID],[Name],[Description])
select 0,'Институция (директор)','Институция (директор)' UNION ALL
select 1,'МОН - Администратор','МОН - Администратор' UNION ALL
select 2,'РУО - Експерт ИО/АИ','РУО - Експерт ИО/АИ' UNION ALL
select 3,'Община','Община' UNION ALL
select 4,'Финансираща институция','Финансираща институция' UNION ALL
select 5,'Учител','Учител' UNION ALL
select 6,'Ученик','Ученик' UNION ALL
select 7,'Родител','Родител' UNION ALL
select 8,'Учител - класен ръководител','Учител - класен ръководител' UNION ALL
select 9,'РУО - Експерт','РУО - Експерт' UNION ALL
select 10,'ЦИОО','ЦИОО' UNION ALL
select 11,'Експерти от външни институции','Експерти от външни институции' UNION ALL
select 12,'МОН - експерт','МОН - експерт' UNION ALL
select 13,'МОН - Администратор потребители','МОН - Администратор потребители' UNION ALL
select 14,'Институция (IT Администратор)','Институция (IT Администратор)' UNION ALL
select 15,'МОН – д. ОБГУМ','МОН – д. ОБГУМ' UNION ALL
select 16,'МОН – д. ОБГУМи д. "Финанси"','МОН – д. ОБГУМи д. "Финанси"' UNION ALL
select 17,'МОН – д. ЧРАО','МОН – д. ЧРАО' UNION ALL
select 18,'Консорциум - Helpdesk','Консорциум - Helpdesk' UNION ALL
select 19,'НИО','НИО' UNION ALL
select 20,'Счетоводител','Счетоводител' UNION ALL
select 21,'Външни доставчици','Външни доставчици' UNION ALL
select 22,'Преподавател - ВУ','Преподавател - ВУ' UNION ALL
select 23,'Издател','Издател' UNION ALL
select 24,'Неделно училище','Неделно училище';

set identity_insert [core].[SysRole] off;

GO
