GO
PRINT 'Insert [inst_nom].[SPPOOProfession]'
GO

insert [inst_nom].[SPPOOProfession] ([SPPOOProfessionID],[ProfAreaID],[Name],[Description],[SPPOOProfessionCode],[IsValid],[ValidFrom],[ValidTo],[CanChoose],[ProfGroupMONID],[NameEN],[NameDE],[NameFR])
select -1,-1,'не е приложимо',NULL,NULL,1,'2020-11-19 00:00:00.000',NULL,0,-1,NULL,NULL,NULL UNION ALL
select 1,95,'Стругар',NULL,'1',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 2,96,'Електронен техник - компютърни системи',NULL,'1',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 3,95,'Шлосер',NULL,'2',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 4,96,'Шивач - моделиер',NULL,'2',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 5,95,'Монтьор на електродомакински уреди',NULL,'3',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 6,96,'Икономист - информатик',NULL,'3',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 7,95,'Бобиньор',NULL,'4',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 8,96,'Строителен техник',NULL,'4',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 9,95,'Автотенекеджия',NULL,'5',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 10,96,'Хлебар',NULL,'5',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 11,95,'Автокаросерист',NULL,'6',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 12,96,'Техник в дървообработването',NULL,'6',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 13,95,'Автобояджия',NULL,'7',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 14,96,'Програмист',NULL,'7',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 15,95,'Радиаторджия',NULL,'8',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 16,96,'Техник - електроинсталации',NULL,'8',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 17,95,'Ресорджия',NULL,'9',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 18,96,'Техник по газоснабдяване',NULL,'9',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 19,95,'Автотапицер',NULL,'10',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 20,96,'Секретар - администратор',NULL,'10',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 21,95,'Акумулаторджия',NULL,'11',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 22,96,'Фермер',NULL,'11',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 23,95,'Работник в производство на финна керамика',NULL,'12',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 24,96,'Телекомуникационен техник',NULL,'12',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 25,95,'Работник в производство на плоско стъкло',NULL,'13',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 26,96,'Геодезист',NULL,'13',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 27,95,'Работник в производство на свързващи вещества',NULL,'14',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 28,96,'Икономист - организатор',NULL,'14',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 29,95,'Зидар - кофражист',NULL,'15',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 30,96,'Хотел и кетъринг',NULL,'15',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 31,95,'Зидар - мазач',NULL,'16',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 32,96,'Техник на битова техника',NULL,'16',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 33,95,'Мозайкаджия - облицовчик',NULL,'17',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 34,96,'Машинен техник',NULL,'17',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 35,95,'Бояджия - тапетаджия',NULL,'18',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 36,96,'Автомобилен механик',NULL,'18',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 37,95,'Тракторист - машинист в растениевъдството и водач на МПС, кат. "Т"',NULL,'19',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 38,95,'Тракторист - машинист в животновъдството и водач на МПС, кат. "Т"',NULL,'20',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 39,95,'Мебелист',NULL,'21',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 40,95,'Дограмаджия',NULL,'22',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 41,95,'Тапицер',NULL,'23',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 42,95,'Работник в консервно производство',NULL,'24',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 43,95,'Работник в производство на месо',NULL,'25',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 44,95,'Работник в производство на захар и захарни изделия',NULL,'26',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 45,95,'Работник в производство на фураж',NULL,'27',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 46,95,'Работник в производство на мазнини и сапуни',NULL,'28',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 47,95,'Работник в производство на месни продукти',NULL,'29',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 48,95,'Работник в производство на млечни продукти',NULL,'30',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 49,95,'Работник в производство на цигари',NULL,'31',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 50,95,'Работник в производство на алкохолни напитки',NULL,'32',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 51,95,'Работник в производство на хляб',NULL,'33',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 52,95,'Работник в производство на хлебни изделия',NULL,'34',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 53,95,'Работник в производство на сладкарски изделия',NULL,'35',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 54,95,'Работник в общественото хранене',NULL,'36',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 55,95,'Камериер',NULL,'37',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 56,95,'Предач',NULL,'38',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 57,95,'Тъкач',NULL,'39',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 58,95,'Плетач',NULL,'40',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 59,95,'Шивач',NULL,'41',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 60,95,'Обущар',NULL,'42',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 61,95,'Кожар',NULL,'43',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 62,95,'Бръснар - фризьор',NULL,'44',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 63,95,'Земеделски работник',NULL,'45',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 64,95,'Механо-шлосер по текущо поддържане на машини и съоръжения в открити рудници',NULL,'46',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 65,1,'Геология и проучване на полезни изкопаеми',NULL,'170',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 66,1,'Проучвателна геофизика',NULL,'171',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 67,1,'Техника на проучване и сондажна електромеханика',NULL,'172',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 68,1,'Дълбоко сондиране и експлоатация на нефтени и газови находища',NULL,'173',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 69,1,'Добивни и строителни минни технологии',NULL,'174',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 70,1,'Минна електромеханика',NULL,'175',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 71,1,'Обогатяване на полезни изкопаеми',NULL,'176',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 72,1,'Добив и обработка на скални материали',NULL,'177',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 73,1,'Механизация за добив и обработка на скални материали',NULL,'178',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 74,1,'Производство на художествени изделия от скални материали',NULL,'179',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 75,1,'Маркшайдерство и геодезия',NULL,'180',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 76,1,'Хисрология и хидрогеология',NULL,'181',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 77,1,'Метеорология',NULL,'182',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 78,2,'Металургия на черните метали',NULL,'270',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 79,2,'Металургия на цветните метали',NULL,'271',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 80,3,'Ядрена топлоенергетика',NULL,'370',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 81,3,'Термични и водноенергетични машини и съоръжения',NULL,'371',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 82,3,'Топлинна и хладилна техника',NULL,'372',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 83,4,'Технология на машиностроенето - студена обработка',NULL,'471',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 84,4,'Технология на машиностроенето - уредостроене',NULL,'472',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 85,4,'Технология на машиностроенето - топла обработка',NULL,'473',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 86,4,'Автоматизация на производството (за машиностроенето)',NULL,'474',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 87,4,'Робототехника',NULL,'475',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 88,4,'Очна оптика',NULL,'476',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 89,4,'Оптика, оптико-механични и оптико-електронни уреди',NULL,'477',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 90,4,'Кинотехника и видеотехника',NULL,'478',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 91,4,'Лазерна техника',NULL,'479',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 92,4,'Измервателна техника',NULL,'480',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 93,4,'Филмов монтаж',NULL,'481',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 94,4,'Монтаж на промишлени съоръжения и машини',NULL,'482',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 95,4,'Машини и апарати в химическата промишленост',NULL,'483',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 96,4,'Машини и апарати в хранително-вкусовата промишленост',NULL,'484',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 97,4,'Машини и съоръжения в леката промишленост',NULL,'485',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 98,4,'Биотехника',NULL,'486',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 99,4,'Хидро и пневмотехника',NULL,'487',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 100,4,'Машини и апарати за очистване на въздуха',NULL,'488',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 101,4,'Роботизирани и гъвкави автоматизирани производствени системи',NULL,'489',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 102,4,'Технолог - програмист на системи с ЦПУ',NULL,'490',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 103,4,'Ремонт и поддържане на системи с ЦПУ',NULL,'491',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 104,4,'Индустриален механик',NULL,'494',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 105,5,'Електрически машини и апарати',NULL,'570',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 106,5,'Електрообзавеждане на промишлени предприятия',NULL,'571',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 107,5,'Електрически централи и мрежи',NULL,'572',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 108,5,'Електрификация на селското стопанство',NULL,'573',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 109,5,'Електротехника на автомобилния транспорт',NULL,'574',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 110,5,'Електрообзавеждане на електрически превозни средства за градски транспорт',NULL,'575',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 111,5,'Асансьорна техника',NULL,'576',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 112,5,'Експлоатация и ремонт на електрически автомобили за вътрешно-заводски транспорт',NULL,'577',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 113,5,'Електрообзавеждане на кораби',NULL,'578',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 114,5,'Радиотехника и телевизия',NULL,'579',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 115,5,'Съобщителна техника',NULL,'580',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 116,5,'Електронна техника',NULL,'581',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 117,5,'Конструиране и технология на електронни елементи',NULL,'582',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 118,5,'Конструиране и технология на радио и хидроакустична апаратура',NULL,'583',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 119,5,'Автоматизация на производството',NULL,'584',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 120,5,'Микропроцесорна техника',NULL,'585',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 121,5,'Програмно осигуряване и информационни технологии',NULL,'586',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 122,5,'Компютърна техника и технология',NULL,'587',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 123,5,'Микроелектроника и градивни елементи',NULL,'588',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 124,5,'Промишлена естетика и дизайн в електрониката и машиностроенето',NULL,'589',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 125,6,'Автомобили и кари',NULL,'670',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 126,6,'Управление на транспортното предприятие',NULL,'671',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 127,6,'Железопътна техника',NULL,'672',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 128,6,'Експлоатация и ремонт на електролокомотиви за вътрешно-заводски ж.п. транспорт',NULL,'673',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 129,6,'Корабоводене - речно',NULL,'674',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 130,6,'Корабоводене - морско',NULL,'675',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 131,6,'Експлоатация на пристанищата и флотата',NULL,'676',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 132,6,'Корабостоене',NULL,'677',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 133,6,'Технология на риболова, рибопреработването и аквакултури',NULL,'678',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 134,6,'Експлоатация на вътрешно-заводски ж.п. транспорт',NULL,'679',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 135,6,'Експлоатация и ремонт на самолети',NULL,'680',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 136,6,'Ремонт на корабни машини и механизми',NULL,'681',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 137,6,'Корабни машини и механизми',NULL,'682',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 138,6,'Автоматизация в ж.п. транспорт',NULL,'683',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 139,6,'Радиоелектронно оборудване на летателни апарати',NULL,'684',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 140,7,'Химични технологии',NULL,'771',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 141,7,'Биотехнологии',NULL,'772',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 142,7,'Силикатни технологии',NULL,'773',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 143,7,'Дизайн в силикатното производство',NULL,'774',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 144,7,'Технология на опазване на околната среда',NULL,'775',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 145,8,'Стоителство и архитектура',NULL,'871',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 146,8,'Водно строителство',NULL,'872',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 147,8,'Строителство на метрополитени',NULL,'873',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 148,8,'Геодезия',NULL,'874',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 149,8,'Транспортно строителство',NULL,'875',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 150,9,'Растениевъдство',NULL,'970',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 151,9,'Животновъдство',NULL,'971',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 152,9,'Ветеринарна медицина',NULL,'972',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 153,9,'Механизация на селското стопанство',NULL,'973',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 154,9,'Пчеларство, бубарство и преработка на пчелни продукти',NULL,'974',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 155,9,'Земеделски техник',NULL,'977',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 156,9,'Цветарство',NULL,'978',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 157,9,'Коневъдство и конен спорт',NULL,'980',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 158,10,'Горско стопанство и дърводобив',NULL,'1070',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 159,10,'Механизация на горското стопанство и дърводобива',NULL,'1071',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 160,10,'Механична технология на дървесината',NULL,'1072',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 161,10,'Мебелно производство',NULL,'1073',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 162,10,'Вътрешна архитектура',NULL,'1074',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 163,10,'Дърворезбарство',NULL,'1075',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 164,10,'Моделчество и дървостругарство',NULL,'1076',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 165,10,'Тапицерство и декораторство',NULL,'1077',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 166,10,'Производство на музикални инструменти',NULL,'1078',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 167,10,'Парково строителство',NULL,'1079',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 168,10,'Реставрация на стилни мебели и дограма',NULL,'1080',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 169,10,'Горско и ловно стопанство',NULL,'1081',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 170,11,'Техника и технология на хляба и хебните изделия',NULL,'1170',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 171,11,'Технология на зърносъхранението, зърнопреработването и фуражите',NULL,'1171',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 172,11,'Техника и технология на захарта и захарните изделия',NULL,'1172',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 173,11,'Техника и технология на млякото и млечните продукти',NULL,'1173',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 174,11,'Техника и технология на алкохолните и безалкохолните напитки',NULL,'1174',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 175,11,'Техника и технология на месото и месните продукти',NULL,'1175',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 176,11,'Технология на тютюна и тютюневите изделия',NULL,'1176',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 177,11,'Техника и технология на растителните мазнини и сапуни',NULL,'1177',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 178,11,'Техника и технология на консервното производство',NULL,'1178',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 179,11,'Технология на производството и обслужването в общественото хранене',NULL,'1179',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 180,11,'Експлоатация и поддържане на хладилна техника в хранителната промишленост',NULL,'1180',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 181,11,'Мениджмънт в хотелиерството',NULL,'1181',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 182,11,'Технологичен и микробиологичен контрол',NULL,'1182',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 183,11,'Технология и мениджмънт на производството и обслужването в заведенията за хранене',NULL,'1183',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 184,11,'Мениджмънт на туризма',NULL,'1185',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 185,11,'Мениджмънт в хлебопроизводството и сладкарството',NULL,'1186',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 186,12,'Технология на предачеството',NULL,'1270',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 187,12,'Технология на тъкачеството',NULL,'1271',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 188,12,'Технология на плетачеството',NULL,'1272',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 189,12,'Технология на апретурата, багренето и печатането',NULL,'1273',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 190,12,'Художествено оформление на текстилни площни изделия',NULL,'1274',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 191,12,'Моделиране и конструиране на облекло',NULL,'1275',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 192,12,'Технология на облеклото',NULL,'1276',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 193,12,'Моделиране, конструиране и технология на коженото и кожухарското облекло',NULL,'1277',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 194,12,'Технология на кожарското и кожухарското производство',NULL,'1278',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 195,12,'Технология на обувното производство',NULL,'1279',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 196,12,'Моделиране и конструиране на обувните изделия',NULL,'1280',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 197,12,'Технология на кожено-галантерийното производство',NULL,'1281',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 198,12,'Моделиране и конструиране на кожено-галантерийни изделия',NULL,'1282',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 199,12,'Автоматизация на производството в леката промишленост',NULL,'1283',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 200,13,'Икономика, управление и финанси на търговията',NULL,'1371',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 201,13,'Икономика, управление и финанси на кооперациите',NULL,'1372',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 202,13,'Банково, застрахователно и осигурително дело',NULL,'1373',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 203,13,'Машинна обработка на информацията',NULL,'1374',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 204,13,'Организация и експлоатация на съобщенията',NULL,'1375',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 205,13,'Организатор на производството',NULL,'1376',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 206,13,'Организация и управление на туризма',NULL,'1377',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 207,13,'Икономика на промишлеността',NULL,'1378',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 208,13,'Икономика на земеделското стопанство',NULL,'1379',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 209,13,'Счетоводна отчетност',NULL,'1380',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 210,13,'Стопански мениджмънт',NULL,'1381',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 211,13,'Бизнес-администрация',NULL,'1382',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 212,3,'Инсталационен техник',NULL,'373',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 213,4,'Машинен техник - Ортопедичен техник и бандажист',NULL,'401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 214,4,'Техник по слухови апарати',NULL,'402',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 215,4,'Технология на машиностроенето - производство на боеприпаси',NULL,'493',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 216,4,'Техник - технолог на художествени изделия от метал',NULL,'495',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 217,4,'Програмист - настройчик на машини с ЦПУ, роботи, ГАПС',NULL,'497',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 218,7,'Технология на фармацевтични и парфюмерийно-козметични производства',NULL,'777',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 219,9,'Лозаро-винар',NULL,'976',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 220,11,'Технология и мениджмънт на производството в ХВП',NULL,'1184',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 221,12,'Компютърно проектиране на текстилни площни изделия',NULL,'1284',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 222,13,'Икономист - посредник на трудовата борса',NULL,'1383',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 223,13,'Икономист - посредник в митническата дейност',NULL,'1384',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 224,13,'Икономист - маркетолог',NULL,'1385',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 225,13,'Спортен мениджмънт',NULL,'1386',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 226,13,'Икономист в банковото дело',NULL,'1387',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 227,13,'Икономика и мениджмънт',NULL,'1388',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 228,13,'Икономист - организатор',NULL,'1389',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 229,13,'Организация и управление на свободното време',NULL,'1390',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 230,13,'Туристически водач',NULL,'1397',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 231,97,'Изобразителни изкуства',NULL,'1470',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 232,97,'Дизайн',NULL,'1471',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 233,97,'Театрален, кино и телевизионен декор',NULL,'1472',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 234,97,'Лютиер',NULL,'1473',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 235,97,'Организация на театър, кино и телевизия',NULL,'1474',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 236,16,'Оператор в хранително-вкусовата промишленост',NULL,'10302',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 237,74,'Оператор на металургични агрегати',NULL,'10302',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 238,16,'Оператор в производството на хляб, хлебни и сладкарски изделия',NULL,'10303',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 239,16,'Оператор в риболова и рибопреработването',NULL,'10304',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 240,17,'Оператор на металорежещи машини',NULL,'10401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 241,75,'Техник - енергетик',NULL,'10401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 242,17,'Оператор-настройчик на металорежещи машини с ЦПУ',NULL,'10402',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 243,75,'Инсталационен техник',NULL,'10402',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 244,17,'Оператор на роботизирани и гъвкави автоматизирани производствени системи',NULL,'10403',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 245,75,'Оператор - енергетик',NULL,'10403',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 246,17,'Оператор на машини за леене и формоване',NULL,'10404',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 247,75,'Електротехник',NULL,'10404',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 248,17,'Оператор на машини за топла обработка на металите (щанцьор-пресовчик)',NULL,'10405',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 249,75,'Оператор по транспорт на природен газ',NULL,'10405',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 250,17,'Оператор на машини за производство на опаковки от хартия и картон',NULL,'10406',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 251,75,'Оператор на машини в електропромишленото производство',NULL,'10406',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 252,75,'Електромонтьор',NULL,'10407',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 253,75,'Ел. бобиньор',NULL,'10408',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 254,75,'Монтьор на ел.домакински уреди',NULL,'10409',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 255,18,'Оператор на енергийни агрегати',NULL,'10501',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 256,76,'Машинен техник',NULL,'10501',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 257,18,'Оператор на машини и съоръжения за производство на кабели и проводници',NULL,'10502',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 258,76,'Техник - технолог на художествени изделия от метал',NULL,'10502',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 259,18,'Оператр в производство на електровакуумни изделия',NULL,'10503',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 260,76,'Програмист - настройчик на машини с ЦПУ, роботи и ГАПС',NULL,'10503',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 261,18,'Оператор в производството на полупроводникови елементи',NULL,'10504',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 262,76,'Оператор на металорежещи машини',NULL,'10504',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 263,18,'Оператор в производството на химични източници на електрически ток',NULL,'10505',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 264,76,'Оператор на машини за топла обработка на металите',NULL,'10505',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 265,76,'Оператор на роботи и ГАПС',NULL,'10506',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 266,76,'Монтьор на машини и съоръжения',NULL,'10507',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 267,76,'Инструменталчик',NULL,'10508',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 268,76,'Заварчик',NULL,'10509',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 269,76,'Леяр - формовчик',NULL,'10510',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 270,76,'Автокаросерист',NULL,'10511',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 271,76,'Ресьорджия',NULL,'10512',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 272,76,'Стругар фрезист',NULL,'10513',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 273,76,'Шлосер',NULL,'10514',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 274,76,'Ковач - пресовчик',NULL,'10515',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 275,76,'Бояджия на метал',NULL,'10516',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 276,76,'Тенекеджия',NULL,'10517',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 277,19,'Оператор в дървообработващата промишленост',NULL,'10601',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 278,77,'Техник оптик',NULL,'10601',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 279,19,'Оператор в мебелната промишленост',NULL,'10602',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 280,77,'Техник лазерна техника',NULL,'10602',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 281,19,'Оператор за обработка на дървесина',NULL,'10603',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 282,77,'Кинотехник',NULL,'10603',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 283,77,'Техник - уредостроител',NULL,'10604',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 284,77,'Лаборант - физикомеханичен анализ',NULL,'10605',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 285,77,'Часовникар',NULL,'10606',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 286,20,'Оператор в предачното производство',NULL,'10701',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 287,78,'Радиотелевизионен техник',NULL,'10701',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 288,20,'Оператор в тъкачното производство',NULL,'10702',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 289,78,'Електронен техник',NULL,'10702',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 290,20,'Оператор в плетачното производство',NULL,'10703',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 291,78,'Техник съобщителна техника',NULL,'10703',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 292,78,'Програмист на ЕИМ',NULL,'10704',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 293,78,'Монтьор на електронна техника',NULL,'10705',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 294,78,'Монтьор на съобщителна и осигурителна техника',NULL,'10706',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 295,78,'Оператор в производство на полупроводникови елементи',NULL,'10707',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 296,78,'Оператор на телефонно и радиосъобщителна техника',NULL,'10708',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 297,78,'Телефонист',NULL,'10709',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 298,78,'Телеграфист',NULL,'10710',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 299,21,'Оператор в шевно производство',NULL,'10801',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 300,79,'Технолог химик',NULL,'10801',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 301,21,'Оператор в кожено-галантерийно производство',NULL,'10802',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 302,79,'Биотехнолог',NULL,'10802',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 303,21,'Оператор в обувно производство',NULL,'10803',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 304,79,'Лаборант химик',NULL,'10803',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 305,79,'Технолог - еколог',NULL,'10804',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 306,79,'Оператор на машини в химическото производство',NULL,'10805',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 307,79,'Галванизатор',NULL,'10806',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 308,79,'Акумулаторчик',NULL,'10807',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 309,80,'Технолог - силикатчик',NULL,'10901',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 310,80,'Гравьор - декоратор на стъкло и керамика',NULL,'10902',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 311,22,'Оператор в събщенията',NULL,'10903',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 312,80,'Оператор на машини в силикатно производство',NULL,'10903',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 313,22,'Търговска екслоатация на ж.п. транспорт',NULL,'10904',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 314,80,'Грънчар',NULL,'10904',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 315,80,'Духач на стъкло',NULL,'10905',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 316,23,'Монтьор на машини, апарати, уреди и съоръжения',NULL,'20001',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 317,23,'Монтьор на оптико-механични и оптико-електронни уреди',NULL,'20002',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 318,23,'Минен електромашинен монтьор',NULL,'20003',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 319,23,'Заварчик',NULL,'20004',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 320,23,'Шлосер-инструменталчик',NULL,'20005',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 321,24,'Монтьор на електрически машини, апарати, уреди и устройства',NULL,'20101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 322,81,'Техник - технолог в производство на хранителни продукти',NULL,'20101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 323,24,'Монтьор по електрообзавеждане',NULL,'20102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 324,81,'Оператор на машини в производство на хранителни продукти',NULL,'20102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 325,24,'Монтьор на електрически мрежи и уредби за високо и ниско напрежение',NULL,'20103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 326,81,'Оператор на машини в риболова и рибопроизводството',NULL,'20103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 327,24,'Монтьор на електронна техника',NULL,'20104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 328,81,'Винар',NULL,'20104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 329,24,'Монтьор на съобщителна и осигурителна техника',NULL,'20105',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 330,81,'Колбасар',NULL,'20105',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 331,24,'Монтьор на контролно-измервателна и регулираща апаратура',NULL,'20106',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 332,81,'Бозаджия',NULL,'20106',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 333,81,'Мандраджия',NULL,'20107',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 334,25,'Монтажник на строителни инсталации',NULL,'20201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 335,25,'Монтажник на промишлени и енергийни съоръжения',NULL,'20202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 336,26,'Механизатор за подземен добив на полезни изкопаеми',NULL,'30101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 337,82,'Автомобилен техник',NULL,'30101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 338,26,'Механизатор за открит добив на полезни изкопаеми',NULL,'30102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 339,82,'Корабоводител',NULL,'30102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 340,82,'Корабен механик (техник)',NULL,'30103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 341,82,'Авиотехник',NULL,'30104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 342,82,'Машинист на корабни машинни механизми',NULL,'30105',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 343,82,'Техник по железопътна техника',NULL,'30106',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 344,82,'Техник на подемна техника',NULL,'30107',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 345,82,'Помощник - локомотивен машинист',NULL,'30108',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 346,82,'Гаров оператор',NULL,'30109',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 347,82,'Монтьор на жп механизация',NULL,'30110',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 348,82,'Монтьор и водач на МПС',NULL,'30111',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 349,82,'Монтьор - водач в градския ел.транспорт',NULL,'30112',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 350,82,'Пристанищен механизатор и водач на МПС кат. "Т" и "С"',NULL,'30113',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 351,82,'Моряк - машинист',NULL,'30114',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 352,82,'Крановик',NULL,'30115',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 353,27,'Строител-механизатор',NULL,'30201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 354,27,'Машинист-крановик',NULL,'30202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 355,28,'Земезелски стопанин (фермер)',NULL,'30300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 356,28,'Механизатор-растениевъд и водач на МПС, кат."Т"',NULL,'30301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 357,28,'Механизатор-животновъд и водач на МПС, кат."Т"',NULL,'30302',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 358,28,'Механизатор в горското стопанство',NULL,'30303',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 359,28,'Паркостроител',NULL,'30304',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 360,28,'Помощник-лесовъд',NULL,'30305',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 361,28,'Земеделец',NULL,'30306',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 362,28,'Монтьор на селскостопанска техника в водач на МПС категория "Т" и "В"',NULL,'30307',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 363,29,'Машинист-монтьор на пътно-строителни машини и водач на МПС, категория "Т" и "С"',NULL,'30401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 364,29,'Машинист-монтьор на ж.п. механизация',NULL,'30402',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 365,29,'Монтьор и водач на МПС',NULL,'30403',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 366,29,'Монтьор - водач в градски електротранспорт',NULL,'30404',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 367,29,'Пристанищен механизатор и водач на МПС, кат."Т" и "С"',NULL,'30405',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 368,29,'Моряк-моторист',NULL,'30406',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 369,29,'Гаров оператор',NULL,'30407',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 370,29,'Помощник-локомотивен машинист',NULL,'30408',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 372,30,'Строител-монтажник',NULL,'40101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 373,83,'Текстилен техник',NULL,'40101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 374,30,'Строител за външно и вътрешно оформяне на сгради',NULL,'40102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 375,83,'Оператор на машини в текстилното производство',NULL,'40102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 376,83,'Предач',NULL,'40103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 377,83,'Тъкач',NULL,'40104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 378,83,'Плетач',NULL,'40105',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 379,83,'Апретурист',NULL,'40106',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 380,83,'Текстилен бояджия',NULL,'40107',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 381,83,'Текстилен печатар',NULL,'40108',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 382,83,'Килимар',NULL,'40109',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 383,31,'Хидростроител',NULL,'40201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 384,84,'Моделиер - конструктор на облекла',NULL,'40201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 385,31,'Подземен строител',NULL,'40202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 386,84,'Технолог - шивач',NULL,'40202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 387,31,'Пътен строител',NULL,'40203',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 388,84,'Моделиер конструктор на галантерия',NULL,'40203',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 389,31,'Пещостроител и строител на изолации',NULL,'40204',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 390,84,'Оператор на машини в шевното производство',NULL,'40204',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 391,84,'Оператор на машини в галантерийното производство',NULL,'40205',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 392,84,'Бродирист',NULL,'40206',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 393,84,'Шивач на облекло',NULL,'40207',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 394,84,'Юрганджия - дюшекчия',NULL,'40208',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 395,84,'Шивач на бельо',NULL,'40209',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 396,84,'Шивач на корабни платна и палатки',NULL,'40210',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 397,85,'Техник - кожар',NULL,'40301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 398,85,'Техник - кожухар',NULL,'40302',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 399,85,'Кожар',NULL,'40303',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 400,85,'Кожухар',NULL,'40304',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 401,85,'Галантерист',NULL,'40305',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 402,86,'Моделиер - конструктор на обувки',NULL,'40401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 403,86,'Техник - обущар',NULL,'40402',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 404,86,'Оператор на машини в обувното производство',NULL,'40403',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 405,86,'Обущар',NULL,'40404',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 406,32,'Администратор-документалист',NULL,'50001',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 407,32,'Продавач-консултант',NULL,'50003',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 408,32,'Кулинар',NULL,'50004',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 409,32,'Бръснар-фризьор',NULL,'50005',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 410,32,'Хотелиер',NULL,'50006',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 411,32,'Козметик',NULL,'50007',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 412,87,'Техник - полиграфист',NULL,'50101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 413,87,'Печатар',NULL,'50102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 414,87,'Клавиатурист',NULL,'50103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 415,87,'Книговезец',NULL,'50104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 416,33,'Организатор движение на градски и пътнически транспорт',NULL,'60001',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 417,33,'Гравьор-декоратор на стъкларски и керамични изделия',NULL,'60002',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 418,33,'Корабостроител',NULL,'60003',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 419,33,'Асистент-учител',NULL,'60004',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 420,33,'Социален работник',NULL,'60005',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 421,88,'Строителен техник',NULL,'60101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 422,88,'Геодезист - картограф',NULL,'60102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 423,88,'Строител',NULL,'60103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 424,88,'Монтажник на строителни инсталации',NULL,'60104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 425,88,'Зидар - кофражист',NULL,'60105',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 426,88,'Облицовчик',NULL,'60106',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 427,88,'Мозайкаджия',NULL,'60107',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 428,88,'Бетонджия',NULL,'60108',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 429,88,'Мазач',NULL,'60109',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 430,88,'Бояджия',NULL,'60110',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 431,88,'Тапетаджия',NULL,'60111',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 432,88,'Дограмаджия',NULL,'60112',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 433,89,'Земеделски техник',NULL,'70101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 434,89,'Ветеринарен техник',NULL,'70102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 435,89,'Техник на селскостопански машини',NULL,'70103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 436,89,'Агробиолог',NULL,'70104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 437,89,'Земеделец',NULL,'70105',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 438,89,'Животновъд - растениевъд',NULL,'70106',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 439,89,'Машинист - тракторист',NULL,'70107',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 440,90,'Техник - лесовъд',NULL,'70201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 441,90,'Техник - дърводобив',NULL,'70202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 442,90,'Техник - озеленител',NULL,'70203',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 443,90,'Монтьор на дърводобивни машини и водач на МПС, кат. "Т" и "С"',NULL,'70204',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 444,90,'Техник - вътрешна архитектура',NULL,'70205',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 445,90,'Техник - конструктор на мебели',NULL,'70206',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 446,90,'Тапицер - декоратор',NULL,'70207',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 447,90,'Дърворезбар - реставратор',NULL,'70208',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 448,90,'Техник, конструктор на музикални инструменти',NULL,'70209',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 449,90,'Мебелист',NULL,'70210',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 450,90,'Моделчик - дървостругар',NULL,'70211',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 451,90,'Оператор на машини в дървообработването',NULL,'70212',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 452,90,'Оператор на машини в мебелното производство',NULL,'70213',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 453,90,'Дърводелец',NULL,'70214',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 454,90,'Бъчвар',NULL,'70215',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 455,90,'Тапицер',NULL,'70216',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 456,91,'Икономист организатор',NULL,'80101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 457,91,'Екскурзовод',NULL,'80102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 458,91,'Икономист - информатор',NULL,'80103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 459,91,'Счетоводител',NULL,'80104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 460,91,'Посредник',NULL,'80105',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 461,91,'Стоковед',NULL,'80106',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 462,91,'Продавач - консултант',NULL,'80107',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 463,92,'Секретар',NULL,'80201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 464,92,'Администратор - документалист',NULL,'80202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 465,93,'Социален работник',NULL,'90101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 466,93,'Технолог в заведенията за хранене',NULL,'90102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 467,93,'Готвач',NULL,'90103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 468,93,'Сладкар',NULL,'90104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 469,93,'Сервитьор - барман',NULL,'90105',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 470,93,'Хлебар - пекар',NULL,'90106',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 471,93,'Стюард',NULL,'90107',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 472,93,'Фризьор - стилист',NULL,'90108',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 473,93,'Козметик',NULL,'90109',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 474,93,'Камериер',NULL,'90110',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 475,93,'Маникюрист - педикюрист',NULL,'90111',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 476,93,'Оператор на техника за химическо чистене и пране',NULL,'90112',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 477,93,'Чистач на сгради',NULL,'90113',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 478,93,'Помощник - готвач',NULL,'90114',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 479,93,'Помощник - сладкар',NULL,'90115',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 480,93,'Баничар',NULL,'90116',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 481,93,'Пиколо',NULL,'90117',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 482,94,'Художник',NULL,'100101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 483,94,'Артист - инструменталист',NULL,'100102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 484,94,'Артист - вокалист',NULL,'100103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 485,94,'Артист - балетист',NULL,'100104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 486,94,'Хореограф',NULL,'100105',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 487,34,'Художник - изпълнител',NULL,'211010',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 488,35,'Музикант - инструменталист',NULL,'212010',1,'2020-11-19 00:00:00.000',NULL,1,6,'Musician - instrumentalist ','Musiker-Instrumentalist','Musicien-instrumentiste' UNION ALL
select 489,35,'Акордьор',NULL,'212020',1,'2020-11-19 00:00:00.000',NULL,1,6,'Tuner','Stimmer','Accordeur d’instruments' UNION ALL
select 490,35,'Музикант - вокалист',NULL,'212030',1,'2020-11-19 00:00:00.000',NULL,1,6,'Musician - lead singer','Musiker-Sänger','Musicien-chanteur' UNION ALL
select 491,35,'Балетист',NULL,'212040',1,'2020-11-19 00:00:00.000',NULL,1,6,'Ballet dancer','Balletttänzer','Danseur de ballet' UNION ALL
select 492,35,'Танцьор',NULL,'212050',1,'2020-11-19 00:00:00.000',NULL,1,6,'Dancer','Tänzer','Danseur' UNION ALL
select 493,35,'Цирков и вариететен изпълнител',NULL,'212060',1,'2020-11-19 00:00:00.000',NULL,1,6,'Circus and variety performer','Zirkus- und Varietékünstler','Interprète de cirque et de variétés' UNION ALL
select 494,35,'Актьор',NULL,'212070',1,'2020-11-19 00:00:00.000',NULL,1,6,'Actor','Schauspieler','Acteur' UNION ALL
select 495,35,'Режисьор - изпълнител',NULL,'212080',1,'2020-11-19 00:00:00.000',NULL,1,6,'Executive director','Regisseur','Metteur en scène-interprète' UNION ALL
select 496,36,'Монтажист',NULL,'213010',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 497,36,'Фотограф',NULL,'213020',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 498,36,'Полиграфист',NULL,'213030',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 499,36,'Тоноператор',NULL,'213040',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 500,36,'Компютърен аниматор',NULL,'213050',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 501,36,'Компютърен график',NULL,'213060',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 502,36,'Графичен дизайнер',NULL,'213070',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 503,36,'Музикален оформител',NULL,'213080',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 504,36,'Аниматор',NULL,'213090',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 505,36,'Режисьор - изпълнител',NULL,'213100',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 506,36,'Оператор - изпълнител',NULL,'213110',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 507,36,'Продуцент',NULL,'213120',0,'2020-11-19 00:00:00.000',NULL,1,8,NULL,NULL,NULL UNION ALL
select 508,36,'Режисьор на пулт',NULL,'213130',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 509,36,'Репортер и водещ на телевизионна програма',NULL,'213140',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 510,36,'Репортер и водещ на радиопредаване',NULL,'213150',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 511,37,'Дизайнер - изпълнител',NULL,'214010',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 512,38,'Лютиер',NULL,'215010',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 513,38,'Ювелир',NULL,'215020',0,'2020-11-19 00:00:00.000',NULL,1,8,NULL,NULL,NULL UNION ALL
select 514,38,'Бижутер',NULL,'215030',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 515,38,'Дърворезбар',NULL,'215040',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 516,38,'Керамик',NULL,'215050',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 517,38,'Каменоделец',NULL,'215060',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 518,39,'Икономист - издател',NULL,'341010',0,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 519,39,'Продавач - консултант',NULL,'341020',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 520,40,'Маркетолог',NULL,'342010',0,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 521,41,'Финансист',NULL,'343010',1,'2020-11-19 00:00:00.000',NULL,1,5,'Financial agent','Finanzier','Financier' UNION ALL
select 522,42,'Счетоводител',NULL,'344010',0,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 523,42,'Данъчен и митнически посредник',NULL,'344020',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 524,43,'Икономист - мениджър',NULL,'345010',0,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 525,43,'Фирмен мениджър',NULL,'345020',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 526,43,'Организатор производство',NULL,'345030',0,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 527,43,'Сътрудник в бизнес - услуги',NULL,'345040',1,'2020-11-19 00:00:00.000',NULL,1,5,'Business services associate','Mitarbeiter im Bereich Geschäftsdienstleistungen','Assistant aux services aux entreprises' UNION ALL
select 528,43,'Сътрудник в малък и среден бизнес',NULL,'345050',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 529,43,'Касиер',NULL,'345060',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 530,43,'Калкулант',NULL,'345070',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 531,43,'Снабдител',NULL,'345080',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 532,43,'Деловодител-архивист',NULL,'345090',0,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 533,43,'Оператор на компютър',NULL,'345100',1,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 534,44,'Офис мениджър',NULL,'346010',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 535,44,'Офис - секретар',NULL,'346020',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 536,45,'Техник геолог',NULL,'443010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician geologist','Geologie-Techniker','Géologue technicien' UNION ALL
select 537,45,'Техник - хидрометеоролог',NULL,'443020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician - hydrometeorologist','Techniker für Hydrometeorologie','Technicien en hydrométéorologie' UNION ALL
select 538,46,'Икономист - информатик',NULL,'482010',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 539,47,'Машинен техник',NULL,'521010',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 540,47,'Техник - приложник',NULL,'521020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician-applied works','Anwendungs-techniker','Technicien- applicateur' UNION ALL
select 541,47,'Машинен оператор',NULL,'521030',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 542,47,'Машинен монтьор',NULL,'521040',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 543,47,'Техник на прецизна техника',NULL,'521050',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 544,47,'Монтьор на прецизна техника',NULL,'521060',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 545,47,'Техник - металург',NULL,'521070',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 546,47,'Оператор в металургията',NULL,'521080',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 547,47,'Заварчик',NULL,'521090',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 548,47,'Стругар',NULL,'521100',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 549,47,'Шлосер',NULL,'521110',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 550,47,'Леяр',NULL,'521120',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 551,47,'Ковач',NULL,'521130',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 552,48,'Електротехник',NULL,'522010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Electrical engineer','Elektriker','Électricien' UNION ALL
select 553,48,'Електромонтьор',NULL,'522020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Electric fitter','Elektromonteur','Monteur d’appareils électriques' UNION ALL
select 554,48,'Техник на енергийни съоръжения и инсталации',NULL,'522030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician of energy facilities and installations','Techniker für Energieanlagen und -installationen','Technicien des équipements et installations énergétiques' UNION ALL
select 555,48,'Монтьор на енергийни съоръжения и инсталации',NULL,'522040',1,'2020-11-19 00:00:00.000',NULL,1,3,'Fitter of energy facilities and installations','Monteur für Energieanlagen und -installationen','Installateur d''équipements et d''installations énergétiques' UNION ALL
select 556,48,'Огняр',NULL,'522060',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 557,49,'Техник по комуникационни системи',NULL,'523010',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 558,49,'Монтьор по комуникационни системи',NULL,'523020',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 559,49,'Техник на електронна техника',NULL,'523030',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 560,49,'Монтьор на електронна техника',NULL,'523040',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 561,49,'Техник на компютърни системи',NULL,'523050',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 562,49,'Монтьор на компютърни системи',NULL,'523060',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 563,49,'Техник по автоматизация',NULL,'523070',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 564,49,'Монтьор по автоматзация',NULL,'523080',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 565,49,'Програмист',NULL,'523090',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 566,50,'Химик - технолог',NULL,'524010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Chemist - technologist','Chemiker-Technologe','Chimiste-technologue' UNION ALL
select 567,50,'Биотехнолог',NULL,'524020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Biotechnologist','Biotechnologe','Biotechnologue' UNION ALL
select 568,50,'Оператор в биотехнологични производства',NULL,'524030',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 569,50,'Лаборант',NULL,'524040',1,'2020-11-19 00:00:00.000',NULL,1,3,'Laboratory assistant','Laborant','Assistant de laboratoire' UNION ALL
select 570,50,'Декоратор в силикатното производство',NULL,'524050',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 571,50,'Химик - оператор',NULL,'524060',1,'2020-11-19 00:00:00.000',NULL,1,3,'Chemist - operator','Chemiker-Operator','Opérateur chimiste' UNION ALL
select 572,50,'Стъклар',NULL,'524070',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 573,50,'Керамик',NULL,'524080',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 574,50,'Формовач на полимери',NULL,'524090',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 575,51,'Техник по транспортна техника',NULL,'525010',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 576,51,'Монтьор на транспортна техника',NULL,'525020',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 577,51,'Техник по подемно - транспортна техника',NULL,'525050',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 578,51,'Монтьор на подемно - транспортна техника',NULL,'525060',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 579,51,'Техник по железопътна техника',NULL,'525070',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 580,51,'Монтьор на железопътна техника',NULL,'525080',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 581,51,'Авиационен техник',NULL,'525090',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 582,51,'Корабен техник',NULL,'525100',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 583,51,'Корабен монтьор',NULL,'525110',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 584,51,'Автобояджия',NULL,'525120',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 585,51,'Акумулаторджия',NULL,'525130',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 586,51,'Радиаторджия',NULL,'525140',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 587,51,'Гумаджия',NULL,'525150',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 588,51,'Автотапицер',NULL,'525160',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 589,51,'Автотенекеджия',NULL,'525170',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 590,52,'Техник - технолог в хранително - вкусовата промишленост',NULL,'541010',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 591,52,'Оператор в хранително - вкусовата промишленост',NULL,'541020',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 592,52,'Хлебар - сладкар',NULL,'541030',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 593,52,'Техник - технолог по експлоатация и поддържане на хладилната техника в хранителната промишленост',NULL,'541040',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 594,52,'Работник в хранително - вкусовата промишленост',NULL,'541050',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 595,53,'Десенатор на текстил',NULL,'542010',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 596,53,'Текстилен техник',NULL,'542020',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 597,53,'Оператор в текстилно производство',NULL,'542030',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 598,53,'Моделиер - технолог на облекло',NULL,'542040',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 599,53,'Оператор в производството на облекло',NULL,'542050',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 600,53,'Моделиер - технолог на обувни и кожено - галантерийни изделия',NULL,'542060',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 601,53,'Оператор в производство на обувни и кожено - галантерийни изделия',NULL,'542070',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 602,53,'Работник в текстилно производство',NULL,'542080',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 603,53,'Работник в производство на облекло',NULL,'542090',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 604,53,'Работник в обувно и кожено - галантерийно производство',NULL,'542100',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 605,53,'Шивач',NULL,'542110',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 606,53,'Обущар',NULL,'542120',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 607,53,'Килимар',NULL,'542130',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 608,53,'Плетач',NULL,'542140',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 609,53,'Бродировач',NULL,'542150',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 610,54,'Техник - технолог в дървообработването',NULL,'543010',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 611,54,'Оператор в дървообработването',NULL,'543020',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 612,54,'Дърводелец',NULL,'543030',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 613,54,'Мебелист',NULL,'543040',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 614,54,'Дограмаджия',NULL,'543050',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 615,54,'Тапицер',NULL,'543060',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 616,54,'Бъчвар',NULL,'543070',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 617,54,'Кошничар',NULL,'543080',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 618,55,'Минен техник',NULL,'544010',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 619,55,'Оператор в минната промишленост',NULL,'544020',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 620,55,'Сондажен техник',NULL,'544030',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 621,55,'Сондьор',NULL,'544040',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 622,55,'Маркшайдер',NULL,'544050',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 623,56,'Строителен техник',NULL,'582010',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 624,56,'Геодезист',NULL,'582020',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 625,56,'Строител',NULL,'582030',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 626,56,'Строител – монтажник',NULL,'582040',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 627,56,'Монтажник на ВиК мрежи',NULL,'582050',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 628,56,'Пътен строител',NULL,'582060',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 629,56,'Пещостроител',NULL,'582070',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 630,57,'Техник-растениевъд',NULL,'621010',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 631,57,'Техник в лозаровинарството',NULL,'621020',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 632,57,'Растениевъд',NULL,'621030',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 633,57,'Техник-животновъд',NULL,'621040',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 634,57,'Животновъд',NULL,'621050',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 635,57,'Фермер',NULL,'621060',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 636,57,'Техник на селскостопанска техника',NULL,'621070',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 637,57,'Монтьор на селскостопанска техника',NULL,'621080',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 638,58,'Техник – озеленител',NULL,'622010',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 639,58,'Озеленител',NULL,'622020',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 640,59,'Техник- лесовъд',NULL,'623010',1,'2020-11-19 00:00:00.000',NULL,1,2,'Technician - forester','Forsttechniker','Technicien sylviculteur' UNION ALL
select 641,59,'Техник – механизатор',NULL,'623020',1,'2020-11-19 00:00:00.000',NULL,1,2,'Technician- mechanizer ','Techniker-Maschinenführer','Technicien- mécanisateur' UNION ALL
select 642,59,'Механизатор на горска техника',NULL,'623030',1,'2020-11-19 00:00:00.000',NULL,1,2,'Forestry equipment mechanizer','Forstmaschinenführer','Mécanisateur de matériel forestier' UNION ALL
select 643,59,'Лесовъд',NULL,'623040',1,'2020-11-19 00:00:00.000',NULL,1,2,'Forester','Förster','Sylviculteur' UNION ALL
select 644,59,'Дивечовъд ',NULL,'623050',1,'2020-11-19 00:00:00.000',NULL,1,2,'Game breeder','Wildzüchter','Éleveur de gibier' UNION ALL
select 645,59,'Работник в горското стопанство',NULL,'623060',1,'2020-11-19 00:00:00.000',NULL,1,2,'Worker in forestry','Forstarbeiter','Ouvrier à la sylviculture' UNION ALL
select 646,60,'Рибовъд',NULL,'624010',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 647,60,'Техник  по промишлен риболов и аквакултури',NULL,'624020',1,'2020-11-19 00:00:00.000',NULL,1,2,'Technician in industrial fishing and aquacultures','Techniker für industrielle Fischerei und Aquakulturen ','Technicien en pêche industrielle et aquacultures' UNION ALL
select 648,61,'Ветеринарен техник',NULL,'640010',1,'2020-11-19 00:00:00.000',NULL,1,2,'Veterinary technician','Veterinärtechniker','Technicien vétérinaire' UNION ALL
select 649,61,'Ветеринарен лаборант',NULL,'640020',1,'2020-11-19 00:00:00.000',NULL,1,2,'Veterinary laboratory assistant','Veterinärlaborant','Assistante de laboratoire vétérinaire' UNION ALL
select 650,62,'Техник по очна оптика',NULL,'725010',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 651,62,'Техник - оптометрист',NULL,'725020',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 652,62,'Техник по ортопедична техника',NULL,'725030',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 653,62,'Техник на слухови апарати',NULL,'725040',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 654,63,'Посредник на трудовата борса',NULL,'762010',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 655,63,'Социален работник',NULL,'762020',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 656,64,'Хотелиер',NULL,'811010',1,'2020-11-19 00:00:00.000',NULL,1,4,'Hotel-keeper','Hotelier','Exploitant d’un établissement hôtelier' UNION ALL
select 657,64,'Администратор в хотелиерството',NULL,'811020',1,'2020-11-19 00:00:00.000',NULL,1,4,'Administrator in hotel-keeping','Administrator im Hotelgewerbe','Administrateur en hôtellerie' UNION ALL
select 658,64,'Камериер',NULL,'811030',1,'2020-11-19 00:00:00.000',NULL,1,4,'Chambermaid','Kammerdiener','Femme de chambre' UNION ALL
select 659,64,'Портиер-пиколо',NULL,'811040',1,'2020-11-19 00:00:00.000',NULL,1,4,'Porter-bellman','Piccolo-Concierge','Concierge-piccolo' UNION ALL
select 660,64,'Работник в помощно стопанство в хотелиерството',NULL,'811050',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 661,64,'Ресторантьор',NULL,'811060',1,'2020-11-19 00:00:00.000',NULL,1,4,'Restaurant-keeper','Restaurateur','Restaurateur' UNION ALL
select 662,64,'Готвач',NULL,'811070',1,'2020-11-19 00:00:00.000',NULL,1,4,'Cook','Koch','Cuisinier' UNION ALL
select 663,64,'Сервитьор-барман',NULL,'811080',1,'2020-11-19 00:00:00.000',NULL,1,4,'Waiter-barkeeper','Kellner-Barkeeper','Serveur-barman' UNION ALL
select 664,64,'Работник в заведенията за хранене и развлечения',NULL,'811090',1,'2020-11-19 00:00:00.000',NULL,1,4,'Worker in the public catering and entertainment establishments','Arbeiter in den Gastronomie- und Vergnügungsbetrieben','Ouvrier aux restaurants et locaux de divertissement' UNION ALL
select 665,65,'Организатор на туристическа агентска дейност',NULL,'812010',1,'2020-11-19 00:00:00.000',NULL,1,4,'Organizer of tourist agent''s activity','Veranstalter von Reisebürotätigkeiten','Organisateur de l''activité d''agence de voyages' UNION ALL
select 666,65,'Планински водач',NULL,'812020',1,'2020-11-19 00:00:00.000',NULL,1,4,'Mountain guide','Bergführer','Guide de montagne' UNION ALL
select 667,65,'Екскурзовод',NULL,'812030',1,'2020-11-19 00:00:00.000',NULL,1,4,'Tour guide','Reiseführer','Guide touristique' UNION ALL
select 668,65,'Аниматор',NULL,'812040',0,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 669,66,'Помощник-инструктор по спортно-туристическа дейност',NULL,'813010',1,'2020-11-19 00:00:00.000',NULL,1,4,'Assistant instructor in sports and tourist activity','Hilfsausbilder für sportliche und touristische Aktivitäten','Instructeur adjoint en activité sportive et touristique' UNION ALL
select 670,67,'Помощник - възпитател',NULL,'814010',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 671,68,'Фризьор',NULL,'815010',1,'2020-11-19 00:00:00.000',NULL,1,4,'Hairdresser','Friseur','Coiffeur' UNION ALL
select 672,68,'Козметик',NULL,'815020',1,'2020-11-19 00:00:00.000',NULL,1,4,'Cosmetician ','Kosmetiker','Cosméticien' UNION ALL
select 673,69,'Корабоводител',NULL,'840010',1,'2020-11-19 00:00:00.000',NULL,1,1,'Ship navigator','Schiffsnavigation','Navigateur' UNION ALL
select 674,69,'Моряк',NULL,'840020',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 675,69,'Организатор по експлоатация в ж.п. инфраструктура',NULL,'840030',1,'2020-11-19 00:00:00.000',NULL,1,1,'Organizer of exploitation in railway infrastructure','Organisator der Betriebs in der Eisenbahinfrastruktur','Organisateur d''exploitation en Infrastructure ferroviaire' UNION ALL
select 676,69,'Инструктор за обучение на водачи на МПС',NULL,'840040',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 677,69,'Летец-пилот',NULL,'840050',1,'2020-11-19 00:00:00.000',NULL,1,1,'Pilot','Pilot','Aviateur-pilote' UNION ALL
select 678,50,'Оператор в силикатните производства',NULL,'524120',1,'2020-11-19 00:00:00.000',NULL,1,3,'Operator in silicate productions','Bediener in der Silikatherstellung','Opérateur aux productions de silicate' UNION ALL
select 679,57,'Лозаровинар',NULL,'621090',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 680,66,'Инструктор по спортно-туристическа дейност',NULL,'813020',1,'2020-11-19 00:00:00.000',NULL,1,4,'Instructor in sports and tourist activity','Ausbilder für sportliche und touristische Aktivitäten','Instructeur en activités sportives et touristiques' UNION ALL
select 681,70,'Еколог',NULL,'851010',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 682,70,'Консултант на опасни товари',NULL,'851020',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 683,71,'Охранител',NULL,'861010',0,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 684,4,'Техник - организатор в машиностроенето',NULL,'496',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 685,32,'Организатор на малко предприятие',NULL,'50008',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 686,43,'Икономист',NULL,'345110',0,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 862,66,'Инструктор по адаптирана физическа активност и спорт за хора с увреждания',NULL,'813040',1,'2020-11-19 00:00:00.000',NULL,1,4,'Instructor of adapted physical activity and sport for disabled people','Ausbilder für angepasste körperliche Aktivität und Sport für Menschen mit Behinderungen','Instructeur d''activité physique adaptée et de sport pour personnes handicapées' UNION ALL
select 863,66,'Инструктор по фитнес',NULL,'813050',1,'2020-11-19 00:00:00.000',NULL,1,4,'Fitness instructor','Fitnesstrainer','Instructeur de remise en forme physique' UNION ALL
select 864,66,'Спортен масажист',NULL,'813060',1,'2020-11-19 00:00:00.000',NULL,1,4,'Sports massage therapist','Sportmasseur','Masseur sportif' UNION ALL
select 865,71,'Спасител при бедствия, аварии и катастрофи',NULL,'861020',0,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 866,50,'Технолог в силикатните производства',NULL,'524110',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technologist in silicate productions','Technologe in der Silikatherstellung','Technologue aux productions de silicate' UNION ALL
select 870,14,'Оператор на сондажни машини',NULL,'10101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 871,72,'Техник геолог',NULL,'10101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 872,14,'Оператор на машини и съоръжения за обогатяване на полезни изкопаеми',NULL,'10102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 873,72,'Сондьор',NULL,'10102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 874,14,'Оператор на машини за обработване на скални материали',NULL,'10103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 875,72,'Проучвател на морски шелф',NULL,'10103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 876,4,'Технология на машиностроенето - специално машиностроене',NULL,'498',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 877,14,'Оператор на машини и съоръжения за проучване и добив на продукти от морския шелф',NULL,'10104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 878,15,'Оператор в черната металургия',NULL,'10201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 879,73,'Минен техник',NULL,'10201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 880,15,'Оператор в цветната металургия',NULL,'10202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 881,73,'Технолог - художествено каменоделство',NULL,'10202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 882,15,'Оператор в праховата метлургия',NULL,'10203',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 883,73,'Оператор на машини за обогатяване на полезни изкопаеми',NULL,'10203',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 884,15,'Оператор на химико-технологични процеси',NULL,'10204',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 885,73,'Оператор на машини за обработка на скални материали',NULL,'10204',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 886,15,'Оператор в преработка на полимерни материали',NULL,'10205',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 887,73,'Миньор',NULL,'10205',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 888,15,'Оператор в силикатното производство',NULL,'10206',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 889,73,'Солодобивчик',NULL,'10206',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 890,15,'Оператор в кожаркото и кожухарското производство',NULL,'10207',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 891,15,'Оператор в апретурно и багрилно производство, химическо чистене и пране',NULL,'10208',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 892,15,'Лаборант',NULL,'10209',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 893,16,'Оператор в биотехнологични процеси',NULL,'10301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 894,74,'Техник - металург',NULL,'10301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL,NULL UNION ALL
select 895,69,'Организатор по експлоатация на автомобилния транспорт',NULL,'840070',1,'2020-11-19 00:00:00.000',NULL,1,1,'Organizer of the exploitation of automobile transport','Organisator für den Betrieb von Straßentransporten','Organisateur de l''exploitation du transport routier' UNION ALL
select 896,69,'Организатор по търговска експлоатация на железопътния транспорт',NULL,'840080',1,'2020-11-19 00:00:00.000',NULL,1,1,'Organizer of commercial exploitation of railway transport','Organisator für Handels-betrieb des Schienenverkehrs','Organisateur de l’exploitation commerciale du transport ferroviaire' UNION ALL
select 897,66,'Треньор',NULL,'813030',1,'2020-11-19 00:00:00.000',NULL,1,4,'Coach','Trainer','Entraîneur' UNION ALL
select 899,69,'Организатор по експлоатация на пристанищата и флота',NULL,'840060',1,'2020-11-19 00:00:00.000',NULL,1,1,'Organizer of the exploitation of ports and navy','Organisator für den Betrieb der Häfen und der Flotte ','Organisateur de l''exploitation des ports et de la flotte' UNION ALL
select 900,34,'Художник',NULL,'211010',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 901,36,'Репортер и водещ',NULL,'213140',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 902,37,'Дизайнер',NULL,'214010',1,'2020-11-19 00:00:00.000',NULL,1,6,'Designer','Designer','Concepteur' UNION ALL
select 903,50,'Работник в стъкларското производство',NULL,'524070',1,'2020-11-19 00:00:00.000',NULL,1,3,'Worker in glassware industry','Arbeiter in der Glasherstellung','Ouvrier dans l''industrie du verre' UNION ALL
select 904,50,'Работник в керамичното производство',NULL,'524080',1,'2020-11-19 00:00:00.000',NULL,1,3,'Worker in ceramics industry','Arbeiter in der Keramikherstellung','Ouvrier en production de céramique' UNION ALL
select 905,50,'Работник в полимерните производства',NULL,'524090',1,'2020-11-19 00:00:00.000',NULL,1,3,'Worker in polymeric productions','Arbeiter in Polymerherstellung','Travailleur à productions de polymères' UNION ALL
select 906,62,'Оптик - оптометрист',NULL,'725020',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 907,50,'Дизайн в силикатното производство',NULL,'524050',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 908,98,'Сержант (Старшина за ВМ сили) - командир',NULL,'863010',1,'2020-11-19 00:00:00.000',NULL,1,4,'Sergeant (Sergeant-major for the navy) - commander','Sergeant (Sergeant für die Seestreitkräfte) - Kommandant','Sergent (maître de la marine) - commandant' UNION ALL
select 909,98,'Сержант (Старшина за ВМ сили) - логистик',NULL,'863020',1,'2020-11-19 00:00:00.000',NULL,1,4,'Sergeant (Sergeant-major for the navy) - logistician','Sergeant (Sergeant für die Seestreitkräfte) - Kommandant','Sergent (maître de la marine militaire) - logistique' UNION ALL
select 910,98,'Сержант (Старшина за ВМ сили) - техник',NULL,'863030',0,'2020-11-19 00:00:00.000',NULL,1,9,NULL,NULL,NULL UNION ALL
select 911,98,'Сержант (Старшина за ВМ сили) - администратор',NULL,'863040',1,'2020-11-19 00:00:00.000',NULL,1,4,'Sergeant (Sergeant-major for the navy) - administrator','Sergeant (Sergeant für die Seestreitkräfte) – Administrator','Sergent (maître de la marine) – administrateur' UNION ALL
select 912,98,'Сержант (Старшина за ВМ сили) - инструктор',NULL,'863050',1,'2020-11-19 00:00:00.000',NULL,1,4,'Sergeant (Sergeant-major for the navy) - instructor','Sergeant (Sergeant für die Seestreitkräfte) – Ausbilder','Sergent (maître de la marine) – instructeur' UNION ALL
select 913,41,'Финансов отчетник',NULL,'343020',1,'2020-11-19 00:00:00.000',NULL,1,5,'Financial reporter','Rechnungsleger','Comptable financier' UNION ALL
select 914,42,'Оперативен счетоводител',NULL,'344010',0,'2020-11-19 00:00:00.000',NULL,1,4,NULL,NULL,NULL UNION ALL
select 915,44,'Деловодител',NULL,'346030',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 916,51,'Работник по транспортна техника',NULL,'525120',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 917,52,'Техник - технолог по качеството на храни и напитки',NULL,'541060',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 918,54,'Работник в дървообработването',NULL,'543030',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 919,56,'Помощник в строителството',NULL,'582080',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 920,56,'Помощник пътен строител',NULL,'582090',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 921,57,'Работник в растениевъдството',NULL,'621110',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 922,57,'Работник в животновъдството',NULL,'621120',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 923,57,'Кинолог',NULL,'621130',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 924,58,'Работник в озеленяването',NULL,'622030',0,'2020-11-19 00:00:00.000',NULL,1,2,NULL,NULL,NULL UNION ALL
select 925,60,'Техник - рибовъд',NULL,'624010',1,'2020-11-19 00:00:00.000',NULL,1,2,'Technician - fishfarmer','Techniker - Fischzüchter','Technicien - pisciculteur' UNION ALL
select 926,60,'Рибовъд',NULL,'624030',1,'2020-11-19 00:00:00.000',NULL,1,2,'Fishfarmer','Fischzüchter','Pisciculteur' UNION ALL
select 927,60,'Работник в рибовъдството',NULL,'624040',1,'2020-11-19 00:00:00.000',NULL,1,2,'Worker in the fishery','Arbeiter in Fischzucht','Ouvrier à la pisciculture' UNION ALL
select 928,63,'Сътрудник социални дейности',NULL,'762020',0,'2020-11-19 00:00:00.000',NULL,1,5,NULL,NULL,NULL UNION ALL
select 929,67,'Гувернантка',NULL,'814020',1,'2020-11-19 00:00:00.000',NULL,1,4,'Governess','Gouvernante','Gouvernante' UNION ALL
select 930,69,'Водач на МПС за обществен превоз',NULL,'840090',1,'2020-11-19 00:00:00.000',NULL,1,1,'Driver of public transport motor vehicle','Fahrer eines öffentlichen Verkehrsmittels','*Conducteur de véhicules de transport en commun' UNION ALL
select 931,51,'Пристанищен работник',NULL,'525130',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 932,99,'Художник - изящни изкуства',NULL,'211010',1,'2020-11-19 00:00:00.000',NULL,1,6,'Artist - Fine arts','Maler - Schöne Künste','Artiste - beaux-arts' UNION ALL
select 933,122,'Монтажист',NULL,'213010',1,'2020-11-19 00:00:00.000',NULL,1,6,'Continuity editor','Filmeditor','Monteur' UNION ALL
select 934,122,'Фотограф',NULL,'213020',1,'2020-11-19 00:00:00.000',NULL,1,6,'Photographer','Fotograf','Photographe' UNION ALL
select 935,122,'Полиграфист',NULL,'213030',1,'2020-11-19 00:00:00.000',NULL,1,6,'Printing worker','Polygraf','Polygraphiste' UNION ALL
select 936,122,'Тоноператор',NULL,'213040',1,'2020-11-19 00:00:00.000',NULL,1,6,'Sound-operator','Tontechniker','Ingénieur du son' UNION ALL
select 937,122,'Компютърен аниматор',NULL,'213050',1,'2020-11-19 00:00:00.000',NULL,1,6,'Computer animator','Computer-Animator','Animateur par ordinateur' UNION ALL
select 938,122,'Компютърен график',NULL,'213060',1,'2020-11-19 00:00:00.000',NULL,1,6,'Computer graphic artist','Computergrafiker','Graphique par ordinateur' UNION ALL
select 939,122,'Графичен дизайнер',NULL,'213070',1,'2020-11-19 00:00:00.000',NULL,1,6,'Graphic designer','Grafikdesigner','Concepteur graphique' UNION ALL
select 940,122,'Музикален оформител',NULL,'213080',1,'2020-11-19 00:00:00.000',NULL,1,6,'Music director','Musikbearbeiter','Technicien musical' UNION ALL
select 941,122,'Аниматор',NULL,'213090',1,'2020-11-19 00:00:00.000',NULL,1,6,'Animator','Animator','Animateur' UNION ALL
select 942,122,'Режисьор - изпълнител',NULL,'213100',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 943,122,'Оператор - изпълнител',NULL,'213110',0,'2020-11-19 00:00:00.000',NULL,1,6,NULL,NULL,NULL UNION ALL
select 944,122,'Режисьор на пулт',NULL,'213130',1,'2020-11-19 00:00:00.000',NULL,1,6,'Vision mixer','Bildmischer','Réalisateur en pupitre de régie' UNION ALL
select 945,122,'Репортер и водещ',NULL,'213140',1,'2020-11-19 00:00:00.000',NULL,1,6,'Reporter and presenter','Reporter und Moderator','Reporter et présentateur' UNION ALL
select 946,100,'Лютиер',NULL,'215010',1,'2020-11-19 00:00:00.000',NULL,1,6,'Luthier','Geigenbauer','Luthier' UNION ALL
select 948,100,'Бижутер',NULL,'215030',1,'2020-11-19 00:00:00.000',NULL,1,6,'Jeweler ','Juwelier','Bijoutier' UNION ALL
select 949,100,'Дърворезбар',NULL,'215040',1,'2020-11-19 00:00:00.000',NULL,1,6,'Wood-carver','Holzbildhauer','Sculpteur sur bois' UNION ALL
select 950,100,'Керамик',NULL,'215050',1,'2020-11-19 00:00:00.000',NULL,1,6,'Ceramist','Keramiker','Sculpteur céramique' UNION ALL
select 951,100,'Каменоделец',NULL,'215060',1,'2020-11-19 00:00:00.000',NULL,1,6,'Stone-cutter','Steinbildhauer','Tailleur de pierre' UNION ALL
select 952,100,'Художник - приложни изкуства',NULL,'215070',1,'2020-11-19 00:00:00.000',NULL,1,6,'Artist - Fine arts','Maler - Angewandte Kunst','Artiste - arts appliqués' UNION ALL
select 953,101,'Църковнослужител. Свещенослужител',NULL,'221010',1,'2020-11-19 00:00:00.000',NULL,1,6,'Church official Cleric','Geistlicher Priester','Employé d’église Prêtre' UNION ALL
select 954,123,'Продавач - консултант',NULL,'341020',1,'2020-11-19 00:00:00.000',NULL,1,5,'Shop-assistant','Verkaufsberater','Conseiller de vente' UNION ALL
select 955,123,'Брокер',NULL,'341030',1,'2020-11-19 00:00:00.000',NULL,1,5,'Broker','Makler','Courtier' UNION ALL
select 956,123,'Търговски представител',NULL,'341040',1,'2020-11-19 00:00:00.000',NULL,1,5,'Sales representative','Handels- vertreter','Représentant commercial' UNION ALL
select 957,40,'Сътрудник в маркетингови дейности',NULL,'342020',1,'2020-11-19 00:00:00.000',NULL,1,5,'Assistant in marketing activities','Marketing-Mitarbeiter','Assistant aux activités marketing' UNION ALL
select 958,102,'Данъчен и митнически посредник',NULL,'344020',1,'2020-11-19 00:00:00.000',NULL,1,5,'Tax and customs broker','Steuer- und Zollvermittler','Intermédiaire fiscal et douanier' UNION ALL
select 959,102,'Оперативен счетоводител',NULL,'344030',1,'2020-11-19 00:00:00.000',NULL,1,5,'Operating accountant','Operativer Buchhalter','Comptable opérationnel' UNION ALL
select 960,103,'Фирмен мениджър',NULL,'345020',1,'2020-11-19 00:00:00.000',NULL,1,5,'Company manager','Firmen-leiter','Manager d’entreprise' UNION ALL
select 961,103,'Сътрудник в бизнес - услуги',NULL,'345040',1,'2020-11-19 00:00:00.000',NULL,1,5,'Business services associate','Mitarbeiter im Bereich Geschäftsdienstleistungen','Assistant aux services aux entreprises' UNION ALL
select 962,103,'Сътрудник в малък и среден бизнес',NULL,'345050',1,'2020-11-19 00:00:00.000',NULL,1,5,'Small and medium business associate','Mitarbeiter Klein- und Mittelbetriebe','Assistant aux petites et moyennes entreprises' UNION ALL
select 963,103,'Касиер',NULL,'345060',1,'2020-11-19 00:00:00.000',NULL,1,5,'Cashier','Kassierer','Caissier' UNION ALL
select 964,103,'Калкулант',NULL,'345070',1,'2020-11-19 00:00:00.000',NULL,1,5,'Calculator','Kalkulant','Calculant' UNION ALL
select 965,103,'Снабдител',NULL,'345080',1,'2020-11-19 00:00:00.000',NULL,1,5,'Supplier','Einkäufer','Fournisseur' UNION ALL
select 967,103,'Икономист',NULL,'345120',1,'2020-11-19 00:00:00.000',NULL,1,5,'Economist','Ökonom','Économiste' UNION ALL
select 968,104,'Офис мениджър',NULL,'346010',1,'2020-11-19 00:00:00.000',NULL,1,5,'Office manager','Büro-leiter','Manager de bureau' UNION ALL
select 969,104,'Офис - секретар',NULL,'346020',1,'2020-11-19 00:00:00.000',NULL,1,5,'Office secretary','Büroassistent','Assistant de bureau' UNION ALL
select 970,104,'Деловодител',NULL,'346030',1,'2020-11-19 00:00:00.000',NULL,1,5,'Clerk','Sachbearbeiter','Secrétaire' UNION ALL
select 971,105,'Програмист',NULL,'481010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Programmer','Programmierer','Programmeur' UNION ALL
select 972,105,'Системен програмист',NULL,'481020',1,'2020-11-19 00:00:00.000',NULL,1,3,'System programmer','Systemprogrammierer','Programmeur de système' UNION ALL
select 973,106,'Икономист - информатик',NULL,'482010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Economist - computer scientist','Wirtschaftsinformatiker','Économiste-informaticien' UNION ALL
select 974,106,'Оператор информационно осигуряване',NULL,'482020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Information provision operator','Bediener für Informationsbereitstellung','Opérateur de sécurité de l''information' UNION ALL
select 975,106,'Оператор на компютър',NULL,'482030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Computer operator','Computerbediener','Opérateur d''ordinateur' UNION ALL
select 976,106,'Организатор Интернет приложения',NULL,'482040',1,'2020-11-19 00:00:00.000',NULL,1,3,'Organizer of Internet applications ','Organisator für Internet-anwendungen','Organisateur d’applications Internet' UNION ALL
select 977,107,'Машинен техник',NULL,'521010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Mechanical technician','Maschinentechniker','Technicien de machines' UNION ALL
select 978,107,'Техник - приложник',NULL,'521020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician-applied works','Anwendungs-techniker','Technicien- applicateur' UNION ALL
select 979,107,'Машинен оператор',NULL,'521030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Machine operator','Maschinenbediener','Opérateur des machines' UNION ALL
select 980,107,'Машинен монтьор',NULL,'521040',1,'2020-11-19 00:00:00.000',NULL,1,3,'Mechanical fitter','Maschinenmonteur','Monteur de machines' UNION ALL
select 981,107,'Техник на прецизна техника',NULL,'521050',1,'2020-11-19 00:00:00.000',NULL,1,3,'Precise equipment technician','Feinmechaniker','Technicien d''équipements de précision' UNION ALL
select 982,107,'Монтьор на прецизна техника',NULL,'521060',1,'2020-11-19 00:00:00.000',NULL,1,3,'Precise equipment fitter','Monteur für Präzisionstechnik','Monteur d''équipements de précision' UNION ALL
select 983,107,'Техник - металург',NULL,'521070',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician - metallurgist ','Techniker für Metallurgie','Technicien métallurgiste' UNION ALL
select 984,107,'Оператор в металургията',NULL,'521080',1,'2020-11-19 00:00:00.000',NULL,1,3,'Operator in metallurgy','Bediener in der Metallurgie','Opérateur à la métallurgie' UNION ALL
select 985,107,'Заварчик',NULL,'521090',1,'2020-11-19 00:00:00.000',NULL,1,3,'Welder','Schweißer','Soudeur' UNION ALL
select 986,107,'Стругар',NULL,'521100',1,'2020-11-19 00:00:00.000',NULL,1,3,'Turner','Dreher','Tourneur' UNION ALL
select 987,107,'Шлосер',NULL,'521110',1,'2020-11-19 00:00:00.000',NULL,1,3,'Fitter','Schlosser','Serrurier' UNION ALL
select 988,107,'Леяр',NULL,'521120',1,'2020-11-19 00:00:00.000',NULL,1,3,'Founder','Gießer','Fondeur' UNION ALL
select 989,107,'Ковач',NULL,'521130',1,'2020-11-19 00:00:00.000',NULL,1,3,'Blacksmith','Schmied','Forgeron' UNION ALL
select 990,108,'Техник по комуникационни системи',NULL,'523010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician of communication systems','Techniker für Kommunikationssysteme','Technicien en systèmes de communication' UNION ALL
select 991,108,'Монтьор по комуникационни системи',NULL,'523020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Fitter of communication systems','Monteur für Kommunikationssysteme','Installateur de systèmes de communication' UNION ALL
select 992,108,'Техник на електронна техника',NULL,'523030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician of electronic equipment','Elektroniker','Technicien de technique électronique' UNION ALL
select 993,108,'Монтьор на електронна техника',NULL,'523040',1,'2020-11-19 00:00:00.000',NULL,1,3,'Fitter of electronic equipment','Installateur von elektronischen Geräten','Installateur d''équipements électroniques' UNION ALL
select 994,108,'Техник на компютърни системи',NULL,'523050',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician of computer systems','Computersystemtechniker','Technicien de systèmes informatiques' UNION ALL
select 995,108,'Монтьор на компютърни системи',NULL,'523060',1,'2020-11-19 00:00:00.000',NULL,1,3,'Fitter of computer systems','Computersysteminstallateur','Installateur de systèmes informatiques' UNION ALL
select 996,108,'Техник по автоматизация',NULL,'523070',1,'2020-11-19 00:00:00.000',NULL,1,3,'Automation technician','Techniker für Automatisierung','Technicien en automatisation' UNION ALL
select 997,108,'Монтьор по автоматизация',NULL,'523080',1,'2020-11-19 00:00:00.000',NULL,1,3,'Automation fitter','Automatisierungsmonteur','Monteur d''automatismes' UNION ALL
select 998,108,'Проектант компютърни мрежи',NULL,'523100',1,'2020-11-19 00:00:00.000',NULL,1,3,'Computer networks designer','Entwerfer von Computernetzen','Concepteur de réseaux informatiques' UNION ALL
select 999,109,'Техник по транспортна техника',NULL,'525010',0,'2020-11-19 00:00:00.000',NULL,1,1,NULL,NULL,NULL UNION ALL
select 1000,109,'Монтьор на транспортна техника',NULL,'525020',1,'2020-11-19 00:00:00.000',NULL,1,1,'Fitter of transport equipment','Monteur von Transporttechnik','Monteur des engins de transport' UNION ALL
select 1001,109,'Техник по подемно - транспортна техника',NULL,'525050',1,'2020-11-19 00:00:00.000',NULL,1,1,'Technician of hoisting and lifting equipment','Techniker für Hebe-und Transporttechnik','Technicien en technique de levage et de transport' UNION ALL
select 1002,109,'Монтьор на подемно - транспортна техника',NULL,'525060',1,'2020-11-19 00:00:00.000',NULL,1,1,'Fitter of hoisting transport equipment','Monteur von Hebe- und Transporttechnik','Installateur de matériel de levage et de transport' UNION ALL
select 1003,109,'Техник по железопътна техника',NULL,'525070',1,'2020-11-19 00:00:00.000',NULL,1,1,'Technician of railway equipment','Eisenbahntechniker','Technicien de matériel ferroviaire' UNION ALL
select 1004,109,'Монтьор на железопътна техника',NULL,'525080',1,'2020-11-19 00:00:00.000',NULL,1,1,'Fitter of railway equipment','Monteur von Eisenbahntechnik','Installateur d’équipement électrique du matériel ferroviaire' UNION ALL
select 1005,109,'Авиационен техник',NULL,'525090',1,'2020-11-19 00:00:00.000',NULL,1,1,'Aviation technician','Luftfahrttechniker','Technicien aéronautique' UNION ALL
select 1006,109,'Корабен техник',NULL,'525100',1,'2020-11-19 00:00:00.000',NULL,1,1,'Marine technician','Schiffstechniker','Technicien de navire' UNION ALL
select 1007,109,'Корабен монтьор',NULL,'525110',1,'2020-11-19 00:00:00.000',NULL,1,1,'Ship fitter','Schiffsmonteur','Monteur de navires' UNION ALL
select 1009,109,'Работник по транспортна техника',NULL,'525120',1,'2020-11-19 00:00:00.000',NULL,1,1,'Transport equipment worker','Arbeiter für Transporttechnik','Ouvrier en matériel de transport' UNION ALL
select 1011,109,'Пристанищен работник',NULL,'525130',1,'2020-11-19 00:00:00.000',NULL,1,1,'Docker','Hafenarbeiter','Ouvrier portuaire' UNION ALL
select 1012,109,'Корабостроителен техник',NULL,'525140',1,'2020-11-19 00:00:00.000',NULL,1,1,'Shipbuilding technician','Schiffbautechniker','Technicien en construction navale' UNION ALL
select 1013,110,'Техник - технолог в хранително - вкусовата промишленост',NULL,'541010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician - technologist in food industry','Techniker-Technologe in der Lebensmittel-industrie','Technicien-technologue en industrie' UNION ALL
select 1014,110,'Оператор в хранително - вкусовата промишленост',NULL,'541020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Operator in the food industry ','Bediener in der Lebensmittel-industrie','Opérateur dans industrie' UNION ALL
select 1015,110,'Хлебар - сладкар',NULL,'541030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Baker - confectioner ','Bäcker-Konditor','Boulanger-pâtissier' UNION ALL
select 1016,110,'Техник - технолог по експлоатация и поддържане на хладилната техника в хранителната промишленост',NULL,'541040',0,'2020-11-19 00:00:00.000',NULL,1,3,NULL,NULL,NULL UNION ALL
select 1017,110,'Работник в хранително - вкусовата промишленост',NULL,'541050',1,'2020-11-19 00:00:00.000',NULL,1,3,'Worker in the food industry','Arbeiter in der Lebensmittel-industrie','Opérateur dans industrie' UNION ALL
select 1018,110,'Техник - технолог по качеството на храни и напитки',NULL,'541060',1,'2020-11-19 00:00:00.000',NULL,1,3,'Quality assurance technician - technologist of foods and beverages','Techniker-Technologe für Lebensmittel- und Getränkequalitätssicherung','Technicien-technologue pour la qualité des aliments et des boissons' UNION ALL
select 1019,111,'Десенатор на текстил',NULL,'542010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Textile designer','Textildesigner','Dessinateur textile' UNION ALL
select 1020,111,'Текстилен техник',NULL,'542020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Textile technician','Textiltechniker','Technicien textile' UNION ALL
select 1021,111,'Оператор в текстилно производство',NULL,'542030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Operator in the textile industry','Bediener in Textil-produktion','Opérateur dans la production' UNION ALL
select 1022,111,'Моделиер - технолог на облекло',NULL,'542040',1,'2020-11-19 00:00:00.000',NULL,1,3,'Clothes designer - technologist','Modedesigner - Bekleidungstechnologe','Couturier de mode - technologue en vêtements' UNION ALL
select 1023,111,'Оператор в производството на облекло',NULL,'542050',1,'2020-11-19 00:00:00.000',NULL,1,3,'Operator in the production of clothes','Bediener in Bekleidungsproduktion','Opérateur à fabrication de vêtements' UNION ALL
select 1024,111,'Моделиер - технолог на обувни и кожено - галантерийни изделия',NULL,'542060',1,'2020-11-19 00:00:00.000',NULL,1,3,'Designer - technologist of shoe and leather haberdasher articles','Modedesigner - Technologe für Schuhe und Lederwaren','Couturier - technologue de la chaussure et de la maroquinerie' UNION ALL
select 1025,111,'Оператор в производство на обувни и кожено - галантерийни изделия',NULL,'542070',1,'2020-11-19 00:00:00.000',NULL,1,3,'Operator in the production of shoe and leather haberdasher articles','Bediener in Produktion von Schuhen und Lederwaren','Opérateur à production de chaussures et de la maroquinerie' UNION ALL
select 1026,111,'Работник в текстилно производство',NULL,'542080',1,'2020-11-19 00:00:00.000',NULL,1,3,'Operator in the textile industry','Arbeiter in Textil-industrie','Ouvrier à la production' UNION ALL
select 1027,111,'Работник в производство на облекло',NULL,'542090',1,'2020-11-19 00:00:00.000',NULL,1,3,'Worker in the production of clothes','Arbeiter in Bekleidungsproduktion','Travailleur à la confection de vêtements' UNION ALL
select 1028,111,'Работник в обувно и кожено - галантерийно производство',NULL,'542100',1,'2020-11-19 00:00:00.000',NULL,1,3,'Worker in the shoe and leather haberdasher industry','Arbeiter in Schuh- und Lederwareniproduktion ','Ouvrier en production de chaussures et de la maroquinerie' UNION ALL
select 1029,111,'Шивач',NULL,'542110',1,'2020-11-19 00:00:00.000',NULL,1,3,'Tailor','Schneider','Tailleur' UNION ALL
select 1030,111,'Обущар',NULL,'542120',1,'2020-11-19 00:00:00.000',NULL,1,3,'Shoemaker','Schuhmacher','Cordonnier' UNION ALL
select 1031,111,'Килимар',NULL,'542130',1,'2020-11-19 00:00:00.000',NULL,1,3,'Carpet weaver','Teppichweber','Artisan en tapis' UNION ALL
select 1032,111,'Плетач',NULL,'542140',1,'2020-11-19 00:00:00.000',NULL,1,3,'Knitter','Stricker','Tricoteur' UNION ALL
select 1033,111,'Бродировач',NULL,'542150',1,'2020-11-19 00:00:00.000',NULL,1,3,'Embroiderer','Sticker','Brodeur' UNION ALL
select 1034,112,'Техник - технолог в дървообработването',NULL,'543010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician-technologist in wood-processing','Techniker-Technologe in der Holzverarbeitung','Technicien-technologue à traitement du bois' UNION ALL
select 1035,112,'Оператор в дървообработването',NULL,'543020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Operator in wood-processing','Bediener in der Holzverarbeitung ','Opérateur au traitement du bois' UNION ALL
select 1036,112,'Работник в дървообработването',NULL,'543090',1,'2020-11-19 00:00:00.000',NULL,1,3,'Worker in wood-processing','Arbeiter in der Holzverarbeitung','Ouvrier au traitement du bois' UNION ALL
select 1037,112,'Организатор в дървообработването и производството на мебели',NULL,'543100',1,'2020-11-19 00:00:00.000',NULL,1,3,'Operator (organizer) in wood-processing and furniture production','Organisator in der Holzverarbeitung und Möbelproduktion','Organisateur en travail du bois et à la fabrication de meubles' UNION ALL
select 1038,113,'Минен техник',NULL,'544010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Mining technician','Bergbautechniker','Technicien minier' UNION ALL
select 1039,113,'Оператор в минната промишленост',NULL,'544020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Operator in the mining industry','Bediener in der Bergbau-industrie','Opérateur dans industrie' UNION ALL
select 1040,113,'Сондажен техник',NULL,'544030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Drilling technician','Bohrtechniker','Technicien de forage' UNION ALL
select 1041,113,'Сондьор',NULL,'544040',1,'2020-11-19 00:00:00.000',NULL,1,3,'Driller','Bohrer','Sondeur' UNION ALL
select 1042,113,'Маркшайдер',NULL,'544050',1,'2020-11-19 00:00:00.000',NULL,1,3,'Markschreider','Markscheider','Géomètre minier' UNION ALL
select 1043,114,'Геодезист',NULL,'581010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Geodesist','Geodät','Géodésien' UNION ALL
select 1044,115,'Строителен техник',NULL,'582010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Construction technician','Bautechniker','Technicien en construction' UNION ALL
select 1045,115,'Строител',NULL,'582030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Builder','Bauarbeiter','Constructeur' UNION ALL
select 1046,115,'Строител – монтажник',NULL,'582040',1,'2020-11-19 00:00:00.000',NULL,1,3,'Constructor - assembler','Erbauer-Monteur','Constructeur-installateur' UNION ALL
select 1047,115,'Монтажник на водоснабдителни и канализационни мрежи',NULL,'582050',1,'2020-11-19 00:00:00.000',NULL,1,3,'Assembler of water supply and sewerage networks','Installateur von Wasserversorgungs- und Kanalisationsnetzen','Installateur de réseaux d''approvisionnement d''eau et d''assainissement' UNION ALL
select 1048,115,'Пътен строител',NULL,'582060',1,'2020-11-19 00:00:00.000',NULL,1,3,'Road builder','Straßenbau','Constructeur routier' UNION ALL
select 1049,115,'Пещостроител',NULL,'582070',1,'2020-11-19 00:00:00.000',NULL,1,3,'Furnace builder','Ofenbauer','Constructeur de fours' UNION ALL
select 1050,115,'Помощник в строителството',NULL,'582080',1,'2020-11-19 00:00:00.000',NULL,1,3,'Assistant in construction','Hilfskraft im Bau','Aide à la construction' UNION ALL
select 1051,115,'Помощник пътен строител',NULL,'582090',1,'2020-11-19 00:00:00.000',NULL,1,3,'Assistant road builder','Hilfsstraßenbauer','Aide constructeur de route' UNION ALL
select 1052,116,'Техник-растениевъд',NULL,'621010',1,'2020-11-19 00:00:00.000',NULL,1,2,'Technician - plant grower','Techniker-Pflanzenzüchter','Technicien - producteur de plantes' UNION ALL
select 1053,116,'Техник в лозаровинарството',NULL,'621020',1,'2020-11-19 00:00:00.000',NULL,1,2,'Technician in vine-growing','Techniker für Weinbau und Weinproduktion','Technicien en viticulture' UNION ALL
select 1054,116,'Растениевъд',NULL,'621030',1,'2020-11-19 00:00:00.000',NULL,1,2,'Plant grower','Pflanzenzüchter','Producteur de plantes' UNION ALL
select 1055,116,'Техник-животновъд',NULL,'621040',1,'2020-11-19 00:00:00.000',NULL,1,2,'Technician - animal breeder','Viehtechniker','Technicien - élеveur' UNION ALL
select 1056,116,'Животновъд',NULL,'621050',1,'2020-11-19 00:00:00.000',NULL,1,2,'Animal breeder','Viehzüchter','Éleveur' UNION ALL
select 1057,116,'Фермер',NULL,'621060',1,'2020-11-19 00:00:00.000',NULL,1,2,'Farmer','Farmer','Fermier' UNION ALL
select 1058,116,'Техник на селскостопанска техника',NULL,'621070',1,'2020-11-19 00:00:00.000',NULL,1,2,'Technician of agricultural equipment','Landmaschinentechniker','Technicien de technique agricole' UNION ALL
select 1059,116,'Монтьор на селскостопанска техника',NULL,'621080',1,'2020-11-19 00:00:00.000',NULL,1,2,'Fitter of agricultural equipment','Monteur landwirtschaftlicher Maschinen ','Installateur d’équipement agricole' UNION ALL
select 1060,116,'Лозаровинар',NULL,'621090',1,'2020-11-19 00:00:00.000',NULL,1,2,'Vine-grower','Weinbauer','Viticulteur' UNION ALL
select 1061,116,'Работник в растениевъдството',NULL,'621110',1,'2020-11-19 00:00:00.000',NULL,1,2,'Worker in plant growing','Pflanzenanbauarbeiter','Ouvrier à la culture de plantes' UNION ALL
select 1062,116,'Работник в животновъдството',NULL,'621120',1,'2020-11-19 00:00:00.000',NULL,1,2,'Worker in animal breeding','Viehzuchtarbeiter','Ouvrier à l’élevage' UNION ALL
select 1063,116,'Кинолог',NULL,'621130',1,'2020-11-19 00:00:00.000',NULL,1,2,'Cynologist','Kynologe','Cynologue' UNION ALL
select 1064,117,'Техник – озеленител',NULL,'622010',1,'2020-11-19 00:00:00.000',NULL,1,2,'Technician - landscaper','Landschaftsbautechniker','Technicien d’aménagement paysager' UNION ALL
select 1065,117,'Озеленител',NULL,'622020',1,'2020-11-19 00:00:00.000',NULL,1,2,'Landscaper','Landschaftsgestalter','Paysagiste' UNION ALL
select 1066,117,'Работник в озеленяването',NULL,'622030',1,'2020-11-19 00:00:00.000',NULL,1,2,'Worker in landscaping','Landschaftbauarbeiter','Ouvrier à l’aménagement paysager' UNION ALL
select 1067,118,'Техник по очна оптика',NULL,'725010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Eye optics technician','Techniker für Augenoptik','Technicien en optique oculaire' UNION ALL
select 1069,118,'Оптик - оптометрист',NULL,'725020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Optician - optometrist','Augenoptiker','Opticien-optométriste' UNION ALL
select 1070,118,'Техник по ортопедична техника',NULL,'725030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician of orthopedic equipment','Orthopädietechniker','Technicien orthopédiste' UNION ALL
select 1071,118,'Техник на слухови апарати',NULL,'725040',1,'2020-11-19 00:00:00.000',NULL,1,3,'Hearing aid technician','Hörgerätetechniker','Technicien en audioprothèse' UNION ALL
select 1072,119,'Посредник на трудовата борса',NULL,'762010',1,'2020-11-19 00:00:00.000',NULL,1,5,'Intermediary on the labour market','Vermittler im Arbeitsamt','Intermédiaire dans la bourse du travail' UNION ALL
select 1074,119,'Сътрудник социални дейности',NULL,'762020',1,'2020-11-19 00:00:00.000',NULL,1,5,'Associate in social activities','Mitarbeiter für soziale Tätigkeiten','Assistant aux activités sociales' UNION ALL
select 1075,119,'Помощник - възпитател',NULL,'762030',1,'2020-11-19 00:00:00.000',NULL,1,5,'Assistant teacher','Hilfserzieher','Assistant-éducateur' UNION ALL
select 1076,119,'Социален асистент',NULL,'762040',1,'2020-11-19 00:00:00.000',NULL,1,5,'Social assistant','Sozialassistent','Assistant social' UNION ALL
select 1077,119,'Преводач жестомимичен език',NULL,'762050',1,'2020-11-19 00:00:00.000',NULL,1,5,'Translator from and into Bulgarian sign language','Dolmetscher für die bulgarische Gebärdensprache','Traducteur de et vers la langue des signes bulgare' UNION ALL
select 1078,65,'Аниматор в туризма',NULL,'812040',1,'2020-11-19 00:00:00.000',NULL,1,4,'Animator in tourism','Unterhalter im Tourismus','Animateur en tourisme' UNION ALL
select 1079,66,'Инструктор по ергономия',NULL,'813070',1,'2020-11-19 00:00:00.000',NULL,1,4,'Ergonomics instructor','Ausbilder für Ergonomie ','Instructeur en ergonomie' UNION ALL
select 1080,66,'Организатор на спортни прояви и първенства',NULL,'813080',1,'2020-11-19 00:00:00.000',NULL,1,4,'Organizer of sports events and championships','Organisator von Sportveranstaltungen und Meisterschaften','Organisateur d''événements sportifs et de championnats' UNION ALL
select 1081,66,'Помощник - инструктор по фитнес',NULL,'813090',1,'2020-11-19 00:00:00.000',NULL,1,4,'Assistant fitness instructor','Assistenztrainer für Fitness','Aide instructeur de remise en forme physique' UNION ALL
select 1082,66,'Помощник - треньор',NULL,'813100',1,'2020-11-19 00:00:00.000',NULL,1,4,'Assistant coach','Assistenztrainer','Entraîneur adjoint' UNION ALL
select 1083,67,'Организатор на обредно-ритуални дейности',NULL,'814030',1,'2020-11-19 00:00:00.000',NULL,1,4,'Organizer of ceremonial and ritual activities','Organisator von rituellen Aktivitäten','Organisateur d''activités rituelles' UNION ALL
select 1084,120,'Еколог',NULL,'851010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Ecologist','Ökologe','Écologue' UNION ALL
select 1085,120,'Консултант на опасни товари',NULL,'851020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Consultant of hazardous freights','Gefahrgutberater','Conseiller en marchandises dangereuses' UNION ALL
select 1086,121,'Охранител',NULL,'861010',1,'2020-11-19 00:00:00.000',NULL,1,4,'Security guard','Bewacher','Agent de sécurité' UNION ALL
select 1087,121,'Спасител при бедствия, аварии и катастрофи',NULL,'861020',1,'2020-11-19 00:00:00.000',NULL,1,4,'Rescuer in disasters, breakdowns and accidents','Rettungskraft bei Naturkatastrophen, Unfällen und Katastrophen','Sauveur dans des catastrophes naturelles, avaries et accidents routiers' UNION ALL
select 1088,124,'Библиотекар',NULL,'322010',1,'2020-11-19 00:00:00.000',NULL,1,5,'Librarian','Bibliothekar','Bibliothécaire' UNION ALL
select 1089,122,'Режисьор',NULL,'213100',1,'2020-11-19 00:00:00.000',NULL,1,6,'Director','Regisseur','Réalisateur' UNION ALL
select 1090,122,'Оператор',NULL,'213110',1,'2020-11-19 00:00:00.000',NULL,1,6,'Cinematographer:','Kameramann','Employé' UNION ALL
select 1091,107,'Мехатроника',NULL,'521140',1,'2020-11-19 00:00:00.000',NULL,1,3,'Mechatronics','Mechatronik','Mécatronique' UNION ALL
select 1092,109,'Техник по транспортна техника',NULL,'525010',1,'2020-11-19 00:00:00.000',NULL,1,1,'Technician of transport equipment','Techniker für Transportmittel','Technicien en matériel de transport' UNION ALL
select 1093,110,'Техник - технолог по експлоатация и поддържане на хладилна и климатична техника в хранително-вкусовата промишленост',NULL,'541040',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician - technologist in exploitation and maintenance of refrigerating and air-conditioning equipment in food industry','Techniker-Technologe für Betrieb und Wartung von Kälte- und Klimatechnik in der Lebensmittel-industrie','Technicien-technologue pour l''exploitation et l''entretien de la technique de réfrigération dans industrie' UNION ALL
select 1094,116,'Агроеколог',NULL,'621140',1,'2020-11-19 00:00:00.000',NULL,1,2,'Agroecologist','Agrarökologe','Agroécologue' UNION ALL
select 1095,69,'Куриер',NULL,'840100',1,'2020-11-19 00:00:00.000',NULL,1,1,'Courier','Kurier','Courrier' UNION ALL
select 1096,125,'Сътрудник по управление на индустриални отношения',NULL,'347010',1,'2020-11-19 00:00:00.000',NULL,1,5,'Industrial relations management associate','Mitarbeiter im Bereich Arbeitsbeziehungsmanagement','Assistant en gestion des relations industrielles' UNION ALL
select 1097,126,'Здравен асистент',NULL,'723010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Health assistant','Gesundheitsassistent','Assistant de santé' UNION ALL
select 1098,126,'Болногледач',NULL,'723020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Hospital attendant','Krankenpfleger','Aide-soignant' UNION ALL
select 1099,127,'Изпълнител на термални процедури',NULL,'726010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Performer of thermal procedures','Durchführer von Thermal-behandlungsprozeduren','Exécuteur de procédures thermales' UNION ALL
select 1100,68,'Маникюрист-педикюрист',NULL,'815030',1,'2020-11-19 00:00:00.000',NULL,1,4,'Manicurist - pedicurist','Hand-Fußpfleger','Manucuriste-pédicuriste' UNION ALL
select 1101,126,'Парамедик',NULL,'723030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Paramedic','Rettungssanitäter','Personnel paramédical' UNION ALL
select 1102,119,'Приемен родител',NULL,'762060',1,'2020-11-19 00:00:00.000',NULL,1,5,'Foster parent','Pflegeelternteil','Parent adoptif' UNION ALL
select 1103,68,'Инструктор',NULL,'815040',1,'2020-11-19 00:00:00.000',NULL,1,4,'Instructor','Ausbilder','Instructeur' UNION ALL
select 1104,48,'Оператор на парни и водогрейни съоръжения',NULL,'522050',1,'2020-11-19 00:00:00.000',NULL,1,3,'Operator of steam and water heating facilities','Bediener von Dampf- und Heißwasseranlagen ','Opérateur des installations de vapeur et de chauffage d''eau' UNION ALL
select 1105,122,'Дисководещ',NULL,'213150',1,'2020-11-19 00:00:00.000',NULL,1,6,'DJ','Diskjockey','Disc-jockey' UNION ALL
select 1106,128,'Асистент на лекар по дентална медицина',NULL,'724010',1,'2020-11-19 00:00:00.000',NULL,1,3,'Assistant of dental physician','Zahnarztassistent','Assistant du médecin dentiste' UNION ALL
select 1107,114,'Реставратор-изпълнител',NULL,'581020',1,'2020-11-19 00:00:00.000',NULL,1,3,'Restorer-performer','Restaurator','Restaurateur-exécuteur' UNION ALL
select 1108,114,'Техник-реставратор',NULL,'581030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Technician restorer','Restaurierungstechniker','Technicien en restauration' UNION ALL
select 1109,64,'Карвинг-декоратор',NULL,'811110',1,'2020-11-19 00:00:00.000',NULL,1,4,'Carving decorator','Carving-Dekorateur','Carving décorateur' UNION ALL
select 1110,64,'Инструктор-декоратор',NULL,'811120',1,'2020-11-19 00:00:00.000',NULL,1,4,'Instructor - decorator','Ausbilder-Dekorateur','Instructeir-dcorateur' UNION ALL
select 1112,69,'Спедитор - логистик',NULL,'840110',1,'2020-11-19 00:00:00.000',NULL,1,1,'Forwarder - logistician ','Spediteur-Logistiker','Expéditeur-logistique' UNION ALL
select 1113,105,'Приложен програмист',NULL,'481030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Applied programmer','Anwendungsprogrammierer','Développeur d''applications' UNION ALL
select 1114,111,'Модист',NULL,'542041',1,'2020-11-19 00:00:00.000',NULL,1,3,'Fashion designer','Modist','Modiste' UNION ALL
select 1115,104,'Съдебен служител',NULL,'346040',1,'2020-11-19 00:00:00.000',NULL,1,5,'Court employee','Justizbeamte','Officier de justice' UNION ALL
select 1116,98,'Военен оркестрант',NULL,'863060',0,'2020-11-19 00:00:00.000',NULL,1,-1,NULL,NULL,NULL UNION ALL
select 1117,50,'Консултант козметични, парфюмерийни, биологични продукти и битова химия',NULL,'524030',1,'2020-11-19 00:00:00.000',NULL,1,3,'Consultant of cosmetic, perfumery, biological products and household chemistry','Berater Kosmetik, Parfümerie, Bio-Produkte und Haushaltschemie ','Conseillère en cosmétique, parfums, produits biologiques et produits chimiques ménagers' UNION ALL
select 1118,119,'Преводач от и на български жестов език',NULL,'762050',1,'2020-11-19 00:00:00.000',NULL,1,5,'Translator from and into Bulgarian sign language','Dolmetscher für die bulgarische Gebärdensprache','Traducteur de et vers la langue des signes bulgare' UNION ALL
select 1119,105,'Програмист на изкуствен интелект','','481040',1,'2021-04-13 00:00:00.000',NULL,1,3,'Artificial intelligence programmer','KI-Programmierer','Programmeur intelligence artificielle' UNION ALL
select 1120,105,'Програмист на роботи','','481050',1,'2021-04-13 00:00:00.000',NULL,1,3,'Robot programmer','Roboterprogrammierer','Programmeur de robots' UNION ALL
select 1121,98,'Сержант (Старшина за ВМС) - киберсигурност и кибероперации','','863080',1,'2021-01-04 00:00:00.000',NULL,1,4,'Sergeant (sergeant-major for the navy) - Cybersecurity and cyberoperations','Sergeant (Sergeant für die Seestreitkräfte) - Cybersicherheit und Cyberoperationen','Sergent (maître de la marine) cybersécurité et cyberopérations';

GO
