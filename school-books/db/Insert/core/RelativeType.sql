GO
PRINT 'Insert [family].[RelativeType]'
GO

set identity_insert [family].[RelativeType] on;

insert [family].[RelativeType] ([RelativeTypeID],[Name],[Description])
select 1,'баща','баща' UNION ALL
select 2,'майка','майка' UNION ALL
select 3,'дядо','дядо' UNION ALL
select 4,'баба','баба' UNION ALL
select 5,'настойник','настойник' UNION ALL
select 6,'приемен родител','приемен родител' UNION ALL
select 7,'друг','друг';

set identity_insert [family].[RelativeType] off;

GO
