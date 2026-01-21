GO
PRINT 'Insert Municipality';
GO

insert [location].[Municipality] ([MunicipalityID],[Name],[Code],[RegionID],[Description])
select 1,'Банско','BLG01',1,'' UNION ALL
select 2,'Белица','BLG02',1,'' UNION ALL
select 3,'Благоевград','BLG03',1,'' UNION ALL
select 4,'Гоце Делчев','BLG11',1,'' UNION ALL
select 5,'Гърмен','BLG13',1,'' UNION ALL
select 6,'Кресна','BLG28',1,'' UNION ALL
select 7,'Петрич','BLG33',1,'' UNION ALL
select 8,'Разлог','BLG37',1,'' UNION ALL
select 9,'Сандански','BLG40',1,'' UNION ALL
select 10,'Сатовча','BLG42',1,'' UNION ALL
select 11,'Симитли','BLG44',1,'' UNION ALL
select 12,'Струмяни','BLG49',1,'' UNION ALL
select 13,'Хаджидимово','BLG52',1,'' UNION ALL
select 14,'Якоруда','BLG53',1,'' UNION ALL
select 15,'Айтос','BGS01',2,'' UNION ALL
select 16,'Бургас','BGS04',2,'' UNION ALL
select 17,'Камено','BGS08',2,'' UNION ALL
select 18,'Карнобат','BGS09',2,'' UNION ALL
select 19,'Малко Търново','BGS12',2,'' UNION ALL
select 20,'Несебър','BGS15',2,'' UNION ALL
select 21,'Поморие','BGS17',2,'' UNION ALL
select 22,'Приморско','BGS27',2,'' UNION ALL
select 23,'Руен','BGS18',2,'' UNION ALL
select 24,'Созопол','BGS21',2,'' UNION ALL
select 25,'Средец','BGS06',2,'' UNION ALL
select 26,'Сунгурларе','BGS23',2,'' UNION ALL
select 27,'Царево','BGS13',2,'' UNION ALL
select 28,'Аврен','VAR01',3,'' UNION ALL
select 29,'Аксаково','VAR02',3,'' UNION ALL
select 30,'Белослав','VAR04',3,'' UNION ALL
select 31,'Бяла','VAR05',3,'' UNION ALL
select 32,'Варна','VAR06',3,'' UNION ALL
select 33,'Ветрино','VAR08',3,'' UNION ALL
select 34,'Вълчи дол','VAR09',3,'' UNION ALL
select 35,'Девня','VAR14',3,'' UNION ALL
select 36,'Долни чифлик','VAR13',3,'' UNION ALL
select 37,'Дългопол','VAR16',3,'' UNION ALL
select 38,'Провадия','VAR24',3,'' UNION ALL
select 39,'Суворово','VAR26',3,'' UNION ALL
select 40,'Велико Търново','VTR04',4,'' UNION ALL
select 41,'Горна Оряховица','VTR06',4,'' UNION ALL
select 42,'Елена','VTR13',4,'' UNION ALL
select 43,'Златарица','VTR14',4,'' UNION ALL
select 44,'Лясковец','VTR20',4,'' UNION ALL
select 45,'Павликени','VTR22',4,'' UNION ALL
select 46,'Полски Тръмбеш','VTR26',4,'' UNION ALL
select 47,'Свищов','VTR28',4,'' UNION ALL
select 48,'Стражица','VTR31',4,'' UNION ALL
select 49,'Сухиндол','VTR32',4,'' UNION ALL
select 50,'Белоградчик','VID01',5,'' UNION ALL
select 51,'Бойница','VID03',5,'' UNION ALL
select 52,'Брегово','VID06',5,'' UNION ALL
select 53,'Видин','VID09',5,'' UNION ALL
select 54,'Грамада','VID15',5,'' UNION ALL
select 55,'Димово','VID16',5,'' UNION ALL
select 56,'Кула','VID22',5,'' UNION ALL
select 57,'Макреш','VID25',5,'' UNION ALL
select 58,'Ново село','VID30',5,'' UNION ALL
select 59,'Ружинци','VID33',5,'' UNION ALL
select 60,'Чупрене','VID37',5,'' UNION ALL
select 61,'Борован','VRC05',6,'' UNION ALL
select 62,'Бяла Слатина','VRC08',6,'' UNION ALL
select 63,'Враца','VRC10',6,'' UNION ALL
select 64,'Козлодуй','VRC20',6,'' UNION ALL
select 65,'Криводол','VRC21',6,'' UNION ALL
select 66,'Мездра','VRC27',6,'' UNION ALL
select 67,'Мизия','VRC28',6,'' UNION ALL
select 68,'Оряхово','VRC31',6,'' UNION ALL
select 69,'Роман','VRC32',6,'' UNION ALL
select 70,'Хайредин','VRC35',6,'' UNION ALL
select 71,'Габрово','GAB05',7,'' UNION ALL
select 72,'Дряново','GAB12',7,'' UNION ALL
select 73,'Севлиево','GAB29',7,'' UNION ALL
select 74,'Трявна','GAB35',7,'' UNION ALL
select 75,'Балчик','DOB03',8,'' UNION ALL
select 76,'Генерал Тошево','DOB12',8,'' UNION ALL
select 77,'Добрич','DOB28',8,'' UNION ALL
select 78,'Добричка','DOB15',8,'' UNION ALL
select 79,'Каварна','DOB17',8,'' UNION ALL
select 80,'Крушари','DOB20',8,'' UNION ALL
select 81,'Тервел','DOB27',8,'' UNION ALL
select 82,'Шабла','DOB29',8,'' UNION ALL
select 83,'Ардино','KRZ02',9,'' UNION ALL
select 84,'Джебел','KRZ08',9,'' UNION ALL
select 85,'Кирково','KRZ14',9,'' UNION ALL
select 86,'Крумовград','KRZ15',9,'' UNION ALL
select 87,'Кърджали','KRZ16',9,'' UNION ALL
select 88,'Момчилград','KRZ21',9,'' UNION ALL
select 89,'Черноочене','KRZ35',9,'' UNION ALL
select 90,'Бобов дол','KNL04',10,'' UNION ALL
select 91,'Бобошево','KNL05',10,'' UNION ALL
select 92,'Дупница','KNL48',10,'' UNION ALL
select 93,'Кочериново','KNL27',10,'' UNION ALL
select 94,'Кюстендил','KNL29',10,'' UNION ALL
select 95,'Невестино','KNL31',10,'' UNION ALL
select 96,'Рила','KNL38',10,'' UNION ALL
select 97,'Сапарева баня','KNL41',10,'' UNION ALL
select 98,'Трекляно','KNL50',10,'' UNION ALL
select 99,'Априлци','LOV02',11,'' UNION ALL
select 100,'Летница','LOV17',11,'' UNION ALL
select 101,'Ловеч','LOV18',11,'' UNION ALL
select 102,'Луковит','LOV19',11,'' UNION ALL
select 103,'Тетевен','LOV33',11,'' UNION ALL
select 104,'Троян','LOV34',11,'' UNION ALL
select 105,'Угърчин','LOV36',11,'' UNION ALL
select 106,'Ябланица','LOV38',11,'' UNION ALL
select 107,'Берковица','MON02',12,'' UNION ALL
select 108,'Бойчиновци','MON04',12,'' UNION ALL
select 109,'Брусарци','MON07',12,'' UNION ALL
select 110,'Вълчедръм','MON11',12,'' UNION ALL
select 111,'Вършец','MON12',12,'' UNION ALL
select 112,'Георги Дамяново','MON14',12,'' UNION ALL
select 113,'Лом','MON24',12,'' UNION ALL
select 114,'Медковец','MON26',12,'' UNION ALL
select 115,'Монтана','MON29',12,'' UNION ALL
select 116,'Чипровци','MON36',12,'' UNION ALL
select 117,'Якимово','MON38',12,'' UNION ALL
select 118,'Батак','PAZ03',13,'' UNION ALL
select 119,'Белово','PAZ04',13,'' UNION ALL
select 120,'Брацигово','PAZ06',13,'' UNION ALL
select 121,'Велинград','PAZ08',13,'' UNION ALL
select 122,'Лесичово','PAZ14',13,'' UNION ALL
select 123,'Пазарджик','PAZ19',13,'' UNION ALL
select 124,'Панагюрище','PAZ20',13,'' UNION ALL
select 125,'Пещера','PAZ21',13,'' UNION ALL
select 126,'Ракитово','PAZ24',13,'' UNION ALL
select 127,'Септември','PAZ29',13,'' UNION ALL
select 128,'Стрелча','PAZ32',13,'' UNION ALL
select 129,'Брезник','PER08',14,'' UNION ALL
select 130,'Земен','PER19',14,'' UNION ALL
select 131,'Ковачевци','PER22',14,'' UNION ALL
select 132,'Перник','PER32',14,'' UNION ALL
select 133,'Радомир','PER36',14,'' UNION ALL
select 134,'Трън','PER51',14,'' UNION ALL
select 135,'Белене','PVN03',15,'' UNION ALL
select 136,'Гулянци','PVN08',15,'' UNION ALL
select 137,'Долна Митрополия','PVN10',15,'' UNION ALL
select 138,'Долни Дъбник','PVN11',15,'' UNION ALL
select 139,'Искър','PVN23',15,'' UNION ALL
select 140,'Кнежа','PVN39',15,'' UNION ALL
select 141,'Левски','PVN16',15,'' UNION ALL
select 142,'Никопол','PVN21',15,'' UNION ALL
select 143,'Плевен','PVN24',15,'' UNION ALL
select 144,'Пордим','PVN27',15,'' UNION ALL
select 145,'Червен бряг','PVN37',15,'' UNION ALL
select 146,'Асеновград','PDV01',16,'' UNION ALL
select 147,'Брезово','PDV07',16,'' UNION ALL
select 148,'Калояново','PDV12',16,'' UNION ALL
select 149,'Карлово','PDV13',16,'' UNION ALL
select 150,'Кричим','PDV39',16,'' UNION ALL
select 151,'Куклен','PDV42',16,'' UNION ALL
select 152,'Лъки','PDV15',16,'' UNION ALL
select 153,'Марица','PDV17',16,'' UNION ALL
select 154,'Перущица','PDV40',16,'' UNION ALL
select 155,'Пловдив','PDV22',16,'' UNION ALL
select 156,'Първомай','PDV23',16,'' UNION ALL
select 157,'Раковски','PDV25',16,'' UNION ALL
select 158,'Родопи','PDV26',16,'' UNION ALL
select 159,'Садово','PDV28',16,'' UNION ALL
select 160,'Сопот','PDV43',16,'' UNION ALL
select 161,'Стамболийски','PDV41',16,'' UNION ALL
select 162,'Съединение','PDV33',16,'' UNION ALL
select 163,'Хисаря','PDV37',16,'' UNION ALL
select 164,'Завет','RAZ11',17,'' UNION ALL
select 165,'Исперих','RAZ14',17,'' UNION ALL
select 166,'Кубрат','RAZ16',17,'' UNION ALL
select 167,'Лозница','RAZ17',17,'' UNION ALL
select 168,'Разград','RAZ26',17,'' UNION ALL
select 169,'Самуил','RAZ29',17,'' UNION ALL
select 170,'Цар Калоян','RAZ36',17,'' UNION ALL
select 171,'Борово','RSE03',18,'' UNION ALL
select 172,'Бяла','RSE04',18,'' UNION ALL
select 173,'Ветово','RSE05',18,'' UNION ALL
select 174,'Две могили','RSE08',18,'' UNION ALL
select 175,'Иваново','RSE13',18,'' UNION ALL
select 176,'Русе','RSE27',18,'' UNION ALL
select 177,'Сливо поле','RSE33',18,'' UNION ALL
select 178,'Ценово','RSE37',18,'' UNION ALL
select 179,'Алфатар','SLS01',19,'' UNION ALL
select 180,'Главиница','SLS07',19,'' UNION ALL
select 181,'Дулово','SLS10',19,'' UNION ALL
select 182,'Кайнарджа','SLS15',19,'' UNION ALL
select 183,'Силистра','SLS31',19,'' UNION ALL
select 184,'Ситово','SLS32',19,'' UNION ALL
select 185,'Тутракан','SLS34',19,'' UNION ALL
select 186,'Котел','SLV11',20,'' UNION ALL
select 187,'Нова Загора','SLV16',20,'' UNION ALL
select 188,'Сливен','SLV20',20,'' UNION ALL
select 189,'Твърдица','SLV24',20,'' UNION ALL
select 190,'Баните','SML02',21,'' UNION ALL
select 191,'Борино','SML05',21,'' UNION ALL
select 192,'Девин','SML09',21,'' UNION ALL
select 193,'Доспат','SML10',21,'' UNION ALL
select 194,'Златоград','SML11',21,'' UNION ALL
select 195,'Мадан','SML16',21,'' UNION ALL
select 196,'Неделино','SML18',21,'' UNION ALL
select 197,'Рудозем','SML27',21,'' UNION ALL
select 198,'Смолян','SML31',21,'' UNION ALL
select 199,'Чепеларе','SML38',21,'' UNION ALL
select 200,'Антон','SFO54',23,'' UNION ALL
select 201,'Божурище','SFO06',23,'' UNION ALL
select 202,'Ботевград','SFO07',23,'' UNION ALL
select 203,'Годеч','SFO09',23,'' UNION ALL
select 204,'Горна Малина','SFO10',23,'' UNION ALL
select 205,'Долна баня','SFO59',23,'' UNION ALL
select 206,'Драгоман','SFO16',23,'' UNION ALL
select 207,'Елин Пелин','SFO17',23,'' UNION ALL
select 208,'Етрополе','SFO18',23,'' UNION ALL
select 209,'Златица','SFO47',23,'' UNION ALL
select 210,'Ихтиман','SFO20',23,'' UNION ALL
select 211,'Копривщица','SFO24',23,'' UNION ALL
select 212,'Костенец','SFO25',23,'' UNION ALL
select 213,'Костинброд','SFO26',23,'' UNION ALL
select 214,'Мирково','SFO56',23,'' UNION ALL
select 215,'Пирдоп','SFO55',23,'' UNION ALL
select 216,'Правец','SFO34',23,'' UNION ALL
select 217,'Самоков','SFO39',23,'' UNION ALL
select 218,'Своге','SFO43',23,'' UNION ALL
select 219,'Сливница','SFO45',23,'' UNION ALL
select 220,'Столична','SOF46',22,'' UNION ALL
select 221,'Чавдар','SFO57',23,'' UNION ALL
select 222,'Челопеч','SFO58',23,'' UNION ALL
select 223,'Братя Даскалови','SZR04',24,'' UNION ALL
select 224,'Гурково','SZR37',24,'' UNION ALL
select 225,'Гълъбово','SZR07',24,'' UNION ALL
select 226,'Казанлък','SZR12',24,'' UNION ALL
select 227,'Мъглиж','SZR22',24,'' UNION ALL
select 228,'Николаево','SZR38',24,'' UNION ALL
select 229,'Опан','SZR23',24,'' UNION ALL
select 230,'Павел баня','SZR24',24,'' UNION ALL
select 231,'Раднево','SZR27',24,'' UNION ALL
select 232,'Стара Загора','SZR31',24,'' UNION ALL
select 233,'Чирпан','SZR36',24,'' UNION ALL
select 234,'Антоново','TGV02',25,'' UNION ALL
select 235,'Омуртаг','TGV22',25,'' UNION ALL
select 236,'Опака','TGV23',25,'' UNION ALL
select 237,'Попово','TGV24',25,'' UNION ALL
select 238,'Търговище','TGV35',25,'' UNION ALL
select 239,'Димитровград','HKV09',26,'' UNION ALL
select 240,'Ивайловград','HKV11',26,'' UNION ALL
select 241,'Любимец','HKV17',26,'' UNION ALL
select 242,'Маджарово','HKV18',26,'' UNION ALL
select 243,'Минерални бани','HKV19',26,'' UNION ALL
select 244,'Свиленград','HKV28',26,'' UNION ALL
select 245,'Симеоновград','HKV29',26,'' UNION ALL
select 246,'Стамболово','HKV30',26,'' UNION ALL
select 247,'Тополовград','HKV32',26,'' UNION ALL
select 248,'Харманли','HKV33',26,'' UNION ALL
select 249,'Хасково','HKV34',26,'' UNION ALL
select 250,'Велики Преслав','SHU23',27,'' UNION ALL
select 251,'Венец','SHU07',27,'' UNION ALL
select 252,'Върбица','SHU10',27,'' UNION ALL
select 253,'Каолиново','SHU18',27,'' UNION ALL
select 254,'Каспичан','SHU19',27,'' UNION ALL
select 255,'Никола Козлево','SHU21',27,'' UNION ALL
select 256,'Нови пазар','SHU22',27,'' UNION ALL
select 257,'Смядово','SHU25',27,'' UNION ALL
select 258,'Хитрино','SHU11',27,'' UNION ALL
select 259,'Шумен','SHU30',27,'' UNION ALL
select 260,'Болярово','JAM03',28,'' UNION ALL
select 261,'Елхово','JAM07',28,'' UNION ALL
select 262,'Стралджа','JAM22',28,'' UNION ALL
select 263,'Тунджа','JAM25',28,'' UNION ALL
select 264,'Ямбол','JAM26',28,'' UNION ALL
select 265,'други','FRN1',29,'' UNION ALL
select 266,'Сърница','',13,'';