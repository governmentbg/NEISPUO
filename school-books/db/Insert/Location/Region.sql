GO
PRINT 'Insert Region';
GO

insert [location].[Region] ([RegionID],[Name],[Code],[Description])
select 1,'Благоевград','BLG','' UNION ALL
select 2,'Бургас','BGS','' UNION ALL
select 3,'Варна','VAR','' UNION ALL
select 4,'Велико Търново','VTR','' UNION ALL
select 5,'Видин','VID','' UNION ALL
select 6,'Враца','VRC','' UNION ALL
select 7,'Габрово','GAB','' UNION ALL
select 8,'Добрич','DOB','' UNION ALL
select 9,'Кърджали','KRZ','' UNION ALL
select 10,'Кюстендил','KNL','' UNION ALL
select 11,'Ловеч','LOV','' UNION ALL
select 12,'Монтана','MON','' UNION ALL
select 13,'Пазарджик','PAZ','' UNION ALL
select 14,'Перник','PER','' UNION ALL
select 15,'Плевен','PVN','' UNION ALL
select 16,'Пловдив','PDV','' UNION ALL
select 17,'Разград','RAZ','' UNION ALL
select 18,'Русе','RSE','' UNION ALL
select 19,'Силистра','SLS','' UNION ALL
select 20,'Сливен','SLV','' UNION ALL
select 21,'Смолян','SML','' UNION ALL
select 22,'София-град','SOF','' UNION ALL
select 23,'София-област','SFO','' UNION ALL
select 24,'Стара Загора','SZR','' UNION ALL
select 25,'Търговище','TGV','' UNION ALL
select 26,'Хасково','HKV','' UNION ALL
select 27,'Шумен','SHU','' UNION ALL
select 28,'Ямбол','JAM','' UNION ALL
select 29,'други','FRN','';