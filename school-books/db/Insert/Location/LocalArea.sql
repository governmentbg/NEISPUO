GO
PRINT 'Insert LocalArea';
GO

insert [location].[LocalArea] ([LocalAreaID],[Name],[TownCode])
select 1,'Аспарухово',10135 UNION ALL
select 2,'Владислав Варненчик',10135 UNION ALL
select 3,'Младост',10135 UNION ALL
select 4,'Одесос',10135 UNION ALL
select 5,'Приморски',10135 UNION ALL
select 11,'Западен',56784 UNION ALL
select 12,'Източен',56784 UNION ALL
select 13,'Северен',56784 UNION ALL
select 14,'Тракия',56784 UNION ALL
select 15,'Централен',56784 UNION ALL
select 16,'Южен',56784 UNION ALL
select 21,'Средец',68134 UNION ALL
select 22,'Красно село',68134 UNION ALL
select 23,'Възраждане',68134 UNION ALL
select 24,'Оборище',68134 UNION ALL
select 25,'Сердика',68134 UNION ALL
select 26,'Подуяне',68134 UNION ALL
select 27,'Слатина',68134 UNION ALL
select 28,'Изгрев',68134 UNION ALL
select 29,'Лозенец',68134 UNION ALL
select 30,'Триадица',68134 UNION ALL
select 31,'Красна поляна',68134 UNION ALL
select 32,'Илинден',68134 UNION ALL
select 33,'Надежда',68134 UNION ALL
select 34,'Искър',68134 UNION ALL
select 35,'Младост',68134 UNION ALL
select 36,'Студентски',68134 UNION ALL
select 37,'Витоша',68134 UNION ALL
select 38,'Овча Купел',68134 UNION ALL
select 39,'Люлин',68134 UNION ALL
select 40,'Връбница',68134 UNION ALL
select 41,'Нови Искър',68134 UNION ALL
select 42,'Кремиковци',68134 UNION ALL
select 43,'Панчарево',68134 UNION ALL
select 44,'Банкя',68134;