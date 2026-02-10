GO
PRINT 'Insert [inst_nom].[SPPOOSpeciality]'
GO

insert [inst_nom].[SPPOOSpeciality] ([SPPOOSpecialityID],[ProfessionID],[Name],[Description],[VETLevel],[SPPOOSpecialityCode],[IsValid],[ValidFrom],[ValidTo],[CanChoose],[NameEN],[NameDE],[NameFR])
select -1,-1,'не е приложимо',NULL,NULL,NULL,1,'2020-11-19 00:00:00.000',NULL,0,NULL,NULL,NULL UNION ALL
select 1,487,'Живопис',NULL,3,'2110101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 2,487,'Стенопис',NULL,3,'2110102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 3,487,'Графика',NULL,3,'2110103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 4,487,'Скулптура',NULL,3,'2110104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 5,487,'Рекламна графика',NULL,3,'2110105',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 6,487,'Художествена дърворезба',NULL,3,'2110106',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 7,487,'Художествена керамика',NULL,3,'2110107',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 8,487,'Художествена тъкан',NULL,3,'2110108',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 9,487,'Художествена обработка на  метали',NULL,3,'2110109',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 10,487,'Иконопис',NULL,3,'2110110',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 11,487,'Илюстрация и оформление на книгата',NULL,3,'2110111',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 12,488,'Пиано',NULL,3,'2120101',1,'2020-11-19 00:00:00.000',NULL,1,'Piano','Klavier','Piano' UNION ALL
select 13,488,'Цигулка',NULL,3,'2120102',1,'2020-11-19 00:00:00.000',NULL,1,'Violin','Violine','Violon' UNION ALL
select 14,488,'Виола',NULL,3,'2120103',1,'2020-11-19 00:00:00.000',NULL,1,'Viola','Viola','Alto' UNION ALL
select 15,488,'Виолончело',NULL,3,'2120104',1,'2020-11-19 00:00:00.000',NULL,1,'Cello','Violoncello','Violoncelle' UNION ALL
select 16,488,'Контрабас',NULL,3,'2120105',1,'2020-11-19 00:00:00.000',NULL,1,'Contrabass','Kontrabass','Contrebasse' UNION ALL
select 17,488,'Флейта',NULL,3,'2120106',1,'2020-11-19 00:00:00.000',NULL,1,'Flute','Flöte','Flûte' UNION ALL
select 18,488,'Обой',NULL,3,'2120107',1,'2020-11-19 00:00:00.000',NULL,1,'Oboe','Oboe','Hautbois' UNION ALL
select 19,488,'Кларнет',NULL,3,'2120108',1,'2020-11-19 00:00:00.000',NULL,1,'Clarinet','Klarinette','Clarinette' UNION ALL
select 20,488,'Фагот',NULL,3,'2120109',1,'2020-11-19 00:00:00.000',NULL,1,'Bassoon','Fagott','Basson' UNION ALL
select 21,488,'Валдхорна',NULL,3,'2120110',1,'2020-11-19 00:00:00.000',NULL,1,'French horn','Waldhorn','Waldhorn' UNION ALL
select 22,488,'Тромпет',NULL,3,'2120111',1,'2020-11-19 00:00:00.000',NULL,1,'Trumpet','Trompete','Trompette' UNION ALL
select 23,488,'Цугтромбон',NULL,3,'2120112',1,'2020-11-19 00:00:00.000',NULL,1,'Trombone','Posaune ','Trombone à coulisse' UNION ALL
select 24,488,'Туба',NULL,3,'2120113',1,'2020-11-19 00:00:00.000',NULL,1,'Tuba','Tuba','Tuba' UNION ALL
select 25,488,'Саксофон',NULL,3,'2120114',1,'2020-11-19 00:00:00.000',NULL,1,'Saxophone','Saxophon','Saxophone' UNION ALL
select 26,488,'Ударни инструменти',NULL,3,'2120115',1,'2020-11-19 00:00:00.000',NULL,1,'Drums','Schlaginstrumente','Instruments à percussion' UNION ALL
select 27,488,'Арфа',NULL,3,'2120116',1,'2020-11-19 00:00:00.000',NULL,1,'Harp','Harfe','Harpe' UNION ALL
select 28,488,'Класическа китара',NULL,3,'2120117',1,'2020-11-19 00:00:00.000',NULL,1,'Classical guitar','Klassische Gitarre ','Guitare classique' UNION ALL
select 29,488,'Акордеон',NULL,3,'2120118',1,'2020-11-19 00:00:00.000',NULL,1,'Accordion','Akkordeon','Accordéon' UNION ALL
select 30,488,'Гайда',NULL,3,'2120119',1,'2020-11-19 00:00:00.000',NULL,1,'Bagpipe','Gajda','Gaïda' UNION ALL
select 31,488,'Кавал',NULL,3,'2120120',1,'2020-11-19 00:00:00.000',NULL,1,'Kaval','Kaval','Kaval' UNION ALL
select 32,488,'Гъдулка',NULL,3,'2120121',1,'2020-11-19 00:00:00.000',NULL,1,'Rebeck','Gadulka','Gadulka' UNION ALL
select 33,488,'Тамбура',NULL,3,'2120122',1,'2020-11-19 00:00:00.000',NULL,1,'Mandolin ','Tambura','Tambura' UNION ALL
select 34,489,'Акордиране',NULL,3,'2120201',1,'2020-11-19 00:00:00.000',NULL,1,'Tuning','Stimmung','Accordage' UNION ALL
select 35,490,'Класическо пеене',NULL,3,'2120301',1,'2020-11-19 00:00:00.000',NULL,1,'Classical singing','Klassischer Gesang','Chant classique' UNION ALL
select 36,490,'Народно пеене',NULL,3,'2120302',1,'2020-11-19 00:00:00.000',NULL,1,'Folk singing','Volksgesang','Chant folklorique' UNION ALL
select 37,491,'Класически танци',NULL,3,'2120401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 38,492,'Български танци',NULL,3,'2120501',1,'2020-11-19 00:00:00.000',NULL,1,'Bulgarian dances','Bulgarische Tänze','Danses bulgares' UNION ALL
select 39,492,'Модерни танци',NULL,3,'2120502',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 40,493,'Цирково и вариететно изкуство',NULL,3,'2120601',1,'2020-11-19 00:00:00.000',NULL,1,'Circus and variety art','Zirkus- und Varietékünst','Art du cirque et des variétés' UNION ALL
select 41,494,'Драматичен театър',NULL,3,'2120701',1,'2020-11-19 00:00:00.000',NULL,1,'Drama theater','Dramatisches Theater','Théâtre dramatique' UNION ALL
select 42,494,'Пантомима',NULL,3,'2120702',1,'2020-11-19 00:00:00.000',NULL,1,'Pantomime ','Pantomime','Pantomime' UNION ALL
select 43,494,'Куклен театър',NULL,3,'2120703',1,'2020-11-19 00:00:00.000',NULL,1,'Puppet theatre','Puppentheater','Théâtre de marionnettes' UNION ALL
select 44,495,'Драматичен театър',NULL,4,'2120801',1,'2020-11-19 00:00:00.000',NULL,1,'Drama theatre','Dramatisches Theater','Théâtre dramatique' UNION ALL
select 45,495,'Шоу - програми',NULL,4,'2120802',1,'2020-11-19 00:00:00.000',NULL,1,'Show-programmes','Showprogramme','Show-programmes' UNION ALL
select 46,495,'Куклен театър',NULL,4,'2120803',1,'2020-11-19 00:00:00.000',NULL,1,'Puppet theatre','Puppentheater','Théâtre de marionnettes' UNION ALL
select 47,496,'Филмов монтаж',NULL,3,'2130101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 48,496,'Видеомонтаж',NULL,3,'2130102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 49,496,'Компютърен монтаж',NULL,3,'2130103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 50,497,'Фотография',NULL,3,'2130201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 51,498,'Полиграфия',NULL,3,'2130301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 52,499,'Тоноператорство',NULL,3,'2130401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 53,500,'Компютърна анимация',NULL,3,'2130501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 54,501,'Компютърна графика',NULL,3,'2130601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 55,502,'Графичен дизайн',NULL,3,'2130701',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 56,503,'Музикално оформление',NULL,3,'2130801',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 57,504,'Анимация',NULL,4,'2130901',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 58,505,'Кино и телевизия',NULL,4,'2131001',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 59,506,'Кино и телевизия',NULL,4,'2131101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 60,507,'Аудио-визуални изкуства',NULL,4,'2131201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 61,508,'Телевизия',NULL,4,'2131301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 62,509,'Телевизия',NULL,4,'2131401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 63,510,'Радио',NULL,4,'2131501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 64,511,'Театрален, кино и телевизионен декор',NULL,3,'2140101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 65,511,'Художествено осветление за театър, кино и телевизия',NULL,3,'2140102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 66,511,'Театрален грим и перуки',NULL,3,'2140103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 67,511,'Аранжиране',NULL,3,'2140104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 68,511,'Детски играчки и сувенири',NULL,3,'2140105',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 69,511,'Интериорен дизайн',NULL,3,'2140106',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 70,511,'Силикатен дизайн',NULL,3,'2140107',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 71,511,'Моден дизайн',NULL,3,'2140108',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 72,511,'Пространствен дизайн',NULL,3,'2140109',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 73,511,'Промишлен дизайн',NULL,3,'2140110',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 74,512,'Класически инструменти',NULL,3,'2150101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 75,512,'Народни инструменти',NULL,3,'2150102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 76,513,'Ювелирство',NULL,3,'2150201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 77,514,'Бижутерия',NULL,2,'2150301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 78,515,'Дърворезбарство',NULL,2,'2150401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 79,516,'Керамика',NULL,2,'2150501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 80,517,'Каменоделство',NULL,2,'2150601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 81,517,'Декоративни скални облицовки',NULL,2,'2150602',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 82,518,'Икономист - издател',NULL,3,'3410101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 83,519,'Продавач - консултант',NULL,2,'3410201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 84,520,'Маркетинг',NULL,3,'3420101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 85,521,'Банково дело',NULL,3,'3430101',1,'2020-11-19 00:00:00.000',NULL,1,'Banking','Bankwesen','Activité bancaire' UNION ALL
select 86,521,'Застрахователно и осигурително дело',NULL,3,'3430102',1,'2020-11-19 00:00:00.000',NULL,1,'Insurance and social security activity','Versicherungswesen','Activités d’assurance et de sécurité sociale' UNION ALL
select 87,522,'Счетоводна отчетност',NULL,3,'3440101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 88,523,'Митническа и данъчна администрация',NULL,3,'3440201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 89,524,'Индустрия',NULL,3,'3450101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 90,524,'Търговия',NULL,3,'3450102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 91,524,'Земеделско стопанство',NULL,3,'3450103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 92,524,'Икономика и мениджмънт',NULL,3,'3450104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 93,524,'Предприемачество и мениджмънт',NULL,3,'3450105',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 94,525,'Специалности по професионални направления',NULL,4,'3450201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 95,525,'Мениджмънт в машиностроенето',NULL,4,'3450202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 96,525,'Мениджмънт в строителството',NULL,4,'3450203',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 97,525,'Мениджмънт в хотелиерството и ресторантьорството',NULL,4,'3450204',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 98,525,'Мениджмънт в туризма',NULL,3,'3450205',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 99,525,'Мениджмънт в спорта',NULL,3,'3450206',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 100,525,'Мениджмънт в транспорта',NULL,4,'3450207',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 101,525,'Мениджмънт във фризьорството и козметиката',NULL,4,'3450208',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 102,525,'Мениджмънт в производството на обувки и кожено-галантерийни изделия',NULL,4,'3450209',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 103,525,'Мениджмънт в производството на облекло и в модния дизайн',NULL,4,'3450210',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 104,526,'Специалности по професионални направления',NULL,3,'3450301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 105,526,'Организация на театър, кино и телевизия',NULL,3,'3450302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 106,526,'Организация в машиностроенето',NULL,3,'3450303',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 107,526,'Организация в производството на храни и напитки',NULL,3,'3450304',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 108,526,'Организация в производството на текстил',NULL,3,'3450305',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 109,526,'Организация в производството на облекло',NULL,3,'3450306',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 110,526,'Организация в производството на хляб, хлебни и сладкарски изделия',NULL,3,'3450307',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 111,526,'Организация в производството на химични продукти',NULL,3,'3450308',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 112,527,'Бизнес - услуги',NULL,2,'3450401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 113,528,'Малък и среден бизнес',NULL,2,'3450501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 114,529,'Касиер',NULL,1,'3450601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 115,530,'Калкулант',NULL,1,'3450701',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 116,531,'Снабдител',NULL,1,'3450801',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 117,532,'Деловодство и архив',NULL,1,'3450901',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 118,533,'Текстообработване',NULL,1,'3451001',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 119,534,'Бизнес администрация',NULL,3,'3460101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 120,535,'Административно обслужване',NULL,2,'3460201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 121,536,'Геология и геофизика',NULL,3,'4430101',1,'2020-11-19 00:00:00.000',NULL,1,'Geology and geophysics','Geologie und Genphysik','Géologie et géophysique' UNION ALL
select 122,536,'Хидрогеология и геотехника',NULL,3,'4430102',1,'2020-11-19 00:00:00.000',NULL,1,'Hydrogeology and geotechnics','Hydrogeologie und Geotechnik ','Hydrogéologie et géotechnique' UNION ALL
select 123,537,'Хидрология и метеорология',NULL,3,'4430201',1,'2020-11-19 00:00:00.000',NULL,1,'Hydrology and meteorology','Hydrologie und Meteorologie','Hydrologie et météorologie' UNION ALL
select 124,538,'Икономическа информатика',NULL,3,'4820101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 125,539,'Машини и съоръжения за обработка на металите',NULL,3,'5210101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 126,539,'Специално машиностроене',NULL,3,'5210102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 127,539,'Машини и съоръжения в металургията',NULL,3,'5210103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 128,539,'Машини и съоръжения в хранително - вкусовата промишленост',NULL,3,'5210104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 129,539,'Машини и системи с ЦПУ',NULL,3,'5210105',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 130,539,'Машини и съоръжения в хидро- и пневмотехниката',NULL,3,'5210106',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 131,539,'Машини и съоръжения в шевната и обувната промишленост',NULL,3,'5210107',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 132,539,'Машини и съоръжения в дървообработващата промишленост',NULL,3,'5210108',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 133,539,'Машини и съоръжения в химическата промишленост',NULL,3,'5210109',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 134,539,'Машини и съоръжения в текстилната промишленост',NULL,3,'5210110',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 135,539,'Машини и съоръжения в минната промишленост',NULL,3,'5210111',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 136,539,'Машини и съоръжения за сондиране',NULL,3,'5210112',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 137,539,'Машини и съоръжения за заваряване',NULL,3,'5210113',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 138,539,'Машини и съоръжения за производство на строителни материали',NULL,3,'5210114',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 139,539,'Автоматизирани и роботизирани системи',NULL,3,'5210115',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 140,540,'Художествени изделия от метал',NULL,3,'5210201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 141,541,'Металорежещи машини',NULL,2,'5210301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 142,541,'Машини за гореща обработка на металите',NULL,2,'5210302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 143,541,'Машини и съоръжения за заваряване',NULL,2,'5210303',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 144,542,'Машини и съоръжения за обработка на металите',NULL,2,'5210401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 145,542,'Специално машиностроене',NULL,2,'5210402',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 146,542,'Машини и съоръжения в металургията',NULL,2,'5210403',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 147,542,'Машини и съоръжения в хранително - вкусовата промишленост',NULL,2,'5210404',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 148,542,'Машини и системи с ЦПУ',NULL,2,'5210405',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 149,542,'Машини и съоръжения в хидро- и пневмотехниката',NULL,2,'5210406',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 150,542,'Машини и съоръжения в шевната и обувната промишленост',NULL,2,'5210407',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 151,542,'Машини и съоръжения в дървообработващата промишленост',NULL,2,'5210408',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 152,542,'Машини и съоръжения в химическата промишленост',NULL,2,'5210409',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 153,542,'Машини и съоръжения в текстилната промишленост',NULL,2,'5210410',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 154,542,'Машини и съоръжения в минната промишленост',NULL,2,'5210411',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 155,542,'Машини и съоръжения за сондиране',NULL,2,'5210412',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 156,542,'Машини и съоръжения за производство на строителни материали',NULL,2,'5210413',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 157,543,'Измервателна и организационна техника',NULL,3,'5210501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 158,543,'Лазерна техника',NULL,3,'5210502',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 159,543,'Оптична техника',NULL,3,'5210503',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 160,544,'Измервателна и организационна техника',NULL,2,'5210601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 161,544,'Лазерна техника',NULL,2,'5210602',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 162,544,'Оптична техника',NULL,2,'5210603',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 163,545,'Металургия на черните метали',NULL,3,'5210701',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 164,545,'Металургия на цветните метали',NULL,3,'5210702',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 165,546,'Металургия на черните метали',NULL,2,'5210801',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 166,546,'Металургия на цветните метали',NULL,2,'5210802',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 167,547,'Заваряване',NULL,1,'5210901',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 168,548,'Стругарство',NULL,1,'5211001',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 169,549,'Шлосерство',NULL,1,'5211101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 170,550,'Леярство',NULL,1,'5211201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 171,551,'Ковачество',NULL,1,'5211301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 172,552,'Електрически машини и апарати',NULL,3,'5220101',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical machines and apparatuses','Elektrische Maschinen und Apparate','Machines et appareils électriques' UNION ALL
select 173,552,'Електроенергетика',NULL,3,'5220102',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical energy','Elektroenergetik','Électro-énergie' UNION ALL
select 174,552,'Електрообзавеждане на производството',NULL,3,'5220103',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of production','Elektrische Ausrüstung der Fertigung','Équipement électrique de la production' UNION ALL
select 175,552,'Електрообзавеждане на кораби',NULL,3,'5220104',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of ships','Elektrische Ausrüstung von Schiffen','Équipement électrique des navires' UNION ALL
select 176,552,'Електрообзавеждане на железопътна техника',NULL,3,'5220105',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of railway applications','Elektrische Ausrüstung von Bahnanlagen','Équipement électrique du matériel ferroviaire' UNION ALL
select 177,552,'Електрообзавеждане на транспортна техника',NULL,3,'5220106',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of transport applications','Elektrische Ausrüstung von Transportmitteln','Équipement électrique des engins de transport' UNION ALL
select 178,552,'Електрообзавеждане на електрически превозни средства за градски транспорт',NULL,3,'5220107',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of electric vehicles for the public transport','Elektrische Ausrüstung von Elektrofahrzeugen für den Stadtverkehr ','Équipement électrique des véhicules électriques pour les transports en commun' UNION ALL
select 179,552,'Електрообзавеждане на подемна и асансьорна техника',NULL,3,'5220108',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of hoisting and lifting devices','Elektrische Ausrüstung von Hebe- und Aufzuganlagen','Équipement électrique des équipements de levage et d''ascenseur' UNION ALL
select 180,552,'Електрически инсталации',NULL,3,'5220109',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical installations','Elektrische Installationen','Installations électriques' UNION ALL
select 181,552,'Електродомакинска техника',NULL,3,'5220110',1,'2020-11-19 00:00:00.000',NULL,1,'Household electrical appliances','Elektrische Haushaltsgeräte','Appareils ménagers' UNION ALL
select 182,553,'Електрически машини и апарати',NULL,2,'5220201',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical machines and apparatuses','Elektrische Maschinen und Apparate','Machines et appareils électriques' UNION ALL
select 183,553,'Електрически централи и подстанции',NULL,2,'5220202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 184,553,'Електрически мрежи',NULL,2,'5220203',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 185,553,'Електрообзавеждане на производството',NULL,2,'5220204',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of production','Elektrische Ausrüstung der Fertigung','Équipement électrique de la production' UNION ALL
select 186,553,'Електрообзавеждане на кораби',NULL,2,'5220205',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of ships','Elektrische Ausrüstung von Schiffen','Équipement électrique des navires' UNION ALL
select 187,553,'Електрообзавеждане на железопътна техника',NULL,2,'5220206',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of railway applications','Elektrische Ausrüstung von Bahnanlagen','Équipement électrique du matériel ferroviaire' UNION ALL
select 188,553,'Електрообзавеждане на транспортна техника',NULL,2,'5220207',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of transport applications','Elektrische Ausrüstung von Transportmitteln','Équipement électrique des engins de transport' UNION ALL
select 189,553,'Електрообзавеждане на електрически превозни средства за градски транспорт',NULL,2,'5220208',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of electric vehicles for the public transport','Elektrische Ausrüstung von Elektrofahrzeugen für den Stadtverkehr','Équipement électrique des véhicules électriques pour les transports en commun' UNION ALL
select 190,553,'Електрообзавеждане на подемна и асансьорна техника',NULL,2,'5220209',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical equipment of hoisting and lifting devices','Elektrische Ausrüstung von Hebe- und Aufzuganlagen','Équipement électrique des équipements de levage et d''ascenseur' UNION ALL
select 191,553,'Електрически инсталации',NULL,2,'5220210',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical installations','Elektrische Installationen','Installations électriques' UNION ALL
select 192,553,'Електродомакинска техника',NULL,2,'5220211',1,'2020-11-19 00:00:00.000',NULL,1,'Household electrical appliances','Elektrische Haushaltsgeräte','Appareils ménagers' UNION ALL
select 193,554,'Топлоенергетика',NULL,3,'5220301',1,'2020-11-19 00:00:00.000',NULL,1,'Heat energy','Wärmeenergetik','Énergie thermique' UNION ALL
select 194,554,'Ядрена  енергетика',NULL,3,'5220302',1,'2020-11-19 00:00:00.000',NULL,1,'Nuclear energy','Kernenergetik','Énergétique nucléaire' UNION ALL
select 195,554,'Хидроенергетика',NULL,3,'5220303',1,'2020-11-19 00:00:00.000',NULL,1,'Hydroenergy','Hydroenergetik','Hydroénergétique' UNION ALL
select 196,554,'Топлинна техника',NULL,3,'5220304',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 197,554,'Хладилна техника',NULL,3,'5220305',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 198,554,'Газова техника',NULL,3,'5220306',1,'2020-11-19 00:00:00.000',NULL,1,'Gas energy','Gastechnik','Technologie du gaz' UNION ALL
select 199,554,'Климатична и вентилационна техника',NULL,3,'5220307',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 200,555,'Топлоенергетика',NULL,2,'5220401',1,'2020-11-19 00:00:00.000',NULL,1,'Heat energy','Wärmeenergetik','Énergie thermique' UNION ALL
select 201,555,'Ядрена енергетика',NULL,2,'5220402',1,'2020-11-19 00:00:00.000',NULL,1,'Nuclear energy','Kernenergetik','Énergétique nucléaire' UNION ALL
select 202,555,'Хидроенергетика',NULL,2,'5220403',1,'2020-11-19 00:00:00.000',NULL,1,'Hydroenergy','Hydroenergetik','Hydroénergétique' UNION ALL
select 203,555,'Топлинна техника',NULL,2,'5220404',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 204,555,'Хладилна техника',NULL,2,'5220405',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 205,555,'Газова техника',NULL,2,'5220406',1,'2020-11-19 00:00:00.000',NULL,1,'Gas energy','Gastechnik','Technologie du gaz' UNION ALL
select 206,555,'Климатична и вентилационна техника',NULL,2,'5220407',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 207,556,'Огнярство',NULL,1,'5220601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 208,557,'Радио и телевизионна техника',NULL,3,'5230101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 209,557,'Телекомуникационни системи',NULL,3,'5230102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 210,557,'Радиолокация, радионавигация и хидроакустични системи',NULL,3,'5230103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 211,557,'Кинотехника, аудио и видеосистеми',NULL,3,'5230104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 212,558,'Радио и телевизионна техника',NULL,2,'5230201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 213,558,'Телекомуникационни системи',NULL,2,'5230202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 214,558,'Радиолокация, радионавигация и хидроакустични системи',NULL,2,'5230203',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 215,558,'Кинотехника, аудио и видеосистеми',NULL,2,'5230204',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 216,559,'Промишлена електроника',NULL,3,'5230301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 217,559,'Микропроцесорна техника',NULL,3,'5230302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 218,559,'Електронно уредостроене',NULL,3,'5230303',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 219,560,'Промишлена електроника',NULL,2,'5230401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 220,560,'Микропроцесорна техника',NULL,2,'5230402',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 221,560,'Електронно уредостроене',NULL,2,'5230403',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 222,561,'Компютърна техника и технологии',NULL,3,'5230501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 223,561,'Компютърни мрежи',NULL,3,'5230502',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 224,562,'Компютърна техника и технологии',NULL,2,'5230601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 225,562,'Компютърни мрежи',NULL,2,'5230602',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 226,563,'Автоматизация на непрекъснати производства',NULL,3,'5230701',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 227,563,'Автоматизация на дискретни производства',NULL,3,'5230702',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 228,563,'Автоматизирани и роботизирани системи',NULL,4,'5230703',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 229,563,'Осигурителни и комуникационни системи в ж.п.инфраструктура',NULL,3,'5230704',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 230,564,'Автоматизирани системи',NULL,2,'5230801',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 231,564,'Осигурителни и комуникационни системи в ж.п.инфраструктура',NULL,2,'5230802',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 232,565,'Програмно осигуряване',NULL,2,'5230901',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 233,565,'Системно програмиране',NULL,3,'5230902',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 234,566,'Технология на неорганичните вещества',NULL,3,'5240101',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of inorganic substances','Technologie anorganischer Stoffe','Technologie des substances inorganiques' UNION ALL
select 235,566,'Технология на стъкларското производство',NULL,3,'5240102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 236,566,'Технология на керамичното производство',NULL,3,'5240103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 237,566,'Технология на свързващите вещества',NULL,3,'5240104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 238,566,'Технология на органичните вещества',NULL,3,'5240105',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of organic substances','Technologie organischer Stoffe','Technologie des substances organiques' UNION ALL
select 239,566,'Технология на полимерите',NULL,3,'5240106',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of polymers','Polymertechnologie ','Technologie des polymères' UNION ALL
select 240,566,'Технология на химичните влакна',NULL,3,'5240107',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of chemical fibers','Technologie der Chemiefasern','Technologie des fibres chimiques' UNION ALL
select 241,566,'Технология на нефта, газа и твърдите горива',NULL,3,'5240108',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of petroleum, gas and solid fuels','Öl-, Gas- und Festbrennstofftechnologie','Technologie du pétrole, du gaz et des combustibles solides' UNION ALL
select 242,566,'Технология на целулозата, хартията и опаковките',NULL,3,'5240109',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of cellulose, paper and packages','Zellstoff-, Papier- und Verpackungstechnologie','Technologie de la cellulose, du papier et des emballages' UNION ALL
select 243,566,'Технология на фармацевтични и парфюмерийно - козметични продукти',NULL,3,'5240110',1,'2020-11-19 00:00:00.000',NULL,1,'Technologie des produits pharmaceutiques et de parfumerie et cosmétique','Technologie der Pharmazie und Parfümerie und Kosmetik ',NULL UNION ALL
select 244,566,'Технология за обработка на кожи',NULL,3,'5240111',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of fur processing','Lederverarbeitungstechnologie','Technologie de traitement du cuir' UNION ALL
select 245,567,'Биотехнологии в химичните производства',NULL,3,'5240201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 246,567,'Биотехнологии в хранително - вкусови производства',NULL,2,'5240202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 247,567,'Биотехнологии в растениевъдството',NULL,2,'5240203',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 248,567,'Биотехнологии в животновъдството',NULL,2,'5240204',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 249,568,'Биотехнологии в химични производства',NULL,2,'5240301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 250,568,'Биотехнологии в хранително - вкусови производства',NULL,2,'5240302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 251,568,'Биотехнология в растениевъдството',NULL,2,'5240303',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 252,568,'Биотехнология в животновъдството',NULL,2,'5240304',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 253,569,'Технологичен и микробиологичен контрол в химични производства',NULL,3,'5240401',1,'2020-11-19 00:00:00.000',NULL,1,'Technological and microbiological control in chemical productions','Technologische und mikrobiologische Kontrolle in der chemischen Produktion','Contrôle technologique et microbiologique dans la production chimique' UNION ALL
select 254,569,'Технологичен и микробиологичен контрол в хранително - вкусови производства',NULL,3,'5240402',1,'2020-11-19 00:00:00.000',NULL,1,'Technological and microbiological control in food productions','Technologische und mikrobiologische Kontrolle in der Lebensmittel Produktion','Contrôle technologique et microbiologique dans la production alimentaire' UNION ALL
select 255,569,'Контрол на качеството в металургията',NULL,3,'5240403',1,'2020-11-19 00:00:00.000',NULL,1,'Quality control in metallurgy','Qualitätsüberwachung in der Metallurgie','Contrôle de la qualité en métallurgie' UNION ALL
select 256,570,'Декоратор в силикатното производство',NULL,3,'5240501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 257,571,'Технология на неорганичните вещества',NULL,2,'5240601',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of inorganic substances','Technologie anorganischer Stoffe','Technologie des substances inorganiques' UNION ALL
select 258,571,'Технология на стъкларското производство',NULL,2,'5240602',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 259,571,'Технология на керамичното производство',NULL,2,'5240603',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 260,571,'Технология на свързващите вещества',NULL,2,'5240604',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 261,571,'Технология на органичните вещества',NULL,2,'5240605',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of organic substances','Technologie organischer Stoffe','Technologie des substances organiques' UNION ALL
select 262,571,'Технология на полимерите',NULL,2,'5240606',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of polymers','Polymertechnologie','Technologie des polymères' UNION ALL
select 263,571,'Технология на химичните влакна',NULL,2,'5240607',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of chemical fibers','Technologie der Chemiefasern','Technologie des fibres chimiques' UNION ALL
select 264,571,'Технология на нефта, газа и твърдите горива',NULL,2,'5240608',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of petroleum, gas and solid fuels','Öl-, Gas- und Festbrennstofftechnologie','Technologie du pétrole, du gaz et des combustibles solides' UNION ALL
select 265,571,'Технология на целулозата, хартията и опаковките',NULL,2,'5240609',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of cellulose, paper and packages','Zellstoff-, Papier- und Verpackungstechnologie','Technologie de la cellulose, du papier et des emballages' UNION ALL
select 266,571,'Технология на фармацевтични и парфюмерийно - козметични продукти',NULL,2,'5240610',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of pharmaceutical and perfumery-cosmetic products','Technologie der Pharmazie und Parfümerie und Kosmetik','Technologie des produits pharmaceutiques et de parfumerie et cosmétique' UNION ALL
select 267,571,'Технология за обработка на кожи',NULL,2,'5240611',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of fur processing','Lederverarbeitungstechnologie','Technologie de traitement du cuir' UNION ALL
select 268,571,'Биотехнологични производства',NULL,2,'5240612',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 269,572,'Технология на стъкларското производство',NULL,3,'5240701',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 270,573,'Технология на керамичното производство',NULL,3,'5240801',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 271,574,'Технология на полимерите',NULL,3,'5240901',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 272,575,'Автотранспортна техника',NULL,3,'5250101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 273,575,'Пътно - строителна техника',NULL,3,'5250102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 274,576,'Автотранспортна техника',NULL,2,'5250201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 275,576,'Пътностроителна техника',NULL,2,'5250202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 276,577,'Подемно - транспортна техника, монтирана на пътни транспортни средства',NULL,3,'5250501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 277,577,'Подемно - транспортна техника с електрозадвижване',NULL,3,'5250502',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 278,578,'Подемно - транспортна техника, монтирана на пътни транспортни средства',NULL,2,'5250601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 279,578,'Подемно - транспортна техника с електрозадвижване',NULL,2,'5250602',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 280,578,'Пристанищна механизация',NULL,2,'5250603',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 281,579,'Локомотиви и вагони',NULL,3,'5250701',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 282,579,'Подемно - транспортна, пътностроителна и ремонтна ж.п техника',NULL,3,'5250702',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 283,580,'Локомотиви и вагони',NULL,2,'5250801',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 284,580,'Подемно - транспортна, пътностроителна и ремонтна ж.п техника',NULL,2,'5250802',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 285,581,'Експлоатация и ремонт на летателни апарати',NULL,3,'5250901',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 286,581,'Експлоатация и ремонт на електронно - приборна авиационна техника',NULL,3,'5250902',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 287,582,'Корабни машини и механизми',NULL,3,'5251001',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 288,582,'Корабостроене',NULL,3,'5251002',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 289,583,'Корабни машини и механизми',NULL,2,'5251101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 290,583,'Корабни тръбни системи',NULL,2,'5251102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 291,583,'Ремонт на кораби',NULL,2,'5251103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 292,583,'Корабостроене',NULL,2,'5251104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 293,584,'Автобояджия',NULL,1,'5251201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 294,585,'Акумулаторджия',NULL,1,'5251301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 295,586,'Радиаторджия',NULL,1,'5251401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 296,587,'Гумаджия',NULL,1,'5251501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 297,588,'Автотапицер',NULL,1,'5251601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 298,589,'Автотенекеджия',NULL,1,'5251701',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 299,590,'Зърносъхранение, зърнопреработка и производство  на фуражи',NULL,3,'5410101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 300,590,'Производство на хляб, хлебни и сладкарски изделия',NULL,3,'5410102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 301,590,'Производство и преработка на на мляко и млечни продукти',NULL,3,'5410103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 302,590,'Производство на месо, месни продукти и риба',NULL,3,'5410104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 303,590,'Производство на консерви',NULL,3,'5410105',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 304,590,'Производство на алкохолни и безалкохолни напитки',NULL,3,'5410106',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 305,590,'Производство на захар и захарни изделия',NULL,3,'5410107',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 306,590,'Производство на тютюн и тютюневи изделия',NULL,3,'5410108',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 307,590,'Производство на растителни мазнини, сапуни и етерични масла',NULL,3,'5410109',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 308,591,'Зърносъхранение, зърнопреработка и производство  на фуражи',NULL,2,'5410201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 309,591,'Производство и преработка на мляко и млечни продукти',NULL,2,'5410202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 310,591,'Производство на месо, месни продукти и риба',NULL,2,'5410203',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 311,591,'Производство на консерви',NULL,2,'5410204',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 312,591,'Производство на алкохолни и безалкохолни напитки',NULL,2,'5410205',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 313,591,'Производство на захар и захарни изделия',NULL,2,'5410206',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 314,591,'Производство на тютюн и тютюневи изделия',NULL,2,'5410207',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 315,591,'Производство на растителни мазнини, сапуни и етерични масла',NULL,2,'5410208',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 316,592,'Производство на хляб и хлебни изделия',NULL,2,'5410301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 317,592,'Производство на сладкарски изделия',NULL,2,'5410302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 318,593,'Експлоатация и поддържане на хладилната техника в хранителната промишленост',NULL,3,'5410401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 319,594,'Хранително - вкусова промишленост',NULL,1,'5410501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 320,595,'Десениране на тъкани площни изделия',NULL,3,'5420101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 321,595,'Десениране на плетени площни изделия',NULL,3,'5420102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 322,596,'Предачно производство',NULL,3,'5420201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 323,596,'Тъкачно производство',NULL,3,'5420202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 324,596,'Плетачно производство',NULL,3,'5420203',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 325,596,'Апретура, багрене, печатане и химическо чистене',NULL,3,'5420204',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 326,597,'Предачно производство',NULL,2,'5420301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 327,597,'Тъкачно производство',NULL,2,'5420302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 328,597,'Плетачно производство',NULL,2,'5420303',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 329,597,'Апретура, багрене, печатане и химическо чистене',NULL,2,'5420304',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 330,598,'Конструиране, моделиране и технология на облекло от текстил',NULL,3,'5420401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 331,598,'Конструиране, моделиране и технология на облекло от кожи',NULL,3,'5420402',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 332,599,'Производство на облекло от текстил',NULL,2,'5420501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 333,599,'Производство на облекло от кожи',NULL,2,'5420502',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 334,600,'Конструиране, моделиране и технология на обувни изделия',NULL,3,'5420601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 335,600,'Конструиране, моделиране и технология на кожено - галантерийни изделия',NULL,3,'5420602',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 336,601,'Производство на обувни изделия',NULL,2,'5420701',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 337,601,'Производство на кожено - галантерийни изделия',NULL,2,'5420702',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 338,602,'Текстилно производство',NULL,1,'5420801',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 339,603,'Производство на облекло',NULL,1,'5420901',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 340,604,'Обувно и кожено - галантерийно производство',NULL,1,'5421001',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 341,605,'Шивачество',NULL,1,'5421101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 342,606,'Обущарство',NULL,1,'5421201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 343,607,'Ръчно изработване на килими',NULL,1,'5421301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 344,607,'Ръчно изработване на гоблени, губери, козяци',NULL,1,'5421302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 345,608,'Ръчно художествено плетиво',NULL,1,'5421401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 346,608,'Плетач на настолна плетачна машина',NULL,1,'5421402',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 347,609,'Бродерия',NULL,1,'5421501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 348,610,'Мебелно производство',NULL,3,'5430101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 349,610,'Проектиране и производство на вътрешно обзавеждане',NULL,3,'5430102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 350,610,'Производство на стилни  мебели',NULL,3,'5430103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 351,610,'Реставрация на стилни мебели и дограма',NULL,3,'5430104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 352,610,'Производство на струнни музикални инструменти',NULL,3,'5430105',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 353,610,'Тапицерство и декораторство',NULL,3,'5430106',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 354,610,'Производство на врати и прозорци',NULL,3,'5430107',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 355,610,'Моделчество и дървостругарство',NULL,3,'5430108',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 356,610,'Дърворезни и амбалажни производства',NULL,3,'5430109',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 357,610,'Производство на плочи и слоиста дървесина',NULL,3,'5430110',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 358,610,'Производство на строителни изделия от дървесина',NULL,3,'5430111',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 359,611,'Производство на мебели',NULL,2,'5430201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 360,611,'Производство на врати и прозорци',NULL,2,'5430202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 361,611,'Производство на тапицирани изделия',NULL,2,'5430203',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 362,611,'Производство и монтаж на вътрешно обзавеждане на кораби',NULL,2,'5430204',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 363,611,'Моделчество и дървостругарство',NULL,2,'5430205',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 364,611,'Дърворезно, амбалажно и паркетно производство',NULL,2,'5430206',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 365,611,'Производство на дървесни плочи',NULL,2,'5430207',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 366,611,'Производство на фурнир и слоиста дъресина',NULL,2,'5430208',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 367,611,'Производство на дървени детски играчки',NULL,2,'5430209',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 368,612,'Дърводелство',NULL,2,'5430301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 369,613,'Мебелно производство',NULL,1,'5430401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 370,614,'Дърводелство',NULL,1,'5430501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 371,615,'Тапицерство',NULL,1,'5430601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 372,616,'Бъчварство',NULL,1,'5430701',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 373,617,'Кошничарство',NULL,1,'5430801',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 374,618,'Добивни и строителни минни технологии',NULL,3,'5440101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 375,618,'Обогатителни, преработващи и рециклационни технологии',NULL,3,'5440102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 376,618,'Минна електромеханика',NULL,3,'5440103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 377,619,'Подземен и открит добив на полезни изкопаеми',NULL,2,'5440201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 378,619,'Добив на скални материали',NULL,2,'5440202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 379,619,'Обогатяване на полезни изкопаеми',NULL,2,'5440203',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 380,619,'Обработка на скални материали',NULL,2,'5440204',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 381,620,'Сондажни технологии',NULL,3,'5440301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 382,620,'Сондажна техника',NULL,3,'5440302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 383,621,'Проучвателно сондиране',NULL,2,'5440401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 384,622,'Маркшайдерство',NULL,3,'5440501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 385,623,'Строителство и архитектура',NULL,3,'5820101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 386,623,'Архитектурен дизайн',NULL,4,'5820102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 387,623,'Водно строителство',NULL,3,'5820103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 388,623,'Транспортно строителство',NULL,3,'5820104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 389,624,'Геодезия',NULL,3,'5820201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 390,625,'Помощник в строителството',NULL,1,'5820301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 391,625,'Кофражи',NULL,2,'5820302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 392,625,'Армировка и бетон',NULL,2,'5820303',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 393,625,'Зидария',NULL,2,'5820304',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 394,625,'Мазилки и шпакловки',NULL,2,'5820305',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 395,625,'Вътрешни облицовки и настилки',NULL,2,'5820306',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 396,625,'Външни облицовки и настилки',NULL,2,'5820307',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 397,625,'Мозайки',NULL,2,'5820308',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 398,625,'Бояджийски работи',NULL,2,'5820309',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 399,625,'Строително дърводелство',NULL,2,'5820310',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 400,625,'Строително тенекеджийство',NULL,2,'5820311',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 401,625,'Покриви',NULL,2,'5820312',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 402,626,'Стоманобетонни конструкции ',NULL,2,'5820401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 403,626,'Метални конструкции',NULL,2,'5820402',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 404,626,'Сухо строителство',NULL,2,'5820403',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 405,626,'Дограма и стъклопоставяне',NULL,2,'5820404',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 406,627,'Вътрешни ВиК мрежи',NULL,2,'5820501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 407,627,'Външни ВиК мрежи',NULL,2,'5820502',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 408,628,'Строител на пътища, магистрали и съоръжения към тях',NULL,1,'5820601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 409,628,'Строител на релсови пътища и съоръжения към тях',NULL,1,'5820602',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 410,629,'Пещостроителство',NULL,2,'5820701',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 411,630,'Полевъдство',NULL,3,'6210101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 412,630,'Зеленчукопроизводство',NULL,3,'6210102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 413,630,'Трайни насаждения',NULL,3,'6210103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 414,630,'Селекция и семепроизводство',NULL,3,'6210104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 415,630,'Тютюнопроизводство',NULL,3,'6210105',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 416,630,'Гъбопроизводство',NULL,3,'6210106',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 417,630,'Растителна защита и агрохимия',NULL,3,'6210107',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 418,631,'Лозаровинарство',NULL,3,'6210201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 419,632,'Растениевъдство',NULL,2,'6210301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 420,633,'Говедовъдство',NULL,3,'6210401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 421,633,'Овцевъдство',NULL,3,'6210402',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 422,633,'Свиневъдство',NULL,3,'6210403',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 423,633,'Птицевъдство',NULL,3,'6210404',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 424,633,'Зайцевъдство',NULL,3,'6210405',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 425,633,'Пчеларство и бубарство',NULL,3,'6210406',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 426,633,'Коневъдство и конна езда ',NULL,3,'6210407',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 427,634,'Животновъдство',NULL,2,'6210501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 428,635,'Земеделец',NULL,2,'6210601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 429,635,'Производител на селскостопанска продукция',NULL,3,'6210602',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 430,636,'Механизация на селското стопанство',NULL,3,'6210701',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 431,637,'Механизация на селското стопанство',NULL,2,'6210801',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 432,638,'Цветарство',NULL,3,'6220101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 433,638,'Парково строителство и озеленяване',NULL,3,'6220102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 434,639,'Цветарство',NULL,2,'6220201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 435,639,'Парково строителство и озеленяване',NULL,2,'6220202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 436,640,'Горско и ловно стопанство',NULL,3,'6230101',1,'2020-11-19 00:00:00.000',NULL,1,'Forestry and hunting','Forstwirtschaft und Jagdwirtschaft ','Sylviculture et chasse' UNION ALL
select 437,640,'Горско стопанство и дърводобив',NULL,3,'6230102',1,'2020-11-19 00:00:00.000',NULL,1,'Forestry and wood industry','Forstwirtschaft und Holzgewinnung','Sylviculture et exploitation forestière' UNION ALL
select 438,641,'Механизация на горското стопанство',NULL,3,'6230201',1,'2020-11-19 00:00:00.000',NULL,1,'Forestry mechanization','Mechanisierung der Forstwirtschaft','Mécanisation de la sylviculture' UNION ALL
select 439,642,'Механизация на горското стопанство',NULL,2,'6230301',1,'2020-11-19 00:00:00.000',NULL,1,'Forestry mechanization','Mechanisierung der Forstwirtschaft','Mécanisation de la sylviculture' UNION ALL
select 440,643,'Горско стопанство',NULL,2,'6230401',1,'2020-11-19 00:00:00.000',NULL,1,'Forestry','Forstwirtschaft','Sylviculture' UNION ALL
select 441,644,'Горско и ловно стопанство',NULL,2,'6230501',1,'2020-11-19 00:00:00.000',NULL,1,'Forestry and hunting','Forstwirtschaft und Jagdwirtschaft','Sylviculture et chasse' UNION ALL
select 442,645,'Лесокултурни дейности',NULL,1,'6230601',1,'2020-11-19 00:00:00.000',NULL,1,'Silvicultural activities','Forstwirtschaftliche Tätigkeiten','Activités de sylviculture' UNION ALL
select 443,645,'Дърводобив',NULL,1,'6230602',1,'2020-11-19 00:00:00.000',NULL,1,'Wood industry','Holzgewinnung','Exploitation forestière' UNION ALL
select 444,645,'Дивечовъдство',NULL,1,'6230603',1,'2020-11-19 00:00:00.000',NULL,1,'Game breeding','Wildzucht','Élevage de gibier' UNION ALL
select 445,645,'Билкарство',NULL,1,'6230604',1,'2020-11-19 00:00:00.000',NULL,1,'Herbalism','Sammeln von Kräutern','Herboristerie' UNION ALL
select 446,646,'Рибовъдство',NULL,3,'6240101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 447,647,'Промишлен риболов и аквакултури',NULL,3,'6240201',1,'2020-11-19 00:00:00.000',NULL,1,'industrial fishing and aquacultures','Industrielle Fischerei und Aquakulturen','Pêche industrielle et aquacultures' UNION ALL
select 448,648,'Ветеринарен техник',NULL,3,'6400101',1,'2020-11-19 00:00:00.000',NULL,1,'Veterinary technician','Veterinärtechniker','Technicien vétérinaire' UNION ALL
select 449,649,'Ветеринарен лаборант',NULL,3,'6400201',1,'2020-11-19 00:00:00.000',NULL,1,'Veterinary laboratory assistant','Veterinärlaborant','Assistante de laboratoire vétérinaire' UNION ALL
select 450,650,'Очна оптика',NULL,3,'7250101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 451,651,'Оптометрия',NULL,4,'7250201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 452,652,'Ортопедична техника и бандажи',NULL,3,'7250301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 453,653,'Слухови апарати',NULL,3,'7250401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 454,654,'Посредник на трудовата борса',NULL,3,'7620101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 455,655,'Социални услуги на деца и семейства в риск',NULL,3,'7620201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 456,655,'Социални услуги на деца и възрастни с хронични заболявания, с физически и сензорни увреждания',NULL,3,'7620202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 457,656,'Организация на хотелиерството',NULL,3,'8110101',1,'2020-11-19 00:00:00.000',NULL,1,'Organization of hotel-keeping','Organisation des Hotelgewerbes','Organisation de l''hôtellerie' UNION ALL
select 458,656,'Организация и управление на хотелиерството',NULL,4,'8110102',1,'2020-11-19 00:00:00.000',NULL,1,'Organization and management of hotel-keeping','Organisation und Verwaltung des Hotelgewerbes','Organisation et gestion de l''industrie hôtelière' UNION ALL
select 459,657,'Организация на обслужването в хотелиерството',NULL,3,'8110201',1,'2020-11-19 00:00:00.000',NULL,1,'Organization of hotel-keeping services','Organisation von Hoteldienstleistungen','Organisation des services hôteliers' UNION ALL
select 460,658,'Хотелиерство',NULL,1,'8110301',1,'2020-11-19 00:00:00.000',NULL,1,'Hotel industry','Hotelgewerbe','Hôtellerie' UNION ALL
select 461,659,'Хотелиерство',NULL,1,'8110401',1,'2020-11-19 00:00:00.000',NULL,1,'Hotel industry','Hotelgewerbe','Hôtellerie' UNION ALL
select 462,660,'Хотелиерство',NULL,1,'8110501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 463,661,'Организация и управление в ресторантьорството',NULL,4,'8110601',1,'2020-11-19 00:00:00.000',NULL,1,'Organization and management of restaurant-keeping','Organisation und Verwaltung des Gaststättengewerbes','Organisation et gestion de la restauration' UNION ALL
select 464,661,'Производство и обслужване в заведенията за хранене и развлечения',NULL,3,'8110602',1,'2020-11-19 00:00:00.000',NULL,1,'Production and service in the public catering and entertainment establishments','Produktion und Service in den Gastronomie- und Vergnügungsbetrieben','Production et service dans les restaurants et les locaux de divertissement' UNION ALL
select 465,661,'Кетъринг',NULL,3,'8110603',1,'2020-11-19 00:00:00.000',NULL,1,'Catering','Catering','Restauration collective' UNION ALL
select 466,662,'Производство на кулинарни изделия и напитки',NULL,2,'8110701',1,'2020-11-19 00:00:00.000',NULL,1,'Production of culinary articles and beverages','Herstellung von kulinarischen Produkten und Getränken','Fabrication de produits culinaires et de boissons' UNION ALL
select 467,663,'Обслужване на заведения в обществено хранене',NULL,2,'8110801',1,'2020-11-19 00:00:00.000',NULL,1,'Service in public catering establishments','Restaurantdienstleistungen','Service des établissements de restauration collective' UNION ALL
select 468,664,'Работник в производството на кулинарни изделия в заведенията за хранене и развлечения',NULL,1,'8110901',1,'2020-11-19 00:00:00.000',NULL,1,'Worker in the production of culinary articles in public catering and entertainment establishments','Arbeiter in den Herstellung von kulinarischen Produkten  in den Gastronomie- und Vergnügungsbetrieben','Ouvrier dans la production de produits culinaires dans les restaurants et les locaux de divertissement' UNION ALL
select 469,664,'Работник в обслужване на заведения за хранене и развлечения',NULL,1,'8110902',1,'2020-11-19 00:00:00.000',NULL,1,'Worker providing services in the public catering and entertainment establishments','Arbeitskraft in den Gastronomie- und Vergnügungsbetrieben','Ouvrier au service dans les restaurants et les locaux de divertissement' UNION ALL
select 470,665,'Организация на туризма и свободното време',NULL,3,'8120101',1,'2020-11-19 00:00:00.000',NULL,1,'Organization of travelling, tourism and leisure time','Organisation von Tourismus und Freizeit','Organisation du tourisme et des loisirs' UNION ALL
select 471,665,'Селски туризъм',NULL,3,'8120102',1,'2020-11-19 00:00:00.000',NULL,1,'Rural tourism','Landtourismus','Tourisme rural' UNION ALL
select 472,666,'Туризъм',NULL,3,'8120201',1,'2020-11-19 00:00:00.000',NULL,1,'Tourism','Tourismus','Tourisme' UNION ALL
select 473,667,'Екскурзоводство',NULL,3,'8120301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 474,668,'Туристическа анимация',NULL,3,'8120401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 475,669,'Спортно-туристически дейности',NULL,3,'8130101',1,'2020-11-19 00:00:00.000',NULL,1,'Sports and tourist activity','Sportliche und touristische Aktivitäten','Activités sportives et touristiques' UNION ALL
select 476,670,'Помощник- възпитател в отглеждането и възпитанието на деца',NULL,3,'8140101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 477,671,'Дамско фризьорство',NULL,2,'8150101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 478,671,'Мъжко фризьорство',NULL,2,'8150102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 479,672,'Козметика',NULL,2,'8150201',1,'2020-11-19 00:00:00.000',NULL,1,'Cosmetics','Kosmetik','Cosmétique' UNION ALL
select 480,673,'Корабоводене - морско',NULL,3,'8400101',1,'2020-11-19 00:00:00.000',NULL,1,'Ship navigation - marine','Seeschiffsnavigation','Navigation - en mer' UNION ALL
select 481,673,'Корабоводене - речно',NULL,3,'8400102',1,'2020-11-19 00:00:00.000',NULL,1,'Ship navigation - river','Flußschiffsnavigation','Navigation - fluviale' UNION ALL
select 482,674,'Моряк',NULL,2,'8400201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 483,675,'Експлоатация на ж.п.инфраструктура',NULL,3,'8400301',1,'2020-11-19 00:00:00.000',NULL,1,'Exploitation of the railway infrastructure','Betrieb der Eisenbahinfrastruktur','Exploitation des infrastructures ferroviaires' UNION ALL
select 484,676,'Обучение на водачи на МПС',NULL,4,'8400401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 485,677,'Летателна експлоатация на самолет',NULL,4,'8400501',1,'2020-11-19 00:00:00.000',NULL,1,'Flying exploitation of an airplane','Flugzeugbetrieb','Exploitation de vol d’un avion' UNION ALL
select 486,677,'Летателна експлоатация на вертолет',NULL,4,'8400502',1,'2020-11-19 00:00:00.000',NULL,1,'Flying exploitation of a helicopter','Hubschrauberbetrieb','Exploitation de vol d’un hélicoptère' UNION ALL
select 487,899,'Експлоатация на пристанищата и флота',NULL,3,'8400601',1,'2020-11-19 00:00:00.000',NULL,1,'Exploitation of ports and the navy','Betrieb der Häfen und der Flotte','Exploitation des ports et de la flotte' UNION ALL
select 488,895,'Експлоатация на автомобилния транспорт',NULL,3,'8400701',1,'2020-11-19 00:00:00.000',NULL,1,'Exploitation of automobile transport','Betrieb von Straßentransporten','Exploitation du transport routier' UNION ALL
select 489,896,'Търговска експлоатация на железопътния транспорт',NULL,3,'8400801',1,'2020-11-19 00:00:00.000',NULL,1,'Commercial exploitation of railway transport','Handelsbetrieb des Schienenverkehrs','Exploitation commerciale du transport ferroviaire' UNION ALL
select 490,681,'Екология и опазване на околната среда',NULL,3,'8510101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 491,682,'Екология и опазване на околната среда',NULL,4,'8510201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 492,683,'Охрана на обществения ред и личността',NULL,3,'8610101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 493,65,'Проучване на полезни изкопаеми','Проучване на полезни изкопаеми',NULL,'17001',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 497,67,'Дълбоко нефтено сондиране',NULL,NULL,'17202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 499,68,'Дълбоко сондиране и експлоатация на нефтени и газови находища',NULL,NULL,'17300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 500,69,'Открито добиване на полезни изкопаеми','Открито добиване на полезни изкопаеми',NULL,'17401',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 503,70,'Механизация на минните предприятия',NULL,NULL,'17501',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 504,70,'Електроснабдяване и електрообзавеждане на минните предприятия',NULL,NULL,'17502',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 506,72,'Добив и обработка на скални материали',NULL,NULL,'17700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 507,73,'Механизация за добив и обработка на скални материали','Механизация за добив и обработка на скални материали',NULL,'17800',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 508,74,'Производство на художествени изделия от скални материали',NULL,NULL,'17900',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 509,75,'Маркшайдерство и геодезия',NULL,NULL,'18000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 510,76,'Хидрология и хидрогеология',NULL,NULL,'18100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 512,78,'Добиване на черни метали',NULL,NULL,'27001',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 516,80,'Ядрена топлоенергетика',NULL,NULL,'37000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 517,81,'Термични и водоенергетични машини и съоръжения',NULL,NULL,'37100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 518,82,'Централизирано топлоснабдяване',NULL,NULL,'37201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 519,82,'Климатична, вентилационна и отоплителна техника',NULL,NULL,'37202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 520,82,'Хладилна техника',NULL,NULL,'37203',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 521,83,'Технология на машиностроенето - студена обработка',NULL,NULL,'47100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 522,84,'Технология на машиностроенето - уредостроене',NULL,NULL,'47200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 523,85,'Леярство','Леярство',NULL,'47301',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 524,85,'Обработка на металите чрез пластична деформация','Обработка на металите чрез пластична деформация',NULL,'47302',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 525,85,'Заваряване на металите',NULL,NULL,'47303',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 526,85,'Термична обработка на металите','Термична обработка на металите',NULL,'47304',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 527,86,'Автоматизация на производството',NULL,NULL,'47400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 528,87,'Роботостроене',NULL,NULL,'47501',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 531,89,'Оптика, оптико-механични и оптико-електронни уреди',NULL,NULL,'47700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 532,90,'Кинотехника и видеотехника',NULL,NULL,'47800',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 533,91,'Лазерна техника',NULL,NULL,'47900',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 534,92,'Измервателна техника',NULL,NULL,'48000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 536,94,'Монтаж на промишлени съоръжения и машини',NULL,NULL,'48200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 537,95,'Машини и апарати в химическата промишленост','Машини и апарати в химическата промишленост',NULL,'48300',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 538,96,'Машини и апарати в хранително-вкусовата промишленост',NULL,NULL,'48400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 540,97,'Машини и съоръжения в тъкачното производство',NULL,NULL,'48502',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 541,97,'Машини и съоръжения в плетачното производство','Машини и съоръжения в плетачното производство',NULL,'48503',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 542,97,'Машини и съоръжения в апретурното, багрилното и печатното производство','Машини и съоръжения в апретурното, багрилното и печатното производство',NULL,'48504',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 543,97,'Машини и съоръжения в шевното производство',NULL,NULL,'48505',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 546,98,'Биотехника',NULL,NULL,'48600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 547,99,'Хидро и пневмотехника',NULL,NULL,'48700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 548,100,'Машини и апарати за очистване на въздуха',NULL,NULL,'48800',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 549,101,'Роботизирани и гъвкави автоматизирани производствени системи',NULL,NULL,'48900',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 550,102,'Технолог - програмист на системи с ЦПУ',NULL,NULL,'49000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 551,103,'Ремонт и поддържане на системи с ЦПУ',NULL,NULL,'49100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 552,105,'Електрически машини и апарати',NULL,NULL,'57000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 553,106,'Електрообзавеждане на промишлени предприятия',NULL,NULL,'57100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 554,107,'Електрически централи и мрежи',NULL,NULL,'57200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 555,108,'Електрификация на селското стопанство',NULL,NULL,'57300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 556,109,'Електротехника на автомобилния транспорт',NULL,NULL,'57400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 557,110,'Електрообзавеждане на електрически превозни средства за градски транспорт',NULL,NULL,'57500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 558,111,'Асансьорна техника',NULL,NULL,'57600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 560,113,'Електрообзавеждане на кораби',NULL,NULL,'57800',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 561,114,'Радиотехника и телевизия',NULL,NULL,'57900',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 562,115,'Конструиране и технология на съобщителната апаратура',NULL,NULL,'58001',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 563,115,'Строителство и експлоатация на съобщителни системи',NULL,NULL,'58002',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 564,116,'Промишлена електроника',NULL,NULL,'58101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 565,116,'Електронно-изчислителна техника',NULL,NULL,'58102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 566,116,'Ядрена електротехника',NULL,NULL,'58103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 567,116,'Нискочестотна техника',NULL,NULL,'58104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 568,116,'Електровакуумна техника','Електровакуумна техника',NULL,'58105',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 570,116,'Микропроцесорна техника',NULL,NULL,'58107',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 571,117,'Конструиране и технология на електронни елементи','Конструиране и технология на електронни елементи',NULL,'58200',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 573,119,'Автоматизация на производството',NULL,NULL,'58400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 574,120,'Микропроцесорна техника','Микропроцесорна техника',NULL,'58500',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 575,121,'Програмно осигуряване и информационни технологии',NULL,NULL,'58600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 576,122,'Компютърна техника и технологии',NULL,NULL,'58700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 578,125,'Автомобили и кари',NULL,NULL,'67000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 579,126,'Управление на транспортното предприятие в автомобилния транспорт',NULL,NULL,'67101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 580,126,'Управление на транспортното предприяние в ж.п. транспорт',NULL,NULL,'67102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 581,126,'Управление на транспортното предприятие в градския транспорт','Управление на транспортното предприятие в градския транспорт',NULL,'67103',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 582,127,'Железопътна техника',NULL,NULL,'67200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 584,129,'Корабоводене - речно',NULL,NULL,'67400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 585,130,'Корабоводене - морско',NULL,NULL,'67500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 586,131,'Експлоатация на пристанищата и флотата',NULL,NULL,'67600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 587,132,'Корабостроене',NULL,NULL,'67700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 588,133,'Технология на риболова, рибопреработването и аквакултури',NULL,NULL,'67800',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 590,135,'Експлоатация и ремонт на самолети',NULL,NULL,'68000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 591,136,'Ремонт на корабни машини и механизми',NULL,NULL,'68100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 592,137,'Корабни машини и механизми',NULL,NULL,'68200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 593,138,'Автоматизация в ж.п. транспорт',NULL,NULL,'68300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 594,140,'Технология на неорганичните вещества',NULL,NULL,'77101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 595,140,'Технология на органичните вещества',NULL,NULL,'77102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 596,140,'Технология на полимерите',NULL,NULL,'77103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 598,140,'Технология на целулозата, хартията и картона',NULL,NULL,'77105',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 599,140,'Технология на нефта, газа и твърдите горива',NULL,NULL,'77106',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 600,140,'Технология на неорганичните и органичните вещества',NULL,NULL,'77107',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 601,141,'Биотехнологии',NULL,NULL,'77200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 602,142,'Технология на стъкларското производство',NULL,NULL,'77301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 603,142,'Технология на порцелановото и фаянсовото производство',NULL,NULL,'77302',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 604,142,'Технология на строителната керамика и огнеупорните материали','Технология на строителната керамика и огнеупорните материали',NULL,'77303',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 606,143,'Дизайн в силикатното производство',NULL,NULL,'77400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 607,144,'Технология на опазване на околната среда',NULL,NULL,'77500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 608,145,'Строителство и архитектура',NULL,NULL,'87100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 609,146,'Водно строителство',NULL,NULL,'87200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 611,148,'Геодезия',NULL,NULL,'87400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 612,149,'Транспортно строителство',NULL,NULL,'87500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 613,150,'Полевъдство',NULL,NULL,'97001',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 615,150,'Трайни насаждения',NULL,NULL,'97003',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 616,150,'Растителна защита и агрохимия',NULL,NULL,'97004',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 617,150,'Селскостопански мелиорации','Селскостопански мелиорации',NULL,'97005',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 619,151,'Говедовъдство и овцевъдство',NULL,NULL,'97101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 620,151,'Свиневъдство и птицевъдство','Свиневъдство и птицевъдство',NULL,'97102',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 622,152,'Ветеринарен техник',NULL,NULL,'97201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 623,152,'Ветеринарен лаборант',NULL,NULL,'97202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 624,153,'Експлоатация на селскостопанската техника',NULL,NULL,'97301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 625,153,'Ремонт на селскоктопанска техника',NULL,NULL,'97302',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 627,155,'Земеделски техник',NULL,NULL,'97700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 628,158,'Горско стопанство и дърводобив',NULL,NULL,'107000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 629,159,'Механизация на горското стопанство и дърводобива',NULL,NULL,'107100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 630,160,'Механична технология на дървесината','Механична технология на дървесината',NULL,'107200',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 631,161,'Мебелно производство',NULL,NULL,'107300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 632,162,'Вътрешна архитектура',NULL,NULL,'107400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 633,163,'Дърворезбарство',NULL,NULL,'107500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 635,165,'Тапицерство и декораторство',NULL,NULL,'107700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 636,166,'Производство на музикални инструменти',NULL,NULL,'107800',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 637,167,'Парково строителство',NULL,NULL,'107900',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 638,168,'Реставрация на стилни мебели и дограма',NULL,NULL,'108000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 639,170,'Техника и технология на хляба и хлебните изделия',NULL,NULL,'117000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 640,171,'Технология на зърносъхранението, зърнопреработването и фуражите',NULL,NULL,'117100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 641,172,'Техника и технология на захарта и захарните изделия',NULL,NULL,'117200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 642,173,'Техника и технология на млякото и млечните продукти',NULL,NULL,'117300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 643,174,'Техника и технология на алкохолните и безалкохолните напитки',NULL,NULL,'117400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 644,175,'Техника и технология на месото и месните продукти',NULL,NULL,'117500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 645,176,'Технология на тютюна и тютюневите изделия',NULL,NULL,'117600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 646,177,'Техника и технология на растителните мазнини и сапуни',NULL,NULL,'117700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 647,178,'Техника и технология на консервното производство',NULL,NULL,'117800',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 648,179,'Технология на производството и обслужването в общественото хранене',NULL,NULL,'117900',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 649,180,'Експлоатация и поддържане на хладилна техника в хранителната промишленост',NULL,NULL,'118000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 650,186,'Технология на предачеството',NULL,NULL,'127000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 651,187,'Технология на тъкачеството',NULL,NULL,'127100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 652,188,'Технология на плетачеството',NULL,NULL,'127200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 653,189,'Технология на апретурата, багренето и печатането',NULL,NULL,'127300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 654,190,'Художествено оформление на тъкани площни изделия',NULL,NULL,'127401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 655,190,'Художествено оформление на плетени площни изделия',NULL,NULL,'127402',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 656,191,'Моделиране и конструиране на дамско облекло',NULL,NULL,'127501',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 657,191,'Моделиране и конструиране на мъжко облекло','Моделиране и конструиране на мъжко облекло',NULL,'127502',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 658,192,'Технология на дамското облекло',NULL,NULL,'127601',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 659,192,'Технология на мъжкото облекло',NULL,NULL,'127602',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 660,193,'Технология на коженото и кожухарско облекло',NULL,NULL,'127701',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 661,193,'Моделиране и конструиране на кожено и кожухарско облекло',NULL,NULL,'127702',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 662,194,'Технология на кожите',NULL,NULL,'127801',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 666,195,'Цялостно изработване на обувките',NULL,NULL,'127903',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 667,196,'Моделиране и конструиране на обувните изделия',NULL,NULL,'128000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 668,197,'Технология на кожено-галантерийното производство',NULL,NULL,'128100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 669,198,'Моделиране и конструиране на кожено-галантерийни изделия',NULL,NULL,'128200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 670,199,'Автоматизация на производството в текстилното производство',NULL,NULL,'128301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 673,200,'Икономика, управление и финанси на търговията',NULL,NULL,'137100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 674,201,'Икономика, управление и финанси на кооперациите',NULL,NULL,'137200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 675,202,'Банково, застрахователно и осигурително дело',NULL,NULL,'137300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 676,203,'Машинна обработка на информацията',NULL,NULL,'137400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 678,205,'Организатор на текстилното производство',NULL,NULL,'137601',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 679,205,'Организатор на шивашкото производство',NULL,NULL,'137602',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 680,205,'Организатор в хранително-вкусовата промишленост',NULL,NULL,'137603',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 682,206,'Организация и управление на туризма',NULL,NULL,'137700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 683,207,'Икономика на промишлеността',NULL,NULL,'137800',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 684,208,'Икономика на земеделското стопанство',NULL,NULL,'137901',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 685,209,'Счетоводна отчетност',NULL,NULL,'138000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 686,210,'Стопански мениджмънт',NULL,NULL,'138100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 687,211,'Бизнес-администрация',NULL,NULL,'138200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 688,870,'Оператор на машини за дълбоко нефтено сондиране','Оператор на машини за дълбоко нефтено сондиране',NULL,'1010101',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 697,878,'Оператор на обработващи металургични агрегати','Оператор на обработващи металургични агрегати',NULL,'1020102',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 702,884,'Оператор на химико-технологични процеси (по производства)',NULL,NULL,'1020400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 703,886,'Оператор за преработка на пластмаси','Оператор за преработка на пластмаси',NULL,'1020501',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 704,886,'Оператор за преработка на каучук','Оператор за преработка на каучук',NULL,'1020502',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 705,888,'Оператор в стъкларското производство','Оператор в стъкларското производство',NULL,'1020601',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 706,888,'Оператор в керамично производство',NULL,NULL,'1020602',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 711,891,'Оператор в химическото чистене и пране',NULL,NULL,'1020802',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 712,892,'Лаборант','Лаборант',NULL,'1020900',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 713,893,'Оператор в биотехнологични процеси (по производства)','Оператор в биотехнологични процеси (по производства)',NULL,'1030100',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 714,236,'Оператор в консервното производство',NULL,NULL,'1030201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 715,236,'Оператор по зърнопреработка',NULL,NULL,'1030202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 716,236,'Оператор в производство на захар и захарни изделия',NULL,NULL,'1030203',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 718,236,'Оператор в производство на мазнини и сапуни',NULL,NULL,'1030205',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 719,236,'Оператор в производство на месни продукти',NULL,NULL,'1030206',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 720,236,'Оператор в производство на млечни продукти',NULL,NULL,'1030207',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 721,236,'Оператор в производство на цигари','Оператор в производство на цигари',NULL,'1030208',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 722,236,'Оператор в производство на безалкохолни и алкохолни напитки',NULL,NULL,'1030209',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 724,238,'Оператор в производство на хляб и хлебни изделия',NULL,NULL,'1030301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 725,238,'Оператор в производство на сладкарски изделия','Оператор в производство на сладкарски изделия',NULL,'1030302',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 727,240,'Стругар',NULL,NULL,'1040101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 728,240,'Фрезист','Фрезист',NULL,'1040102',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 730,242,'Оператор - настройчик на металорежещи машини с ЦПУ - за стругови машини',NULL,NULL,'1040201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 734,246,'Леяр-формовчик','Леяр-формовчик',NULL,'1040401',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 740,255,'Оператор на енергийни агрегати','Оператор на енергийни агрегати',NULL,'1050100',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 746,277,'Оператор в дърворезно, амбалажно и паркетно производство',NULL,NULL,'1060101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 749,279,'Оператор в производство на корпусна и решетъчна мебел',NULL,NULL,'1060201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 750,279,'Оператор в производство на врати и прозорци',NULL,NULL,'1060202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 751,279,'Оператор в производство на тапицирани изделия',NULL,NULL,'1060203',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 756,286,'Оператор в предачното производство','Оператор в предачното производство',NULL,'1070100',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 757,288,'Оператор в тъкачното производство',NULL,NULL,'1070200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 758,290,'Оператор в трикотажното производство','Оператор в трикотажното производство',NULL,'1070301',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 759,290,'Оператор в чорапното производство','Оператор в чорапното производство',NULL,'1070302',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 760,290,'Оператор в производство на нетъкан текстил','Оператор в производство на нетъкан текстил',NULL,'1070303',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 761,299,'Оператор в производство на облекло от тъкани и трикотаж',NULL,NULL,'1080101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 763,299,'Оператор в производство на облекло по поръчка',NULL,NULL,'1080103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 764,301,'Оператор в кожено-галантерийно производство',NULL,NULL,'1080200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 766,303,'Оператор на машини за ушиване на лицеви и хастарски детайли на обувки',NULL,NULL,'1080302',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 767,303,'Оператор на машини за цялостно изработване на обувки',NULL,NULL,'1080303',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 771,313,'Кондуктор в ж.п. транспорт','Кондуктор в ж.п. транспорт',NULL,'1090402',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 773,316,'Монтьор на машини, апарати, уреди и съоръжения - енергийни съоръжения','Монтьор на машини, апарати, уреди и съоръжения - енергийни съоръжения',NULL,'2000101',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 774,316,'Монтьор на машини, апарати, уреди и съоръжения - топлопреносни мрежи и отоплителни инсталации',NULL,NULL,'2000102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 775,316,'Монтьор на машини, апарати, уреди и съоръжения - климатична техника',NULL,NULL,'2000103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 776,316,'Монтьор на машини, апарати, уреди и съоръжения - хладилници и хл.инсталации',NULL,NULL,'2000104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 777,316,'Монтьор на машини, апарати, уреди и съоръжения - металургични агрегати',NULL,NULL,'2000105',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 779,316,'Монтьор на машини, апарати, уреди и съоръжения - металорежещи машини',NULL,NULL,'2000107',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 780,316,'Монтьор на машини, апарати, уреди и съоръжения - машини и апарати в химическото производство',NULL,NULL,'2000108',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 781,316,'Монтьор на машини, апарати, уреди и съоръжения - текстилни машини','Монтьор на машини, апарати, уреди и съоръжения - текстилни машини',NULL,'2000109',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 782,316,'Монтьор на машини, апарати, уреди и съоръжения - разкрояващи и шевни машини',NULL,NULL,'2000110',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 783,316,'Монтьор на машини, апарати, уреди и съоръжения - подвижен ж.п. състав','Монтьор на машини, апарати, уреди и съоръжения - подвижен ж.п. състав',NULL,'2000111',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 784,316,'Монтьор на машини, апарати, уреди и съоръжения - подемно-транспортни машини и съоръжения','Монтьор на машини, апарати, уреди и съоръжения - подемно-транспортни машини и съоръжения',NULL,'2000112',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 785,316,'Монтьор на машини, апарати, уреди и съоръжения - машини в хранително-вкусовата промишленост',NULL,NULL,'2000113',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 787,316,'Монтьор на машини, апарати, уреди и съоръжения - корабни машини и съоръжения',NULL,NULL,'2000115',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 788,316,'Монтьор на машини, апарати, уреди и съоръжения - корабни тръбни системи',NULL,NULL,'2000116',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 791,316,'Монтьор на машини, апарати, уреди и съоръжения - двигатели с вътрешно горене',NULL,NULL,'2000119',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 792,317,'Монтьор на оптико-механични уреди','Монтьор на оптико-механични уреди',NULL,'2000201',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 794,317,'Монтьор на лазерна техника','Монтьор на лазерна техника',NULL,'2000203',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 796,318,'Минен електромашинен монтьор','Минен електромашинен монтьор',NULL,'2000300',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 797,319,'Заварчик',NULL,NULL,'2000400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 798,320,'Шлосер-инструменталчик','Шлосер-инструменталчик',NULL,'2000500',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 799,321,'Монтьор на електрически машини и трансформатори',NULL,NULL,'2010101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 800,321,'Монтьор на електрически апарати, уреди и устройства',NULL,NULL,'2010102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 801,323,'Монтьор по електрообзавеждане на промишлени и селскостопански предприятия',NULL,NULL,'2010201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 802,323,'Монтьор по електрообзавеждане на селскостопанска техника','Монтьор по електрообзавеждане на селскостопанска техника',NULL,'2010202',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 803,323,'Монтьор по електрообзавеждане на кораби',NULL,NULL,'2010203',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 804,323,'Монтьор по електрообзавеждане на подвижен ж.п. състав','Монтьор по електрообзавеждане на подвижен ж.п. състав',NULL,'2010204',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 805,323,'Монтьор по електрообзавеждане на асансьори',NULL,NULL,'2010205',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 808,325,'Монтьор на електроинсталации в сгради, на мрежи и уредби за ниско напрежение',NULL,NULL,'2010301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 809,325,'Монтьор на електрически мрежи и уредби за високо напрежение',NULL,NULL,'2010302',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 810,325,'Монтьор на контактни мрежи и тягови подстанции',NULL,NULL,'2010303',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 811,327,'Монтьор на радиоелектронна техника',NULL,NULL,'2010401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 812,327,'Монтьор на промишлена електроника',NULL,NULL,'2010402',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 813,327,'Монтьор на електронно-изчислителна техника',NULL,NULL,'2010403',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 814,327,'Монтьор на съобщителна техника',NULL,NULL,'2010404',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 815,327,'Монтьор на електроакустична техника','Монтьор на електроакустична техника',NULL,'2010405',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 816,327,'Монтьор на градивни елементи','Монтьор на градивни елементи',NULL,'2010406',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 818,329,'Монтьор на стационарни телеграфни и телефонни съоръжения',NULL,NULL,'2010501',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 820,329,'Монтьор на радиотелевизионни съоръжения','Монтьор на радиотелевизионни съоръжения',NULL,'2010503',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 823,331,'Монтьор на контролно-измервателна и регулираща апаратура',NULL,NULL,'2010600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 824,334,'Монтажник на В и К инсталации',NULL,NULL,'2020101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 825,334,'Монтажник на отоплителни инсталации',NULL,NULL,'2020102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 826,334,'Монтажник на вентилационни и климатични инсталации и уредби',NULL,NULL,'2020103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 827,335,'Монтажник на промишлени съоръжения','Монтажник на промишлени съоръжения',NULL,'2020201',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 830,336,'Механизатор за подземен добив на въглища','Механизатор за подземен добив на въглища',NULL,'3010101',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 831,336,'Механизатор за подземен добив на руди','Механизатор за подземен добив на руди',NULL,'3010102',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 832,338,'Механизатор за открит добив на въглища','Механизатор за открит добив на въглища',NULL,'3010201',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 840,354,'Крановик на кранове стрелкови тип, монтирани на автомобили, на самоходни шасита автомобил тип и др. и на несамоходни шасита до 16 т и водач на МПС, "С"',NULL,NULL,'3020204',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 844,356,'Механизатор на трайни насаждения','Механизатор на трайни насаждения',NULL,'3030101',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 846,356,'Механизатор в полевъдството','Механизатор в полевъдството',NULL,'3030103',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 847,357,'Механизатор в говедовъдството','Механизатор в говедовъдството',NULL,'3030201',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 849,357,'Механизатор в свиневъдството','Механизатор в свиневъдството',NULL,'3030203',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 852,358,'Механизатор в дърводобива и водач на МПС, кат. "Т" и "С"',NULL,NULL,'3030301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 854,359,'Паркостроител','Паркостроител',NULL,'3030400',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 856,363,'Машинист - монтьор на пътно-строителни машини и водач на МПС, категория "Т" и "С"',NULL,NULL,'3040101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 858,365,'Монтьор на автомобили и водач на МПС, кат. "С" или "В"',NULL,NULL,'3040301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 859,365,'Монтьор на кари и водач на МПС, кат. "Т" или "С"',NULL,NULL,'3040302',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 860,365,'Автокаросерист и водач на МПС, кат. "В" или "С"',NULL,NULL,'3040303',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 861,365,'Монтьор на селскостопанска техника и водач на МПС, кат. "Т", "В" или "С"','Монтьор на селскостопанска техника и водач на МПС, кат. "Т", "В" или "С"',NULL,'3040304',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 862,365,'Монтьор на трактори и водач на МПС категория "Т", "В" или "С"',NULL,NULL,'3040305',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 863,365,'Монтьор по електрообзавеждане на МПС и водач на МПС, кат. "В" или "С"',NULL,NULL,'3040306',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 864,366,'Монтьор-водач на тролейбус',NULL,NULL,'3040401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 866,367,'Пристанищен механизатор и водач на МПС, кат. "Т" и "С"',NULL,NULL,'3040500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 868,369,'Маневрен и кабинен оператор','Маневрен и кабинен оператор',NULL,'3040701',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 870,370,'Помощник - локомотивен машинист',NULL,NULL,'3040800',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 872,372,'Строител - монтажник на стоманенобетонни конструкции и изделия','Строител - монтажник на стоманенобетонни конструкции и изделия',NULL,'4010102',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 873,372,'Строител в производство на метални конструкции и изделия','Строител в производство на метални конструкции и изделия',NULL,'4010103',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 875,372,'Строител в производство и монтаж на кофражни елементи и армировъчни изделия','Строител в производство и монтаж на кофражни елементи и армировъчни изделия',NULL,'4010105',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 876,374,'Строител на облицовки, настилки и мазилки',NULL,NULL,'4010201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 877,374,'Строителен бояджия и изпълнител на декоративни покрития',NULL,NULL,'4010202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 878,374,'дърводелец и монтажник на вътрешно обзавеждане','дърводелец и монтажник на вътрешно обзавеждане',NULL,'4010203',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 879,374,'Строителен железар и монтажник на ламаринени изделия','Строителен железар и монтажник на ламаринени изделия',NULL,'4010204',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 880,374,'Строител - реставратор','Строител - реставратор',NULL,'4010205',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 881,374,'Зидар - кофражист',NULL,NULL,'4010206',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 887,387,'Строител на пътища, магистрали и съоръжения към тях','Строител на пътища, магистрали и съоръжения към тях',NULL,'4020301',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 891,406,'Стенограф - машинописец',NULL,NULL,'5000101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 892,406,'Документалист','Документалист',NULL,'5000102',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 893,406,'Организатор по административно обслужване',NULL,NULL,'5000103',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 894,407,'Продавач - консултант и касиер на хранителни стоки','Продавач - консултант и касиер на хранителни стоки',NULL,'5000301',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 895,407,'Продавач - консултант и касиер на промишлени стоки','Продавач - консултант и касиер на промишлени стоки',NULL,'5000302',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 897,408,'Готвач',NULL,NULL,'5000401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 898,408,'Сладкар',NULL,NULL,'5000402',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 899,408,'Сервитьор',NULL,NULL,'5000403',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 900,409,'Бръснар - фризьор',NULL,NULL,'5000500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 902,411,'Козметик',NULL,NULL,'5000700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 905,418,'Корабостроител','Корабостроител',NULL,'6000300',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 918,879,'Подземен добив на полезни изкопаеми','Подземен добив на полезни изкопаеми',NULL,'1020102',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 937,243,'Климатична техника',NULL,NULL,'1040201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 938,243,'Отоплителна техника',NULL,NULL,'1040202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 939,243,'Хладилна техника',NULL,NULL,'1040203',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 940,243,'Газови и водни инсталации',NULL,NULL,'1040204',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 943,247,'Електрически машини и апарати',NULL,NULL,'1040401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 956,252,'Електрообзавеждане на предприятия','Електрообзавеждане на предприятия',NULL,'1040703',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 961,252,'Ел.мрежи и уредби','Ел.мрежи и уредби',NULL,'1040708',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 969,258,'Техник-технолог на художествени изделия от метал',NULL,NULL,'1050201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 996,268,'Заварчик','Заварчик',NULL,'1050901',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1017,289,'Компютърни системи',NULL,NULL,'1070203',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1022,293,'Промишлена електроника','Промишлена електроника',NULL,'1070502',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1101,345,'Помощник - локомотивен машинист',NULL,NULL,'3010801',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1104,348,'Автомобили кат. "С" (или "В")',NULL,NULL,'3011101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1106,348,'Селскостопанска техника кат. "Т" (или "С")','Селскостопанска техника кат. "Т" (или "С")',NULL,'3011103',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1110,350,'Пристанищен механизатор и водач на МПС кат. "Т" и "С"',NULL,NULL,'3011301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1114,373,'Тъкачество',NULL,NULL,'4010102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1133,386,'Облекло от текстил',NULL,NULL,'40202201',0,'2021-11-26 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1158,421,'Строителство и архитектура','Строителство и архитектура',NULL,'6010101',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1161,422,'Геодезист - картограф','Геодезист - картограф',NULL,'60102',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1164,423,'Зидаро-мазачество','Зидаро-мазачество',NULL,'6010303',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1167,423,'Облицовъчни работи и полагане на настилки','Облицовъчни работи и полагане на настилки',NULL,'6010306',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1172,423,'Хидростроителни работи','Хидростроителни работи',NULL,'6010311',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1176,424,'В и К инсталации','В и К инсталации',NULL,'6010401',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1193,437,'Оранжерийно зеленчуково производство','Оранжерийно зеленчуково производство',NULL,'7010501',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1216,442,'Техник - озеленител',NULL,NULL,'7020301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1245,459,'Счетоводител','Счетоводител',NULL,'8010401',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1249,460,'Трудова борса',NULL,NULL,'8010504',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1251,462,'Продавач - консултант',NULL,NULL,'8010701',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1254,464,'Администратор - документалист',NULL,NULL,'8020201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1257,467,'Готвач','Готвач',NULL,'9010301',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1262,472,'Дамско фризьорство',NULL,NULL,'9010801',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1265,473,'Козметик',NULL,NULL,'9010901',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1266,474,'Камериер',NULL,NULL,'9011001',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1274,482,'Изобразителни изкуства',NULL,NULL,'10010101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1277,482,'Каменоделство',NULL,NULL,'10010104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1282,482,'Дизайн',NULL,NULL,'10010109',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1285,483,'Пиано','Пиано',NULL,'10010201',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1306,484,'Артист - вокалист',NULL,NULL,'10010301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1307,485,'Класически танци',NULL,NULL,'10010401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1312,482,'Художник',NULL,NULL,'10010100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1316,1,'Стругар',NULL,NULL,'66660101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1317,3,'Шлосер',NULL,NULL,'66660201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1318,5,'Монтьор на електродомакински уреди',NULL,NULL,'66660301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1336,39,'Мебелист',NULL,NULL,'66662101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1338,41,'Тапицер',NULL,NULL,'66662301',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1350,53,'Работник в производство на сладкарски изделия',NULL,NULL,'66663501',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1351,54,'Помощник - готвач',NULL,NULL,'66663601',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1352,54,'Помощник - сладкар',NULL,NULL,'66663602',0,'2021-11-26 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1358,59,'Шивач',NULL,NULL,'66664101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1363,487,'Художник - изпълнител',NULL,3,'2110100',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1364,488,'Музикант - инструменталист',NULL,3,'2120100',1,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1365,490,'Музикант - вокалист',NULL,3,'2120300',1,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1366,2,'Електронен техник - компютърни системи',NULL,NULL,'1',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1367,4,'Шивач моделиер - Технолог на облекло',NULL,NULL,'201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1369,8,'Строителен техник',NULL,NULL,'4',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1371,12,'Техник в дървообработването',NULL,NULL,'6',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1372,14,'Програмист',NULL,NULL,'7',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1373,16,'Техник - електроинсталации',NULL,NULL,'8',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1374,18,'Техник по газоснабдяване',NULL,NULL,'9',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1375,20,'Секретар - администратор',NULL,NULL,'10',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1376,22,'Фермер',NULL,NULL,'11',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1377,24,'Телекомуникационен техник',NULL,NULL,'12',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1378,26,'Геодезист',NULL,NULL,'13',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1379,28,'Икономист - организатор',NULL,NULL,'14',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1380,30,'Хотел и кетъринг',NULL,NULL,'15',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1381,32,'Техник на битова техника',NULL,NULL,'16',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1382,34,'Машинен техник',NULL,NULL,'17',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1383,36,'Автомобилен механик',NULL,NULL,'18',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1384,126,'Управление на транспортното предприятие във водния транспорт',NULL,NULL,'67104',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1385,184,'Мениджмънт на туризма',NULL,NULL,'118500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1386,219,'Лозаро-винар',NULL,NULL,'97600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1387,185,'Мениджмънт в хлебопроизводството и сладкарството',NULL,NULL,'118600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1388,104,'Индустриален механик',NULL,NULL,'49400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1389,124,'Промишлена естетика и дизайн в електрониката и машиностроенето',NULL,NULL,'58900',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1390,419,'Асистент - учител',NULL,NULL,'6000400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1391,420,'Социален работник',NULL,NULL,'6000500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1392,169,'Горско и ловно стопанство',NULL,NULL,'108100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1393,181,'Мениджмънт в хотелиерството',NULL,NULL,'118100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1394,182,'Лаборант - технолог в химическата промишленост',NULL,NULL,'118201',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1395,182,'Лаборант - технолог в хранително-вкусовата промишленост',NULL,NULL,'118202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1397,183,'Технология и мениджмънт на производството и обслужването в заведенията за хранене',NULL,NULL,'118300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1398,361,'Земеделец',NULL,NULL,'3030600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1399,372,'Строител в производство и монтаж на кофражни изделия','Строител в производство и монтаж на кофражни изделия',NULL,'4010106',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1401,374,'Строител на облицовки и настилки',NULL,NULL,'4010207',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1402,374,'Строител на зидарии и мазилки',NULL,NULL,'4010208',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1404,139,'Радиоелектронно оборудване на летателни апарати',NULL,NULL,'68400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1405,156,'Цветарство',NULL,NULL,'97800',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1406,157,'Коневъдство и конен спорт',NULL,NULL,'98000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1407,362,'Монтьор на селскостопанска техника и водач на МПС категория "Т" и "В"',NULL,NULL,'3030700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1408,408,'Сервитьор - готвач',NULL,NULL,'5000404',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1409,408,'Сервитьор - барман',NULL,NULL,'5000405',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1410,217,'Програмист - настройчик на машини с ЦПУ, роботи и ГАПС',NULL,NULL,'49700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1411,215,'Технология на машиностроенето - производство на боеприпаси',NULL,NULL,'49300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1412,216,'Техник - технолог на художествени изделия от метал',NULL,NULL,'49500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1413,120,'Електронен техник - компютърни системи',NULL,NULL,'59001',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1414,213,'Машинен техник - Ортопедичен техник и бандажист',NULL,NULL,'40100',0,'2021-11-26 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1415,684,'Техник - организатор в машиностроенето',NULL,NULL,'49600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1416,372,'Сухо строителство',NULL,NULL,'4010108',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1417,192,'Технология на облеклото',NULL,NULL,'127600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1418,191,'Моделиране и конструиране на облеклото',NULL,NULL,'127500',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1419,4,'Шивач моделиер - Моделиер конструктор',NULL,NULL,'202',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1420,238,'Оператор в производството на хляб, хлебни и сладкарски изделия',NULL,NULL,'1030300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1421,220,'Технология и мениджмънт на производството на мляко и млечни произведения',NULL,NULL,'118401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1422,220,'Технология и мениджмънт на производството на месо и месни продукти',NULL,NULL,'118402',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1423,115,'Съобщителна техника',NULL,NULL,'58000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1424,218,'Технология на фармацевтични и парфюмерийно-козметични производства',NULL,NULL,'77700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1425,222,'Икономист - посредник на трудовата борса',NULL,NULL,'138300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1426,223,'Икономист - посредник в митническата дейност',NULL,NULL,'138400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1428,225,'Спортен мениджмънт',NULL,NULL,'138600',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1430,227,'Икономика и мениджмънт',NULL,NULL,'138800',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1432,229,'Организация и управление на свободното време',NULL,NULL,'139000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1433,205,'Организатор в химическата промишленост',NULL,NULL,'137604',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1434,140,'Технология на взривните вещества','Технология на взривните вещества',NULL,'77108',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1435,221,'Компютърно проектиране на текстилни площни изделия',NULL,NULL,'128400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1436,221,'Компютърно проектиране на плетени площни изделия',NULL,NULL,'128402',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1437,685,'Организатор на малко предприятие',NULL,NULL,'5000800',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1438,463,'Организатор на среден и дребен бизнес',NULL,NULL,'8010200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1439,220,'Технология и мениджмънт на зърносъхранението, зърнопреработката и фуражите',NULL,NULL,'118403',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1440,205,'Организатор на производство на хляб, хлебни и сладкарски изделия',NULL,NULL,'137606',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1441,153,'Механизация на селското стопанство',NULL,NULL,'97300',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1442,321,'Електромонтьор на битова техника',NULL,NULL,'2010700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1443,65,'Геология и проучване на полезни изкопаеми','Геология и проучване на полезни изкопаеми',NULL,'17000',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1444,67,'Техника на проучване и сондажна електромеханика','Техника на проучване и сондажна електромеханика',NULL,'17200',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1445,69,'Добивни и строителни минни технологии','Добивни и строителни минни технологии',NULL,'17400',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1446,70,'Минна електромеханика','Минна електромеханика',NULL,'17500',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1447,78,'Металургия на черните метали',NULL,NULL,'27000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1448,79,'Металургия на цветните метали',NULL,NULL,'27100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1449,82,'Топлинна и хладилна техника',NULL,NULL,'37200',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1451,85,'Технология на машиностроенето - топла обработка','Технология на машиностроенето - топла обработка',NULL,'47300',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1452,87,'Робототехника','Робототехника',NULL,'47500',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1453,97,'Машини и съоръжения в леката промишленост','Машини и съоръжения в леката промишленост',NULL,'48500',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1455,116,'Електронна техника',NULL,NULL,'58100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1457,140,'Химични технологии','Химични технологии',NULL,'77100',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1459,150,'Растениевъдство',NULL,NULL,'97000',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1460,151,'Животновъдство',NULL,NULL,'97100',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1461,152,'Ветеринарна медицина',NULL,NULL,'97200',0,'2021-11-26 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1463,182,'Технологичен и микробиологичен контрол','Технологичен и микробиологичен контрол',NULL,'118200',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1464,220,'Технология и мениджмънт на производството в ХВП',NULL,NULL,'118400',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1468,195,'Технология на обувното производство','Технология на обувното производство',NULL,'127900',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1470,221,'Компютърно проектиране на тъкани площни изделия',NULL,NULL,'128401',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1471,205,'Организатор на производството','Организатор на производството',NULL,'137600',0,NULL,NULL,1,NULL,NULL,NULL UNION ALL
select 1472,230,'Туристически водач',NULL,NULL,'139700',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1480,232,'Промишлен дизайн',NULL,NULL,'147101',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1481,232,'Пространствен дизайн',NULL,NULL,'147102',0,'2021-08-23 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1492,490,'Поп и джаз пеене',NULL,3,'2120303',1,'2020-11-19 00:00:00.000',NULL,1,'Pop and jazz singing','Pop- und Jazzgesang','Chant pop et jazz' UNION ALL
select 1493,902,'Театрална кукла',NULL,3,'2140111',1,'2020-11-19 00:00:00.000',NULL,1,'Theatrical puppet','Theaterpuppe','Marionnette théâtrale' UNION ALL
select 1494,520,'Търговско посредничество',NULL,2,'3420102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1495,521,'Финансово посредничество',NULL,2,'3430103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1496,522,'Счетоводство',NULL,2,'3440102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1497,523,'Митническо и данъчно обслужване',NULL,2,'3440202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1498,686,'Индустрия',NULL,2,'3451101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1499,686,'Търговия',NULL,2,'3451102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1500,686,'Земеделско стопанство',NULL,2,'3451103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1501,686,'Икономика и мениджмънт',NULL,2,'3451104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1502,686,'Предприемачество и мениджмънт',NULL,2,'3451105',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1503,538,'Икономическо информационно осигуряване',NULL,2,'4820102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1504,539,'Промишлена естетика и дизайн',NULL,3,'5210116',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1505,866,'Технология на стъкларското производство',NULL,3,'5241101',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of glassware production','Technologe in der Glasherstellung ','Technologie de la production de verre' UNION ALL
select 1506,866,'Технология на керамичното производство',NULL,3,'5241102',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of ceramics production','Technologie der Keramikherstellung','Technologie de la production de céramique' UNION ALL
select 1507,866,'Технология на свързващите вещества',NULL,3,'5241103',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of adhesives','Technologie der Bindemittel','Technologie des agents liants' UNION ALL
select 1508,678,'Технология на силикатните производства',NULL,2,'5241201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1509,678,'Технология на керамичното производство',NULL,2,'5241202',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of ceramics production','Technologie der Keramikherstellung','Technologie de la production de céramique' UNION ALL
select 1510,678,'Технология на свързващите вещества',NULL,2,'5241203',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of adhesives','Technologie der Bindemittel','Technologie des agents liants' UNION ALL
select 1511,567,'Технология в биопроизводствата',NULL,3,'5240201',1,'2020-11-19 00:00:00.000',NULL,1,'Technology in bioproductions','Technologie in der Bioproduktion','Technologie en production biologique' UNION ALL
select 1512,566,'Апретура, багрене, печатане и химическо чистене',NULL,3,'5240112',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1513,679,'Лозаровинарство',NULL,2,'6210901',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1514,680,'Спортно-туристически дейности',NULL,4,'8130201',1,'2020-11-19 00:00:00.000',NULL,1,'Sports and tourist activity','Sportliche und touristische Aktivitäten','Activités sportives et touristiques' UNION ALL
select 1515,897,'По вид спорт',NULL,4,'8130301',1,'2020-11-19 00:00:00.000',NULL,1,'By type of sport','Nach Sportart','Par type de sport' UNION ALL
select 1516,862,'Адаптирана физическа активност и спорт за хора с увреждания',NULL,4,'8130401',1,'2020-11-19 00:00:00.000',NULL,1,'Adapted physical activity and sports for disabled people','Angepasste körperliche Aktivität und Sport für Menschen mit Behinderungen','Activité physique adaptée et de sport pour personnes handicapées' UNION ALL
select 1517,863,'Фитнес',NULL,4,'8130501',1,'2020-11-19 00:00:00.000',NULL,1,'Fitness','Fitness','Remise en forme physique' UNION ALL
select 1518,864,'Спортен масаж',NULL,4,'8130601',1,'2020-11-19 00:00:00.000',NULL,1,'Sports massage','Sportmassage','Massage sportif' UNION ALL
select 1519,683,'Лична охрана',NULL,3,'8610102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1520,683,'Физическа охрана на обекти',NULL,3,'8610103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1521,683,'Организация на охранителната дейност',NULL,4,'8610104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1522,865,'Търсене, спасяване и извършване на аварийно-възстановителни работи',NULL,3,'8610201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1524,571,'Апретура, багрене, печатане и химическо чистене',NULL,2,'5240612',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1525,671,'Фризьорство',NULL,2,'8150101',1,'2020-11-19 00:00:00.000',NULL,1,'Hairdresser''s','Friseurwesen','Coiffure' UNION ALL
select 1526,683,'Банкова охрана и инкасова дейност',NULL,3,'8610101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1527,900,'Художник',NULL,3,'2110100',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1528,900,'Живопис',NULL,3,'2110101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1529,900,'Стенопис',NULL,3,'2110102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1530,900,'Графика',NULL,3,'2110103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1531,900,'Скулптура',NULL,3,'2110104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1532,900,'Рекламна графика',NULL,3,'2110105',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1533,900,'Художествена дърворезба',NULL,3,'2110106',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1534,900,'Художествена керамика',NULL,3,'2110107',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1535,900,'Художествена тъкан',NULL,3,'2110108',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1536,900,'Художествена обработка на  метали',NULL,3,'2110109',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1537,900,'Иконопис',NULL,3,'2110110',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1538,900,'Илюстрация и оформление на книгата',NULL,3,'2110111',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1539,491,'Класически танц',NULL,3,'2120401',1,'2020-11-19 00:00:00.000',NULL,1,'Classical dance','Klassischer Tanz','Danse classique' UNION ALL
select 1540,901,'На радиопредаване',NULL,4,'2131401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1541,901,'На телевизионно предаване',NULL,4,'2131402',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1542,902,'Театрален, кино и телевизионен декор',NULL,3,'2140101',1,'2020-11-19 00:00:00.000',NULL,1,'Theatrical, cinema and television settings','Theater-, Kino- und Fernsehdekoration','Décor de théâtre, de cinéma et de télévision' UNION ALL
select 1543,902,'Художествено осветление за театър, кино и телевизия',NULL,3,'2140102',1,'2020-11-19 00:00:00.000',NULL,1,'Artistic lighting for theatre, cinema and television','Künstlerische Beleuchtung für Theater, Kino und Fernsehen','Éclairage artistique pour le théâtre, le cinéma et la télévision' UNION ALL
select 1544,902,'Театрален грим и перуки',NULL,3,'2140103',1,'2020-11-19 00:00:00.000',NULL,1,'Theatrical makeup and wigs','Theaterschminke und Perücken','Maquillage de théâtre et perruques' UNION ALL
select 1545,902,'Аранжиране',NULL,3,'2140104',1,'2020-11-19 00:00:00.000',NULL,1,'Arrangement','Arrangieren','Arrangement' UNION ALL
select 1546,902,'Детски играчки и сувенири',NULL,3,'2140105',1,'2020-11-19 00:00:00.000',NULL,1,'Toys for children and souvenirs','Spielzeug und Souvenirs','Jouets et souvenirs' UNION ALL
select 1547,902,'Интериорен дизайн',NULL,3,'2140106',1,'2020-11-19 00:00:00.000',NULL,1,'Interior design','Innenarchitektur','Design d''intérieur' UNION ALL
select 1548,902,'Силикатен дизайн',NULL,3,'2140107',1,'2020-11-19 00:00:00.000',NULL,1,'Silicate design','Silikat-Design','Design en silicate' UNION ALL
select 1549,902,'Моден дизайн',NULL,3,'2140108',1,'2020-11-19 00:00:00.000',NULL,1,'Fashion design','Modedesign ','Création de mode' UNION ALL
select 1550,902,'Пространствен дизайн',NULL,3,'2140109',1,'2020-11-19 00:00:00.000',NULL,1,'Spatial design','Raumdesign','Conception spatiale' UNION ALL
select 1551,902,'Промишлен дизайн',NULL,3,'2140110',1,'2020-11-19 00:00:00.000',NULL,1,'Industrial design','Industrielles Design','Design industriel' UNION ALL
select 1552,902,'Дизайнер',NULL,3,'2140100',1,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1553,903,'Стъкларско производство',NULL,1,'5240701',1,'2020-11-19 00:00:00.000',NULL,1,'Glassware industry','Glasherstellung','Industrie du verre' UNION ALL
select 1554,904,'Керамично производство',NULL,1,'5240801',1,'2020-11-19 00:00:00.000',NULL,1,'Ceramics industry','Keramikherstellung','Production de céramique' UNION ALL
select 1555,905,'Преработване на полимери',NULL,1,'5240901',1,'2020-11-19 00:00:00.000',NULL,1,'Polymer processing','Polymerverarbeitung','Traitement des polymères' UNION ALL
select 1556,595,'Компютърно проектиране и десениране на тъкани площни изделия',NULL,3,'5420101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1557,595,'Компютърно проектиране и десениране на плетени площни изделия',NULL,3,'5420102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1558,906,'Оптометрия',NULL,4,'7250201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1559,907,'Дизайн в силикатното производство',NULL,3,'5240501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1560,678,'Технология на стъкларското производство',NULL,2,'5241201',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of glassware production','Technologe in der Glasherstellung','Technologie de la production de verre' UNION ALL
select 1561,908,'Командване на войскови единици до взвод',NULL,4,'8630101',1,'2020-11-19 00:00:00.000',NULL,1,'Commanding army forces up to platoon','Führung von Militäreinheiten bis zu einem Zug ','Commandement d''unités militaires à un peloton' UNION ALL
select 1563,910,'Експлоатация и ремонт на автомобилна и бронетанкова техника',NULL,4,'8630301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1564,910,'Експлоатация и ремонт на инженерна, химическа и специална техника',NULL,4,'8630302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1565,910,'Експлоатация и ремонт на радиотехническа, информационна и компютърна техника',NULL,4,'8630303',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1566,910,'Експлоатация и ремонт на артилерийска и оптическа техника. Работа с бойни припаси',NULL,4,'8630304',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1567,910,'Експлоатация и ремонт на зенитно въоръжение и радио-локационни станции',NULL,4,'8630305',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1568,910,'Експлоатация и ремонт на авиационна и навигационна техника',NULL,4,'8630306',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1569,910,'Експлоатация и ремонт на корабна техника',NULL,4,'8630307',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1570,910,'Взривни технологии и неутрализиране на невзривени бойни припаси',NULL,4,'8630308',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1571,911,'Обща и специализирана администрация',NULL,4,'8630401',1,'2020-11-19 00:00:00.000',NULL,1,'Total and specialized administration','Allgemeine und spezialisierte Verwaltung','Administration générale et spécialisée' UNION ALL
select 1572,912,'Инструкторска дейност по общовойскова и специална подготовка',NULL,4,'8630501',1,'2020-11-19 00:00:00.000',NULL,1,'Instructor activity in general military and special preparation','Ausbildungstätigkeit in allgemeiner militärischer und spezieller Ausbildung','Activité d''instruction dans la formation militaire générale et spéciale' UNION ALL
select 1573,525,'Мениджмънт в услугите в прецизната техника',NULL,4,'3450211',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1574,539,'Автоматизация на дискретните производства',NULL,3,'5210115',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1575,592,'Декорация на сладкарски изделия',NULL,2,'5410303',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1576,913,'Финансова отчетност',NULL,2,'3430201',1,'2020-11-19 00:00:00.000',NULL,1,'Financial reporting','Rechnungslegung','Comptabilité financière' UNION ALL
select 1577,914,'Оперативно счетоводство',NULL,3,'3440101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1578,525,'Предприемачество и мениджмънт по професионални направления',NULL,4,'3450201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1579,525,'Предприемачество и мениджмънт в машиностроенето',NULL,4,'3450202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1580,525,'Предприемачество и мениджмънт в строителство',NULL,4,'3450203',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1581,525,'Предприемачество и мениджмънт в хотелиерството и ресторантьорството',NULL,4,'3450204',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1582,525,'Предприемачество и мениджмънт в туризма',NULL,4,'3450205',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1583,525,'Предприемачество и мениджмънт в спорта',NULL,4,'3450206',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1584,525,'Предприемачество и мениджмънт в транспорта',NULL,4,'3450207',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1585,525,'Предприемачество и мениджмънт във фризьорството и козметиката',NULL,4,'3450208',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1586,525,'Предприемачество и мениджмънт в производството на обувки и кожено-галантерийни изделия',NULL,4,'3450209',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1587,525,'Предприемачество и мениджмънт в производството на облекло и в модния дизайн',NULL,4,'3450210',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1588,525,'Предприемачество и мениджмънт на услугите в прецизната техника',NULL,4,'3450211',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1589,915,'Деловодство и архив',NULL,1,'3460301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1590,571,'Декорация на силикатни изделия',NULL,2,'5240613',1,'2020-11-19 00:00:00.000',NULL,1,'Decoration of silicate articles','Dekoration von Silikatprodukten','Décoration de produits en silicate' UNION ALL
select 1591,916,'Автобояджийство',NULL,1,'5251201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1592,916,'Автотенекеджийство',NULL,1,'5251202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1593,931,'Обработване на товари',NULL,1,'5251301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1594,590,'Производство на растителни масла, маслопродукти и етерични масла',NULL,3,'5410109',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1595,591,'Производство на растителни масла, маслопродукти и етерични масла',NULL,2,'5410208',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1596,917,'Контрол на качеството и безопасност на храни и напитки',NULL,3,'5410601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1597,918,'Производство на фасонирани материали',NULL,1,'5430301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1598,918,'Производство на тапицирани изделия',NULL,1,'5430302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1599,918,'Производство на бъчви',NULL,1,'5430303',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1600,918,'Производство на плетени изделия от дървесина',NULL,1,'5430304',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1601,918,'Производство на врати и прозорци',NULL,1,'5430305',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1602,619,'Минни съоръжения',NULL,2,'5440205',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1603,919,'Основни и довършителни работи',NULL,1,'5820801',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1604,920,'Пътища, магистрали и съоръжения',NULL,1,'5820901',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1605,920,'Релсови пътища и съоръжения',NULL,1,'5820902',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1606,921,'Зеленчукопроизводство',NULL,1,'6211101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1607,921,'Трайни насаждения',NULL,1,'6211102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1608,921,'Тютюнопроизводство',NULL,1,'6211103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1609,921,'Гъбопроизводство и билки, етерични-маслени и медоносни култури',NULL,1,'6211104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1610,922,'Говедовъдство и биволовъдство',NULL,1,'6211201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1611,922,'Овцевъдство и козевъдство',NULL,1,'6211202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1612,922,'Свиневъдство',NULL,1,'6211203',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1613,922,'Птицевъдство',NULL,1,'6211204',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1614,922,'Зайцевъдство',NULL,1,'6211205',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1615,922,'Пчеларство',NULL,1,'6211206',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1616,922,'Бубарство',NULL,1,'6211207',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1617,923,'Отглеждане на кучета',NULL,2,'6211301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1618,923,'Специализирано обучение и селекционно развъждане на кучета',NULL,3,'6211302',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1619,924,'Озеленяване и цветарство',NULL,1,'6220301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1620,925,'Рибовъдство',NULL,3,'6240101',1,'2020-11-19 00:00:00.000',NULL,1,'Fish farming','Fischzucht','Pisciculture' UNION ALL
select 1621,926,'Рибовъдство',NULL,2,'6240301',1,'2020-11-19 00:00:00.000',NULL,1,'Fish farming','Fischzucht','Pisciculture' UNION ALL
select 1622,927,'Рибовъдство',NULL,1,'6240401',1,'2020-11-19 00:00:00.000',NULL,1,'Fish farming','Fischzucht','Pisciculture' UNION ALL
select 1623,928,'Социална работа с деца и семейства в риск',NULL,3,'7620201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1624,928,'Социална работа с деца и възрастни с увреждания и хронични заболявания',NULL,3,'7620202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1625,929,'Отглеждане и възпитание на деца в домашни условия',NULL,4,'8140201',1,'2020-11-19 00:00:00.000',NULL,1,'Raising and education of children at home','Erziehung der Kinder zu Hause','Élever et éduquer des enfants à la maison' UNION ALL
select 1626,930,'Вътрешни и международни превози на пътници',NULL,1,'8400901',1,'2020-11-19 00:00:00.000','2021-12-16 00:00:00.000',1,'Domestic and international transport of passengers','Personenbeförderung im In- und Ausland','Transport de passagers national et international' UNION ALL
select 1627,930,'Вътрешни и международни превози на товари',NULL,1,'8400902',1,'2020-11-19 00:00:00.000','2021-12-16 00:00:00.000',1,'Domestic and international transport of freights','Frachtbeförderung im In- und Ausland','Transport de fret national et international' UNION ALL
select 1630,909,'Логистика',NULL,4,'8630201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1631,561,'Компютърни мрежи',NULL,4,'5230502',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1632,628,'Строител на пътища, магистрали и съоръжения към тях',NULL,2,'5820601',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1633,628,'Строител на релсови пътища и съоръжения към тях',NULL,2,'5820602',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1634,667,'Екскурзоводство',NULL,4,'8120301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1635,668,'Туристическа анимация',NULL,4,'8120401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1636,673,'Корабоводене - морско',NULL,4,'8400101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1637,581,'Експлоатация и ремонт на летателни апарати',NULL,4,'5250901',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1638,581,'Експлоатация и ремонт на електронно - приборна авиационна техника',NULL,4,'5250902',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1639,520,'Маркетинг',NULL,4,'3420101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1640,524,'Икономика и мениджмънт',NULL,4,'3450104',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1641,525,'Мениджмънт в туризма',NULL,4,'3450205',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1642,524,'Предприемачество и мениджмънт',NULL,4,'3450105',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1643,525,'Мениджмънт в спорта',NULL,4,'3450206',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1644,567,'Технология в биопроизводствата',NULL,2,'5240201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1645,569,'Технологичен и микробиологичен контрол в химични производства',NULL,2,'5240401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1646,569,'Технологичен и микробиологичен контрол в хранително - вкусови производства',NULL,2,'5240402',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1647,634,'Животновъдство',NULL,1,'6210501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1648,632,'Растениевъдство',NULL,1,'6210301',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1649,639,'Цветарство',NULL,1,'6220201',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1650,639,'Парково строителство и озеленяване',NULL,1,'6220202',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1651,554,'Възобновяеми енергийни източници',NULL,3,'5220308',1,'2020-11-19 00:00:00.000',NULL,1,'Renewable energy sources','Erneuerbaren Energiequellen','Sources d''énergie renouvelable' UNION ALL
select 1653,555,'Възобновяеми енергийни източници',NULL,2,'5220408',1,'2020-11-19 00:00:00.000',NULL,1,'Renewable energy sources','Erneuerbaren Energiequellen','Sources d''énergie renouvelable' UNION ALL
select 1654,932,'Художник - изящни изкуства',NULL,3,'2110100',1,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1655,932,'Живопис',NULL,3,'2110101',1,'2020-11-19 00:00:00.000',NULL,1,'Painting','Malerei','Peinture' UNION ALL
select 1656,932,'Стенопис',NULL,3,'2110102',1,'2020-11-19 00:00:00.000',NULL,1,'Fresco painting','Wandmalerei','Fresque' UNION ALL
select 1657,932,'Графика',NULL,3,'2110103',1,'2020-11-19 00:00:00.000',NULL,1,'Graphic art','Graphik','Graphique' UNION ALL
select 1658,932,'Скулптура',NULL,3,'2110104',1,'2020-11-19 00:00:00.000',NULL,1,'Sculpture','Skulptur','Sculpture' UNION ALL
select 1659,932,'Илюстрация и оформление на книгата',NULL,3,'2110111',1,'2020-11-19 00:00:00.000',NULL,1,'Book illustration and layout','Illustration und Buchgestaltung','Illustration et mise en page du livre' UNION ALL
select 1660,933,'Филмов монтаж',NULL,3,'2130101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1661,933,'Видеомонтаж',NULL,3,'2130102',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1662,933,'Компютърен монтаж',NULL,3,'2130103',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1663,934,'Фотография',NULL,3,'2130201',1,'2020-11-19 00:00:00.000',NULL,1,'Photography','Fotografie','Photographie' UNION ALL
select 1664,935,'Полиграфия',NULL,3,'2130301',1,'2020-11-19 00:00:00.000',NULL,1,'Printing industry','Polygrafie','Polygraphie' UNION ALL
select 1665,936,'Тоноператорство',NULL,3,'2130401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1666,937,'Компютърна анимация',NULL,3,'2130501',1,'2020-11-19 00:00:00.000',NULL,1,'Computer animation','Computeranimation','Animation par ordinateur' UNION ALL
select 1667,938,'Компютърна графика',NULL,3,'2130601',1,'2020-11-19 00:00:00.000',NULL,1,'Computer graphics','Computergrafik ','Graphique par ordinateur' UNION ALL
select 1668,939,'Графичен дизайн',NULL,3,'2130701',1,'2020-11-19 00:00:00.000',NULL,1,'Graphic design','Grafikdesign','Conception graphique' UNION ALL
select 1669,940,'Музикално оформление',NULL,3,'2130801',1,'2020-11-19 00:00:00.000',NULL,1,'Musical setting','Musikbearbeitung','Création musicale' UNION ALL
select 1670,941,'Анимация',NULL,4,'2130901',1,'2020-11-19 00:00:00.000',NULL,1,'Animation','Animation','Animation' UNION ALL
select 1671,942,'Кино и телевизия',NULL,4,'2131001',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1672,943,'Кино и телевизия',NULL,4,'2131101',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1673,944,'Телевизия',NULL,4,'2131301',1,'2020-11-19 00:00:00.000',NULL,1,'Television','Fernsehen','Télévision' UNION ALL
select 1674,945,'На радиопредаване',NULL,4,'2131401',1,'2020-11-19 00:00:00.000',NULL,1,'of radio transmission','Radiomoderator','Dans une radio émission' UNION ALL
select 1675,945,'На телевизионно предаване',NULL,4,'2131402',1,'2020-11-19 00:00:00.000',NULL,1,'of television transmission','Fernsehmoderator','Dans une émission de télévision' UNION ALL
select 1676,902,'Рекламна графика',NULL,3,'2140112',1,'2020-11-19 00:00:00.000',NULL,1,'Advertising graphics','Werbegrafik','Graphique publicitaire' UNION ALL
select 1677,946,'Класически инструменти',NULL,3,'2150101',1,'2020-11-19 00:00:00.000',NULL,1,'Classical instruments','Klassische Musikinstrumente','Instruments classiques' UNION ALL
select 1678,946,'Народни инструменти',NULL,3,'2150102',1,'2020-11-19 00:00:00.000',NULL,1,'Folk instruments','Volksmusikinstrumente','Instruments folkloriques' UNION ALL
select 1679,948,'Бижутерия',NULL,2,'2150301',1,'2020-11-19 00:00:00.000',NULL,1,'Jewellery','Juweliere','Bijouterie' UNION ALL
select 1680,949,'Дърворезбарство',NULL,2,'2150401',1,'2020-11-19 00:00:00.000',NULL,1,'Wood-carving','Holzbildhauerei','Sculpture sur bois' UNION ALL
select 1681,950,'Керамика',NULL,2,'2150501',1,'2020-11-19 00:00:00.000',NULL,1,'Ceramics','Keramik','Céramique' UNION ALL
select 1682,951,'Каменоделство',NULL,2,'2150601',1,'2020-11-19 00:00:00.000',NULL,1,'Stone-cutting','Steinbildhauerei','Taille de pierre' UNION ALL
select 1683,951,'Декоративни скални облицовки',NULL,2,'2150602',1,'2020-11-19 00:00:00.000',NULL,1,'Decorative rock facing','Dekorative Felsverkleidungen','Parements de roche décoratifs' UNION ALL
select 1694,952,'Художествена дърворезба',NULL,3,'2150701',1,'2020-11-19 00:00:00.000',NULL,1,'Artistic wood-carving','Künstlerische Holzbildhauerei','Sculpture artistique sur bois' UNION ALL
select 1695,952,'Художествена керамика',NULL,3,'2150702',1,'2020-11-19 00:00:00.000',NULL,1,'Artistic ceramics','Künstlerische Keramik','Céramique d''art' UNION ALL
select 1696,952,'Художествена тъкан',NULL,3,'2150703',1,'2020-11-19 00:00:00.000',NULL,1,'Artistic weaving','Künstlerische Stoffe','Tissu artistique' UNION ALL
select 1697,952,'Художествена обработка на метали',NULL,3,'2150704',1,'2020-11-19 00:00:00.000',NULL,1,'Artistic metal processing','Künstlerische Bearbeitung von Metallen','Traitement artistique des métaux' UNION ALL
select 1698,952,'Иконопис',NULL,3,'2150705',1,'2020-11-19 00:00:00.000',NULL,1,'Icon painting','Ikonenmalerei','Peinture d''icônes' UNION ALL
select 1699,953,'Източноправославно християнство',NULL,3,'2210101',1,'2020-11-19 00:00:00.000',NULL,1,'East Orthodox Christianity','Orthodoxes Christentum ','Christianisme orthodoxe oriental' UNION ALL
select 1700,954,'Продавач - консултант',NULL,2,'3410201',1,'2020-11-19 00:00:00.000',NULL,1,'Shop-assistant','Verkaufsberater','Conseiller de vente' UNION ALL
select 1701,955,'Недвижими имоти',NULL,3,'3410301',1,'2020-11-19 00:00:00.000',NULL,1,'Real estates','Immobilien','Biens immobiliers' UNION ALL
select 1702,956,'Търговия на едро и дребно',NULL,3,'3410401',1,'2020-11-19 00:00:00.000',NULL,1,'Wholesale and retail trade','Groß-und Einzelhandel','Commerce en gros et en détail' UNION ALL
select 1703,957,'Маркетингови проучвания',NULL,2,'3420201',1,'2020-11-19 00:00:00.000',NULL,1,'Marketing research','Marktforschung','Recherches en marketing' UNION ALL
select 1704,958,'Митническа и данъчна администрация',NULL,3,'3440201',1,'2020-11-19 00:00:00.000',NULL,1,'Customs and tax administration','Zoll- und Steuerverwaltung','Administration douanière et fiscale' UNION ALL
select 1705,958,'Митническо и данъчно обслужване',NULL,2,'3440202',1,'2020-11-19 00:00:00.000',NULL,1,'Customs and tax service','Zoll- und Steuerservice','Services douaniers et fiscaux' UNION ALL
select 1706,959,'Оперативно счетоводство',NULL,3,'3440301',1,'2020-11-19 00:00:00.000',NULL,1,'Operating accountancy','Operative Buchhaltung','Comptable opérationnel' UNION ALL
select 1707,960,'Предприемачество и мениджмънт',NULL,4,'3450212',1,'2020-11-19 00:00:00.000',NULL,1,'Entrepreneurship and management','Unternehmerschaft und Management','Entrepreneuriat et gestion' UNION ALL
select 1708,961,'Бизнес - услуги',NULL,2,'3450401',1,'2020-11-19 00:00:00.000',NULL,1,'Business services','Geschäftsdienstleistungen','Services aux entreprises' UNION ALL
select 1709,962,'Малък и среден бизнес',NULL,2,'3450501',1,'2020-11-19 00:00:00.000',NULL,1,'Small and medium business','Klein- und Mittelbetriebe','Petites et moyennes entreprises' UNION ALL
select 1710,963,'Касиер',NULL,1,'3450601',1,'2020-11-19 00:00:00.000',NULL,1,'Cashier','Kassierer','Caissier' UNION ALL
select 1711,964,'Калкулант',NULL,1,'3450701',1,'2020-11-19 00:00:00.000',NULL,1,'Calculator','Kalkulant','Calculant' UNION ALL
select 1712,965,'Снабдител',NULL,1,'3450801',1,'2020-11-19 00:00:00.000',NULL,1,'Supplier','Einkäufer','Fournisseur' UNION ALL
select 1713,967,'Индустрия',NULL,3,'3451201',1,'2020-11-19 00:00:00.000',NULL,1,'Industry','Industrie','Industrie' UNION ALL
select 1714,967,'Търговия',NULL,3,'3451202',1,'2020-11-19 00:00:00.000',NULL,1,'Commerce','Handel','Commerce' UNION ALL
select 1715,967,'Земеделско стопанство',NULL,3,'3451203',1,'2020-11-19 00:00:00.000',NULL,1,'Agricultural farm','Landwirtschaft','Agriculture' UNION ALL
select 1716,967,'Икономика и мениджмънт',NULL,3,'3451204',1,'2020-11-19 00:00:00.000',NULL,1,'Economy and management','Wirtschaft und Management','Économie et gestion' UNION ALL
select 1717,968,'Бизнес администрация',NULL,3,'3460101',1,'2020-11-19 00:00:00.000',NULL,1,'Business administration','Geschäftsverwaltung','Administration des affaires' UNION ALL
select 1718,969,'Административно обслужване',NULL,2,'3460201',1,'2020-11-19 00:00:00.000',NULL,1,'Administrative service','Verwaltungsdienst','Service administratif' UNION ALL
select 1719,970,'Деловодство и архив',NULL,1,'3460301',1,'2020-11-19 00:00:00.000',NULL,1,'Registry and archive','Aktenführung und Archiv','Secrétariat et archivage' UNION ALL
select 1720,971,'Програмно осигуряване',NULL,2,'4810101',1,'2020-11-19 00:00:00.000',NULL,1,'Software programmes','Software','Logiciel' UNION ALL
select 1721,972,'Системно програмиране',NULL,3,'4810201',1,'2020-11-19 00:00:00.000',NULL,1,'System programming','Systemprogrammierung','Programmation de système' UNION ALL
select 1722,973,'Икономическа информатика',NULL,3,'4820101',1,'2020-11-19 00:00:00.000',NULL,1,'Economical computer science','Wirtschaftsinformatik','Informatique économique' UNION ALL
select 1723,974,'Икономическо информационно осигуряване',NULL,2,'4820201',1,'2020-11-19 00:00:00.000',NULL,1,'Economic information provision ','Wirtschaftsinformationsbereitstellung','Fourniture d''informations économiques' UNION ALL
select 1724,975,'Текстообработване',NULL,1,'4820301',1,'2020-11-19 00:00:00.000',NULL,1,'Text processing','Textbearbeitung','Traitement de texte' UNION ALL
select 1725,976,'Електронна търговия',NULL,3,'4820401',1,'2020-11-19 00:00:00.000',NULL,1,'E-trade','E-Commerce','Commerce électronique' UNION ALL
select 1726,977,'Машини и съоръжения в металургията',NULL,3,'5210103',1,'2020-11-19 00:00:00.000',NULL,1,'Machines and facilities in metallurgy','Maschinen und Anlagen in der Metallurgie','Machines et équipements en métallurgie' UNION ALL
select 1727,977,'Машини и системи с ЦПУ',NULL,3,'5210105',1,'2020-11-19 00:00:00.000',NULL,1,'CNC machines and systems','CNC-Maschinen und -Systeme','Machines et systèmes à CNC' UNION ALL
select 1728,977,'Машини и съоръжения за заваряване',NULL,3,'5210113',1,'2020-11-19 00:00:00.000',NULL,1,'Welding machines and facilities','Schweißmaschinen und -anlagen','Machines et équipements de soudage' UNION ALL
select 1729,977,'Технология на машиностроенето',NULL,3,'5210117',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of machine building','Maschinenbautechnologie','Technologies de l’industrie des machines-outils' UNION ALL
select 1730,977,'Машини и съоръжения за химическата и хранително-вкусовата промишленост',NULL,3,'5210118',1,'2020-11-19 00:00:00.000',NULL,1,'Machines and facilities for the chemical and food industry','Maschinen und Anlagen für die Chemie- und Lebensmittelindustrie','Machines et équipements pour l''industrie chimique et alimentaire' UNION ALL
select 1731,977,'Машини и съоръжения в текстилната, обувната и дървообработващата промишленост',NULL,3,'5210119',1,'2020-11-19 00:00:00.000',NULL,1,'Machines and facilities in the textile, shoe making and wood processing industry','Maschinen und Anlagen in der Textil-, Schuh- und Holzverarbeitungsindustrie','Machines et équipements dans les industries du textile, de la chaussure et du bois' UNION ALL
select 1732,977,'Машини и съоръжения за добивната промишленост и строителството',NULL,3,'5210120',1,'2020-11-19 00:00:00.000',NULL,1,'Machines and facilities for the mining (extractive)  industry and construction','Maschinen und Anlagen für die Bergbauindustrie und den Bau','Machines et équipements pour l''industrie extractive et la construction' UNION ALL
select 1733,978,'Художествени изделия от метал',NULL,3,'5210201',1,'2020-11-19 00:00:00.000',NULL,1,'Artistic articles from metal','Künstlerische Produkte aus Metall','Produits métalliques artistiques' UNION ALL
select 1734,978,'Промишлена естетика и дизайн',NULL,3,'5210202',1,'2020-11-19 00:00:00.000',NULL,1,'Industrial aesthetics and design','Industrielle Ästhetik und Design','Esthétique et design industriel' UNION ALL
select 1735,979,'Металорежещи машини',NULL,2,'5210301',1,'2020-11-19 00:00:00.000',NULL,1,'Metal cutting machines','Zerspannungsmaschinen','Machines-outils' UNION ALL
select 1736,979,'Машини за гореща обработка на металите',NULL,2,'5210302',1,'2020-11-19 00:00:00.000',NULL,1,'Machines for hot processing of metals','Maschinen für Warmbearbeitung von Metallen','Machines pour le traitement à chaud des métaux' UNION ALL
select 1737,979,'Машини и съоръжения за заваряване',NULL,2,'5210303',1,'2020-11-19 00:00:00.000',NULL,1,'Welding machines and facilities','Schweißmaschinen und -anlagen','Machines et équipements de soudage' UNION ALL
select 1738,980,'Машини и съоръжения в металургията',NULL,2,'5210403',1,'2020-11-19 00:00:00.000',NULL,1,'Machines and facilities in metallurgy','Maschinen und Anlagen in der Metallurgie','Machines et équipements en métallurgie' UNION ALL
select 1739,980,'Металообработващи машини',NULL,2,'5210414',1,'2020-11-19 00:00:00.000',NULL,1,'Metal processing machines','Metallbearbeitungsmaschinen','Machines-outils' UNION ALL
select 1740,980,'Машини и съоръжения за химическата и хранително-вкусовата промишленост',NULL,2,'5210415',1,'2020-11-19 00:00:00.000',NULL,1,'Machines and facilities for the chemical and food industry','Maschinen und Anlagen für die Chemie- und Lebensmittelindustrie','Machines et équipements pour l''industrie chimique et alimentaire' UNION ALL
select 1741,980,'Машини и съоръжения в текстилната, обувната и дървообработващата промишленост',NULL,2,'5210416',1,'2020-11-19 00:00:00.000',NULL,1,'Machines and facilities in the textile, shoe making and wood processing industry','Maschinen und Anlagen in der Textil-, Schuh- und Holzverarbeitungsindustrie','Machines et équipements dans les industries du textile, de la chaussure et du bois' UNION ALL
select 1742,980,'Машини и съоръжения за добивната промишленост и строителството',NULL,2,'5210417',1,'2020-11-19 00:00:00.000',NULL,1,'Machines and facilities for the mining (extractive) industry and construction','Maschinen und Anlagen für die Bergbauindustrie und den Bau','Machines et équipements pour l''industrie extractive et la construction' UNION ALL
select 1743,980,'Машини и съоръжения в дървообработващата промишленост',NULL,1,'5210408',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1744,981,'Измервателна и организационна техника',NULL,3,'5210501',1,'2020-11-19 00:00:00.000',NULL,1,'Measuring and organizational equipment','Mess- und Organisationstechnik','Appareils de mesure et d''organisation' UNION ALL
select 1745,981,'Лазерна и оптична техника',NULL,3,'5210504',1,'2020-11-19 00:00:00.000',NULL,1,'Laser and optical equipment','Laser und optische Geräte','Équipement laser et optique' UNION ALL
select 1746,981,'Медицинска техника',NULL,3,'5210505',1,'2020-11-19 00:00:00.000',NULL,1,'Medical equipment','Medizintechnik','Équipement médical' UNION ALL
select 1747,982,'Измервателна и организационна техника',NULL,2,'5210601',1,'2020-11-19 00:00:00.000',NULL,1,'Measuring and organizational equipment','Mess- und Organisationstechnik','Appareils de mesure et d''organisation' UNION ALL
select 1748,982,'Лазерна и оптична техника',NULL,2,'5210604',1,'2020-11-19 00:00:00.000',NULL,1,'Laser and optical equipment','Laser und optische Geräte','Équipement laser et optique' UNION ALL
select 1749,982,'Медицинска техника',NULL,2,'5210605',1,'2020-11-19 00:00:00.000',NULL,1,'Medical equipment','Medizintechnik','Équipement médical' UNION ALL
select 1750,983,'Металургия на черните метали',NULL,3,'5210701',1,'2020-11-19 00:00:00.000',NULL,1,'Metallurgy of ferrous metals','Metallurgie von Eisenmetallen','Métallurgie des métaux ferreux' UNION ALL
select 1751,983,'Металургия на цветните метали',NULL,3,'5210702',1,'2020-11-19 00:00:00.000',NULL,1,'Metallurgy of non-ferrous metals','Nichteisenmetallurgie','Métallurgie des métaux non ferreux' UNION ALL
select 1752,984,'Металургия на черните метали',NULL,2,'5210801',1,'2020-11-19 00:00:00.000',NULL,1,'Metallurgy of ferrous metals','Metallurgie von Eisenmetallen','Métallurgie des métaux ferreux' UNION ALL
select 1753,984,'Металургия на цветните метали',NULL,2,'5210802',1,'2020-11-19 00:00:00.000',NULL,1,'Metallurgy of non-ferrous metals','Nichteisenmetallurgie','Métallurgie des métaux non ferreux' UNION ALL
select 1754,985,'Заваряване',NULL,1,'5210901',1,'2020-11-19 00:00:00.000',NULL,1,'Welding','Schweißen','Soudage' UNION ALL
select 1755,986,'Стругарство',NULL,1,'5211001',1,'2020-11-19 00:00:00.000',NULL,1,'Turning','Drehen','Tournage' UNION ALL
select 1756,987,'Шлосерство',NULL,1,'5211101',1,'2020-11-19 00:00:00.000',NULL,1,'Fitter''s trade','Schlosserei','Serrurerie' UNION ALL
select 1757,988,'Леярство',NULL,1,'5211201',1,'2020-11-19 00:00:00.000',NULL,1,'Foundry work','Gießerei','Fonderie' UNION ALL
select 1758,989,'Ковачество',NULL,1,'5211301',1,'2020-11-19 00:00:00.000',NULL,1,'Blacksmith''s trade','Schmiederei','Travaux de forgeron' UNION ALL
select 1759,553,'Електроенергетика',NULL,2,'5220212',1,'2020-11-19 00:00:00.000',NULL,1,'Electrical energy','Elektroenergetik','Électro-énergie' UNION ALL
select 1760,554,'Топлотехника',NULL,3,'5220309',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1761,555,'Топлотехника',NULL,2,'5220409',1,'2020-11-19 00:00:00.000',NULL,1,'Heating technology - thermal, air-conditioning, ventilation and refrigerating','Wärmetechnik - Wärme, Klima, Lüftung und Kälte','Technique thermique - chaleur, climatisation, ventilation et réfrigération' UNION ALL
select 1762,990,'Радио и телевизионна техника',NULL,3,'5230101',1,'2020-11-19 00:00:00.000',NULL,1,'Radio and television equipment','Radio- und Fernsehtechnik','Matériel radio et télévision' UNION ALL
select 1763,990,'Телекомуникационни системи',NULL,3,'5230102',1,'2020-11-19 00:00:00.000',NULL,1,'Telecommunication systems','Telekommunikationssysteme','Systèmes de télécommunication' UNION ALL
select 1764,990,'Радиолокация, радионавигация и хидроакустични системи',NULL,3,'5230103',1,'2020-11-19 00:00:00.000',NULL,1,'Radiolocation, radionavigation and hydroaccoustic systems','Radartechnik, Funknavigation und hydroakustische Systeme ','Radiolocation, radionavigation et de systèmes hydroacustiques' UNION ALL
select 1765,990,'Кинотехника, аудио и видеосистеми',NULL,3,'5230104',1,'2020-11-19 00:00:00.000',NULL,1,'Cinematographic equipment, audio and video systems','Kinotechnik, Audio- und Videosysteme','Cinématographie, systèmes audio et vidéo' UNION ALL
select 1766,991,'Радио и телевизионна техника',NULL,2,'5230201',1,'2020-11-19 00:00:00.000',NULL,1,'Radio and television equipment','Radio- und Fernsehtechnik','Matériel radio et télévision' UNION ALL
select 1767,991,'Телекомуникационни системи',NULL,2,'5230202',1,'2020-11-19 00:00:00.000',NULL,1,'Telecommunication systems','Telekommunikationssysteme','Systèmes de télécommunication' UNION ALL
select 1768,991,'Радиолокация, радионавигация и хидроакустични системи',NULL,2,'5230203',1,'2020-11-19 00:00:00.000',NULL,1,'Radiolocation, radionavigation and hydroaccoustic systems','Radartechnik, Funknavigation und hydroakustische Systeme','Radiolocation, radionavigation et de systèmes hydroacustiques' UNION ALL
select 1769,991,'Кинотехника, аудио и видеосистеми',NULL,2,'5230204',1,'2020-11-19 00:00:00.000',NULL,1,'Cinematographic equipment, audio and video systems','Kinotechnik, Audio- und Videosysteme','Cinématographie, systèmes audio et vidéo' UNION ALL
select 1770,992,'Промишлена електроника',NULL,3,'5230301',1,'2020-11-19 00:00:00.000',NULL,1,'Industrial electronics','Industrieelektronik','Électronique industrielle' UNION ALL
select 1771,992,'Микропроцесорна техника',NULL,3,'5230302',1,'2020-11-19 00:00:00.000',NULL,1,'Microprocessor equipment','Mikroprozessortechnik','Technologie de microprocesseur' UNION ALL
select 1772,992,'Електронно уредостроене',NULL,3,'5230303',1,'2020-11-19 00:00:00.000',NULL,1,'Electronic instrument engineering','Elektronische Gerätebau','Construction d''appareils électroniques' UNION ALL
select 1773,993,'Промишлена електроника',NULL,2,'5230401',1,'2020-11-19 00:00:00.000',NULL,1,'Industrial electronics','Industrieelektronik','Électronique industrielle' UNION ALL
select 1774,993,'Микропроцесорна техника',NULL,2,'5230402',1,'2020-11-19 00:00:00.000',NULL,1,'Microprocessor equipment','Mikroprozessortechnik','Technologie de microprocesseur' UNION ALL
select 1775,993,'Електронно уредостроене',NULL,2,'5230403',1,'2020-11-19 00:00:00.000',NULL,1,'Electronic instrument engineering','Elektronische Gerätebau','Construction d''appareils électroniques' UNION ALL
select 1776,994,'Компютърна техника и технологии',NULL,3,'5230501',1,'2020-11-19 00:00:00.000',NULL,1,'Computer equipment and technology','Computertechnik und -technologien ','Équipements et technologies informatiques' UNION ALL
select 1777,994,'Компютърни мрежи',NULL,3,'5230502',1,'2020-11-19 00:00:00.000',NULL,1,'Computer networks','Computernetze','Réseaux informatiques' UNION ALL
select 1778,995,'Компютърна техника и технологии',NULL,2,'5230601',1,'2020-11-19 00:00:00.000',NULL,1,'Computer equipment and technology','Computertechnik und -technologien','Équipements et technologies informatiques' UNION ALL
select 1779,995,'Компютърни мрежи',NULL,2,'5230602',1,'2020-11-19 00:00:00.000',NULL,1,'Computer networks','Computernetze','Réseaux informatiques' UNION ALL
select 1780,996,'Автоматизация на непрекъснати производства',NULL,3,'5230701',1,'2020-11-19 00:00:00.000',NULL,1,'Automation of continuous production','Automatisierung der kontinuierlichen Fertigung','Automatisation des productions continues' UNION ALL
select 1781,996,'Автоматизирани и роботизирани системи',NULL,4,'5230703',1,'2020-11-19 00:00:00.000',NULL,1,'Automated and robotic systems','Automatisierte und robotisierte Systeme','Systèmes automatisés et robotisés' UNION ALL
select 1782,996,'Осигурителни и комуникационни системи в ж.п.инфраструктура',NULL,3,'5230704',1,'2020-11-19 00:00:00.000',NULL,1,'Security and communication systems in the railway infrastructure','Sicherheits- und Kommunikationssysteme in der Eisenbahninfrastruktur','Systèmes de sécurité et de communication dans les infrastructures ferroviaires' UNION ALL
select 1783,997,'Автоматизирани системи',NULL,2,'5230801',1,'2020-11-19 00:00:00.000',NULL,1,'Automated systems','Automatisierte Systeme','Systèmes automatisés' UNION ALL
select 1784,997,'Осигурителни и комуникационни системи в ж.п.инфраструктура',NULL,2,'5230802',1,'2020-11-19 00:00:00.000',NULL,1,'Security and communication systems in the railway infrastructure','Sicherheits- und Kommunikationssysteme in der Eisenbahninfrastruktur','Systèmes de sécurité et de communication dans les infrastructures ferroviaires' UNION ALL
select 1785,990,'Оптически комуникационни системи',NULL,3,'5230105',1,'2020-11-19 00:00:00.000',NULL,1,'Optical communication systems','Optische Kommunikationssysteme','Systèmes de communication optique' UNION ALL
select 1786,991,'Оптически комуникационни системи',NULL,2,'5230205',1,'2020-11-19 00:00:00.000',NULL,1,'Optical communication systems','Optische Kommunikationssysteme','Systèmes de communication optique' UNION ALL
select 1787,998,'Компютърни мрежи',NULL,4,'5231001',1,'2020-11-19 00:00:00.000',NULL,1,'Computer networks','Computernetze','Réseaux informatiques' UNION ALL
select 1788,566,'Технология на биогоривата',NULL,3,'5240113',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of biofuels','Technologie der Biokraftstoffe ','Technologie des biocarburants' UNION ALL
select 1789,571,'Технология на биогоривата',NULL,2,'5240614',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of biofuels','Technologie der Biokraftstoffe','Technologie des biocarburants' UNION ALL
select 1790,1092,'Автотранспортна техника',NULL,3,'5250101',1,'2020-11-19 00:00:00.000',NULL,1,'Autotransport equipment','Kraftfahrzeug-Transport-Technik','Matériel de transport automobile' UNION ALL
select 1791,1092,'Пътно - строителна техника',NULL,3,'5250102',1,'2020-11-19 00:00:00.000',NULL,1,'Road construction equipment','Straßenbautechnik','Matériel de construction routière' UNION ALL
select 1792,1000,'Автотранспортна техника',NULL,2,'5250201',1,'2020-11-19 00:00:00.000',NULL,1,'Autotransport equipment','Kraftfahrzeug-Transport-Technik','Matériel de transport automobile' UNION ALL
select 1793,1000,'Пътностроителна техника',NULL,2,'5250202',1,'2020-11-19 00:00:00.000',NULL,1,'Road construction equipment','Straßenbautechnik','Matériel de construction routière' UNION ALL
select 1794,1001,'Подемно - транспортна техника, монтирана на пътни транспортни средства',NULL,3,'5250501',1,'2020-11-19 00:00:00.000',NULL,1,'Hoisting and lifting equipment mounted on road vehicles','Hebe- und Transporttechnik, an Straßenfahrzeugen montiert','Matériel de levage et de transport, installé sur véhicules routiers' UNION ALL
select 1795,1001,'Подемно - транспортна техника с електрозадвижване',NULL,3,'5250502',1,'2020-11-19 00:00:00.000',NULL,1,'Electricity driven hoisting and lifting equipment','Hebe- und Transporttechnik mit Elektroantrieb','Matériel de levage et de transport à traction électrique' UNION ALL
select 1796,1002,'Подемно - транспортна техника, монтирана на пътни транспортни средства',NULL,2,'5250601',1,'2020-11-19 00:00:00.000',NULL,1,'Hoisting and lifting equipment mounted on road vehicles','Hebe- und Transporttechnik, an Straßenfahrzeugen montiert','Matériel de levage et de transport, installé sur véhicules routiers' UNION ALL
select 1797,1002,'Подемно - транспортна техника с електрозадвижване',NULL,2,'5250602',1,'2020-11-19 00:00:00.000',NULL,1,'Electricity driven hoisting and lifting equipment','Hebe- und Transporttechnik mit Elektroantrieb','Matériel de levage et de transport à traction électrique' UNION ALL
select 1798,1002,'Пристанищна механизация',NULL,2,'5250603',1,'2020-11-19 00:00:00.000',NULL,1,'Port mechanization','Hafenmechanisierung','Mécanisation portuaire' UNION ALL
select 1799,1003,'Локомотиви и вагони',NULL,3,'5250701',1,'2020-11-19 00:00:00.000',NULL,1,'Locomotives and wagons','Lokomotiven und Waggons','Locomotives et wagons' UNION ALL
select 1800,1003,'Подемно - транспортна, пътностроителна и ремонтна ж.п техника',NULL,3,'5250702',1,'2020-11-19 00:00:00.000',NULL,1,'Hoisting transport, road construction and repair railway equipment','Hebe- und Transport-, Eisenbahnbau- und Instandsetzungstechnik','Matériel de levage et de transport, installé sur véhicules routiers' UNION ALL
select 1801,1004,'Локомотиви и вагони',NULL,2,'5250801',1,'2020-11-19 00:00:00.000',NULL,1,'Locomotives and wagons','Lokomotiven und Waggons','Locomotives et wagons' UNION ALL
select 1802,1004,'Подемно - транспортна, пътностроителна и ремонтна ж.п техника',NULL,2,'5250802',1,'2020-11-19 00:00:00.000',NULL,1,'Hoisting transport, road construction and repair railway equipment','Hebe- und Transport-, Eisenbahnbau- und Instandsetzungstechnik','Matériel de levage et de transport, installé sur véhicules routiers' UNION ALL
select 1803,1005,'Експлоатация и ремонт на летателни апарати',NULL,4,'5250901',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1804,1005,'Експлоатация и ремонт на електронно - приборна авиационна техника',NULL,4,'5250902',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1805,1006,'Корабни машини и механизми',NULL,3,'5251001',1,'2020-11-19 00:00:00.000',NULL,1,'Marine machines and mechanisms','Schiffsmaschinen und -mechanismen','Machines et mécanismes de navires' UNION ALL
select 1806,1007,'Корабни машини и механизми',NULL,2,'5251101',1,'2020-11-19 00:00:00.000',NULL,1,'Marine machines and mechanisms','Schiffsmaschinen und -mechanismen','Machines et mécanismes de navires' UNION ALL
select 1807,1007,'Корабни тръбни системи',NULL,2,'5251102',1,'2020-11-19 00:00:00.000',NULL,1,'Pipeline systems of ships','Schiffsrohrsysteme','Systèmes de canalisations de navires' UNION ALL
select 1808,1007,'Ремонт на кораби',NULL,2,'5251103',1,'2020-11-19 00:00:00.000',NULL,1,'Repair of ships','Reparatur von Schiffen','Réparation navale' UNION ALL
select 1809,1009,'Автобояджийство',NULL,1,'5251201',1,'2020-11-19 00:00:00.000',NULL,1,'Vehicle painting','Autolackiererei','Peinture automobile' UNION ALL
select 1810,1009,'Автотенекеджийство',NULL,1,'5251202',1,'2020-11-19 00:00:00.000',NULL,1,'Car tinsmithing','AutoSpenglerei','Ferblanterie automobile' UNION ALL
select 1811,1011,'Обработване на товари',NULL,1,'5251301',1,'2020-11-19 00:00:00.000',NULL,1,'Freight processing','Frachtabfertigung','Manutention du fret' UNION ALL
select 1812,1012,'Корабостроене',NULL,3,'5251401',1,'2020-11-19 00:00:00.000',NULL,1,'Shipbuilding','Schiffbau','Construction navale' UNION ALL
select 1813,1013,'Зърносъхранение, зърнопреработка и производство  на фуражи',NULL,3,'5410101',1,'2020-11-19 00:00:00.000',NULL,1,'Grain storage, grain processing and forage production','Getreidelagerung, Getreideverarbeitung und Futtermittelproduktion','Stockage des céréales, transformation des céréales et production de fourrages' UNION ALL
select 1814,1013,'Производство на хляб, хлебни и сладкарски изделия',NULL,3,'5410102',1,'2020-11-19 00:00:00.000',NULL,1,'Production of bread, bakery products and confectionery','Herstellung von Brot, Back- und Konditoreiwaren','Panification, boulangerie et confiserie' UNION ALL
select 1815,1013,'Производство и преработка на мляко и млечни продукти',NULL,3,'5410103',1,'2020-11-19 00:00:00.000',NULL,1,'Production and processing of milk and milk products','Produktion und Verarbeitung von Milch und Milchprodukten','Production et transformation du lait et des produits laitiers' UNION ALL
select 1816,1013,'Производство на месо, месни продукти и риба',NULL,3,'5410104',1,'2020-11-19 00:00:00.000',NULL,1,'Production of meat, meat products and fish','Produktion von Fleisch, Fleischwaren und Fisch','Production de viande, de produits à base de viande et de poisson' UNION ALL
select 1817,1013,'Производство на консерви',NULL,3,'5410105',1,'2020-11-19 00:00:00.000',NULL,1,'Production of tins','Herstellung von Konserven','Production de conserves alimentaires' UNION ALL
select 1818,1013,'Производство на алкохолни и безалкохолни напитки',NULL,3,'5410106',1,'2020-11-19 00:00:00.000',NULL,1,'Production of alcoholic and soft beverages','Herstellung von alkoholhaltigen und alkoholfreien Getränken','Production de boissons alcoolisées et non alcoolisées' UNION ALL
select 1819,1013,'Производство на захар и захарни изделия',NULL,3,'5410107',1,'2020-11-19 00:00:00.000',NULL,1,'Production of sugar and sugar products','Herstellung von Zucker und Zuckerwaren','Fabrication de sucre et de sucreries' UNION ALL
select 1820,1013,'Производство на тютюн и тютюневи изделия',NULL,3,'5410108',1,'2020-11-19 00:00:00.000',NULL,1,'Production of tobacco and tobacco products','Herstellung von Tabak und Tabakwaren','Production de tabac et de produits contenant du tabac' UNION ALL
select 1821,1013,'Производство на растителни масла, маслопродукти и етерични масла',NULL,3,'5410109',1,'2020-11-19 00:00:00.000',NULL,1,'Production of vegetable oils, oil products and essential oils','Herstellung von Pflanzenölen, Ölprodukten und ätherischen Ölen','Production d''huiles végétales, de produits pétroliers et d''huiles essentielles' UNION ALL
select 1822,1014,'Зърносъхранение, зърнопреработка и производство  на фуражи',NULL,2,'5410201',1,'2020-11-19 00:00:00.000',NULL,1,'Grain storage, grain processing and forage production','Getreidelagerung, Getreideverarbeitung und Futtermittelproduktion','Stockage des céréales, transformation des céréales et production de fourrages' UNION ALL
select 1823,1014,'Производство и преработка на мляко и млечни продукти',NULL,2,'5410202',1,'2020-11-19 00:00:00.000',NULL,1,'Production and processing of milk and milk products','Produktion und Verarbeitung von Milch und Milchprodukten','Production et transformation du lait et des produits laitiers' UNION ALL
select 1824,1014,'Производство на месо, месни продукти и риба',NULL,2,'5410203',1,'2020-11-19 00:00:00.000',NULL,1,'Production of meat, meat products and fish','Produktion von Fleisch, Fleischwaren und Fisch','Production de viande, de produits à base de viande et de poisson' UNION ALL
select 1825,1014,'Производство на консерви',NULL,2,'5410204',1,'2020-11-19 00:00:00.000',NULL,1,'Production of tins','Herstellung von Konserven','Production de conserves alimentaires' UNION ALL
select 1826,1014,'Производство на алкохолни и безалкохолни напитки',NULL,2,'5410205',1,'2020-11-19 00:00:00.000',NULL,1,'Production of alcoholic and soft beverages','Herstellung von alkoholhaltigen und alkoholfreien Getränken','Production de boissons alcoolisées et non alcoolisées' UNION ALL
select 1827,1014,'Производство на захар и захарни изделия',NULL,2,'5410206',1,'2020-11-19 00:00:00.000',NULL,1,'Production of sugar and sugar products','Herstellung von Zucker und Zuckerwaren','Fabrication de sucre et de sucreries' UNION ALL
select 1828,1014,'Производство на тютюн и тютюневи изделия',NULL,2,'5410207',1,'2020-11-19 00:00:00.000',NULL,1,'Production of tobacco and tobacco products','Herstellung von Tabak und Tabakwaren','Production de tabac et de produits contenant du tabac' UNION ALL
select 1829,1014,'Производство на растителни масла, маслопродукти и етерични масла',NULL,2,'5410208',1,'2020-11-19 00:00:00.000',NULL,1,'Production of vegetable oils, oil products and essential oils','Herstellung von Pflanzenölen, Ölprodukten und ätherischen Ölen','Production d''huiles végétales, de produits pétroliers et d''huiles essentielles' UNION ALL
select 1830,1015,'Производство на хляб и хлебни изделия',NULL,2,'5410301',1,'2020-11-19 00:00:00.000',NULL,1,'Production of bread and bakery products','Herstellung von Brot und Backwaren','Production de pain et de produits de boulangerie' UNION ALL
select 1831,1015,'Производство на сладкарски изделия',NULL,2,'5410302',1,'2020-11-19 00:00:00.000',NULL,1,'Production of confectionery','Herstellung von Konditoreiwaren','Production de confiserie' UNION ALL
select 1832,1015,'Декорация на сладкарски изделия',NULL,2,'5410303',1,'2020-11-19 00:00:00.000',NULL,1,'Decoration of confectionery','Dekoration von Konditoreiwaren','Décoration de confiserie' UNION ALL
select 1833,1016,'Експлоатация и поддържане на хладилна техника в хранително-вкусовата промишленост',NULL,3,'5410401',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1834,1017,'Хранително - вкусова промишленост',NULL,1,'5410501',1,'2020-11-19 00:00:00.000',NULL,1,'Food industry','Lebensmittelindustrie','Industrie alimentaire' UNION ALL
select 1835,1018,'Контрол на качеството и безопасност на храни и напитки',NULL,3,'5410601',1,'2020-11-19 00:00:00.000',NULL,1,'Quality control and safety of foods and beverages','Qualitäts- und Sicherheitskontrolle von Lebensmitteln und Getränken','Contrôle de la qualité et de la sécurité des aliments et des boissons' UNION ALL
select 1836,1019,'Компютърно проектиране и десениране на тъкани площни изделия',NULL,3,'5420101',1,'2020-11-19 00:00:00.000',NULL,1,'Computer design and design of surface articles','Computergestütztes Design und Musterung von gewebten Textilien','Conception et dessinage assistés par ordinateur de produits de surface tissés' UNION ALL
select 1837,1019,'Компютърно проектиране и десениране на плетени площни изделия',NULL,3,'5420102',1,'2020-11-19 00:00:00.000',NULL,1,'Computer design and design of woven surface articles','Computergestütztes Design und Musterung von gestrickten Textilien','Conception et modelage par dessinage de produits de surface tricotés' UNION ALL
select 1838,1020,'Предачно производство',NULL,3,'5420201',1,'2020-11-19 00:00:00.000',NULL,1,'Spinning ','Spinnenproduktion','Production de filage' UNION ALL
select 1839,1020,'Тъкачно производство',NULL,3,'5420202',1,'2020-11-19 00:00:00.000',NULL,1,'Weaving','Webeproduktion','Fabrication de tissage' UNION ALL
select 1840,1020,'Плетачно производство',NULL,3,'5420203',1,'2020-11-19 00:00:00.000',NULL,1,'Knitting','Strickproduktion','Production de tricot' UNION ALL
select 1841,1021,'Предачно производство',NULL,2,'5420301',1,'2020-11-19 00:00:00.000',NULL,1,'Spinning','Spinnenproduktion','Production de filage' UNION ALL
select 1842,1021,'Тъкачно производство',NULL,2,'5420302',1,'2020-11-19 00:00:00.000',NULL,1,'Weaving','Webeproduktion','Fabrication de tissage' UNION ALL
select 1843,1021,'Плетачно производство',NULL,2,'5420303',1,'2020-11-19 00:00:00.000',NULL,1,'Knitting','Strickproduktion','Production de tricot' UNION ALL
select 1844,1022,'Конструиране, моделиране и технология на облекло от текстил',NULL,3,'5420401',1,'2020-11-19 00:00:00.000',NULL,1,'Construction, modeling and technology of textile clothing','Design, Modellierung und Technologie der Textilkleidung','Conception, modélisation et technologie de vêtements textiles' UNION ALL
select 1845,1022,'Конструиране, моделиране и технология на облекло от кожи',NULL,3,'5420402',1,'2020-11-19 00:00:00.000',NULL,1,'Construction, modeling and technology of leather clothing','Design, Modellierung und Technologie der Lederbekleidung','Conception, modélisation et technologie de vêtements en cuir' UNION ALL
select 1846,1023,'Производство на облекло от текстил',NULL,2,'5420501',1,'2020-11-19 00:00:00.000',NULL,1,'Production of textile clothing','Produktion von Textilbekleidung','Fabrication de vêtements textiles' UNION ALL
select 1847,1023,'Производство на облекло от кожи',NULL,2,'5420502',1,'2020-11-19 00:00:00.000',NULL,1,'Production of leather clothing','Produktion von Lederbekleidung','Fabrication de vêtements en cuir' UNION ALL
select 1848,1024,'Конструиране, моделиране и технология на обувни изделия',NULL,3,'5420601',1,'2020-11-19 00:00:00.000',NULL,1,'Construction, modeling and technology of shoes','Design, Modellierung und Technologie der Schuhe','Conception, modélisation et technologie des chaussures' UNION ALL
select 1849,1024,'Конструиране, моделиране и технология на кожено - галантерийни изделия',NULL,3,'5420602',1,'2020-11-19 00:00:00.000',NULL,1,'Construction, modeling and technology of leather haberdasher articles','Design, Modellierung und Technologie der Lederwaren','Conception, modélisation et technologie de maroquinerie' UNION ALL
select 1850,1025,'Производство на обувни изделия',NULL,2,'5420701',1,'2020-11-19 00:00:00.000',NULL,1,'Production of shoes','Produktion von Schuhen','Production de chaussures' UNION ALL
select 1851,1025,'Производство на кожено - галантерийни изделия',NULL,2,'5420702',1,'2020-11-19 00:00:00.000',NULL,1,'Production of leather haberdasher articles','Produktion von Lederwaren','Production de la maroquinerie' UNION ALL
select 1852,1026,'Текстилно производство',NULL,1,'5420801',1,'2020-11-19 00:00:00.000',NULL,1,'Textile industry','Textilindustrie','Production textile' UNION ALL
select 1853,1027,'Производство на облекло',NULL,1,'5420901',1,'2020-11-19 00:00:00.000',NULL,1,'Production of clothes','Bekleidungsproduktion','Confection de vêtements' UNION ALL
select 1854,1028,'Обувно и кожено - галантерийно производство',NULL,1,'5421001',1,'2020-11-19 00:00:00.000',NULL,1,'Shoe and leather haberdasher industry','Schuh- und Lederwareniproduktion','Fabrication de chaussures et de la maroquinerie' UNION ALL
select 1855,1029,'Шивачество',NULL,1,'5421101',1,'2020-11-19 00:00:00.000',NULL,1,'Tailoring','Schneiderei','Couture' UNION ALL
select 1856,1030,'Обущарство',NULL,1,'5421201',1,'2020-11-19 00:00:00.000',NULL,1,'Shoemaking','Schuherstellung','Cordonnerie' UNION ALL
select 1857,1031,'Ръчно изработване на килими',NULL,1,'5421301',1,'2020-11-19 00:00:00.000',NULL,1,'Manual weaving of carpets','Handfertigung von Teppichen','Tissage manuelle de tapis' UNION ALL
select 1858,1031,'Ръчно изработване на гоблени, губери, козяци',NULL,1,'5421302',1,'2020-11-19 00:00:00.000',NULL,1,'Manual making of tapestry, rugs, goat''s hair rugs','Handfertigung von Wandteppichen, Decken, Teppich aus Ziegenhaar','Production artisanale de tapisseries murales, lutins, peaux de chèvre' UNION ALL
select 1859,1032,'Ръчно художествено плетиво',NULL,1,'5421401',1,'2020-11-19 00:00:00.000',NULL,1,'Manual artistic knitting','Künstlerisches Handstricken','Tricot artistique fait à la main' UNION ALL
select 1860,1032,'Плетач на настолна плетачна машина',NULL,1,'5421402',1,'2020-11-19 00:00:00.000',NULL,1,'Knitter on a table knitting machine','Stricker auf einer Tischstrickmaschine ','Tricoteur sur une machine à tricoter' UNION ALL
select 1861,1033,'Бродерия',NULL,1,'5421501',1,'2020-11-19 00:00:00.000',NULL,1,'Embroidery','Stickerei','Broderie' UNION ALL
select 1862,1020,'Апретурно и багрилно производство',NULL,3,'5420204',1,'2020-11-19 00:00:00.000',NULL,1,'Finishing and coloring industry','Appretier- und Färbereiproduktion','Production d’apprêtage et de teinture' UNION ALL
select 1863,1021,'Апретурно и багрилно производство',NULL,2,'5420304',1,'2020-11-19 00:00:00.000',NULL,1,'Finishing and coloring industry','Appretier- und Färbereiproduktion','Production d’apprêtage et de teinture' UNION ALL
select 1864,1026,'Апретурно и багрилно производство',NULL,1,'5420803',1,'2020-11-19 00:00:00.000',NULL,1,'Finishing and coloring industry','Appretier- und Färbereiproduktion','Production d’apprêtage et de teinture' UNION ALL
select 1865,1034,'Мебелно производство',NULL,3,'5430101',1,'2020-11-19 00:00:00.000',NULL,1,'Furniture production','Möbelproduktion','Fabrication de meubles' UNION ALL
select 1866,1034,'Реставрация на стилни мебели и дограма',NULL,3,'5430104',1,'2020-11-19 00:00:00.000',NULL,1,'Restoration of stylish furniture and joinery','Restaurierung von stilvollen Möbeln und Fenster- und Türrahmen','Restauration de meubles et menuiseries de style' UNION ALL
select 1867,1034,'Тапицерство и декораторство',NULL,3,'5430106',1,'2020-11-19 00:00:00.000',NULL,1,'Upholstery and decoration','Polsterung und Dekoration','Production de produits matélassés et décoration' UNION ALL
select 1868,1034,'Производство на врати и прозорци',NULL,3,'5430107',1,'2020-11-19 00:00:00.000',NULL,1,'Production of doors and windows','Herstellung von Türen und Fenstern','Fabrication de portes et fenêtres' UNION ALL
select 1869,1034,'Дърворезни и амбалажни производства',NULL,3,'5430109',1,'2020-11-19 00:00:00.000',NULL,1,'Wood-cutting and wrapping productions','Holzzuschnitt- und Verpackungsproduktion','Découpe de bois et production d''emballages' UNION ALL
select 1870,1034,'Производство на плочи и слоиста дървесина',NULL,3,'5430110',1,'2020-11-19 00:00:00.000',NULL,1,'Production of wood panels and plywood','Herstellung von Brettern und Sperrholz','Production de planches et de bois lamellé' UNION ALL
select 1871,1034,'Производство на строителни изделия от дървесина',NULL,3,'5430111',1,'2020-11-19 00:00:00.000',NULL,1,'Production of  wooden builders'' joinery','Herstellung von Holzbauprodukten','Fabrication de produits de construction en bois' UNION ALL
select 1872,1035,'Производство на мебели',NULL,2,'5430201',1,'2020-11-19 00:00:00.000',NULL,1,'Furniture production','Möbelproduktion','Fabrication de meubles' UNION ALL
select 1873,1035,'Производство на врати и прозорци',NULL,2,'5430202',1,'2020-11-19 00:00:00.000',NULL,1,'Production of doors and windows','Herstellung von Türen und Fenstern','Fabrication de portes et fenêtres' UNION ALL
select 1874,1035,'Производство на тапицирани изделия',NULL,2,'5430203',1,'2020-11-19 00:00:00.000',NULL,1,'Production of upholstered articles','Herstellung von Polsterprodukten','Fabrication de produits matelassés' UNION ALL
select 1875,1035,'Производство и монтаж на вътрешно обзавеждане на кораби',NULL,2,'5430204',1,'2020-11-19 00:00:00.000',NULL,1,'Production and assembly of interior furnishing of ships','Herstellung und Einbau von Schiffsinnenausstattung','Production et installation d''intérieurs de navires' UNION ALL
select 1876,1035,'Моделчество и дървостругарство',NULL,2,'5430205',1,'2020-11-19 00:00:00.000',NULL,1,'Pattern-making and wood carving','Modellbau und Holzdrehen','Modelage et tournage du bois' UNION ALL
select 1877,1035,'Дърворезно, амбалажно и паркетно производство',NULL,2,'5430206',1,'2020-11-19 00:00:00.000',NULL,1,'Wood cutting, wrapping and parquette production','Holzzuschnitt-, Verpackungs- und Parkettherstellung','Coupe de bois, emballage et production de parquet' UNION ALL
select 1878,1035,'Производство на дървесни плочи',NULL,2,'5430207',1,'2020-11-19 00:00:00.000',NULL,1,'Production of wood panels','Herstellung von Holzplatten','Fabrication de panneaux de bois' UNION ALL
select 1879,1035,'Производство на фурнир и слоиста дъресина',NULL,2,'5430208',1,'2020-11-19 00:00:00.000',NULL,1,'Production of veneer and plywood','Herstellung von Furnier und Sperrholz','Production de placage et de bois lamellé' UNION ALL
select 1880,1035,'Производство на дървени детски играчки',NULL,2,'5430209',1,'2020-11-19 00:00:00.000',NULL,1,'Production of wooden toys for children','Herstellung von Holzspielzeug','Fabrication de jouets en bois' UNION ALL
select 1881,1036,'Производство на фасонирани материали',NULL,1,'5430901',1,'2020-11-19 00:00:00.000',NULL,1,'Production of sawn materials','Herstellung von geformten Materialien','Production de matériaux façonnés' UNION ALL
select 1882,1036,'Производство на тапицирани изделия',NULL,1,'5430902',1,'2020-11-19 00:00:00.000',NULL,1,'Production of upholstered articles','Herstellung von Polsterprodukten','Fabrication de produits matelassés' UNION ALL
select 1883,1036,'Производство на бъчви',NULL,1,'5430903',1,'2020-11-19 00:00:00.000',NULL,1,'Production of barrels','Herstellung von Fässern','Fabrication de fûts' UNION ALL
select 1884,1036,'Производство на плетени изделия от дървесина',NULL,1,'5430904',1,'2020-11-19 00:00:00.000',NULL,1,'Production of knitted articles from wood','Produktion von Strickwaren aus Holz','Fabrication de produits en fil de bois tricoté' UNION ALL
select 1885,1036,'Производство на врати и прозорци',NULL,1,'5430905',1,'2020-11-19 00:00:00.000',NULL,1,'Production of doors and windows','Herstellung von Türen und Fenstern','Fabrication de portes et fenêtres' UNION ALL
select 1886,1036,'Производство на мебели',NULL,1,'5430906',1,'2020-11-19 00:00:00.000',NULL,1,'Furniture production','Möbelproduktion','Fabrication de meubles' UNION ALL
select 1887,1037,'Мебелно производство',NULL,3,'5431001',1,'2020-11-19 00:00:00.000',NULL,1,'Furniture production','Möbelproduktion','Fabrication de meubles' UNION ALL
select 1888,1037,'Дървообработване',NULL,3,'5431002',1,'2020-11-19 00:00:00.000',NULL,1,'Wood processing','Holzverarbeitung','Travail du bois' UNION ALL
select 1889,1038,'Добивни и строителни минни технологии',NULL,3,'5440101',1,'2020-11-19 00:00:00.000',NULL,1,'Extraction and construction mining technologies','Gewinnungs- und Bergbautechnologien','Technologies minières et de construction' UNION ALL
select 1890,1038,'Обогатителни, преработващи и рециклационни технологии',NULL,3,'5440102',1,'2020-11-19 00:00:00.000',NULL,1,'Ore-dressing, processing and recycling technologies','Anreicherungs-, Verarbeitungs- und Recyclingtechnologien','Technologies d''enrichissement, de traitement et de recyclage' UNION ALL
select 1891,1038,'Минна електромеханика',NULL,3,'5440103',1,'2020-11-19 00:00:00.000',NULL,1,'Mining electromechanics','Bergbau-Elektromechanik','Électromécanique minière' UNION ALL
select 1892,1039,'Подземен и открит добив на полезни изкопаеми',NULL,2,'5440201',1,'2020-11-19 00:00:00.000',NULL,1,'Underground and open extraction of mineral resources','Untertage- und Tagebau','Exploitation des ressources du sol souterraine et à ciel ouvert' UNION ALL
select 1893,1039,'Добив на скални материали',NULL,2,'5440202',1,'2020-11-19 00:00:00.000',NULL,1,'Output of rock materials','Gewinnung von Gesteinsmaterialien','Extraction de matériaux rocheux' UNION ALL
select 1894,1039,'Обогатяване на полезни изкопаеми',NULL,2,'5440203',1,'2020-11-19 00:00:00.000',NULL,1,'Ore-dressing of mineral resources','Anreicherung von Mineralstoffen','Enrichissement des ressources du sol' UNION ALL
select 1895,1039,'Обработка на скални материали',NULL,2,'5440204',1,'2020-11-19 00:00:00.000',NULL,1,'Processing of rock materials','Verarbeitung von Gesteinsmaterialien','Traitement des matériaux rocheux' UNION ALL
select 1896,1039,'Минни съоръжения',NULL,2,'5440205',1,'2020-11-19 00:00:00.000',NULL,1,'Mining facilities','Bergbauanlagen','Équipement de minage' UNION ALL
select 1897,1040,'Сондажни технологии',NULL,3,'5440301',1,'2020-11-19 00:00:00.000',NULL,1,'Drilling technologies','Bohrtechnologien','Technologies de forage' UNION ALL
select 1898,1040,'Сондажна техника',NULL,3,'5440302',1,'2020-11-19 00:00:00.000',NULL,1,'Drilling equipment','Bohrtechnik','Matériel de forage' UNION ALL
select 1899,1041,'Проучвателно сондиране',NULL,2,'5440401',1,'2020-11-19 00:00:00.000',NULL,1,'Exploratory drilling','Erkundungsbohrung','Forage d''exploration' UNION ALL
select 1900,1042,'Маркшайдерство',NULL,3,'5440501',1,'2020-11-19 00:00:00.000',NULL,1,'Markschreidering','Markscheiderei','Arpentage' UNION ALL
select 1901,1043,'Геодезия',NULL,3,'5810101',1,'2020-11-19 00:00:00.000',NULL,1,'Geodesy','Geodäsie','Géodésie' UNION ALL
select 1902,1044,'Строителство и архитектура',NULL,3,'5820101',1,'2020-11-19 00:00:00.000',NULL,1,'Construction and architecture','Bau und Architektur','Technicien en construction et architecture' UNION ALL
select 1903,1044,'Водно строителство',NULL,3,'5820103',1,'2020-11-19 00:00:00.000',NULL,1,'Water construction','Wasserbau','Construction d’eau' UNION ALL
select 1904,1044,'Транспортно строителство',NULL,3,'5820104',1,'2020-11-19 00:00:00.000',NULL,1,'Transport construction','Verkehrsbau','Construction de transports' UNION ALL
select 1905,1045,'Кофражи',NULL,2,'5820302',1,'2020-11-19 00:00:00.000',NULL,1,'Shuttering','Schalungen','Coffrage' UNION ALL
select 1906,1045,'Армировка и бетон',NULL,2,'5820303',1,'2020-11-19 00:00:00.000',NULL,1,'Reinforcement and concrete','Bewehrung und Beton ','Armature et béton' UNION ALL
select 1907,1045,'Зидария',NULL,2,'5820304',1,'2020-11-19 00:00:00.000',NULL,1,'Masonry','Mauerwerk','Maçonnerie' UNION ALL
select 1908,1045,'Мазилки и шпакловки',NULL,2,'5820305',1,'2020-11-19 00:00:00.000',NULL,1,'Plaster and ground coating','Putzen und Spachteln','Enduits et plâtre' UNION ALL
select 1909,1045,'Вътрешни облицовки и настилки',NULL,2,'5820306',1,'2020-11-19 00:00:00.000',NULL,1,'Internal paneling and flooring','Innenverkleidungen und Fußböden','Revêtements intérieurs et revêtements de sol' UNION ALL
select 1910,1045,'Външни облицовки и настилки',NULL,2,'5820307',1,'2020-11-19 00:00:00.000',NULL,1,'External paneling and flooring','Außenverkleidung und Fußböden','Revêtements extérieurs et revêtements de sol' UNION ALL
select 1911,1045,'Бояджийски работи',NULL,2,'5820309',1,'2020-11-19 00:00:00.000',NULL,1,'Painting','Malerarbeiten','Travaux de peinture' UNION ALL
select 1912,1045,'Строително дърводелство',NULL,2,'5820310',1,'2020-11-19 00:00:00.000',NULL,1,'Construction woodwork','Bauschreinerei','Charpenterie' UNION ALL
select 1913,1045,'Строително тенекеджийство',NULL,2,'5820311',1,'2020-11-19 00:00:00.000',NULL,1,'Construction tinsmithing','Bauspenglerei','Ferblanterie de construction' UNION ALL
select 1914,1045,'Покриви',NULL,2,'5820312',1,'2020-11-19 00:00:00.000',NULL,1,'Roofs','Dächer','Toits' UNION ALL
select 1915,1046,'Стоманобетонни конструкции ',NULL,2,'5820401',1,'2020-11-19 00:00:00.000',NULL,1,'Reinforced concrete structures','Stahlbetonkonstruktionen','Constructions en béton armé' UNION ALL
select 1916,1046,'Метални конструкции',NULL,2,'5820402',1,'2020-11-19 00:00:00.000',NULL,1,'Metal structures','Metallkonstruktionen','Constructions métalliques' UNION ALL
select 1917,1046,'Сухо строителство',NULL,2,'5820403',1,'2020-11-19 00:00:00.000',NULL,1,'Dry construction','Trockenbau ','Construction sèche' UNION ALL
select 1918,1046,'Дограма и стъклопоставяне',NULL,2,'5820404',1,'2020-11-19 00:00:00.000',NULL,1,'Joinery and window glazing','Tür- und Fensterrahmen und Verglasung','Menuiserie et vitrage' UNION ALL
select 1919,1047,'Вътрешни ВиК мрежи',NULL,2,'5820501',1,'2020-11-19 00:00:00.000',NULL,1,'Internal water supply and sewerage networks','Innere Wasserversorgungs- und Kanalisationsnetze','Réseaux de canalisations internes' UNION ALL
select 1920,1047,'Външни ВиК мрежи',NULL,2,'5820502',1,'2020-11-19 00:00:00.000',NULL,1,'External water supply and sewerage networks','Äußere Wasserversorgungs- und Kanalisationsnetze','Réseaux de canalisations externes' UNION ALL
select 1921,1048,'Строител на пътища, магистрали и съоръжения към тях',NULL,2,'5820601',1,'2020-11-19 00:00:00.000',NULL,1,'Builder of roads, highways and facilities thereto','Erbauer von Straßen, Autobahnen und Einrichtungen zu ihnen','Constructeur de routes, d''autoroutes et d''installations pour eux' UNION ALL
select 1922,1048,'Строител на релсови пътища и съоръжения към тях',NULL,2,'5820602',1,'2020-11-19 00:00:00.000',NULL,1,'Builder of railroads and facilities thereto','Erbauer von Eisenbahnen und Einrichtungen zu ihnen','Constructeur de chemins de fer et d''installations pour eux' UNION ALL
select 1923,1049,'Пещостроителство',NULL,2,'5820701',1,'2020-11-19 00:00:00.000',NULL,1,'Furnace building','Ofenbau','Construction de fours' UNION ALL
select 1924,1050,'Основни и довършителни работи',NULL,1,'5820801',1,'2020-11-19 00:00:00.000',NULL,1,'Main and finishing works','Grund- und Abschlussarbeiten','Travaux principaux et de finition' UNION ALL
select 1925,1051,'Пътища, магистрали и съоръжения',NULL,1,'5820901',1,'2020-11-19 00:00:00.000',NULL,1,'Roads, highways and facilities','Straßen, Autobahnen und Anlagen','Routes, autoroutes et installations' UNION ALL
select 1926,1051,'Релсови пътища и съоръжения',NULL,1,'5820902',1,'2020-11-19 00:00:00.000',NULL,1,'Railroads and facilities','Eisenbahnen und Anlagen ','Chemins de fer et installations' UNION ALL
select 1927,1052,'Полевъдство',NULL,3,'6210101',1,'2020-11-19 00:00:00.000',NULL,1,'Plants-husbandry','Ackerbau','Agriculture' UNION ALL
select 1928,1052,'Зеленчукопроизводство',NULL,3,'6210102',1,'2020-11-19 00:00:00.000',NULL,1,'Vegetable growing','Gemüseproduktion','Production de légumes' UNION ALL
select 1929,1052,'Трайни насаждения',NULL,3,'6210103',1,'2020-11-19 00:00:00.000',NULL,1,'Perennials','Stauden','Cultures permanentes' UNION ALL
select 1930,1052,'Селекция и семепроизводство',NULL,3,'6210104',1,'2020-11-19 00:00:00.000',NULL,1,'Selection and seed growing','Selektion und Samenproduktion','Sélection et production de semences' UNION ALL
select 1931,1052,'Тютюнопроизводство',NULL,3,'6210105',1,'2020-11-19 00:00:00.000',NULL,1,'Tobacco production','Tabakproduktion','Production de tabac' UNION ALL
select 1932,1052,'Гъбопроизводство',NULL,3,'6210106',1,'2020-11-19 00:00:00.000',NULL,1,'Mushroom growing','Pilzproduktion','Production de champignons' UNION ALL
select 1933,1052,'Растителна защита и агрохимия',NULL,3,'6210107',1,'2020-11-19 00:00:00.000',NULL,1,'Plant protection and agrochemistry','Pflanzenschutz und Agrochemie','Protection phytosanitaire et agrochimie' UNION ALL
select 1934,1053,'Лозаровинарство',NULL,3,'6210201',1,'2020-11-19 00:00:00.000',NULL,1,'Vine-growing','Weinbau und Weinproduktion','Viticulture' UNION ALL
select 1935,1054,'Полевъдство',NULL,2,'6210302',1,'2020-11-19 00:00:00.000',NULL,1,'Plant-husbandry','Ackerbau','Agriculture' UNION ALL
select 1936,1054,'Зеленчукопроизводство',NULL,2,'6210303',1,'2020-11-19 00:00:00.000',NULL,1,'Vegetable growing','Gemüseproduktion','Production de légumes' UNION ALL
select 1937,1054,'Трайни насаждения',NULL,2,'6210304',1,'2020-11-19 00:00:00.000',NULL,1,'Perennials','Stauden','Cultures permanentes' UNION ALL
select 1938,1054,'Селекция и семепроизводство',NULL,2,'6210305',1,'2020-11-19 00:00:00.000',NULL,1,'Selection and seed growing','Selektion und Samenproduktion','Sélection et production de semences' UNION ALL
select 1939,1054,'Тютюнопроизводство',NULL,2,'6210306',1,'2020-11-19 00:00:00.000',NULL,1,'Tobacco production','Tabakproduktion','Production de tabac' UNION ALL
select 1940,1054,'Гъбопроизводство и билки, етерични-маслени и медоносни култури',NULL,2,'6210307',1,'2020-11-19 00:00:00.000',NULL,1,'Mushroom growing and herbs, essential oils and melliferous crops','Pilzproduktion und Kräuter, ätherische Öle und Honigkulturen','Production de champignons et de cultures d''herbes, d''huiles essentielles et de miel' UNION ALL
select 1941,1054,'Растителна защита',NULL,2,'6210308',1,'2020-11-19 00:00:00.000',NULL,1,'Plant protection','Pflanzenschutz','Protection phytosanitaire' UNION ALL
select 1942,1055,'Говедовъдство',NULL,3,'6210401',1,'2020-11-19 00:00:00.000',NULL,1,'Cattle breeding','Viehzucht','Élevage bovin' UNION ALL
select 1943,1055,'Овцевъдство',NULL,3,'6210402',1,'2020-11-19 00:00:00.000',NULL,1,'Sheep breeding','Schafzucht','Élevage de moutons' UNION ALL
select 1944,1055,'Свиневъдство',NULL,3,'6210403',1,'2020-11-19 00:00:00.000',NULL,1,'Pig breeding','Schweinezucht','Élevage porcin' UNION ALL
select 1945,1055,'Птицевъдство',NULL,3,'6210404',1,'2020-11-19 00:00:00.000',NULL,1,'Poultry farming','Geflügelzucht','Aviculture' UNION ALL
select 1946,1055,'Зайцевъдство',NULL,3,'6210405',1,'2020-11-19 00:00:00.000',NULL,1,'Rabbit breding','Kaninchenzucht','Élevage de lapins' UNION ALL
select 1947,1055,'Пчеларство и бубарство',NULL,3,'6210406',1,'2020-11-19 00:00:00.000',NULL,1,'Bee-keeping and silkworm breeding','Bienenhaltung und Seidenraupenzucht','Apiculture et sériciculture' UNION ALL
select 1948,1055,'Коневъдство и конна езда ',NULL,3,'6210407',1,'2020-11-19 00:00:00.000',NULL,1,'Horse-breeding and horse riding','Pferdezucht und Pferdereiten','Élevage de chevaux et équitation' UNION ALL
select 1949,1056,'Говедовъдство',NULL,2,'6210502',1,'2020-11-19 00:00:00.000',NULL,1,'Cattle breeding','Viehzucht','Élevage bovin' UNION ALL
select 1950,1056,'Овцевъдство',NULL,2,'6210503',1,'2020-11-19 00:00:00.000',NULL,1,'Sheep breeding','Schafzucht','Élevage de moutons' UNION ALL
select 1951,1056,'Свиневъдство',NULL,2,'6210504',1,'2020-11-19 00:00:00.000',NULL,1,'Pig breeding','Schweinezucht','Élevage porcin' UNION ALL
select 1952,1056,'Птицевъдство',NULL,2,'6210505',1,'2020-11-19 00:00:00.000',NULL,1,'Poultry farming','Geflügelzucht','Aviculture' UNION ALL
select 1953,1056,'Зайцевъдство',NULL,2,'6210506',1,'2020-11-19 00:00:00.000',NULL,1,'Rabbit breding','Kaninchenzucht','Élevage de lapins' UNION ALL
select 1954,1056,'Пчеларство',NULL,2,'6210507',1,'2020-11-19 00:00:00.000',NULL,1,'Bee-keeping','Bienenhaltung','Apiculture' UNION ALL
select 1955,1056,'Коневъдство и конна езда ',NULL,2,'6210508',1,'2020-11-19 00:00:00.000',NULL,1,'Horse-breeding and horse riding','Pferdezucht und Pferdereiten','Élevage de chevaux et équitation' UNION ALL
select 1956,1057,'Земеделец',NULL,2,'6210601',1,'2020-11-19 00:00:00.000',NULL,1,'Agricultural worker','Landwirt','Agriculteur' UNION ALL
select 1957,1057,'Производител на селскостопанска продукция',NULL,3,'6210602',1,'2020-11-19 00:00:00.000',NULL,1,'Producer of agricultural produce','Produzent landwirtschaftlicher Produkte','Producteur de produits agricoles' UNION ALL
select 1958,1058,'Механизация на селското стопанство',NULL,3,'6210701',1,'2020-11-19 00:00:00.000',NULL,1,'Agriculture mechanization','Mechanisierung der Landwirtschaft','Mécanisation de l''agriculture' UNION ALL
select 1959,1059,'Механизация на селското стопанство',NULL,2,'6210801',1,'2020-11-19 00:00:00.000',NULL,1,'Agriculture mechanization','Mechanisierung der Landwirtschaft','Mécanisation de l''agriculture' UNION ALL
select 1960,1060,'Лозаровинарство',NULL,2,'6210901',1,'2020-11-19 00:00:00.000',NULL,1,'Vine-growing','Weinbau und Weinproduktion','Viticulture' UNION ALL
select 1961,1061,'Зеленчукопроизводство',NULL,1,'6211101',1,'2020-11-19 00:00:00.000',NULL,1,'Vegetable growing','Gemüseproduktion','Production de légumes' UNION ALL
select 1962,1061,'Трайни насаждения',NULL,1,'6211102',1,'2020-11-19 00:00:00.000',NULL,1,'Perennials','Stauden','Cultures permanentes' UNION ALL
select 1963,1061,'Тютюнопроизводство',NULL,1,'6211103',1,'2020-11-19 00:00:00.000',NULL,1,'Tobacco production','Tabakproduktion','Production de tabac' UNION ALL
select 1964,1061,'Гъбопроизводство и билки, етерични-маслени и медоносни култури',NULL,1,'6211104',1,'2020-11-19 00:00:00.000',NULL,1,'Mushroom growing and herbs, essential oils and melliferous crops','Pilzproduktion und Kräuter, ätherische Öle und Honigkulturen','Production de champignons et de cultures d''herbes, d''huiles essentielles et de miel' UNION ALL
select 1965,1062,'Говедовъдство и биволовъдство',NULL,1,'6211201',1,'2020-11-19 00:00:00.000',NULL,1,'Cattle breeding and buffalo breeding','Rinderzucht und Büffelzucht','Élevage de bovins et élevage de buffles' UNION ALL
select 1966,1062,'Овцевъдство и козевъдство',NULL,1,'6211202',1,'2020-11-19 00:00:00.000',NULL,1,'Sheep breeding and goat breading','Schafzucht und Ziegenzucht','Elevage ovin et caprin' UNION ALL
select 1967,1062,'Свиневъдство',NULL,1,'6211203',1,'2020-11-19 00:00:00.000',NULL,1,'Pig breeding','Schweinezucht','Élevage porcin' UNION ALL
select 1968,1062,'Птицевъдство',NULL,1,'6211204',1,'2020-11-19 00:00:00.000',NULL,1,'Poultry farming','Geflügelzucht','Aviculture' UNION ALL
select 1969,1062,'Зайцевъдство',NULL,1,'6211205',1,'2020-11-19 00:00:00.000',NULL,1,'Rabbit breding','Kaninchenzucht','Élevage de lapins' UNION ALL
select 1970,1062,'Пчеларство',NULL,1,'6211206',1,'2020-11-19 00:00:00.000',NULL,1,'Bee-keeping','Bienenhaltung','Apiculture' UNION ALL
select 1971,1062,'Бубарство',NULL,1,'6211207',1,'2020-11-19 00:00:00.000',NULL,1,'Silkworm breeding','Seidenraupenzucht','Sériciculture' UNION ALL
select 1972,1062,'Коневъдство',NULL,1,'6211208',1,'2020-11-19 00:00:00.000',NULL,1,'Horse breeding','Pferdezucht','Élevage de chevaux' UNION ALL
select 1973,1063,'Отглеждане на кучета',NULL,2,'6211301',1,'2020-11-19 00:00:00.000',NULL,1,'Dog breeding','Aufzucht von Hunden','Cynologie' UNION ALL
select 1974,1063,'Специализирано обучение и селекционно развъждане на кучета',NULL,3,'6211302',1,'2020-11-19 00:00:00.000',NULL,1,'Special training and selective breeding of dogs','Spezialisiertes Training und selektive Zucht von Hunden','Dressage spécialisé et élevage sélectif de chiens' UNION ALL
select 1975,1064,'Цветарство',NULL,3,'6220101',1,'2020-11-19 00:00:00.000',NULL,1,'Floriculture','Blumenzucht','Floriculture' UNION ALL
select 1976,1064,'Парково строителство и озеленяване',NULL,3,'6220102',1,'2020-11-19 00:00:00.000',NULL,1,'Park construction and landscaping','Park- und Landschaftsbau','Construction de parcs et aménagement paysager' UNION ALL
select 1977,1065,'Цветарство',NULL,2,'6220201',1,'2020-11-19 00:00:00.000',NULL,1,'Floriculture','Blumenzucht','Floriculture' UNION ALL
select 1978,1065,'Парково строителство и озеленяване',NULL,2,'6220202',1,'2020-11-19 00:00:00.000',NULL,1,'Park construction and landscaping','Park- und Landschaftsbau','Construction de parcs et aménagement paysager' UNION ALL
select 1979,1066,'Озеленяване и цветарство',NULL,1,'6220301',1,'2020-11-19 00:00:00.000',NULL,1,'Landscaping and floriculture','Landschaftsbau und Blumenzucht','Aménagement paysager et floriculture' UNION ALL
select 1980,1067,'Очна оптика',NULL,3,'7250101',1,'2020-11-19 00:00:00.000',NULL,1,'Eye optics','Augenoptik','Optique oculaire' UNION ALL
select 1981,1069,'Оптометрия',NULL,4,'7250201',1,'2020-11-19 00:00:00.000',NULL,1,'Optometry','Optometrie','Optométrie' UNION ALL
select 1982,1070,'Ортопедична техника и бандажи',NULL,3,'7250301',1,'2020-11-19 00:00:00.000',NULL,1,'Orthopedic equipment and bandages','Orthopädietechnik und Bandagen','Matériel orthopédique et bandages' UNION ALL
select 1983,1071,'Слухови апарати',NULL,3,'7250401',1,'2020-11-19 00:00:00.000',NULL,1,'Hearing aid','Hörgeräte','Audioprothèses' UNION ALL
select 1984,1072,'Посредник на трудовата борса',NULL,3,'7620101',1,'2020-11-19 00:00:00.000',NULL,1,'Intermediary on the labour market','Vermittler im Arbeitsamt','Intermédiaire dans la bourse du travail' UNION ALL
select 1985,1074,'Социална работа с деца и семейства в риск',NULL,3,'7620201',1,'2020-11-19 00:00:00.000',NULL,1,'Social work with children and families at risk','Soziale Arbeit mit gefährdeten Kindern und Familien','Travail social avec des enfants et des familles à risque' UNION ALL
select 1986,1074,'Социална работа с деца и възрастни с увреждания и хронични заболявания',NULL,3,'7620202',1,'2020-11-19 00:00:00.000',NULL,1,'Social work with children and adults with disabilities and chronic diseases','Soziale Arbeit mit Kindern und Erwachsenen mit Behinderungen und chronischen Erkrankungen','Travail social avec des enfants et d''adultes handicapés et souffrant de maladies chroniques' UNION ALL
select 1987,1075,'Помощник - възпитател в отглеждането и възпитанието на деца',NULL,3,'7620301',1,'2020-11-19 00:00:00.000',NULL,1,'Assistant teacher in raising and education of children','Hilfserzieher für die Erziehung von Kindern','Assistant-éducateur dans l''élevage et l''éducation des enfants' UNION ALL
select 1988,1076,'Подпомагане на деца',NULL,2,'7620401',1,'2020-11-19 00:00:00.000',NULL,1,'Child support','Unterstützung von Kindern','Aide aux enfants' UNION ALL
select 1989,1076,'Подпомагане на възрастни',NULL,2,'7620402',1,'2020-11-19 00:00:00.000',NULL,1,'Adult support','Unterstützung von Erwachsenen','Aide aux adultes' UNION ALL
select 1990,1077,'Жестомимичен език',NULL,3,'7620501',0,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 1991,656,'Организация на дейностите в места за настаняване',NULL,2,'8110103',1,'2020-11-19 00:00:00.000',NULL,1,'Organization of the activities in the places of accommodation','Organisation von Aktivitäten in Beherbergungsstätten','Organisation des activités en hébergement' UNION ALL
select 1992,667,'Екскурзоводско обслужване',NULL,3,'8120302',1,'2020-11-19 00:00:00.000',NULL,1,'Tour guiding services','Reiseführungsdienst','Services de guide touristique' UNION ALL
select 1993,667,'Организация на екскурзоводската дейност',NULL,4,'8120303',1,'2020-11-19 00:00:00.000',NULL,1,'Organization of the tour guiding activity','Organisation der Reiseführertätigkeit ','Organisation des activités de guide touristique' UNION ALL
select 1994,1078,'Туристическа анимация',NULL,3,'8120402',1,'2020-11-19 00:00:00.000',NULL,1,'Tourist animation','Touristische Animation','Animation touristique' UNION ALL
select 1995,1078,'Организация на аниматорската дейност',NULL,4,'8120403',1,'2020-11-19 00:00:00.000',NULL,1,'Organization of the animation activity','Organisation der Animationstätigkeit','Organisation de l’animation touristique' UNION ALL
select 1997,1079,'Спортна ергономия',NULL,4,'8130701',1,'2020-11-19 00:00:00.000',NULL,1,'Sports ergonomics','Sportergonomie','Ergonomie sportive' UNION ALL
select 1998,1079,'Постурална ергономия',NULL,4,'8130702',1,'2020-11-19 00:00:00.000',NULL,1,'Postural ergonomics','Ergonomie der Körperhaltung','Ergonomie posturale' UNION ALL
select 1999,1079,'Професионална ергономия',NULL,4,'8130703',1,'2020-11-19 00:00:00.000',NULL,1,'Professional ergonomics','Ergonomie der Körperhaltung','Ergonomie professionnelle' UNION ALL
select 2000,1080,'Организация на спортни прояви и първенства',NULL,3,'8130801',1,'2020-11-19 00:00:00.000',NULL,1,'Organization of sports events and championships','Organisation von Sportveranstaltungen und Meisterschaften','Organisation d''événements sportifs et de championnats' UNION ALL
select 2001,1081,'Фитнес',NULL,3,'8130901',1,'2020-11-19 00:00:00.000',NULL,1,'Fitness','Fitness','Remise en forme physique' UNION ALL
select 2002,1082,'Спорт',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2003,1083,'Организация на весели обредно-ритуални дейности',NULL,3,'8140301',1,'2020-11-19 00:00:00.000',NULL,1,'Organization of happy and ritual activities','Organisation fröhlicher ritueller Aktivitäten','Organisation d''activités rituelles joyeuses' UNION ALL
select 2004,1083,'Организация на траурни обредно-ритуални дейности',NULL,3,'8140302',1,'2020-11-19 00:00:00.000',NULL,1,'Organization of funerals and ritual activities','Organisation trauriger ritueller Aktivitäten','Organisation d''activités rituelles de deuil' UNION ALL
select 2005,1084,'Екология и опазване на околната среда',NULL,3,'8510101',1,'2020-11-19 00:00:00.000',NULL,1,'Ecology and Environmental Protection','Ökologie und Umweltschutz','Écologie et protection de l’environnement' UNION ALL
select 2006,1085,'Екология и опазване на околната среда',NULL,4,'8510201',1,'2020-11-19 00:00:00.000',NULL,1,'Ecology and Environmental Protection','Ökologie und Umweltschutz','Écologie et protection de l’environnement' UNION ALL
select 2007,1086,'Банкова охрана и инкасова дейност',NULL,3,'8610101',1,'2020-11-19 00:00:00.000',NULL,1,'Bank security and collection activity','Banksicherheits- und Inkassotätigkeit','Activités de sécurité bancaire et de prélèvement bancaire' UNION ALL
select 2008,1086,'Лична охрана',NULL,3,'8610102',1,'2020-11-19 00:00:00.000',NULL,1,'Personal security','Leibwache','Garde du corps' UNION ALL
select 2009,1086,'Физическа охрана на обекти',NULL,3,'8610103',1,'2020-11-19 00:00:00.000',NULL,1,'Pysical safeguarding of sites','Physische Sicherheit von Standorten','Sécurité physique des sites' UNION ALL
select 2010,1086,'Организация на охранителната дейност',NULL,4,'8610104',1,'2020-11-19 00:00:00.000',NULL,1,'Organization of the safeguarding activity','Organisation der Sicherheitstätigkeit','Organisation des activités de sécurité' UNION ALL
select 2011,1087,'Търсене, спасяване и извършване на аварийно-възстановителни работи',NULL,3,'8610201',1,'2020-11-19 00:00:00.000',NULL,1,'Search, rescue and emergency restorative works','Such-, Rettungs- und Bergungsmaßnahmen ','Travaux de recherche, de sauvetage et d''intervention d’avarie et fr réparation' UNION ALL
select 2112,1046,'Изолации в строителството',NULL,2,'5820405',1,'2020-11-19 00:00:00.000',NULL,1,'Insulation in construction','Dämmung im Bau','Isolations dans la construction' UNION ALL
select 2113,1057,'Управление на растениевъдни и животновъдни ферми',NULL,4,'6210603',1,'2020-11-19 00:00:00.000',NULL,1,'Plant growing and animal breeding farm management','Verwaltung von Ackerbau- und Viehbetrieben','Gestion d''exploitations de productions végétales et d''élevage' UNION ALL
select 2114,1005,'Експлоатация и ремонт на летателни апарати',NULL,4,'5250903',1,'2020-11-19 00:00:00.000',NULL,1,'Aircraft exploitation and repair ','Betrieb und Instandsetzung von Luftfahrzeugen ','Exploitation et réparation d''aéronefs' UNION ALL
select 2115,1005,'Експлоатация и ремонт на електронно - приборна авиационна техника',NULL,4,'5250904',1,'2020-11-19 00:00:00.000',NULL,1,'Exploitation and repair of electronic instrumental aviation equipment','Betrieb und Instandsetzung von elektronischer Luftfahrtausrüstung','Exploitation et réparation d''équipements aéronautiques électroniques' UNION ALL
select 2116,1005,'Ремонт на летателни апарати',NULL,3,'5250905',1,'2020-11-19 00:00:00.000',NULL,1,'Aircraft repair','Instandsetzung von Luftfahrzeugen','Réparation d''avions' UNION ALL
select 2117,1005,'Ремонт на електронно - приборна авиационна техника',NULL,3,'5250906',1,'2020-11-19 00:00:00.000',NULL,1,'Repair of electronic instrumental aviation equipment','Instandsetzung von elektronischer Luftfahrtausrüstung','Réparation d''équipements aéronautiques électroniques' UNION ALL
select 2118,933,'Филмов монтаж',NULL,4,'2130101',1,'2020-11-19 00:00:00.000',NULL,1,'Film editing','Filmschnitt','Montage de film' UNION ALL
select 2119,933,'Видеомонтаж',NULL,4,'2130102',1,'2020-11-19 00:00:00.000',NULL,1,'Video editing','Videoschnitt','Montage vidéo' UNION ALL
select 2120,933,'Компютърен монтаж',NULL,4,'2130103',1,'2020-11-19 00:00:00.000',NULL,1,'Computer editing','Computervideoschnitt','Montage par ordinateur' UNION ALL
select 2121,936,'Тоноператорство',NULL,4,'2130401',1,'2020-11-19 00:00:00.000',NULL,1,'Sound-operating industry','Tontechnik','Ingénierie du son' UNION ALL
select 2122,934,'Приложна фотография',NULL,4,'2130202',1,'2020-11-19 00:00:00.000',NULL,1,'Applied photography','Angewandte Fotografie','Photographie appliquée' UNION ALL
select 2123,934,'Фоторепортерство',NULL,4,'2130203',1,'2020-11-19 00:00:00.000',NULL,1,'Press photography','Fotojournalismus','Photojournalisme' UNION ALL
select 2124,941,'Компютърна анимация',NULL,4,'2130902',1,'2020-11-19 00:00:00.000',NULL,1,'Computer animation','Computeranimation','Animation par ordinateur' UNION ALL
select 2125,902,'Сценичен костюм',NULL,3,'2140113',1,'2020-11-19 00:00:00.000',NULL,1,'Stage costume','Bühnenkostüm','Costume de scène' UNION ALL
select 2126,1088,'Библиотекознание',NULL,3,'3220101',1,'2020-11-19 00:00:00.000',NULL,1,'Library science','Bibliothekswissenschaft','Bibliothéconomie' UNION ALL
select 2127,1089,'Кино и телевизия',NULL,4,'2131001',1,'2020-11-19 00:00:00.000',NULL,1,'Кино и телевизия','Kino und Fernsehen','Cinéma et télévision' UNION ALL
select 2128,1090,'Кино и телевизия',NULL,4,'2131101',1,'2020-11-19 00:00:00.000',NULL,1,'Кино и телевизия','Kino und Fernsehen','Cinéma et télévision' UNION ALL
select 2129,1091,'Мехатроника',NULL,3,'5211401',1,'2020-11-19 00:00:00.000',NULL,1,'Mechatronics','Mechatronik','Mécatronique' UNION ALL
select 2130,1092,'Автомобилна мехатроника',NULL,3,'5250103',1,'2020-11-19 00:00:00.000',NULL,1,'Automotive mechatronics','Kraftfahrzeugmechatronik','Mécatronique automobile' UNION ALL
select 2131,1093,'Експлоатация и поддържане на хладилна и климатична техника в хранителнo-вкусовата промишленост',NULL,3,'5410401',1,'2020-11-19 00:00:00.000',NULL,1,'Exploitation and maintenance of refrigerating and air-conditioning equipment in food industry','Betrieb und Wartung von Kälte- und Klimatechnik in der Lebensmittelindustrie','Exploitation et entretien d''équipements de réfrigération et de climatisation dans l''industrie alimentaire' UNION ALL
select 2132,1094,'Агроекология',NULL,3,'6211401',1,'2020-11-19 00:00:00.000',NULL,1,'Agroecology','Agrarökologie','Agroécologie' UNION ALL
select 2133,1095,'Логистика на товари и услуги',NULL,2,'8401001',1,'2020-11-19 00:00:00.000',NULL,1,'Logistics of freights and services','Fracht- und Dienstleistungslogistik','Logistique de frets et de services' UNION ALL
select 2134,492,'Модерен танц',NULL,3,'2120502',1,'2020-11-19 00:00:00.000',NULL,1,'Modern dance','Moderner Tanz','Danse moderne' UNION ALL
select 2135,1096,'Индустриални отношения',NULL,3,'3470101',1,'2020-11-19 00:00:00.000',NULL,1,'Industrial relations','Industrielle Beziehungen','Relations industrielles' UNION ALL
select 2136,1097,'Здравни грижи',NULL,3,'7230101',1,'2020-11-19 00:00:00.000',NULL,1,'Health care','Gesundheitspflegen','Soins de santé' UNION ALL
select 2137,1098,'Здравни грижи',NULL,2,'7230201',1,'2020-11-19 00:00:00.000',NULL,1,'Health care','Gesundheitspflegen','Soins de santé' UNION ALL
select 2138,1099,'Извършване на термални процедури в балнеологични и други възстановителни центрове',NULL,2,'7260101',1,'2020-11-19 00:00:00.000',NULL,1,'Performance of thermal procedures in balneological and other rehabilitation centres','Durchführung von Thermalbehandlungsprozeduren in balneologischen und anderen Rehabilitationszentren','Exécution des procédures thermales dans des centres de balnéologie et d’autres centres de rééducation' UNION ALL
select 2139,1100,'Маникюр, педикюр и ноктопластика',NULL,2,'8150301',1,'2020-11-19 00:00:00.000',NULL,1,'Manicure, pedicure and nail design','Handpflege, Fußpflege und Nagelplastik','Manucure, pédicure et onglerie' UNION ALL
select 2140,1101,'Транспортиране на пострадали и болни хора и оказване на първа помощ',NULL,2,'7230301',1,'2020-11-19 00:00:00.000',NULL,1,'Transportation of injured and sick people and first aid','Transportieren von Verletzten und Kranken und Notfallversorgung','Transport des personnes blessées et malades et prêter le premier secours' UNION ALL
select 2141,1101,'Транспортиране на пострадали и болни хора, оказване на първа помощ и асистиране в спешни отделения',NULL,3,'7230302',1,'2020-11-19 00:00:00.000',NULL,1,'Transport des personnes blessées et malades, prêter le premier secours et assistance successive aux médecins','Transportieren von Verletzten und Kranken, Notfallversorgung und Assistenz in Notaufnahmen',NULL UNION ALL
select 2142,1101,'Транспортиране на пострадали и болни хора, оказване на първа помощ и асистиране на лекаря в спешната помощ',NULL,4,'7230303',1,'2020-11-19 00:00:00.000',NULL,1,'Transport des personnes blessées et malades, prêter le premier secours et assistance successive aux médecins','Transportieren von Verletzten und Kranken, Notfallversorgung und Unterstützung des Arztes in der Notfallversorgung',NULL UNION ALL
select 2143,1084,'Експлоатация на съоръжения за пречистване на води',NULL,2,'8510102',1,'2020-11-19 00:00:00.000',NULL,1,'Exploitation of water purification facilities','Betrieb von Wasserklärungsanlagen','Exploitation des installations de traitement de l''eau' UNION ALL
select 2144,1092,'Електрически превозни средства',NULL,3,'5250104',1,'2020-11-19 00:00:00.000',NULL,1,'Electric vehicles','Elektrofahrzeuge','Véhicules électriques' UNION ALL
select 2145,1000,'Електрически превозни средства',NULL,2,'5250203',1,'2020-11-19 00:00:00.000',NULL,1,'Electric vehicles','Elektrofahrzeuge','Véhicules électriques' UNION ALL
select 2146,671,'Организация и технология на фризьорските услуги',NULL,3,'8150102',1,'2020-11-19 00:00:00.000',NULL,1,'Organization and technology of hairdressing services','Organisation und Technologie der Friseurdienstleistungen','Organisation et technologie des services de coiffure' UNION ALL
select 2147,672,'Организация и технология на козметичните услуги',NULL,3,'8150202',1,'2020-11-19 00:00:00.000',NULL,1,'Organization and technology of cosmetic services','Organisation und Technologie der Kosmetikdienstleistungen','Organisation et technologie des services de cosmétique' UNION ALL
select 2148,1100,'Организация и технология на маникюрната и педикюрната дейност',NULL,3,'8150302',1,'2020-11-19 00:00:00.000',NULL,1,'Organisation et technologie des activités de manucure et pédicure','Organisation und Technologie der Handpflege- und Fußpflegetätigkeit',NULL UNION ALL
select 2149,1102,'Осигуряване на приемна грижа',NULL,2,'7620601',1,'2020-11-19 00:00:00.000',NULL,1,'Ensuring foster care','Vermittlung von Pflegeeltern','Assurer une famille d''accueil' UNION ALL
select 2150,1103,'Инструкторска дейност по фризьорски услуги',NULL,4,'8150401',1,'2020-11-19 00:00:00.000',NULL,1,'Instructor''s activity in hairdressing services','Ausbildungstätigkeit in Friseurdienstleistungen','Activité d’instructeur en services de coiffure' UNION ALL
select 2151,1103,'Инструкторска дейност по козметични услуги',NULL,4,'8150402',1,'2020-11-19 00:00:00.000',NULL,1,'Instructor''s activity in cosmetic services','Ausbildungstätigkeit in Kosmetikdienstleistungen','Activité d’instructeur en services de cosmétique' UNION ALL
select 2152,1103,'Инструкторска дейност по маникюр и педикюр',NULL,4,'8150403',1,'2020-11-19 00:00:00.000',NULL,1,'Instructor''s activity in manicure and pedicure','Ausbildungstätigkeit in Hand- und Fußpflege','Activité d’instructeur en services de manucure et pédicure' UNION ALL
select 2153,1104,'Експлоатация на парни и водогрейни съоръжения',NULL,2,'5220501',1,'2020-11-19 00:00:00.000',NULL,1,'Exploitation of steam and water heating facilities','Betrieb von Dampf- und Heißwasseranlagen','Exploitation des installations de vapeur et de chauffage d''eau' UNION ALL
select 2154,1105,'Водещ на музикално артистични събития',NULL,3,'2131501',1,'2020-11-19 00:00:00.000',NULL,1,'Music and artistic events presenter','Moderator von musikalischen und künstlerischen Veranstaltungen','Animateur d''événements musicaux et artistiques' UNION ALL
select 2155,1106,'Асистент на лекар по дентална медицина',NULL,3,'7240101',1,'2020-11-19 00:00:00.000',NULL,1,'Assistant of dental physician','Zahnarztassistent','Assistant du médecin dentiste' UNION ALL
select 2156,1107,'Конструктивна реставрация',NULL,2,'5810201',1,'2020-11-19 00:00:00.000',NULL,1,'Constructive restoration','Konstruktive Restaurierung','Restauration constructive' UNION ALL
select 2157,1107,'Декоративна реставрация',NULL,2,'5810202',1,'2020-11-19 00:00:00.000',NULL,1,'Decorative restoration','Dekorative Restaurierung','Restauration décorative' UNION ALL
select 2158,1108,'Архитектурна реставрация',NULL,3,'5810301',1,'2020-11-19 00:00:00.000',NULL,1,'Architectural restoration','Architekturrestaurirung','Restauration architecturale' UNION ALL
select 2159,1109,'Декорация на органични материали',NULL,2,'8111101',1,'2020-11-19 00:00:00.000',NULL,1,'Decoration of organic matter','Dekoration von organischen Materialien','Décoration de matières organiques' UNION ALL
select 2160,1110,'Декорация на органични материали',NULL,4,'8111201',1,'2020-11-19 00:00:00.000',NULL,1,'Decoration of organic matter','Dekoration von organischen Materialien','Décoration de matières organiques' UNION ALL
select 2161,1005,'Методи на безразрушителен контрол',NULL,3,'5250907',1,'2020-11-19 00:00:00.000',NULL,1,'Methods of non-destructive testing','Zerstörungsfreie Prüfverfahren','Méthodes de contrôle non destructif' UNION ALL
select 2162,967,'Горско стопанство',NULL,3,'3451205',1,'2020-11-19 00:00:00.000',NULL,1,'Forestry','Forstwirtschaft','Sylviculture' UNION ALL
select 2163,566,'Технология на хомеопатичните и фитопродукти',NULL,3,'5240114',1,'2020-11-19 00:00:00.000',NULL,1,'Technology of homeopathic and phyto-products','Technologie der homöopathischen und Phytoprodukte','Technologie des produits homéopathiques et des phytoproduits' UNION ALL
select 2164,554,'Топлотехника - топлинна, климатична, вентилационна и хладилна',NULL,3,'5220309',1,'2020-11-19 00:00:00.000',NULL,1,'Heating technology - thermal, air-conditioning, ventilation and refrigerating','Wärmetechnik - Wärme, Klima, Lüftung und Kälte','Technique thermique - chaleur, climatisation, ventilation et réfrigération' UNION ALL
select 2165,555,'Топлотехника - топлинна, климатична, вентилационна и хладилна',NULL,2,'5220409',1,'2020-11-19 00:00:00.000',NULL,1,'Heating technology - thermal, air-conditioning, ventilation and refrigerating','Wärmetechnik - Wärme, Klima, Lüftung und Kälte','Technique thermique - chaleur, climatisation, ventilation et réfrigération' UNION ALL
select 2166,672,'Соларни услуги',NULL,3,'8150203',1,'2020-11-19 00:00:00.000',NULL,1,'Solar services','Solariumdienstleistungen','Service de solarium' UNION ALL
select 2167,1112,'Спедиция, транспортна и складова логистика',NULL,3,'8401101',1,'2020-11-19 00:00:00.000',NULL,1,'Forwarding, transportation and storage logistics','Speditions-, Transport- und Lagerlogistik','Expédition, transport et logistique d''entreposage' UNION ALL
select 2168,1112,'Планиране и управление на логистични процеси',NULL,4,'8401102',1,'2020-11-19 00:00:00.000',NULL,1,'Planning and management of logistic processes','Planung und Verwaltung von Logistikprozessen ','Planification et gestion des processus logistiques' UNION ALL
select 2170,954,'Търговия с технически помощни средства',NULL,3,'3410202',1,'2020-11-19 00:00:00.000',NULL,1,'Trade in technical aids','Handel mit technischen Hilfsmitteln','Commerce des aides techniques' UNION ALL
select 2171,954,'Търговия с хранителни добавки',NULL,3,'3410203',1,'2020-11-19 00:00:00.000',NULL,1,'Trade in food additives','Handel mit Nahrungsergänzungsmitteln','Commerce de compléments alimentaires' UNION ALL
select 2172,1072,'Консултант по подкрепена заетост',NULL,3,'7620102',1,'2020-11-19 00:00:00.000',NULL,1,'Supported employment consultant','Berater für unterstützte Beschäftigung','Conseiller en emploi soutenu' UNION ALL
select 2180,1082,'Спорт - Баскетбол',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2181,1082,'Спорт - Бадминтон',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2182,1082,'Спорт - Биатлон',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2183,1082,'Спорт - Борба класически стил',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2184,1082,'Спорт - Борба свободен стил',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2185,1082,'Спорт - Бокс',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2186,1082,'Спорт - Вдигане на тежести',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2187,1082,'Спорт - Волейбол',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2188,1082,'Спорт - Гребане',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2189,1082,'Спорт - Джудо',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2190,1082,'Спорт - Кану-каяк',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2191,1082,'Спорт - Карате',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2192,1082,'Спорт - Кик-бокс',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2193,1082,'Спорт - Колоездене',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2194,1082,'Спорт - Конен спорт',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2195,1082,'Спорт - Лека атлетика',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2196,1082,'Спорт - Модерен петобой',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2197,1082,'Спорт -Плуване',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2198,1082,'Спорт - Самбо',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2199,1082,'Спорт - Ски алпийски дисциплини',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2200,1082,'Спорт - Ски бягане',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2201,1082,'Спорт - Скокове на батут',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2202,1082,'Спорт - Сноуборд',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2203,1082,'Спорт - Спортна акробатика',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2204,1082,'Спорт - Спортна гимнастика',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2205,1082,'Спорт - Спортна стрелба',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2206,1082,'Спорт - Сумо',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2207,1082,'Спорт - Тенис',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2208,1082,'Спорт - Тенис на маса',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2209,1082,'Спорт - Футбол',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2210,1082,'Спорт - Хандбал',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2211,1082,'Спорт - Художествена гимнастика',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2212,1082,'Спорт - Шотокан карате-до',NULL,3,'8131001',1,'2020-11-19 00:00:00.000',NULL,1,'Sport','Sport','Sport' UNION ALL
select 2213,932,'Изобразително изкуство',NULL,3,'2110000',1,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 2214,1113,'Приложно програмиране',NULL,3,'4810301',1,'2020-11-19 00:00:00.000',NULL,1,'Applied programming','Anwendungsprogrammierung','Programmation appliquée' UNION ALL
select 2215,1114,'Облекла по поръчка',NULL,3,'5420411',1,'2020-11-19 00:00:00.000',NULL,1,'Custom-made clothes','Kleidung auf Bestellung','Vêtements sur commande' UNION ALL
select 2216,1114,'Бутикови облекла',NULL,3,'5420412',1,'2020-11-19 00:00:00.000',NULL,1,'Boutique clothes','Boutique-Kleidung','Vêtements boutique' UNION ALL
select 2217,1114,'Моден консултант',NULL,4,'5420413',1,'2020-11-19 00:00:00.000',NULL,1,'Fashion consultant','Modeberater','Conseiller en mode' UNION ALL
select 2218,1115,'Съдебна администрация',NULL,3,'3460401',1,'2020-11-19 00:00:00.000',NULL,1,'Court administration','Justizverwaltung','Administration judiciaire' UNION ALL
select 2219,675,'Ръководител движение',NULL,3,'8400302',1,'2020-11-19 00:00:00.000',NULL,1,'Signalman','Verkehrsleiter','Agent circulation ferroviaire' UNION ALL
select 2220,1116,'Флейта',NULL,4,'8630601',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2221,1116,'Обой',NULL,4,'8630602',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2223,1116,'Кларнет',NULL,4,'8630603',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2224,1116,'Фагот',NULL,4,'8630604',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2225,1116,'Валдхорна',NULL,4,'8630605',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2226,1116,'Тромпет',NULL,4,'8630606',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2227,1116,'Цугтромбон',NULL,4,'8630607',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2228,1116,'Туба',NULL,4,'8630608',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2229,1116,'Саксофон',NULL,4,'8630609',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2230,1116,'Флигорна',NULL,4,'8630610',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2231,1116,'Бас флигорна',NULL,4,'8630611',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2232,1116,'Ударни инструменти',NULL,4,'8630612',0,'2020-11-19 00:00:00.000','2021-04-13 00:00:00.000',1,NULL,NULL,NULL UNION ALL
select 2233,977,'Специално производство (производство на въоръжение и боеприпаси)',NULL,4,'5210121',1,'2020-11-19 00:00:00.000',NULL,1,'Special production(Production of weaponry and ammunition)','Spezielle Produktion (Produktion von Waffen und Munition)','Fabrication spéciale (production d''armes et de munitions)' UNION ALL
select 2334,554,'Управление на радиоактивни отпадъци',NULL,3,'5220310',1,'2020-11-19 00:00:00.000',NULL,1,'Radioactive waste management','Management von radioaktiven Abfällen','Gestion des déchets radioactifs' UNION ALL
select 2335,1117,'Осигуряване на продуктова информация',NULL,3,'5240301',1,'2020-11-19 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 2336,992,'Охранителна техника и системи за сигурност',NULL,3,'5230304',1,'2020-11-19 00:00:00.000',NULL,1,'Safety technology and security systems','Sicherheitstechnik und Sicherheitssysteme','Équipements de sécurité et systèmes de sécurité' UNION ALL
select 2337,993,'Охранителна техника и системи за сигурност',NULL,2,'5230404',1,'2020-11-19 00:00:00.000',NULL,1,'security surveillance technology and security systems','Sicherheitstechnik und Sicherheitssysteme','Équipements de sécurité et systèmes de sécurité' UNION ALL
select 2338,1118,'Български жестов език',NULL,3,'7620501',1,'2020-11-19 00:00:00.000',NULL,1,'Bulgarian sign language','Bulgarische Gebärdensprache','Langue des signes bulgare' UNION ALL
select 2339,1119,'Програмиране на изкуствен интелект','',3,'4810401',1,'2021-04-13 00:00:00.000',NULL,1,'Artificial intelligence programming','KI-Programmierung','Programmation d''intelligence artificielle' UNION ALL
select 2340,1120,'Програмиране на роботи','',3,'4810501',1,'2021-04-13 00:00:00.000',NULL,1,'Robot programming','Roboterprogrammierung','Programmation de robots' UNION ALL
select 2341,1063,'Фризьор на кучета','',2,'6211303',1,'2021-04-13 00:00:00.000',NULL,1,'Dogs’ hairdresser','Hundefriseur','Toiletteur pour chiens' UNION ALL
select 2342,908,'Командване на мотопехотни и разузнавателни формирования','',4,'8630102',1,'2021-01-04 00:00:00.000',NULL,1,'Commanding motorized infantry and intelligence forces','Führung von motorisierten Infanterie- und Aufklärungsformationen','Commandement des élément d''infanterie blindé et de renseignement' UNION ALL
select 2343,908,'Командване на артилерийски формирования','',4,'8630103',1,'2021-01-04 00:00:00.000',NULL,1,'Commanding artillery forces','Führung von Artillerieformationen','Commandement des formations d''artillerie' UNION ALL
select 2344,908,'Командване на инженерни формирования','',4,'8630104',1,'2021-01-04 00:00:00.000',NULL,1,'Commanding engineering forces','Führung von Ingenieurformationen','Commandement des formations d''ingénierie' UNION ALL
select 2345,908,'Командване на формирование за ядрена, химическа и бактериологическа защита','',4,'8630105',1,'2021-01-04 00:00:00.000',NULL,1,'Commanding forces for nuclear, chemical and bacteriological defense','Führung von Formationen für nuklearen, chemischen und bakteriologischen Schutz','Commandement d''une formation de protection nucléaire, chimique et bactériologique' UNION ALL
select 2346,908,'Командване на формирования за комуникационно-информационна поддръжка','',4,'8630106',1,'2021-01-04 00:00:00.000',NULL,1,'Commanding forces for communication and information maintenance','Führung von Formationen zur Kommunikations- und Informationsunterstützung ','Commandement des formations d''appui à la communication et à l''information' UNION ALL
select 2347,908,'Командване на зенитно-ракетни формирования','',4,'8630107',1,'2021-01-04 00:00:00.000',NULL,1,'Commanding anti-aircraft launchers','Führung von Flugabwehrraketenformationen','Commandement des formations de missiles anti-aériens' UNION ALL
select 2348,908,'Радиотехнически средства и комуникационно-информационни системи за ВМС','',4,'8630108',1,'2021-01-04 00:00:00.000',NULL,1,'Radiotechnical devices and communication-information systems for the navy','Funktechnische Mittel und Kommunikations- und Informationssysteme für die Seestreitkräfte','Moyens techniques radio et systèmes de communication et d''information pour la marine militaire' UNION ALL
select 2349,908,'Морски оръжия','',4,'8630109',1,'2021-01-04 00:00:00.000',NULL,1,'Marine weapons','Marinewaffen','« Armes marines' UNION ALL
select 2350,908,'Корабни енергетични уредби за ВМС','',4,'8630110',1,'2021-01-04 00:00:00.000',NULL,1,'Marine energetic devices for the navy','Schiffsenergiesysteme für die Seestreitkräfte ','Appareils énergétiques navals pour la marine militaire' UNION ALL
select 2351,908,'Морско корабоводене за ВМС','',4,'8630111',1,'2021-01-04 00:00:00.000',NULL,1,'Marine navigation for the navy','Schiffsnavigation für die Seestreitkräfte','Navigation navale pour la marine militaire' UNION ALL
select 2352,909,'Обща логистика','',4,'8630201',1,'2021-01-04 00:00:00.000',NULL,1,'General logistics','Allgemeine Logistik','Logistique générale' UNION ALL
select 2353,909,'Експлоатация и ремонт на автомобилна и бронетанкова техника','',4,'8630202',1,'2021-01-04 00:00:00.000',NULL,1,'Exploitation and repair of automotive and armored equipment','Betrieb und Instandsetzung von Kraftfahrzeugen und gepanzerten Fahrzeugen ','Exploitation et réparation de véhicules automobiles et véhicules blindés et moyens de soutien' UNION ALL
select 2354,909,'Експлоатация и ремонт на инженерна, химическа и специална техника','',4,'8630203',1,'2021-01-04 00:00:00.000',NULL,1,'Exploitation and repair of engineering, chemical and special equipment','Betrieb und Instandsetzung von Ingenieur-, Chemie- und Sonderausrüstung','Exploitation et réparation d''équipements techniques, chimiques et spéciaux' UNION ALL
select 2355,909,'Експлоатация и ремонт на радиотехническа, информационна и компютърна техника','',4,'8630204',1,'2021-01-04 00:00:00.000',NULL,1,'Exploitation and repair of radiotechnical, information and computer equipment','Betrieb und Instandsetzung von Funk-, Informations- und Computertechnik','Exploitation et réparation d''équipements radiotechnique, d''information et d''informatique' UNION ALL
select 2356,909,'Експлоатация и ремонт на артилерийска и оптическа техника, работа с бойни припаси','',4,'8630205',1,'2021-01-04 00:00:00.000',NULL,1,'Exploitation and repair of artillery and optical equipment, work with ammunition','Betrieb und Instandsetzung von Artillerie und optische Ausrüstung, Arbeit mit Munition ','Exploitation et réparation d''équipements d''artillerie et d''optique, travail avec des munitions' UNION ALL
select 2357,909,'Експлоатация и ремонт на зенитно въоръжение и радиолокационна техника','',4,'8630206',1,'2021-01-04 00:00:00.000',NULL,1,'Ex;ploitation and repair of anti-aircraft missiles and radiolocation stations','Betrieb und Instandsetzung von Flugabwehrwaffen und Radarstationen ','Exploitation et réparation d''armes antiaériennes et de stations radar' UNION ALL
select 2358,909,'Експлоатация и ремонт на авиационна и навигационна техника','',4,'8630207',1,'2021-01-04 00:00:00.000',NULL,1,'Exploitation and repair of aviation and navigation equipment','Betrieb und Instandsetzung von Luftfahrt- und Navigationsausrüstung','Exploitation et réparation d’équipements aériens et de navigation' UNION ALL
select 2359,1121,'Киберсигурност и кибероперации','',4,'8630801',1,'2021-01-04 00:00:00.000',NULL,1,'Cybersecurity and cyberoperations','Cybersicherheit und Cyberoperationen','Cybersécurité et cyberopérations' UNION ALL
select 2360,930,'Вътрешни и международни превози на пътници',NULL,2,'8400901',0,'2021-12-16 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 2361,930,'Вътрешни и международни превози на товари',NULL,2,'8400902',0,'2021-12-16 00:00:00.000',NULL,1,NULL,NULL,NULL UNION ALL
select 2362,1082,'Спорт - Таекуондо - стил ВТ',NULL,3,'8131001',1,'2024-09-15 00:00:00.000',NULL,1,'Sport','Sport','Sport';

GO
