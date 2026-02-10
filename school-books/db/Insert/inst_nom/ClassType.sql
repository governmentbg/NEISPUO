GO
PRINT 'Insert [inst_nom].[ClassType]'
GO

insert [inst_nom].[ClassType] ([ClassTypeID],[Name],[Description],[IsValid],[ValidFrom],[ValidTo],[ClassKind],[NameEN],[NameDE],[NameFR])
select 1,'Ранно чуждоезиково обучение','РЧЕО',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 2,'РИОП: музика','РИ муз',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 3,'РИОП: хореография','РИ хор',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 4,'РИОП: изобр.изкуство','РИ из',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 5,'Общообразователна','ООП',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 6,'Специализирана','СП',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 10,'Непрофилирана','НЕПР',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 11,'профил: Чуждоезиков','ПР-ЧЕ',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 12,'профил: Природоматематически','ПР-ПМ',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 13,'профил: Хуманитарен','ПР-Х',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 14,'профил: Изкуства','ПР-И',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 15,'профил: Спорт','ПР-С',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 16,'профил: Технологичен','ПР-Т',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 17,'За деца с множество увреждания','ПДМУ',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 20,'Професионална','Проф',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 21,'Профил: Чужди езици','ПР-ЧЕ',1,'2021-01-28 00:00:00.000',NULL,1,'Profile: Foreign languages','Schwerpunkt: Fremdsprachen','Profil : Langues étrangères' UNION ALL
select 22,'Профил: Хуманитарни науки','ПР-ХН',1,'2021-01-28 00:00:00.000',NULL,1,'Profile: Humanities','Schwerpunkt: Humanistische Wissenschaften','Profil : Sciences humaines' UNION ALL
select 23,'Профил: Обществени науки','ПР-ОН',1,'2021-01-28 00:00:00.000',NULL,1,'Profile: Social Sciences','Schwerpunkt: Sozialwissenschaften','Profil : Sciences sociales' UNION ALL
select 24,'Профил: Икономическо развитие','ПР-ИР',1,'2021-01-28 00:00:00.000',NULL,1,'Profile: Economic Development','Schwerpunkt: Wirtschaftsentwicklung','Profil : Développement économique' UNION ALL
select 25,'Профил: Софтуерни и хардуерни науки','ПР-СХН',1,'2021-01-28 00:00:00.000',NULL,1,'Profile: Software and Hardware Sciences','Schwerpunkt: Software- und Hardwarewissenschaften','Profil : Sciences du logiciel et du matériel' UNION ALL
select 26,'Профил: Математически','ПР-ПМ',1,'2021-01-28 00:00:00.000',NULL,1,'Profile: Mathematics','Schwerpunkt: Mathematik','Profil : Mathématiques' UNION ALL
select 27,'Профил: Природни науки','ПР-ПН',1,'2021-01-28 00:00:00.000',NULL,1,'Profile: Natural Sciences','Schwerpunkt: Naturwissenschaften','Profil : Sciences Naturelles' UNION ALL
select 28,'Профил: Изобразително изкуство','ПР-ИИ',1,'2021-01-28 00:00:00.000',NULL,1,'Profile: Fine Arts','Schwerpunkt: Bildende Kunst','Profil : Beaux-Arts' UNION ALL
select 29,'Профил: Музика','ПР-М',1,'2021-01-28 00:00:00.000',NULL,1,'Profile: Music','Schwerpunkt: Musik','Profil : Musique' UNION ALL
select 30,'Профил: Физическо възпитание и спорт','ПР-ФВС',1,'2021-01-28 00:00:00.000',NULL,1,'Profile: Physical Education and Sport','Schwerpunkt: Körpererziehung und Sportunterricht','Profil : Éducation physique et sportive' UNION ALL
select 31,'Профил: Предприемачески','ПР-ПРД',1,'2021-01-28 00:00:00.000',NULL,1,'Profile: Entrepreneurship','Schwerpunkt: Unternehmertum','Profil : Entrepreneuriat' UNION ALL
select 32,'Профил: Изобразително изкуство ПДМУ','ПР-ИИ ПДМУ',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 33,'Профил: Музика ПДМУ','ПР-М ПДМУ',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 34,'Полуинтернатна група','ПИГ',0,'2021-01-28 00:00:00.000',NULL,2,NULL,NULL,NULL UNION ALL
select 35,'Група в общежитие / интернат','гр. ОИ',0,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 36,'Логопедична група','ЛГ',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 37,'Група за ресурсно подпомагане','РПГ',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 38,'Група за целодневна организация','ЦДО',1,'2021-01-28 00:00:00.000',NULL,2,NULL,NULL,NULL UNION ALL
select 39,'Група в общежитие','ОБЩЕЖ',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 40,'Група за дейности за ПЛР','ПЛР',1,'2022-08-31 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 41,'Група в ДОВДЛРГ: предучил.възраст','ДД 3-7',0,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 42,'Група в ДОВДЛРГ: І - ХІІ (ХІІІ) клас','ДД уч.',0,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 48,'ЦСОП - група за целодневна организация','ЦСОП - ЦДО',1,'2021-01-28 00:00:00.000',NULL,2,NULL,NULL,NULL UNION ALL
select 49,'ЦСОП - група в общежитие','ЦСОП - общ.',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 51,'Целодневна','целодн',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 52,'Полудневна','полудн',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 53,'Нощуваща','нощув',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 54,'Сезонна','сез',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 55,'Друга','друга',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 56,'Група за деца със спец.нужди','спец',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 57,'Специална','спец',0,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 60,'извънучилищни дейности','гр.ИУД',0,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 61,'група - ЗП','гр.ЗП',0,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 71,'групова ОПФ: ансамбъл','анс',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 72,'групова ОПФ: експедиция','експ',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 73,'групова ОПФ: клуб','клуб',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 74,'групова ОПФ: кръжок','кръж',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 75,'оркестър','орк',0,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 76,'групова ОПФ: състав','състав',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 77,'групова ОПФ: школа','школа',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 78,'групова ОПФ: филхармония','фирхар',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 79,'групова ОПФ: хор','хор',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 81,'наблюдателна лагер-школа','ЛШ',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 82,'група (НАОП)','обс',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 83,'масова ОПФ','масова',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 84,'групова ОПФ: секция','секция',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 85,'групова ОПФ: отбор','отбор',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 86,'групова ОПФ: друга','друга',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 87,'индивидуална ОПФ','инд.',1,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 88,'ЦСОП - предучилищна група','ЦСОП - предуч.гр.',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 89,'ЦСОП - общообразователна','ЦСОП - ОО гр.',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 90,'ЦСОП - професионална','ЦСОП - проф.гр.',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 91,'Музикална школа','МШ',0,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL UNION ALL
select 92,'ЦСОП - профилирана','ЦСОП - профил.',1,'2021-01-28 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 95,'ИКД','ИКД',0,'2021-01-28 00:00:00.000',NULL,3,NULL,NULL,NULL;

GO
