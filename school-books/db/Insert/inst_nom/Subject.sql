GO
PRINT 'Insert [inst_nom].[Subject]'
GO

set identity_insert [inst_nom].[Subject] on;

insert [inst_nom].[Subject] ([SubjectID],[SubjectName],[SubjectNameShort],[IsValid],[NameEN],[NameDE],[NameFR],[IsMandatory])
select -1,'Чужд език','Чужд език',1,NULL,NULL,NULL,1 UNION ALL
select 1,'Български език и литература','Български език и литература',1,'Bulgarian Language and Literature','Bulgarisch und Literatur','Langue et littérature bulgares',1 UNION ALL
select 2,'Математика','Математика',1,'Mathematics','Mathematik','Mathématiques',1 UNION ALL
select 3,'История','История',0,NULL,NULL,NULL,1 UNION ALL
select 4,'Родинознание','Родинознание',1,NULL,NULL,NULL,1 UNION ALL
select 5,'Философия','Философия',1,'Philosophy','Philosophie','Philosophie',1 UNION ALL
select 6,'Психология','Психология',0,NULL,NULL,NULL,1 UNION ALL
select 7,'Логика','Логика',0,NULL,NULL,NULL,1 UNION ALL
select 8,'Етика','Етика',0,NULL,NULL,NULL,1 UNION ALL
select 9,'Естетика','Естетика',0,NULL,NULL,NULL,1 UNION ALL
select 10,'Роден край','Роден край',0,NULL,NULL,NULL,1 UNION ALL
select 11,'Човекът и обществото','Човекът и обществото',1,NULL,NULL,NULL,1 UNION ALL
select 12,'История и цивилизация','История и цивилизация',0,NULL,NULL,NULL,1 UNION ALL
select 13,'География и икономика','География и икономика',1,'Geography and Economics','Erdkunde und Wirtschaft','Géographie et économie',1 UNION ALL
select 14,'Психология и логика','Психология и логика',1,NULL,NULL,NULL,1 UNION ALL
select 15,'Етика и право','Етика и право',1,NULL,NULL,NULL,1 UNION ALL
select 16,'Свят и личност','Свят и личност',1,NULL,NULL,NULL,1 UNION ALL
select 17,'Гражданско образование','Гражданско образование',1,NULL,NULL,NULL,1 UNION ALL
select 18,'История и цивилизации','История и цивилизации',1,'History and Civilizations','Geschichte und Zivilisation','Histoire et civilisations',1 UNION ALL
select 20,'География','География',0,NULL,NULL,NULL,1 UNION ALL
select 21,'Биология','Биология',0,NULL,NULL,NULL,1 UNION ALL
select 22,'Физика','Физика',0,NULL,NULL,NULL,1 UNION ALL
select 23,'Химия','Химия',0,NULL,NULL,NULL,1 UNION ALL
select 24,'Природознание','Природознание',0,NULL,NULL,NULL,1 UNION ALL
select 25,'Физика и астрономия','Физика и астрономия',1,'Physics and Astronomy','Physik und Astronomie','Physique et astronomie',1 UNION ALL
select 26,'Околен свят','Околен свят',1,NULL,NULL,NULL,1 UNION ALL
select 27,'Човекът и природата','Човекът и природата',1,NULL,NULL,NULL,1 UNION ALL
select 28,'Биология и здравно образование','Биология и здравно образование',1,'Biology and Health Education','Biologie und Gesundheitsbildung','Biologie et éducation à la santé',1 UNION ALL
select 29,'Химия и опазване на околната среда','Химия и опазване на околната среда',1,'Chemistry and Environmental Education','Chemie und Umweltschutz','Chimie et éducation à l''environnement',1 UNION ALL
select 30,'Музика','Музика',1,'Music','Musik','Musique',1 UNION ALL
select 31,'Изобразително изкуство','Изобразително изкуство',1,'Fine Arts','Bildende Kunst','Beaux-arts',1 UNION ALL
select 32,'Ръчен труд','Ръчен труд',0,NULL,NULL,NULL,1 UNION ALL
select 33,'Труд и техника','Труд и техника',0,NULL,NULL,NULL,1 UNION ALL
select 34,'Технологии','Технологии',0,NULL,NULL,NULL,1 UNION ALL
select 35,'Информатика','Информатика',1,'Informatics','Informatik','Informatique',1 UNION ALL
select 36,'Информационни технологии','Информационни технологии',1,'Information Technology','Informationstechnologien','Technologies de l''information',1 UNION ALL
select 37,'Домашен бит и техника','Домашен бит и техника',1,NULL,NULL,NULL,1 UNION ALL
select 38,'Домашна техника и икономика','Домашна техника и икономика',0,NULL,NULL,NULL,1 UNION ALL
select 39,'Теория и методика на спортната тренировка','Теория и методика на спортната тренировка',1,NULL,NULL,NULL,1 UNION ALL
select 40,'Физическа култура и спорт','Физическа култура и спорт',0,NULL,NULL,NULL,1 UNION ALL
select 41,'Организиран отдих и спорт','Организиран отдих и спорт',0,NULL,NULL,NULL,1 UNION ALL
select 42,'Занимания по интереси','Занимания по интереси',1,NULL,NULL,NULL,1 UNION ALL
select 43,'Религия','Религия',1,NULL,NULL,NULL,1 UNION ALL
select 44,'Хореография','Хореография',1,NULL,NULL,NULL,1 UNION ALL
select 45,'Физическо възпитание и спорт','Физическо възпитание и спорт',1,'Physical Education and Sport','Körpererziehung und Sportunterricht','Éducation physique et sportive',1 UNION ALL
select 46,'Организиран отдих и физическа активност','Организиран отдих и физическа активност',1,NULL,NULL,NULL,1 UNION ALL
select 50,'Първи чужд език','Първи чужд език',0,NULL,NULL,NULL,1 UNION ALL
select 51,'Втори чужд език','Втори чужд език',0,NULL,NULL,NULL,1 UNION ALL
select 52,'Трети чужд език','Трети чужд език',0,NULL,NULL,NULL,1 UNION ALL
select 53,'Спортни дейности (бадминтон)','Спортни дейности (бадминтон)',1,NULL,NULL,NULL,1 UNION ALL
select 54,'Спортни дейности (баскетбол )','Спортни дейности (баскетбол )',1,NULL,NULL,NULL,1 UNION ALL
select 55,'Спортни дейности (борба)','Спортни дейности (борба)',1,NULL,NULL,NULL,1 UNION ALL
select 56,'Спортни дейности (волейбол)','Спортни дейности (волейбол)',1,NULL,NULL,NULL,1 UNION ALL
select 57,'Спортни дейности (лека атлетика)','Спортни дейности (лека атлетика)',1,NULL,NULL,NULL,1 UNION ALL
select 58,'Спортни дейности (мини-баскетбол)','Спортни дейности (мини-баскетбол)',1,NULL,NULL,NULL,1 UNION ALL
select 59,'Спортни дейности (мини-волейбол)','Спортни дейности (мини-волейбол)',1,NULL,NULL,NULL,1 UNION ALL
select 60,'Спортни дейности (мини-футбол)','Спортни дейности (мини-футбол)',1,NULL,NULL,NULL,1 UNION ALL
select 61,'Български език','Български език',0,NULL,NULL,NULL,1 UNION ALL
select 62,'Социален свят','Социален свят',0,NULL,NULL,NULL,1 UNION ALL
select 63,'Природен свят','Природен свят',0,NULL,NULL,NULL,1 UNION ALL
select 64,'Художествена информация и литература за деца','Художествена информация и литература за деца',0,NULL,NULL,NULL,1 UNION ALL
select 65,'Конструктивно-технически и битови дейности','Конструктивно-технически и битови дейности',0,NULL,NULL,NULL,1 UNION ALL
select 66,'Игрова култура и пресъздаване','Игрова култура и пресъздаване',0,NULL,NULL,NULL,1 UNION ALL
select 67,'Спортни дейности (мини-хандбал)','Спортни дейности (мини-хандбал)',1,NULL,NULL,NULL,1 UNION ALL
select 68,'Спортни дейности (плуване)','Спортни дейности (плуване)',1,NULL,NULL,NULL,1 UNION ALL
select 69,'Спортни дейности (ски - алпийски дисциплини)','Спортни дейности (ски - алпийски дисциплини)',1,NULL,NULL,NULL,1 UNION ALL
select 70,'Спортни дейности (спортна гимнастика )','Спортни дейности (спортна гимнастика )',1,NULL,NULL,NULL,1 UNION ALL
select 71,'Компютърно моделиране','Компютърно моделиране',1,NULL,NULL,NULL,1 UNION ALL
select 72,'Технологии и предприемачество','Технологии и предприемачество',1,NULL,NULL,NULL,1 UNION ALL
select 73,'Предприемачество','Предприемачество',1,'Entrepreneurship','Unternehmertum','Entrepreneuriat',1 UNION ALL
select 74,'Здравословни и безопасни условия на труд','Здравословни и безопасни условия на труд',1,NULL,NULL,NULL,1 UNION ALL
select 75,'Икономика','Икономика',1,NULL,NULL,NULL,1 UNION ALL
select 76,'Спортни дейности (таекуондо)','Спортни дейности (таекуондо)',1,NULL,NULL,NULL,1 UNION ALL
select 77,'Спортни дейности (футбол )','Спортни дейности (футбол )',1,NULL,NULL,NULL,1 UNION ALL
select 78,'Спортни дейности (хандбал)','Спортни дейности (хандбал)',1,NULL,NULL,NULL,1 UNION ALL
select 79,'Спортни дейности (художествена гимнастика)','Спортни дейности (художествена гимнастика)',1,NULL,NULL,NULL,1 UNION ALL
select 80,'Образователно направление „Български език и литература“','Образователно направление „Български език и литература“',1,NULL,NULL,NULL,1 UNION ALL
select 81,'Образователно направление „Математика“','Образователно направление „Математика“',1,NULL,NULL,NULL,1 UNION ALL
select 82,'Образователно направление „Околен свят“','Образователно направление „Околен свят“',1,NULL,NULL,NULL,1 UNION ALL
select 83,'Образователно направление „Изобразително изкуство“','Образователно направление „Изобразително изкуство“',1,NULL,NULL,NULL,1 UNION ALL
select 84,'Образователно направление „Музика“','Образователно направление „Музика“',1,NULL,NULL,NULL,1 UNION ALL
select 85,'Образователно направление „Конструиране и технологии“','Образователно направление „Конструиране и технологии“',1,NULL,NULL,NULL,1 UNION ALL
select 86,'Образователно направление „Физическа култура“','Образователно направление „Физическа култура“',1,NULL,NULL,NULL,1 UNION ALL
select 87,'Други дейности по обучение, социализация, възпитание и отглеждане на децата','Други дейности по обучение, социализация, възпитание и отглеждане на децата',1,NULL,NULL,NULL,1 UNION ALL
select 88,'Спортни дейности (шахмат)','Спортни дейности (шахмат)',1,NULL,NULL,NULL,1 UNION ALL
select 90,'Чужд език по професията','Чужд език по професията',1,NULL,NULL,NULL,1 UNION ALL
select 91,'Компютърно моделиране и информационни технологии','Компютърно моделиране и информационни технологии',1,NULL,NULL,NULL,1 UNION ALL
select 92,'Проектни и творчески дейности','Проектни и творчески дейности',1,NULL,NULL,NULL,1 UNION ALL
select 99,'Български език като чужд за търсещи или получили закрила','Български език като чужд за търсещи или получили закрила',1,NULL,NULL,NULL,1 UNION ALL
select 100,'Английски език','Английски език',1,'English ','Englisch','Anglais',1 UNION ALL
select 101,'Немски език','Немски език',1,'German','Deutsch','Allemand',1 UNION ALL
select 102,'Френски език','Френски език',1,'French','Französisch','Français',1 UNION ALL
select 103,'Испански език','Испански език',1,'Spanish','Spanisch','Espagnol',1 UNION ALL
select 104,'Италиански език','Италиански език',1,'Italian','Italienisch','Italien',1 UNION ALL
select 105,'Португалски език','Португалски език',1,'Portuguese','Portugiesisch','Portugais',1 UNION ALL
select 106,'Руски език','Руски език',1,'Russian','Russisch','Russe',1 UNION ALL
select 110,'Иврит','Иврит',1,'Hebrew','Hebräisch','Hébreu',1 UNION ALL
select 111,'Арменски език','Арменски език',1,'Armenian','Armenisch','Arménien',1 UNION ALL
select 112,'Турски език','Турски език',1,'Turkish','Türkisch','Turc',1 UNION ALL
select 113,'Ромски език','Ромски език',1,'Romani ','Römisch','Romani ',1 UNION ALL
select 120,'Японски език','Японски език',1,'Japanese','Japanisch','Japonais',1 UNION ALL
select 121,'Китайски език','Китайски език',1,'Chinese ','Chinesisch','Chinoise',1 UNION ALL
select 122,'Персийски език','Персийски език',1,'Persian ','Persisch','Perse',1 UNION ALL
select 123,'Арабски език','Арабски език',1,'Arabic','Arabisch','Arabe',1 UNION ALL
select 124,'Румънски език','Румънски език',1,'Romanian','Rumänisch','Roumain',1 UNION ALL
select 125,'Гръцки език','Гръцки език',1,'Greek','Griechisch','Grec',1 UNION ALL
select 126,'Албански език','Албански език',1,'Albanian','Albanisch','Albanais',1 UNION ALL
select 127,'Шведски език','Шведски език',1,'Swedish','Schwedisch','Suédois',1 UNION ALL
select 128,'Норвежки език','Норвежки език',1,'Norwegian','Norwegisch','Norvégien',1 UNION ALL
select 129,'Финландски език','Финландски език',1,'Finnish','Finnisch','Finlandais',1 UNION ALL
select 130,'Датски език','Датски език',1,'Danish','Dänisch','Danois',1 UNION ALL
select 131,'Холандски език','Холандски език',1,'Dutch','Holländisch','Néerlandais',1 UNION ALL
select 132,'Украински език','Украински език',1,'Ukrainian','Ukrainisch','Ukrainien',1 UNION ALL
select 133,'Полски език','Полски език',1,'Polish','Polnisch','Polonais',1 UNION ALL
select 134,'Чешки език','Чешки език',1,'Czech','Tschechisch','Tchèque',1 UNION ALL
select 135,'Словашки език','Словашки език',1,'Slovak ','Slowakisch','Slovaque',1 UNION ALL
select 136,'Унгарски език','Унгарски език',1,'Hungarian','Ungarisch','Hongrois',1 UNION ALL
select 137,'Сърбохърватски език','Сърбохърватски език',1,'Serbo-Croatian','Serbokroatisch','Serbo-croate',1 UNION ALL
select 138,'Хинди','Хинди',1,'Hindi','Hindi','Hindi',1 UNION ALL
select 139,'Старогръцки език','Старогръцки език',1,'Old Greek','Altgriechisch','Grec ancien',1 UNION ALL
select 140,'Латински език','Латински език',1,'Latin','Latein','Latin',1 UNION ALL
select 141,'Старобългарски език','Старобългарски език',1,'Old Bulgarian','Altbulgarisch','Bulgare ancien',1 UNION ALL
select 142,'Корейски език','Корейски език',1,'Korean ','Koreisch','Coréenne',1 UNION ALL
select 143,'Словенски език','Словенски език',1,'Slovenian','Slowenisch','Slovène',1 UNION ALL
select 180,'Възпитателни дейности','Възпитателни дейности',0,NULL,NULL,NULL,1 UNION ALL
select 181,'Образователни дейности в ДГ','Образователни дейности в ДГ',0,NULL,NULL,NULL,1 UNION ALL
select 196,'Безопасност на движението','Безопасност на движението',1,NULL,NULL,NULL,1 UNION ALL
select 197,'Работа с ученици и родители','Работа с ученици и родители',1,NULL,NULL,NULL,1 UNION ALL
select 198,'Самоподготовка','Самоподготовка',1,NULL,NULL,NULL,1 UNION ALL
select 199,'Час на класа','Час на класа',1,NULL,NULL,NULL,1 UNION ALL
select 204,'- Селскостопански машини','- Селскостопански машини',0,NULL,NULL,NULL,0 UNION ALL
select 206,'# АлгоРитъм','# АлгоРитъм',1,NULL,NULL,NULL,0 UNION ALL
select 215,'.......','.......',1,NULL,NULL,NULL,0 UNION ALL
select 219,'-/Човек и общество','-/Човек и общество',0,NULL,NULL,NULL,0 UNION ALL
select 271,'1ЧЕ АНГЛИЙСКИ','1ЧЕ АНГЛИЙСКИ',0,NULL,NULL,NULL,0 UNION ALL
select 272,'1че-АЕ','1че-АЕ',0,NULL,NULL,NULL,0 UNION ALL
select 298,'3D моделиране - ИТ','3D моделиране - ИТ',1,NULL,NULL,NULL,0 UNION ALL
select 299,'3дравословни и безопасни условия на труд','3дравословни и безопасни условия на труд',0,NULL,NULL,NULL,0 UNION ALL
select 311,'AutoCAD в строителството','AutoCAD в строителството',0,NULL,NULL,NULL,0 UNION ALL
select 501,'II чужд език - Френски език','II чужд език - Френски език',0,NULL,NULL,NULL,0 UNION ALL
select 502,'II чужд език (английски)','II чужд език (английски)',0,NULL,NULL,NULL,0 UNION ALL
select 503,'II чужд език (италиански)','II чужд език (италиански)',0,NULL,NULL,NULL,0 UNION ALL
select 505,'II чужд език- Руски език','II чужд език- Руски език',0,NULL,NULL,NULL,0 UNION ALL
select 506,'II чужд език-руски език','II чужд език-руски език',0,NULL,NULL,NULL,0 UNION ALL
select 507,'III допълнителен час по ФВС Волейбол','III допълнителен час по ФВС Волейбол',1,NULL,NULL,NULL,0 UNION ALL
select 511,'III ФКС модул (футбол)','III ФКС модул (футбол)',0,NULL,NULL,NULL,0 UNION ALL
select 512,'III час по ФВС','III час по ФВС',0,NULL,NULL,NULL,0 UNION ALL
select 513,'III час ФВС','III час ФВС',0,NULL,NULL,NULL,0 UNION ALL
select 514,'III час ФВС - модул спорт','III час ФВС - модул спорт',0,NULL,NULL,NULL,0 UNION ALL
select 515,'III час ФВС - модул спорт (баскетбол)','III час ФВС - модул спорт (баскетбол)',0,NULL,NULL,NULL,0 UNION ALL
select 516,'III час ФВС - модул спорт (плуване)','III час ФВС - модул спорт (плуване)',0,NULL,NULL,NULL,0 UNION ALL
select 518,'III ЧЕ - Френски език','III ЧЕ - Френски език',0,NULL,NULL,NULL,0 UNION ALL
select 519,'îîîî','îîîî',0,NULL,NULL,NULL,0 UNION ALL
select 520,'IIIЧЕ - Руски език','IIIЧЕ - Руски език',0,NULL,NULL,NULL,0 UNION ALL
select 522,'Internet - и WEB - дизайн програмиране','Internet - и WEB - дизайн програмиране',0,NULL,NULL,NULL,0 UNION ALL
select 523,'Internet- и WEB- дизайн програмиране','Internet- и WEB- дизайн програмиране',0,NULL,NULL,NULL,0 UNION ALL
select 526,'IT- Математика','IT- Математика',1,NULL,NULL,NULL,0 UNION ALL
select 527,'IV гвардейски отряд','IV гвардейски отряд',1,NULL,NULL,NULL,0 UNION ALL
select 530,'IЧЕ-Английски език1','IЧЕ-Английски език1',0,NULL,NULL,NULL,0 UNION ALL
select 531,'IЧЕ-Английски език2','IЧЕ-Английски език2',0,NULL,NULL,NULL,0 UNION ALL
select 532,'IЧЕ-Арабски език1','IЧЕ-Арабски език1',0,NULL,NULL,NULL,0 UNION ALL
select 535,'IЧЕ-Френски език2','IЧЕ-Френски език2',0,NULL,NULL,NULL,0 UNION ALL
select 541,'Koнструиране, моделиране и технология на облеклото','Koнструиране, моделиране и технология на облеклото',0,NULL,NULL,NULL,0 UNION ALL
select 544,'Kамерна музика','Kамерна музика',0,NULL,NULL,NULL,0 UNION ALL
select 545,'Kитара','Kитара',0,NULL,NULL,NULL,0 UNION ALL
select 546,'Kитарен ансамбъл','Kитарен ансамбъл',0,NULL,NULL,NULL,0 UNION ALL
select 552,'Kонсултации с ученици','Kонсултации с ученици',0,NULL,NULL,NULL,0 UNION ALL
select 553,'Kофражни планове,определяне количествата на кофражните работи','Kофражни планове,определяне количествата на кофражните работи',1,NULL,NULL,NULL,0 UNION ALL
select 560,'M_изб - Литературата и другите изкуства','M_изб - Литературата и другите изкуства',1,NULL,NULL,NULL,0 UNION ALL
select 561,'M1 - Власт и институции','M1 - Власт и институции',1,NULL,NULL,NULL,0 UNION ALL
select 605,'Oбщa теория на статистиката','Oбщa теория на статистиката',0,NULL,NULL,NULL,0 UNION ALL
select 606,'Oбща теория на счетовoдството','Oбща теория на счетовoдството',0,NULL,NULL,NULL,0 UNION ALL
select 607,'Oбща теория на счетоводната отчетност','Oбща теория на счетоводната отчетност',0,NULL,NULL,NULL,0 UNION ALL
select 608,'Oбществени комуникации','Oбществени комуникации',0,NULL,NULL,NULL,0 UNION ALL
select 638,'Oсветителна техника','Oсветителна техника',0,NULL,NULL,NULL,0 UNION ALL
select 642,'Oснови на земеделието-','Oснови на земеделието-',0,NULL,NULL,NULL,0 UNION ALL
select 657,'PERLES','PERLES',0,NULL,NULL,NULL,0 UNION ALL
select 665,'Spelling bee','Spelling bee',0,NULL,NULL,NULL,0 UNION ALL
select 722,'Xореография1','Xореография1',0,NULL,NULL,NULL,0 UNION ALL
select 724,'А.11 Металообработване','А.11 Металообработване',0,NULL,NULL,NULL,0 UNION ALL
select 725,'А.12 Машинно чертане - II','А.12 Машинно чертане - II',0,NULL,NULL,NULL,0 UNION ALL
select 728,'А.3 Основи на техническата механика','А.3 Основи на техническата механика',0,NULL,NULL,NULL,0 UNION ALL
select 732,'А.7 Кoмуникации','А.7 Кoмуникации',0,NULL,NULL,NULL,0 UNION ALL
select 735,'А.8 Предприятие и пазарна среда','А.8 Предприятие и пазарна среда',0,NULL,NULL,NULL,0 UNION ALL
select 736,'А.9 Информационни технологии','А.9 Информационни технологии',0,NULL,NULL,NULL,0 UNION ALL
select 744,'А1-Машинно чертане','А1-Машинно чертане',0,NULL,NULL,NULL,0 UNION ALL
select 747,'А4-Осн.на ЕЕ-','А4-Осн.на ЕЕ-',0,NULL,NULL,NULL,0 UNION ALL
select 751,'А8-Предприятието и пазарна среда (маркетинг)','А8-Предприятието и пазарна среда (маркетинг)',0,NULL,NULL,NULL,0 UNION ALL
select 754,'АutoCAD','АutoCAD',0,NULL,NULL,NULL,0 UNION ALL
select 755,'АutoCAD в строителството','АutoCAD в строителството',0,NULL,NULL,NULL,0 UNION ALL
select 756,'Аанатомия','Аанатомия',0,NULL,NULL,NULL,0 UNION ALL
select 760,'Аварийна безопасност','Аварийна безопасност',1,NULL,NULL,NULL,0 UNION ALL
select 769,'Аварийна готовност','Аварийна готовност',1,NULL,NULL,NULL,0 UNION ALL
select 771,'Авиационен английски (пилоти)','Авиационен английски (пилоти)',0,NULL,NULL,NULL,0 UNION ALL
select 776,'Авиационна техника','Авиационна техника',1,NULL,NULL,NULL,0 UNION ALL
select 779,'Авиационни двигатели','Авиационни двигатели',0,NULL,NULL,NULL,0 UNION ALL
select 781,'Авиационни материали','Авиационни материали',1,NULL,NULL,NULL,0 UNION ALL
select 783,'Авиационни прибори и електронна автоматика','Авиационни прибори и електронна автоматика',0,NULL,NULL,NULL,0 UNION ALL
select 788,'Авио и корабомоделизъм','Авио и корабомоделизъм',0,NULL,NULL,NULL,0 UNION ALL
select 796,'Авионикс 2 (Радиотехническо оборудване и системи)','Авионикс 2 (Радиотехническо оборудване и системи)',0,NULL,NULL,NULL,0 UNION ALL
select 797,'Авионикс на тип самолет','Авионикс на тип самолет',0,NULL,NULL,NULL,0 UNION ALL
select 851,'Автоматизация и управление','Автоматизация и управление',1,NULL,NULL,NULL,0 UNION ALL
select 856,'Автоматизация и управление на произвоството','Автоматизация и управление на произвоството',0,NULL,NULL,NULL,0 UNION ALL
select 957,'Автомобили и кари','Автомобили и кари',1,NULL,NULL,NULL,0 UNION ALL
select 965,'Автомобилни газови уредби - МТ','Автомобилни газови уредби - МТ',1,NULL,NULL,NULL,0 UNION ALL
select 971,'Автомобилни превози','Автомобилни превози',1,NULL,NULL,NULL,0 UNION ALL
select 993,'Автотранспортна техника','Автотранспортна техника',1,NULL,NULL,NULL,0 UNION ALL
select 1429,'Аксиология','Аксиология',1,NULL,NULL,NULL,0 UNION ALL
select 1729,'Аналогова схемотехника','Аналогова схемотехника',1,NULL,NULL,NULL,0 UNION ALL
select 2329,'Английски език за маркетинг и реклама','Английски език за маркетинг и реклама',1,NULL,NULL,NULL,0 UNION ALL
select 2359,'Английски език ЗП /Изобразително изкуство ЗИП','Английски език ЗП /Изобразително изкуство ЗИП',0,NULL,NULL,NULL,0 UNION ALL
select 3517,'Безопасност на движението по пътищата','Безопасност на движението по пътищата',1,NULL,NULL,NULL,0 UNION ALL
select 4006,'Бизнес английски','Бизнес английски',1,NULL,NULL,NULL,0 UNION ALL
select 4049,'Бизнес комуникации','Бизнес комуникации',1,NULL,NULL,NULL,0 UNION ALL
select 4507,'Биология и геология','Биология и геология',0,NULL,NULL,NULL,0 UNION ALL
select 4726,'Биология и здравно образование/Човекът и природата','Биология и здравно образование/Човекът и природата',0,NULL,NULL,NULL,0 UNION ALL
select 5528,'Български език и литература V, VІ и VІІ/Човекът и природата VІ','Български език и литература V, VІ и VІІ/Човекът и природата VІ',1,NULL,NULL,NULL,0 UNION ALL
select 5531,'Български език и литература VІ / Български език и литература ФУЧ ІХ','Български език и литература VІ / Български език и литература ФУЧ ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 5532,'Български език и литература VІ / История и цивилизации ІХ','Български език и литература VІ / История и цивилизации ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 5607,'Български език и литература/Гражданско образование','Български език и литература/Гражданско образование',1,NULL,NULL,NULL,0 UNION ALL
select 5611,'Български език и литература І и ІІІ / Български език и литература ФУЧ ІХ и Х','Български език и литература І и ІІІ / Български език и литература ФУЧ ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 5612,'Български език и литература І и ІІІ / История и цивилизации ІХ и Х','Български език и литература І и ІІІ / История и цивилизации ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 5613,'Български език и литература І, VІ и VІІІ / Български език и литература ФУЧ ІХ и Х','Български език и литература І, VІ и VІІІ / Български език и литература ФУЧ ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 5619,'Български език и литература ІІ, ІV и VІІІ / История и цивилизации ІХ и Х','Български език и литература ІІ, ІV и VІІІ / История и цивилизации ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 5620,'Български език и литература ІІ, ІІІ и ІV','Български език и литература ІІ, ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 5621,'Български език и литература/Биология и здравно образование','Български език и литература/Биология и здравно образование',1,NULL,NULL,NULL,0 UNION ALL
select 5622,'Български език и литература ІІ, ІІІ, V и VІІ / Български език и литература ФУЧ VІІІ','Български език и литература ІІ, ІІІ, V и VІІ / Български език и литература ФУЧ VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 5623,'Български език и литература ІІІ и VІ / Български език и литература ФУЧ VІІІ и Х','Български език и литература ІІІ и VІ / Български език и литература ФУЧ VІІІ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 5625,'Български език и литература ІІІ, VІ и VІІІ / Български език и литература ФУЧ Х','Български език и литература ІІІ, VІ и VІІІ / Български език и литература ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 5684,'Български език и литература/Български език и литература ФУЧ','Български език и литература/Български език и литература ФУЧ',0,NULL,NULL,NULL,0 UNION ALL
select 5871,'Български езикУП','Български езикУП',0,NULL,NULL,NULL,0 UNION ALL
select 5877,'Български екзерсис - корепетиция','Български екзерсис - корепетиция',1,NULL,NULL,NULL,0 UNION ALL
select 5878,'Български екзерсис (корепетитор)','Български екзерсис (корепетитор)',0,NULL,NULL,NULL,0 UNION ALL
select 5880,'Български и литература+Човекът и обществото','Български и литература+Човекът и обществото',0,NULL,NULL,NULL,0 UNION ALL
select 5881,'Български и национални танци','Български и национални танци',0,NULL,NULL,NULL,0 UNION ALL
select 5903,'Български национални костюми','Български национални костюми',0,NULL,NULL,NULL,0 UNION ALL
select 6163,'Ветрени и хидросъоръжения и инсталации','Ветрени и хидросъоръжения и инсталации',1,NULL,NULL,NULL,0 UNION ALL
select 6408,'Власт и институции','Власт и институции',1,NULL,NULL,NULL,0 UNION ALL
select 6926,'Въведение в професията','Въведение в професията',1,NULL,NULL,NULL,0 UNION ALL
select 7041,'Възобновяеми енергийни източници','Възобновяеми енергийни източници',1,NULL,NULL,NULL,0 UNION ALL
select 7235,'Газови и отоплителни инсталации','Газови и отоплителни инсталации',1,NULL,NULL,NULL,0 UNION ALL
select 7244,'Газови инсталации за втечнени въглеводородни газове - МП','Газови инсталации за втечнени въглеводородни газове - МП',1,NULL,NULL,NULL,0 UNION ALL
select 7250,'Газови котли','Газови котли',1,NULL,NULL,NULL,0 UNION ALL
select 7251,'Газови котли - МП','Газови котли - МП',1,NULL,NULL,NULL,0 UNION ALL
select 7260,'Газови уреди','Газови уреди',1,NULL,NULL,NULL,0 UNION ALL
select 7289,'Газоразпределителни мрежи','Газоразпределителни мрежи',1,NULL,NULL,NULL,0 UNION ALL
select 7291,'Газорегулиращи пунктове - МП','Газорегулиращи пунктове - МП',1,NULL,NULL,NULL,0 UNION ALL
select 7559,'География и икономика VІ, VІІІ и ІХ/История и цивилизации Х','География и икономика VІ, VІІІ и ІХ/История и цивилизации Х',1,NULL,NULL,NULL,0 UNION ALL
select 7560,'География и икономика VІ, VІІІ, ІХ и Х','География и икономика VІ, VІІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 7667,'География и икономика/Гражданско образование','География и икономика/Гражданско образование',1,NULL,NULL,NULL,0 UNION ALL
select 7778,'География на регионите и света','География на регионите и света',1,NULL,NULL,NULL,0 UNION ALL
select 7930,'Геополитическа и обществена култура','Геополитическа и обществена култура',1,NULL,NULL,NULL,0 UNION ALL
select 7949,'Геотермални съоръжения и инсталации','Геотермални съоръжения и инсталации',1,NULL,NULL,NULL,0 UNION ALL
select 8356,'Градивни елементи','Градивни елементи',1,NULL,NULL,NULL,0 UNION ALL
select 8863,'Двигатели с вътрешно горене','Двигатели с вътрешно горене',1,NULL,NULL,NULL,0 UNION ALL
select 9314,'Декоративно рисуване','Декоративно рисуване',1,NULL,NULL,NULL,0 UNION ALL
select 9896,'Диагностика на автомобила','Диагностика на автомобила',1,NULL,NULL,NULL,0 UNION ALL
select 9897,'Диагностика на автотранспортна техника','Диагностика на автотранспортна техника',1,NULL,NULL,NULL,0 UNION ALL
select 10114,'Динамични процеси в ЕПС','Динамични процеси в ЕПС',1,NULL,NULL,NULL,0 UNION ALL
select 11102,'ДПЛР - Диагностична, консултативна и корективна дейност','ДПЛР - Диагностична, консултативна и корективна дейност',1,NULL,NULL,NULL,0 UNION ALL
select 11108,'ДПЛР - работа с ученик по конкретен случай','ДПЛР - работа с ученик по конкретен случай',1,NULL,NULL,NULL,0 UNION ALL
select 11109,'ДПЛР - Работа с ученици, родители и учители','ДПЛР - Работа с ученици, родители и учители',1,NULL,NULL,NULL,0 UNION ALL
select 11168,'Дрги дейности по обучение, социализация, възпитание и отглеждане на децата','Дрги дейности по обучение, социализация, възпитание и отглеждане на децата',1,NULL,NULL,NULL,0 UNION ALL
select 11169,'Дргидейности по обучение, социализация, възпитание и отглеждане на децата','Дргидейности по обучение, социализация, възпитание и отглеждане на децата',1,NULL,NULL,NULL,0 UNION ALL
select 11231,'Други дейности по обучение, социализация, възптание и отглеждане на децата','Други дейности по обучение, социализация, възптание и отглеждане на децата',1,NULL,NULL,NULL,0 UNION ALL
select 12388,'Европа, Азия и България','Европа, Азия и България',1,NULL,NULL,NULL,0 UNION ALL
select 12443,'Език и култура','Език и култура',1,NULL,NULL,NULL,0 UNION ALL
select 12487,'Езикови практики','Езикови практики',1,NULL,NULL,NULL,0 UNION ALL
select 12562,'Езикът чрез литературата','Езикът чрез литературата',1,NULL,NULL,NULL,0 UNION ALL
select 12931,'Експлоатация и диагностика на ЕПС','Експлоатация и диагностика на ЕПС',1,NULL,NULL,NULL,0 UNION ALL
select 13037,'Експлоатация и ремонт на топлотехн.съоръж.и инсталации','Експлоатация и ремонт на топлотехн.съоръж.и инсталации',1,NULL,NULL,NULL,0 UNION ALL
select 13066,'Експлоатация на автомобила','Експлоатация на автомобила',1,NULL,NULL,NULL,0 UNION ALL
select 13073,'Експлоатация на автотранспортна техника','Експлоатация на автотранспортна техника',1,NULL,NULL,NULL,0 UNION ALL
select 13077,'Експлоатация на автотранспортната техника','Експлоатация на автотранспортната техника',1,NULL,NULL,NULL,0 UNION ALL
select 13081,'Експлоатация на АТТ','Експлоатация на АТТ',1,NULL,NULL,NULL,0 UNION ALL
select 13083,'Експлоатация на газови инсталации и разпределителни мрежи','Експлоатация на газови инсталации и разпределителни мрежи',0,NULL,NULL,NULL,0 UNION ALL
select 13084,'Експлоатация на газови инсталации и разпределителни мрежи - МП','Експлоатация на газови инсталации и разпределителни мрежи - МП',1,NULL,NULL,NULL,0 UNION ALL
select 13089,'Експлоатация на ЕПС','Експлоатация на ЕПС',1,NULL,NULL,NULL,0 UNION ALL
select 13148,'Експлоатация на отоплителни инсталации - МП','Експлоатация на отоплителни инсталации - МП',1,NULL,NULL,NULL,0 UNION ALL
select 13507,'Електрически инсталации','Електрически инсталации',1,NULL,NULL,NULL,0 UNION ALL
select 13545,'Електрически машини','Електрически машини',1,NULL,NULL,NULL,0 UNION ALL
select 13553,'Електрически машини и апарати','Електрически машини и апарати',1,NULL,NULL,NULL,0 UNION ALL
select 13620,'Електроенергетика','Електроенергетика',1,NULL,NULL,NULL,0 UNION ALL
select 13638,'Електрозахранващи източници и зарядни станции','Електрозахранващи източници и зарядни станции',1,NULL,NULL,NULL,0 UNION ALL
select 13700,'Електроника','Електроника',1,NULL,NULL,NULL,0 UNION ALL
select 13770,'Електронни системи в автомобила','Електронни системи в автомобила',1,NULL,NULL,NULL,0 UNION ALL
select 13776,'Електронни системи в автотранспортната техника','Електронни системи в автотранспортната техника',1,NULL,NULL,NULL,0 UNION ALL
select 13779,'Електронни системи в АТТ','Електронни системи в АТТ',1,NULL,NULL,NULL,0 UNION ALL
select 13845,'Електрообзавеждане на автомобила','Електрообзавеждане на автомобила',1,NULL,NULL,NULL,0 UNION ALL
select 13852,'Електрообзавеждане на автотранспортна техника','Електрообзавеждане на автотранспортна техника',0,NULL,NULL,NULL,0 UNION ALL
select 13860,'Електрообзавеждане на ЕПС','Електрообзавеждане на ЕПС',1,NULL,NULL,NULL,0 UNION ALL
select 13878,'Електрообзавеждане на подемно - транспортна техника','Електрообзавеждане на подемно - транспортна техника',1,NULL,NULL,NULL,0 UNION ALL
select 13959,'Електротехника','Електротехника',1,NULL,NULL,NULL,0 UNION ALL
select 14048,'Електротехнически дейности, свързани с битови газови уреди - МП','Електротехнически дейности, свързани с битови газови уреди - МП',1,NULL,NULL,NULL,0 UNION ALL
select 14092,'Елементи и механизми на мехатронните системи','Елементи и механизми на мехатронните системи',1,NULL,NULL,NULL,0 UNION ALL
select 14148,'Енергийна ефективност','Енергийна ефективност',1,NULL,NULL,NULL,0 UNION ALL
select 14187,'Ерготерапия','Ерготерапия',1,NULL,NULL,NULL,0 UNION ALL
select 14485,'Заваряване','Заваряване',1,NULL,NULL,NULL,0 UNION ALL
select 15861,'Здравно и екологично образование','Здравно и екологично образование',1,NULL,NULL,NULL,0 UNION ALL
select 16844,'зрителна рехабилитация','зрителна рехабилитация',1,NULL,NULL,NULL,0 UNION ALL
select 17509,'Изграждане и ремонт на газови инсталации и разпределителни мрежи - МП','Изграждане и ремонт на газови инсталации и разпределителни мрежи - МП',1,NULL,NULL,NULL,0 UNION ALL
select 17777,'Изобразително изкуство/Биология и здравно образование/Изобразително изкуство ','Изобразително изкуство/Биология и здравно образование/Изобразително изкуство ',1,NULL,NULL,NULL,0 UNION ALL
select 18011,'Изобразително изкуство V и VІ / История и цивилизации VІІ','Изобразително изкуство V и VІ / История и цивилизации VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 18015,'Изобразително изкуство VІ / Изобразително изкуство ФУЧ ІХ','Изобразително изкуство VІ / Изобразително изкуство ФУЧ ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 18016,'Изобразително изкуство/История и цивилизации','Изобразително изкуство/ История и цивилизации ',1,NULL,NULL,NULL,0 UNION ALL
select 18017,'Изобразително изкуство VІ, VІІІ и ІХ/География и икономика Х','Изобразително изкуство VІ, VІІІ и ІХ/География и икономика Х',1,NULL,NULL,NULL,0 UNION ALL
select 18090,'Изобразително изкуство І / История и цивилизации VІ, VІІІ, ІХ и Х','Изобразително изкуство І / История и цивилизации VІ, VІІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 18091,'Изобразително изкуство І и ІІІ / Изобразително изкуство ФУЧ ІХ / История и цивилизации Х','Изобразително изкуство І и ІІІ / Изобразително изкуство ФУЧ ІХ / История и цивилизации Х',1,NULL,NULL,NULL,0 UNION ALL
select 18092,'Изобразително изкуство І и ІІІ/Изобразително изкуство ФУЧ ІХ/География и икономика Х','Изобразително изкуство І и ІІІ/Изобразително изкуство ФУЧ ІХ/География и икономика Х',1,NULL,NULL,NULL,0 UNION ALL
select 18097,'Изобразително изкуство/Биология и здравно образование/ Изобразително изкуство ФУЧ/Изобразително изкуство','Изобразително изкуство/Биология и здравно образование/ Изобразително изкуство ФУЧ/Изобразително изкуство',1,NULL,NULL,NULL,0 UNION ALL
select 18102,'Изобразително изкуство ІІ и ІV / Биология и здравно образование ФУЧ VІІІ / Биология и здравно образование ІХ и Х','Изобразително изкуство ІІ и ІV / Биология и здравно образование ФУЧ VІІІ / Биология и здравно образование ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 18103,'Изобразително изкуство/Музика','Изобразително изкуство/Музика',1,NULL,NULL,NULL,0 UNION ALL
select 18104,'Изобразително изкуство ІІ, ІІІ и VІІ','Изобразително изкуство ІІ, ІІІ и VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 18105,'Изобразително изкуство ІІ, ІІІ и VІІ / Изобразително изкуство ФУЧ VІІІ','Изобразително изкуство ІІ, ІІІ и VІІ / Изобразително изкуство ФУЧ VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 18106,'Изобразително изкуство ІІ, ІІІ и ІV','Изобразително изкуство ІІ, ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 18111,'Изобразително изкуство ІІІ и VІ / Биология и здравно образование Х','Изобразително изкуство ІІІ и VІ / Биология и здравно образование Х',1,NULL,NULL,NULL,0 UNION ALL
select 18112,'Изобразително изкуство ІІІ и VІІІ/Изобразително изкуство ФУЧ ІІ и VІІ','Изобразително изкуство ІІІ и VІІІ/Изобразително изкуство ФУЧ ІІ и VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 18113,'Изобразително изкуство / Изобразително изкуство ФУЧ','Изобразително изкуство / Изобразително изкуство ФУЧ ',1,NULL,NULL,NULL,0 UNION ALL
select 18118,'Изобразително изкуство ІІІ, VІ и VІІІ','Изобразително изкуство ІІІ, VІ и VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 18146,'Изобразително изкуство ФУЧ','Изобразително изкуство ФУЧ',0,NULL,NULL,NULL,0 UNION ALL
select 18148,'Изобразително изкуство ФУЧ ІІ / Човекът и обществото ІІІ и ІV','Изобразително изкуство ФУЧ ІІ / Човекът и обществото ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 18149,'Изобразително изкуство ФУЧ ІІ и ІV / Изобразително изкуство ІІІ','Изобразително изкуство ФУЧ ІІ и ІV / Изобразително изкуство ІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 18789,'Изящно и приложно изкуство-кр.','Изящно и приложно изкуство-кр.',1,NULL,NULL,NULL,0 UNION ALL
select 19887,'Инсталиране и ремонт на отоплителни инсталации - МП','Инсталиране и ремонт на отоплителни инсталации - МП',1,NULL,NULL,NULL,0 UNION ALL
select 20043,'Интеркултурно образование','Интеркултурно образование',1,NULL,NULL,NULL,0 UNION ALL
select 20428,'Информационни системи в транспорта','Информационни системи в транспорта',1,NULL,NULL,NULL,0 UNION ALL
select 20625,'Информационни технологии VІ, VІІІ и ІХ/Информационни техналагии ФУЧ Х','Информационни технологии VІ, VІІІ и ІХ/Информационни техналагии ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 20646,'Информационни технологии в професията','Информационни технологии в професията',1,NULL,NULL,NULL,0 UNION ALL
select 21230,'История и цивилизации VІ, VІІІ, ІХ и Х','История и цивилизации VІ, VІІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 21521,'История на идеите','История на идеите',1,NULL,NULL,NULL,0 UNION ALL
select 22304,'Картография','Картография',1,NULL,NULL,NULL,0 UNION ALL
select 22633,'Климатична, вентилационна и отоплителна техника','Климатична, вентилационна и отоплителна техника',1,NULL,NULL,NULL,0 UNION ALL
select 24138,'Компютърни системи','Компютърни системи',1,NULL,NULL,NULL,0 UNION ALL
select 24189,'Компютърно моделиране ІV / Информационни технологии VІІІ, ІХ и Х','Компютърно моделиране ІV / Информационни технологии VІІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 24191,'Компютърно моделиране ІІІ / Информационни технологии VІ, VІІІ и Х','Компютърно моделиране ІІІ / Информационни технологии VІ, VІІІ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 24193,'Компютърно моделиране ІІІ/Информационни технологии VІ и VІІІ/Информационни технологии ФУЧ Х','Компютърно моделиране ІІІ/Информационни технологии VІ и VІІІ/Информационни технологии ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 24213,'Информационни технологии/Информационни технологии ФУЧ','Информационни технологии/Информационни технологии ФУЧ ',1,NULL,NULL,NULL,0 UNION ALL
select 24815,'Контрол и регулиране на налягане и температура','Контрол и регулиране на налягане и температура',0,NULL,NULL,NULL,0 UNION ALL
select 24968,'корепетитор','корепетитор',1,NULL,NULL,NULL,0 UNION ALL
select 26025,'Култура и духовност','Култура и духовност',1,NULL,NULL,NULL,0 UNION ALL
select 26047,'Култура и междукултурно общуване','Култура и междукултурно общуване',1,NULL,NULL,NULL,0 UNION ALL
select 26100,'Култура на мисленето','Култура на мисленето',1,NULL,NULL,NULL,0 UNION ALL
select 26423,'Л Ф К','Л Ф К',1,NULL,NULL,NULL,0 UNION ALL
select 26424,'Л Ф К ІІ, ІV, VІІІ и Х','Л Ф К ІІ, ІV, VІІІ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 26425,'Л Ф К ІІ, ІІІ и ІV','Л Ф К ІІ, ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 26426,'Л Ф К ІІ, ІІІ, VІІ и VІІІ','Л Ф К ІІ, ІІІ, VІІ и VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 26427,'Л Ф К ІІІ, VІ и VІІІ','Л Ф К ІІІ, VІ и VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 26733,'Лабораторна - електрически измервания на ЕПС','Лабораторна - електрически измервания на ЕПС',1,NULL,NULL,NULL,0 UNION ALL
select 26734,'Лабораторна - електрически машини','Лабораторна - електрически машини',1,NULL,NULL,NULL,0 UNION ALL
select 27672,'Логопедична подкрепа','Логопедична подкрепа',1,NULL,NULL,NULL,0 UNION ALL
select 27676,'Логопедична помощ','Логопедична помощ',1,NULL,NULL,NULL,0 UNION ALL
select 27732,'Логоритмика','Логоритмика',1,NULL,NULL,NULL,0 UNION ALL
select 28011,'М11 Основи на аеродинамиката','М11 Основи на аеродинамиката',1,NULL,NULL,NULL,0 UNION ALL
select 28013,'М12 Авиационна техника','М12 Авиационна техника',1,NULL,NULL,NULL,0 UNION ALL
select 28014,'М13 Авиационна нормативна уредба','М13 Авиационна нормативна уредба',1,NULL,NULL,NULL,0 UNION ALL
select 28015,'М14 Материали и принадлежности I','М14 Материали и принадлежности I',1,NULL,NULL,NULL,0 UNION ALL
select 28016,'М14 Материали и принадлежности II','М14 Материали и принадлежности II',1,NULL,NULL,NULL,0 UNION ALL
select 28019,'М15 Цифрова техника и електронно-приборни системи','М15 Цифрова техника и електронно-приборни системи',1,NULL,NULL,NULL,0 UNION ALL
select 28020,'М16 Газотурбинни двигатели','М16 Газотурбинни двигатели',1,NULL,NULL,NULL,0 UNION ALL
select 28021,'М17 Бутални двигатели','М17 Бутални двигатели',1,NULL,NULL,NULL,0 UNION ALL
select 28022,'М18 Техническо обслужване','М18 Техническо обслужване',1,NULL,NULL,NULL,0 UNION ALL
select 28171,'М6 Инженерна графика','М6 Инженерна графика',1,NULL,NULL,NULL,0 UNION ALL
select 28172,'М7 Техническа механика и термодинамика','М7 Техническа механика и термодинамика',1,NULL,NULL,NULL,0 UNION ALL
select 28174,'М8 Основи на електротехниката','М8 Основи на електротехниката',1,NULL,NULL,NULL,0 UNION ALL
select 28176,'М9 Основи на електрониката','М9 Основи на електрониката',1,NULL,NULL,NULL,0 UNION ALL
select 28375,'Маркетинг','Маркетинг',1,NULL,NULL,NULL,0 UNION ALL
select 28415,'Маркетинг и реклама','Маркетинг и реклама',1,NULL,NULL,NULL,0 UNION ALL
select 28719,'Алгебра','Алгебра',1,NULL,NULL,NULL,0 UNION ALL
select 29020,'Математика І','Математика І',0,NULL,NULL,NULL,0 UNION ALL
select 29024,'Математика І / Информационни технологии VІ, VІІІ, ІХ и Х','Математика І / Информационни технологии VІ, VІІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 29025,'Математика І / Компютърно моделиране ІІІ / Информационни технологии ІХ и Х','Математика І / Компютърно моделиране ІІІ / Информационни технологии ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 29033,'Математика І, VІ, VІІІ и ІХ / Математика ФУЧ Х','Математика І, VІ, VІІІ и ІХ / Математика ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 29034,'Математика І, ІІІ и ІХ / Математика ФУЧ Х','Математика І, ІІІ и ІХ / Математика ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 29038,'Математика /География и икономика/Математика ФУЧ ','Математика /География и икономика/Математика  ФУЧ ',1,NULL,NULL,NULL,0 UNION ALL
select 29044,'Математика/Биология и здравно образование ИУЧ','Математика/Биология и здравно образование ИУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 29045,'Математика ІІ, ІV и VІІІ /Математика ФУЧ ІІІ','Математика ІІ, ІV и VІІІ /Математика ФУЧ ІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 29046,'Математика ІІ, ІV, VІІІ / География и икономика ІХ и Х','Математика ІІ, ІV, VІІІ / География и икономика ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 29047,'Математика ІІ, ІІІ и ІV/Математика ФУЧ VІІІ','Математика ІІ, ІІІ и ІV/Математика ФУЧ VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 29048,'Математика ІІ, ІІІ, V и VІІ / Математика ФУЧ VІІІ','Математика ІІ, ІІІ, V и VІІ / Математика ФУЧ VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 29050,'Математика ІІІ, VІ и VІІІ/Математика ФУЧ Х','Математика ІІІ, VІ и VІІІ/Математика ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 29095,'Математика ФУЧ ІІ и VІІІ / Математика ІІІ и ІV','Математика ФУЧ ІІ и VІІІ / Математика ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 29096,'Математика ФУЧ ІІ и ІІІ/ Математика ІV','Математика ФУЧ ІІ и ІІІ/ Математика ІV',1,NULL,NULL,NULL,0 UNION ALL
select 29097,'Математика ФУЧ ІІІ и VІІІ / Математика VІ','Математика ФУЧ ІІІ и VІІІ / Математика VІ',1,NULL,NULL,NULL,0 UNION ALL
select 29177,'Математика/Математика ФУЧ','Математика/Математика ФУЧ',0,NULL,NULL,NULL,0 UNION ALL
select 29563,'Материалознание','Материалознание',1,NULL,NULL,NULL,0 UNION ALL
select 30080,'Машинно чертане','Машинно чертане',1,NULL,NULL,NULL,0 UNION ALL
select 30808,'Мехатронни системи в автомобила','Мехатронни системи в автомобила',1,NULL,NULL,NULL,0 UNION ALL
select 30810,'Мехатронни системи в автотранспортната техника','Мехатронни системи в автотранспортната техника',1,NULL,NULL,NULL,0 UNION ALL
select 30941,'Микропроцесорна техника','Микропроцесорна техника',1,NULL,NULL,NULL,0 UNION ALL
select 33604,'Монтаж и ремонт на газови инсталации и разпределителни мрежи','Монтаж и ремонт на газови инсталации и разпределителни мрежи',1,NULL,NULL,NULL,0 UNION ALL
select 33658,'Монтаж, експлоатация и ремонт на асансьори и подемници','Монтаж, експлоатация и ремонт на асансьори и подемници',1,NULL,NULL,NULL,0 UNION ALL
select 33736,'МП11 Основи на аеродинамиката','МП11 Основи на аеродинамиката',1,NULL,NULL,NULL,0 UNION ALL
select 33737,'МП18 Техническо обслужване','МП18 Техническо обслужване',1,NULL,NULL,NULL,0 UNION ALL
select 34119,'Музика VІ / География и икономика ІХ','Музика VІ / География и икономика ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 34120,'Музика VІ, VІІІ и Х/История и цивилизации ІХ','Музика VІ, VІІІ и Х/История и цивилизации ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 34121,'Музика VІ/Музика ФУЧ ІХ','Музика VІ/Музика ФУЧ ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 34122,'Музика/Музика ФУЧ','Музика/Музика ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 34222,'Музика / Изобразително изкуство ФУЧ/Музика ФУЧ ','Музика / Изобразително изкуство ФУЧ/Музика ФУЧ ',1,NULL,NULL,NULL,0 UNION ALL
select 34225,'Музика І, VІ / Музика ФУЧ VІІІ / История и цивилизации ІХ и Х','Музика І, VІ / Музика ФУЧ VІІІ / История и цивилизации ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 34226,'Музика/Музика ФУЧ ','Музика/Музика ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 34231,'Музика І/Музика ФУЧ ІІІ, ІХ и Х','Музика І/Музика ФУЧ ІІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 34233,'Музика ІV и VІІ / Музика ФУЧ ІХ','Музика ІV и VІІ / Музика ФУЧ ІХ',0,NULL,NULL,NULL,0 UNION ALL
select 34236,'Музика ФУЧ/Музика/Музика ФУЧ','Музика ФУЧ/Музика/Музика ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 34237,'Музика ІІ / Музика ФУЧ ІІІ и ІV','Музика ІІ / Музика ФУЧ ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 34238,'Музика/Музика ФУЧ','Музика/Музика ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 34239,'Музика ІІ и ІІІ/Музика ФУЧ ІV','Музика ІІ и ІІІ/Музика ФУЧ ІV',1,NULL,NULL,NULL,0 UNION ALL
select 34242,'Музика ІІ, ІІІ и ІV / Музика ФУЧ VІІІ','Музика ІІ, ІІІ и ІV / Музика ФУЧ VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 34243,'Музика ІІ, ІІІ, V и VІІ / Музика ФУЧ VІІІ','Музика ІІ, ІІІ, V и VІІ / Музика ФУЧ VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 34248,'Музика/Музика ФУЧ ','Музика/Музика ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 34251,'Музика ІІІ / Музика ФУЧ VІІІ / История и цивилизации VІ и Х','Музика ІІІ / Музика ФУЧ VІІІ / История и цивилизации VІ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 34252,'Музика ІІІ, V и VІІ/Музика ФУЧ ІІ и VІІІ','Музика ІІІ, V и VІІ/Музика ФУЧ ІІ и VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 34253,'Музика ІІІ, VІ и Х/Музика ФУЧ VІІІ','Музика ІІІ, VІ и Х/Музика ФУЧ VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 34320,'Музика ФУЧ','Музика ФУЧ',0,NULL,NULL,NULL,0 UNION ALL
select 34321,'Музика ФУЧ VІ, VІІІ, ІХ и Х','Музика ФУЧ VІ, VІІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 34323,'Музика ФУЧ ІІ, ІІІ и ІV','Музика ФУЧ ІІ, ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 34324,'Музика ФУЧ ІІ, ІІІ, и VІІ / Музика V и VІІІ','Музика ФУЧ ІІ, ІІІ, и VІІ / Музика V и VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 34326,'Музика ФУЧ ІІІ / Музика VІ','Музика ФУЧ ІІІ / Музика VІ',1,NULL,NULL,NULL,0 UNION ALL
select 34327,'Музика ФУЧ ІІІ и VІІІ/История и цивилизации VІ и Х','Музика ФУЧ ІІІ и VІІІ/История и цивилизации VІ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 34328,'Музика ФУЧ ІІІ/Български език и литература ІІІ/Музика VІ','Музика ФУЧ ІІІ/Български език и литература ІІІ/Музика VІ',1,NULL,NULL,NULL,0 UNION ALL
select 34382,'Музика/Биология и здравно образование','Музика/Биология и здравно образование',0,NULL,NULL,NULL,0 UNION ALL
select 34473,'Музика/Човекът и обществото','Музика/Човекът и обществото',1,NULL,NULL,NULL,0 UNION ALL
select 34903,'музикотерапия','музикотерапия',1,NULL,NULL,NULL,0 UNION ALL
select 35090,'народно пеене','народно пеене',1,NULL,NULL,NULL,0 UNION ALL
select 35857,'Обазователно направление „Музика“','Обазователно направление „Музика“',1,NULL,NULL,NULL,0 UNION ALL
select 35858,'Обазователно направление „Физическа култура“','Обазователно направление „Физическа култура“',1,NULL,NULL,NULL,0 UNION ALL
select 36028,'Образоателно направление „Музика“','Образоателно направление „Музика“',1,NULL,NULL,NULL,0 UNION ALL
select 36051,'Образователна дейност в ДГ','Образователна дейност в ДГ',1,NULL,NULL,NULL,0 UNION ALL
select 36061,'Образователни дейности в ДГ','Образователни дейности в ДГ',1,NULL,NULL,NULL,0 UNION ALL
select 36243,'Образователно направление „Физиеска култура“','Образователно направление „Физиеска култура“',1,NULL,NULL,NULL,0 UNION ALL
select 36281,'Обраователно направление „Музика“','Обраователно направление „Музика“',0,NULL,NULL,NULL,0 UNION ALL
select 36530,'Обща теория на счетоводната отчетност','Обща теория на счетоводната отчетност',1,NULL,NULL,NULL,0 UNION ALL
select 37068,'Околен свят І / Човекът и обществото ІІІ / История и цивилизации ІХ и Х','Околен свят І / Човекът и обществото ІІІ / История и цивилизации ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 37069,'Околен свят І / Човекът и природата VІ / Биология и здравно образование VІІІ, ІХ и Х','Околен свят І / Човекът и природата VІ / Биология и здравно образование VІІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 37073,'Околен свят ІІ / Човекът и обществото ІV / История и цивилизации ФУЧ VІІІ / История и цивилизации ІХ и Х','Околен свят ІІ / Човекът и обществото ІV / История и цивилизации ФУЧ VІІІ / История и цивилизации ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 37074,'Околен свят ІІ / Човекът и обществото ІІІ / География и икономика V, VІІ и VІІІ','Околен свят ІІ / Човекът и обществото ІІІ / География и икономика V, VІІ и VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 37075,'Околен Свят ІІ / Човекът и природата ІV / Биология и здравно образование ФУЧ VІІІ / Биология и здравно образование ІХ и Х','Околен Свят ІІ / Човекът и природата ІV / Биология и здравно образование ФУЧ VІІІ / Биология и здравно образование ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 37077,'Околен свят ІІ / Човекът и природата ІІІ и ІV','Околен свят ІІ / Човекът и природата ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 37078,'Околен свят ІІ /Човекът и обществото ІІІ / География и икономика V и VІІ','Околен свят ІІ /Човекът и обществото ІІІ / География и икономика V и VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 37095,'Околен свят ФУЧ/Човекът и природата','Околен свят ФУЧ/Човекът и природата',1,NULL,NULL,NULL,0 UNION ALL
select 37096,'Околен свят ФУЧ І / Човекът и природата ІІІ / География и икономика ІХ и Х','Околен свят ФУЧ І / Човекът и природата ІІІ / География и икономика ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 37098,'Околен свят ФУЧ ІІ / Човекът и обществото ІІІ и ІV','Околен свят ФУЧ ІІ / Човекът и обществото ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 37099,'Околен свят ФУЧ ІІ / Човекът и природата ФУЧ ІІІ / Човекът иприродата ІV','Околен свят ФУЧ ІІ / Човекът и природата ФУЧ ІІІ / Човекът иприродата ІV',1,NULL,NULL,NULL,0 UNION ALL
select 37100,'Човекът и природата/История и цивилизации','Човекът и природата/История и цивилизации',1,NULL,NULL,NULL,0 UNION ALL
select 37101,'Компютърно моделиране/Компютърно моделиране и информационни технологии/Информационни технологии','Компютърно моделиране/Компютърно моделиране и информационни технологии/Информационни технологии',1,NULL,NULL,NULL,0 UNION ALL
select 37124,'околен свят/ човекът и природата','околен свят/ човекът и природата',1,NULL,NULL,NULL,0 UNION ALL
select 37503,'Опазване на човека и природата','Опазване на човека и природата',0,NULL,NULL,NULL,0 UNION ALL
select 37763,'Организационна психология','Организационна психология',1,NULL,NULL,NULL,0 UNION ALL
select 37997,'Организация и управление на транспортно предприятие','Организация и управление на транспортно предприятие',1,NULL,NULL,NULL,0 UNION ALL
select 37998,'Организация и управление на транспортното предприятие','Организация и управление на транспортното предприятие',1,NULL,NULL,NULL,0 UNION ALL
select 39130,'Осветителна техника','Осветителна техника',1,NULL,NULL,NULL,0 UNION ALL
select 39196,'Основи на автоматизацията','Основи на автоматизацията',1,NULL,NULL,NULL,0 UNION ALL
select 39204,'Основи на автоматизацията-МТ','Основи на автоматизацията-МТ',1,NULL,NULL,NULL,0 UNION ALL
select 40299,'Пазарна икономика','Пазарна икономика',1,NULL,NULL,NULL,0 UNION ALL
select 40559,'Педагогическа психология','Педагогическа психология',1,NULL,NULL,NULL,0 UNION ALL
select 40579,'Пеене','Пеене',0,NULL,NULL,NULL,0 UNION ALL
select 40766,'Писмено общуване','Писмено общуване',1,NULL,NULL,NULL,0 UNION ALL
select 41177,'Подемно - транспортна техника','Подемно - транспортна техника',1,NULL,NULL,NULL,0 UNION ALL
select 42134,'Практическо обучение в реална paботнa среда','Практическо обучение в реална paботнa среда',1,NULL,NULL,NULL,0 UNION ALL
select 42138,'Практическо обучение в реална работна среда','Практическо обучение в реална работна среда',1,NULL,NULL,NULL,0 UNION ALL
select 42283,'Предприемаческа инициатива','Предприемаческа инициатива',1,NULL,NULL,NULL,0 UNION ALL
select 42426,'Предприемачество и кариерно развитие','Предприемачество и кариерно развитие',1,NULL,NULL,NULL,0 UNION ALL
select 42497,'Предприемачество/Икономика','Предприемачество/Икономика',1,NULL,NULL,NULL,0 UNION ALL
select 42732,'Прилагане на специфични техники на историческото познание','Прилагане на специфични техники на историческото познание',1,NULL,NULL,NULL,0 UNION ALL
select 42761,'Приложен софтуер','Приложен софтуер',1,NULL,NULL,NULL,0 UNION ALL
select 42888,'Приложни изкуства','Приложни изкуства',1,NULL,NULL,NULL,0 UNION ALL
select 43071,'Природа','Природа',1,NULL,NULL,NULL,0 UNION ALL
select 43199,'Природноресурсен потенциал. Устойчиво развитие','Природноресурсен потенциал. Устойчиво развитие',1,NULL,NULL,NULL,0 UNION ALL
select 44069,'Проектиране на асансьори','Проектиране на асансьори',1,NULL,NULL,NULL,0 UNION ALL
select 44072,'Проектиране на битова газификация ','Проектиране на битова газификация - МТ',1,NULL,NULL,NULL,0 UNION ALL
select 44159,'Проектиране на електрическите системи на автомобила','Проектиране на електрическите системи на автомобила',1,NULL,NULL,NULL,0 UNION ALL
select 44230,'Проектиране на инсталации за използване на енергия от ВЕИ','Проектиране на инсталации за използване на енергия от ВЕИ',1,NULL,NULL,NULL,0 UNION ALL
select 44305,'Проектиране на системи от ЕПС','Проектиране на системи от ЕПС',1,NULL,NULL,NULL,0 UNION ALL
select 44327,'Проектиране на топлотехнич.съоръж.и инсталации','Проектиране на топлотехнич.съоръж.и инсталации',1,NULL,NULL,NULL,0 UNION ALL
select 44553,'Производствена практика','Производствена практика',1,NULL,NULL,NULL,0 UNION ALL
select 45119,'Промишлена електроника','Промишлена електроника',1,NULL,NULL,NULL,0 UNION ALL
select 45136,'Промишлени газови инсталации - МП','Промишлени газови инсталации - МП',1,NULL,NULL,NULL,0 UNION ALL
select 45145,'Промишлени котли','Промишлени котли',1,NULL,NULL,NULL,0 UNION ALL
select 45391,'Процеси и машини в шевната промишленост','Процеси и машини в шевната промишленост',1,NULL,NULL,NULL,0 UNION ALL
select 45505,'Психологическа подкрепа','Психологическа подкрепа',1,NULL,NULL,NULL,0 UNION ALL
select 45526,'Психологическо консултиране','Психологическо консултиране',1,NULL,NULL,NULL,0 UNION ALL
select 45587,'Психология на личността','Психология на личността',0,NULL,NULL,NULL,0 UNION ALL
select 46464,'Работа с ученици и родители','Работа с ученици и родители',1,NULL,NULL,NULL,0 UNION ALL
select 47161,'Ремонт на съоръжения и инсталации за производство на енергия от ВЕИ','Ремонт на съоръжения и инсталации за производство на енергия от ВЕИ',1,NULL,NULL,NULL,0 UNION ALL
select 47164,'Ремонт на топлотехнически съоръжения и инсталации','Ремонт на топлотехнически съоръжения и инсталации',1,NULL,NULL,NULL,0 UNION ALL
select 47261,'Ресурсно подпомагане','Ресурсно подпомагане',1,NULL,NULL,NULL,0 UNION ALL
select 47304,'Рехабилитация','Рехабилитация',1,NULL,NULL,NULL,0 UNION ALL
select 47321,'Рехабилитация на слуха и говора','Рехабилитация на слуха и говора',1,NULL,NULL,NULL,0 UNION ALL
select 47325,'Рехабилитация при физически увреждания','Рехабилитация при физически увреждания',0,NULL,NULL,NULL,0 UNION ALL
select 47389,'Рисуване','Рисуване',1,NULL,NULL,NULL,0 UNION ALL
select 47807,'Родинознание/Човекът и природата','Родинознание/Човекът и природата',1,NULL,NULL,NULL,0 UNION ALL
select 47817,'Родолюбие','Родолюбие',1,NULL,NULL,NULL,0 UNION ALL
select 49286,'Светът и моята личност','Светът и моята личност',1,NULL,NULL,NULL,0 UNION ALL
select 49694,'Селско стопанство','Селско стопанство',0,NULL,NULL,NULL,0 UNION ALL
select 50241,'Системи за управление, контрол и защита','Системи за управление, контрол и защита',1,NULL,NULL,NULL,0 UNION ALL
select 50375,'Соларни електроцентрали - МП','Соларни електроцентрали - МП',1,NULL,NULL,NULL,0 UNION ALL
select 50379,'Соларни съоръжения и инсталации','Соларни съоръжения и инсталации',1,NULL,NULL,NULL,0 UNION ALL
select 50540,'Социална психология','Социална психология',1,NULL,NULL,NULL,0 UNION ALL
select 50740,'Спедиционна дейност','Спедиционна дейност',1,NULL,NULL,NULL,0 UNION ALL
select 51602,'Спортни дейности','Спортни дейности',1,NULL,NULL,NULL,0 UNION ALL
select 51743,'Спортни дейности (тенис на маса)','Спортни дейности (тенис на маса)',1,NULL,NULL,NULL,0 UNION ALL
select 52140,'Стартиране на собствен бизнес','Стартиране на собствен бизнес',1,NULL,NULL,NULL,0 UNION ALL
select 52413,'Стругарство','Стругарство',1,NULL,NULL,NULL,0 UNION ALL
select 52641,'Схемотехника','Схемотехника',1,NULL,NULL,NULL,0 UNION ALL
select 52839,'Счетоводство на предприятието','Счетоводство на предприятието',1,NULL,NULL,NULL,0 UNION ALL
select 53056,'Съвременно икономическо развитие','Съвременно икономическо развитие',1,NULL,NULL,NULL,0 UNION ALL
select 53120,'Съоражения и инсталации за производство на енергия от биомаса от отпадъци','Съоражения и инсталации за производство на енергия от биомаса от отпадъци',1,NULL,NULL,NULL,0 UNION ALL
select 53128,'Съоръжения и инсталации за произвадство на енергия от биомаса с растителен произход - МП','Съоръжения и инсталации за произвадство на енергия от биомаса с растителен произход - МП',1,NULL,NULL,NULL,0 UNION ALL
select 53129,'Съоръжения и инсталации за производство на енергия от биомаса','Съоръжения и инсталации за производство на енергия от биомаса',1,NULL,NULL,NULL,0 UNION ALL
select 53131,'Съоръжения и инсталации за производство на енергия от биомаса от отпадъци - МП','Съоръжения и инсталации за производство на енергия от биомаса от отпадъци - МП',1,NULL,NULL,NULL,0 UNION ALL
select 53382,'Танцово изкуство','Танцово изкуство',1,NULL,NULL,NULL,0 UNION ALL
select 53518,'Театрално изкуство','Театрално изкуство',1,NULL,NULL,NULL,0 UNION ALL
select 54015,'Термо- и хидродинамика','Термо- и хидродинамика',1,NULL,NULL,NULL,0 UNION ALL
select 54021,'Термодинамика и топлопренасяне','Термодинамика и топлопренасяне',1,NULL,NULL,NULL,0 UNION ALL
select 54347,'Техническа механика','Техническа механика',1,NULL,NULL,NULL,0 UNION ALL
select 54394,'Техническа термодинамика и топлопренасяне','Техническа термодинамика и топлопренасяне',1,NULL,NULL,NULL,0 UNION ALL
select 54443,'Техническо обслужване и ремонт на автомобила','Техническо обслужване и ремонт на автомобила',1,NULL,NULL,NULL,0 UNION ALL
select 54466,'Техническо чертане','Техническо чертане',1,NULL,NULL,NULL,0 UNION ALL
select 54530,'Техническо чертане и документиране','Техническо чертане и документиране',1,NULL,NULL,NULL,0 UNION ALL
select 54977,'Технологии и предприемачество VІ / История и цивилизации ІХ','Технологии и предприемачество VІ / История и цивилизации ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 54979,'Технологии и предприемачество VІ и VІІІ/География и икономика ІХ и Х','Технологии и предприемачество VІ и VІІІ/География и икономика ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 54989,'Технологии и предприемачество І и ІІІ / Биология и здравно образование ІХ и Х','Технологии и предприемачество І и ІІІ / Биология и здравно образование ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 54993,'Технологии и предприемачество І, VІ и VІІІ / География и икономика ІХ и Х','Технологии и предприемачество І, VІ и VІІІ / География и икономика ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 54995,'Технологии и предприемачество І,VІ и VІІІ/Технологии и предприемачество ФУЧ VІІІ/ География и икономика ІХ и Х','Технологии и предприемачество І,VІ и VІІІ/Технологии и предприемачество ФУЧ VІІІ/ География и икономика ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 54996,'Технологии и предприемачество ІІ / Компютърно моделиране ІІІ и ІV','Технологии и предприемачество ІІ / Компютърно моделиране ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 54997,'Технологии и предприемачество ІІ и ІV / География и икономика ФУЧ VІІІ / География и икономика ІХ и Х','Технологии и предприемачество ІІ и ІV / География и икономика ФУЧ VІІІ / География и икономика ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 54998,'Технологии и предприемачество ІІ и ІІІ / Човекът и природата V','Технологии и предприемачество ІІ и ІІІ / Човекът и природата V',1,NULL,NULL,NULL,0 UNION ALL
select 55000,'Технологии и предприемачество ФУЧ/Човекът и природата ','Технологии и предприемачество/Човекът и природата ',1,NULL,NULL,NULL,0 UNION ALL
select 55001,'Технологии и предприемачество/Човекът и природата','Технологии и предприемачество/Човекът и природата ',1,NULL,NULL,NULL,0 UNION ALL
select 55002,'Технологии и предприемачество ІІІ и VІ','Технологии и предприемачество ІІІ и VІ',1,NULL,NULL,NULL,0 UNION ALL
select 55003,'Технологии и предприемачество ІІІ/Български език и литература ІІІ/Технологии и предприемачество VІ','Технологии и предприемачество ІІІ/Български език и литература ІІІ/Технологии и предприемачество VІ',1,NULL,NULL,NULL,0 UNION ALL
select 55004,'Технологии и предприемачество ІІІ/Математика ІІІ/Технологии и предприемачество VІ','Технологии и предприемачество ІІІ/Математика ІІІ/Технологии и предприемачество VІ',1,NULL,NULL,NULL,0 UNION ALL
select 55008,'Технологии и предприемачество ФУЧ','Технологии и предприемачество ФУЧ',0,NULL,NULL,NULL,0 UNION ALL
select 55009,'Технологии и предприемачество / История и цивилизации','Технологии и предприемачество / История и цивилизации',1,NULL,NULL,NULL,0 UNION ALL
select 55011,'Технологии и предприемачество ФУЧ ІІ / Компютърно моделиране ІІІ и ІV','Технологии и предприемачество ФУЧ ІІ / Компютърно моделиране ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 55012,'Технологии и предприемачество ФУЧ ІІ / Технологии и предприемачество ІІІ и ІV','Технологии и предприемачество ФУЧ ІІ / Технологии и предприемачество ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 55013,'Технологии и предприемачество ФУЧ ІІ / Човекът и обществото ІІІ / История и цивилизации V и VІІ','Технологии и предприемачество ФУЧ ІІ / Човекът и обществото ІІІ / История и цивилизации V и VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 55854,'Технология на материалите','Технология на материалите',1,NULL,NULL,NULL,0 UNION ALL
select 56630,'Товароподемни механизми','Товароподемни механизми',1,NULL,NULL,NULL,0 UNION ALL
select 56654,'Токозахранващи устройства','Токозахранващи устройства',1,NULL,NULL,NULL,0 UNION ALL
select 56669,'Топлинни източници','Топлинни източници',1,NULL,NULL,NULL,0 UNION ALL
select 56685,'Топлотехнически измервания','Топлотехнически измервания',1,NULL,NULL,NULL,0 UNION ALL
select 56900,'Транспортна статистика','Транспортна статистика',1,NULL,NULL,NULL,0 UNION ALL
select 56911,'Транспортно и търговско право','Транспортно и търговско право',1,NULL,NULL,NULL,0 UNION ALL
select 57227,'Трудово обучение','Трудово обучение',0,NULL,NULL,NULL,0 UNION ALL
select 57228,'Трудово политехническо обучение','Трудово политехническо обучение',0,NULL,NULL,NULL,0 UNION ALL
select 57543,'Тягови и спирачни системи на електромобила','Тягови и спирачни системи на електромобила',1,NULL,NULL,NULL,0 UNION ALL
select 57900,'УП - Експлоатация на автотрнспортна техника','УП - Експлоатация на автотрнспортна техника',1,NULL,NULL,NULL,0 UNION ALL
select 57905,'УП - Електрозадвижване и електрообзавеждане на ЕПС','УП - Електрозадвижване и електрообзавеждане на ЕПС',1,NULL,NULL,NULL,0 UNION ALL
select 57923,'УП - Измервателна и диагностична техника','УП - Измервателна и диагностична техника',1,NULL,NULL,NULL,0 UNION ALL
select 58047,'Учебна Практика Обработка на материалите','УП - Обработка на материалите',1,NULL,NULL,NULL,0 UNION ALL
select 58053,'УП - Обслужване и ремонт на автотранспортна техника','УП - Обслужване и ремонт на автотранспортна техника',1,NULL,NULL,NULL,0 UNION ALL
select 58054,'УП - Обслужване и ремонт на ЕПС','УП - Обслужване и ремонт на ЕПС',1,NULL,NULL,NULL,0 UNION ALL
select 58096,'УП - Основи на електротехниката','УП - Основи на електротехниката',1,NULL,NULL,NULL,0 UNION ALL
select 58238,'УП - Технология на сглобяването и ремонта','УП - Технология на сглобяването и ремонта',1,NULL,NULL,NULL,0 UNION ALL
select 58252,'УП - Топлинни източници','УП - Топлинни източници',1,NULL,NULL,NULL,0 UNION ALL
select 58289,'УП - Хладилна техника','УП - Хладилна техника',1,NULL,NULL,NULL,0 UNION ALL
select 58804,'УП по диагностика на АТТ','УП по диагностика на АТТ',1,NULL,NULL,NULL,0 UNION ALL
select 58834,'УП по електротехника и градивни елементи','УП по електротехника и градивни елементи',1,NULL,NULL,NULL,0 UNION ALL
select 59638,'УП-Металообработване и основи на ТДМО','УП-Металообработване и основи на ТДМО',1,NULL,NULL,NULL,0 UNION ALL
select 59870,'Управление на асансьори и подемници','Управление на асансьори и подемници',1,NULL,NULL,NULL,0 UNION ALL
select 59882,'Управление на газоснабдително предприятие - МТ','Управление на газоснабдително предприятие - МТ',1,NULL,NULL,NULL,0 UNION ALL
select 60211,'УП-Технологии за изработване на инсталации','УП-Технологии за изработване на инсталации',1,NULL,NULL,NULL,0 UNION ALL
select 60353,'Устно общуване','Устно общуване',1,NULL,NULL,NULL,0 UNION ALL
select 60523,'Устройство на автомобила','Устройство на автомобила',1,NULL,NULL,NULL,0 UNION ALL
select 60530,'Устройство на асансьори и подемници','Устройство на асансьори и подемници',1,NULL,NULL,NULL,0 UNION ALL
select 60539,'Устройство на ЕПС','Устройство на ЕПС',1,NULL,NULL,NULL,0 UNION ALL
select 61367,'Уч.п-ка - диагностика чрез телеметрия','Уч.п-ка - диагностика чрез телеметрия',1,NULL,NULL,NULL,0 UNION ALL
select 61669,'Уч.пр. Счетоводна отчетност','Уч.пр. Счетоводна отчетност',1,NULL,NULL,NULL,0 UNION ALL
select 63065,'Учебна компания','Учебна компания',1,NULL,NULL,NULL,0 UNION ALL
select 63529,'Учебна практика - електромонтажна','Учебна практика - електромонтажна',1,NULL,NULL,NULL,0 UNION ALL
select 64079,'Учебна практика - по диагностика','Учебна практика - по диагностика',1,NULL,NULL,NULL,0 UNION ALL
select 64121,'Учебна практика - по специалността','Учебна практика - по специалността',1,NULL,NULL,NULL,0 UNION ALL
select 64335,'Учебна практика - специалността','Учебна практика - специалността',1,NULL,NULL,NULL,0 UNION ALL
select 64723,'Учебна практика Въведение в автосервизната дейност','Учебна практика Въведение в автосервизната дейност',1,NULL,NULL,NULL,0 UNION ALL
select 64760,'Учебна практика двигатели с вътрешно горене','Учебна практика двигатели с вътрешно горене',1,NULL,NULL,NULL,0 UNION ALL
select 64781,'Учебна практика Диагностика и ремонт на ЕПС','Учебна практика Диагностика и ремонт на ЕПС',1,NULL,NULL,NULL,0 UNION ALL
select 64782,'Учебна практика Диагностика на автомобила','Учебна практика Диагностика на автомобила',1,NULL,NULL,NULL,0 UNION ALL
select 64783,'Учебна практика Диагностика на автотранспортна техника','Учебна практика Диагностика на автотранспортна техника',1,NULL,NULL,NULL,0 UNION ALL
select 64785,'Учебна практика диагностика на АТТ','Учебна практика диагностика на АТТ',1,NULL,NULL,NULL,0 UNION ALL
select 64816,'Учебна практика Електрически измервания','Учебна практика Електрически измервания',1,NULL,NULL,NULL,0 UNION ALL
select 64827,'Учебна практика Електромонтажна','Учебна практика Електромонтажна',1,NULL,NULL,NULL,0 UNION ALL
select 64839,'Учебна практика Електротехника и градивни елементи','Учебна практика Електротехника и градивни елементи',1,NULL,NULL,NULL,0 UNION ALL
select 65020,'Учебна практика лабораторна','Учебна практика лабораторна',1,NULL,NULL,NULL,0 UNION ALL
select 65056,'Учебна практика Лабораторна по електрически машини','Учебна практика Лабораторна по електрически машини',1,NULL,NULL,NULL,0 UNION ALL
select 65062,'Учебна практика Лабораторна по специалността','Учебна практика Лабораторна по специалността',1,NULL,NULL,NULL,0 UNION ALL
select 65196,'Учебна практика Обработка на материали','Учебна практика Обработка на материали',1,NULL,NULL,NULL,0 UNION ALL
select 65212,'Учебна практика Обслужване и ремонт на автотранспортна техника','Учебна практика Обслужване и ремонт на автотранспортна техника',1,NULL,NULL,NULL,0 UNION ALL
select 65241,'Учебна практика Общопрофесионални умения','Учебна практика Общопрофесионални умения',1,NULL,NULL,NULL,0 UNION ALL
select 65408,'Учебна практика по АТТ','Учебна практика по АТТ',1,NULL,NULL,NULL,0 UNION ALL
select 65546,'Учебна практика по ДВГ','Учебна практика по ДВГ',1,NULL,NULL,NULL,0 UNION ALL
select 65584,'Учебна практика по Диагностика','Учебна практика по Диагностика',1,NULL,NULL,NULL,0 UNION ALL
select 65586,'Учебна практика по диагностика - АТТ','Учебна практика по диагностика - АТТ',0,NULL,NULL,NULL,0 UNION ALL
select 65590,'Учебна практика по диагностика на автомобила','Учебна практика по диагностика на автомобила',1,NULL,NULL,NULL,0 UNION ALL
select 65600,'Учебна практика по диагностика на мехатронните системи в АТТ','Учебна практика по диагностика на мехатронните системи в АТТ',1,NULL,NULL,NULL,0 UNION ALL
select 65653,'Учебна практика по електрически и електронни измервания','Учебна практика по електрически и електронни измервания',1,NULL,NULL,NULL,0 UNION ALL
select 65655,'Учебна практика по електрически и електронни системи в АТТ','Учебна практика по електрически и електронни системи в АТТ',1,NULL,NULL,NULL,0 UNION ALL
select 65688,'Учебна практика по електротехника и градивни елементи','Учебна практика по електротехника и градивни елементи',1,NULL,NULL,NULL,0 UNION ALL
select 66350,'Учебна практика по процеси и машини в шевната промишленост','Учебна практика по процеси и машини в шевната промишленост',1,NULL,NULL,NULL,0 UNION ALL
select 66500,'Учебна практика по специални измервания','Учебна практика по специални измервания',1,NULL,NULL,NULL,0 UNION ALL
select 66509,'Учебна практика по специалността','Учебна практика по специалността',1,NULL,NULL,NULL,0 UNION ALL
select 66721,'Учебна практика по схемотехника','Учебна практика по схемотехника',1,NULL,NULL,NULL,0 UNION ALL
select 67035,'Учебна практика по управление на МПС','Учебна практика по управление на МПС',1,NULL,NULL,NULL,0 UNION ALL
select 67517,'Учебна практика Схемотехника','Учебна практика Схемотехника',1,NULL,NULL,NULL,0 UNION ALL
select 67519,'Учебна практика Счетоводна отчетност','Учебна практика Счетоводна отчетност',1,NULL,NULL,NULL,0 UNION ALL
select 67546,'Учебна практика Техническо обслужване и ремонт на автомобила','Учебна практика Техническо обслужване и ремонт на автомобила',1,NULL,NULL,NULL,0 UNION ALL
select 67582,'Учебна практика Технология на сглобяването и ремонта','Учебна практика Технология на сглобяването и ремонта',1,NULL,NULL,NULL,0 UNION ALL
select 67606,'Учебна практика Топлотехнически измервания','Учебна практика Топлотехнически измервания',1,NULL,NULL,NULL,0 UNION ALL
select 67697,'Учебна практика Шлосерски операции','Учебна практика Шлосерски операции',1,NULL,NULL,NULL,0 UNION ALL
select 68146,'Учебна практика-обработка на материали','Учебна практика-обработка на материали',1,NULL,NULL,NULL,0 UNION ALL
select 70398,'Физическо възпитание и спорт','Физическо възпитание и спорт',1,NULL,NULL,NULL,0 UNION ALL
select 70729,'Физическо възпитание и спорт ІІ, ІV, VІІІ и Х','Физическо възпитание и спорт ІІ, ІV, VІІІ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 70730,'Физическо възпитание и спорт ІІ, ІІІ и ІV','Физическо възпитание и спорт ІІ, ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 70733,'Физическо възпитание и спорт ІІІ, VІ и VІІІ','Физическо възпитание и спорт ІІІ, VІ и VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 71063,'Физическо възпитание ІІ, ІІІ, VІІ и VІІІ','Физическо възпитание ІІ, ІІІ, VІІ и VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 71281,'Философия и ценности','Философия и ценности',1,NULL,NULL,NULL,0 UNION ALL
select 71760,'Фотоволтаични системи - МП','Фотоволтаични системи - МП',1,NULL,NULL,NULL,0 UNION ALL
select 72182,'Хибридни системи','Хибридни системи',1,NULL,NULL,NULL,0 UNION ALL
select 72394,'Хидравлика и хидравлични машини','Хидравлика и хидравлични машини',1,NULL,NULL,NULL,0 UNION ALL
select 72439,'Хидравлични и пневматични устройства','Хидравлични и пневматични устройства',1,NULL,NULL,NULL,0 UNION ALL
select 72451,'Хидравлични и пневматични устройства в АТТ','Хидравлични и пневматични устройства в АТТ',1,NULL,NULL,NULL,0 UNION ALL
select 72462,'Хидравлични машини','Хидравлични машини',1,NULL,NULL,NULL,0 UNION ALL
select 73025,'Хладилна техника','Хладилна техника',1,NULL,NULL,NULL,0 UNION ALL
select 73790,'Цифрова схемотехника','Цифрова схемотехника',1,NULL,NULL,NULL,0 UNION ALL
select 74315,'Човек и общество','Човек и общество',1,NULL,NULL,NULL,0 UNION ALL
select 74573,'Човекът и обществото ІІІ / География и икономика VІ и Х','Човекът и обществото ІІІ / География и икономика VІ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 74959,'Човекът и природата V и VІ / Биология и здравно образование VІІ','Човекът и природата V и VІ / Биология и здравно образование VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 74960,'Човекът и природата V и VІ / История и цивилизации VІІ','Човекът и природата V и VІ / История и цивилизации VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 74983,'Човекът и природата VІ / Биология и здравно образование VІІІ, ІХ и Х','Човекът и природата VІ / Биология и здравно образование VІІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 74984,'Човекът и природата VІ / Биология и здравно образование ІХ','Човекът и природата VІ / Биология и здравно образование ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 74988,'Човекът и природата VІ/Биология и здравно образование ІХ и Х/Биология и здравно образование ФУЧ VІІІ','Човекът и природата VІ/Биология и здравно образование ІХ и Х/Биология и здравно образование ФУЧ VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 75054,'Човекът и природата ІІІ и VІ / Биология и здравно образование Х','Човекът и природата ІІІ и VІ / Биология и здравно образование Х',1,NULL,NULL,NULL,0 UNION ALL
select 75553,'Чужд език - английски език','Чужд език - английски език',1,NULL,NULL,NULL,0 UNION ALL
select 75623,'Чужд език - немски език','Чужд език - немски език',1,NULL,NULL,NULL,0 UNION ALL
select 75657,'Чужд език - Руски език','Чужд език - Руски език',1,NULL,NULL,NULL,0 UNION ALL
select 75803,'Чужд език по професията - Английски','Чужд език по професията - Английски',1,NULL,NULL,NULL,0 UNION ALL
select 76212,'Шлосерски операции','Шлосерски операции',1,NULL,NULL,NULL,0 UNION ALL
select 76281,'Технологии и предприемачество ІІІ и ІV / Музика ФУЧ VІ и ІХ','Технологии и предприемачество ІІІ и ІV / Музика ФУЧ VІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 76328,'Музика/Музика ФУЧ','Музика/Музика ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 76388,'Човекът и природата/География и икономика/Гражданско образование','Човекът и природата/ География и икономика/Гражданско образование',1,NULL,NULL,NULL,0 UNION ALL
select 76398,'Околен свят ФУЧ','Околен свят ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 76485,'Компютърно моделиране/Компютърно моделиране и информационни технологии/Информационни технологии/Информационни технологии ФУЧ','Компютърно моделиране/Компютърно моделиране и информационни технологии/Информационни технологии/Информационни технологии ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 76487,'Човекът и природата ФУЧ ІІІ / География и икономика ІХ','Човекът и природата ФУЧ ІІІ / География и икономика ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 76506,'Физическо възпитание и спорт ІІІ, V и ІХ','Физическо възпитание и спорт ІІІ, V и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 76687,'Музика ФУЧ ІІІ и ІV / География и икономика V','Музика ФУЧ ІІІ и ІV / География и икономика V',1,NULL,NULL,NULL,0 UNION ALL
select 76758,'Компютърно моделиране ІV /Технологии и предприемачество VІ / Информационни технологии VІІІ и ІХ','Компютърно моделиране ІV /Технологии и предприемачество VІ / Информационни технологии VІІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 76783,'М19 Аеродинамика, конструкция и системи на самолетите с турбинни двигатели','М19 Аеродинамика, конструкция и системи на самолетите с турбинни двигатели',1,NULL,NULL,NULL,0 UNION ALL
select 76825,'Експлоатация на съоръжения и инсталации за производство на енергия от ВЕИ','Експлоатация на съоръжения и инсталации за производство на енергия от ВЕИ',1,NULL,NULL,NULL,0 UNION ALL
select 76865,'Технологии и предприемачество І','Технологии и предприемачество І',1,NULL,NULL,NULL,0 UNION ALL
select 76870,'Компютърно моделиране ІV / Информационни технологии VІІІ и ІХ','Компютърно моделиране ІV / Информационни технологии VІІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 76889,'Човекът и природата V и VІ / География и икономика VІІ','Човекът и природата V и VІ / География и икономика VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 77103,'Компютърно моделиране ІІІ / Информационни технологии V и ІХ','Компютърно моделиране ІІІ / Информационни технологии V и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 77115,'Музика І / Музика ФУЧ ІІІ и ІV','Музика І / Музика ФУЧ ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 77193,'Трудово','Трудово',1,NULL,NULL,NULL,0 UNION ALL
select 77238,'Математика  / Биология и здравно образование ','Математика / Биология и здравно образование ',1,NULL,NULL,NULL,0 UNION ALL
select 77271,'Технологии и предприемачества ІІІ, ІV и V','Технологии и предприемачества ІІІ, ІV и V',1,NULL,NULL,NULL,0 UNION ALL
select 77478,'Физическо възпитание и спорт ІІІ, ІV, VІІІ и ІХ','Физическо възпитание и спорт ІІІ, ІV, VІІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 77597,'Околен свят ІІ / Човекът и природата ІV / География и икономика Х','Околен свят ІІ / Човекът и природата ІV / География и икономика Х',1,NULL,NULL,NULL,0 UNION ALL
select 77632,'Математика ІV, VІІ и ІХ / Математика ФУЧ ХІ','Математика ІV, VІІ и ІХ / Математика ФУЧ ХІ',1,NULL,NULL,NULL,0 UNION ALL
select 77694,'Физическо възпитание и спорт ІV, VІІ и ІХ','Физическо възпитание и спорт ІV, VІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 77712,'Родинознание/Човекът и обществото/История и цивилизации/Гражданско образование','Родинознание/ Човекът и обществото/История и цивилизации/Гражданско образование',1,NULL,NULL,NULL,0 UNION ALL
select 77804,'Експлоатация на топлотехнически съоръжения и инсталации','Експлоатация на топлотехнически съоръжения и инсталации',1,NULL,NULL,NULL,0 UNION ALL
select 77817,'Технологии и предприемачество ІІ и ІV','Технологии и предприемачество ІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 77891,'Български език и литература  / Български език и литература ИУЧ','Български език и литература/Български език и литература ИУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 77921,'Информационни технологии VІІ / Информационни технологии ФУЧ Х','Информационни технологии VІІ / Информационни технологии ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 77932,'Математика VІІ','Математика VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 78003,'Изобразително изкуство І','Изобразително изкуство І',1,NULL,NULL,NULL,0 UNION ALL
select 78133,'Български език и литература ІІІ и ІV / Човекът и природата V','Български език и литература ІІІ и ІV / Човекът и природата V',1,NULL,NULL,NULL,0 UNION ALL
select 78139,'Физическо възпитание и спорт VІІ, ІХ и Х','Физическо възпитание и спорт VІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 78175,'Математика ІІІ и V /География и икономика ІХ и Х','Математика ІІІ и V /География и икономика ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 78340,'Родинознание ФУЧ / История и цивилизации','Родинознание ФУЧ  / История и цивилизации',1,NULL,NULL,NULL,0 UNION ALL
select 78392,'Л Ф К VІІ, ІХ и Х','Л Ф К VІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 78494,'Музика ФУЧ  / Музика  / Музика ФУЧ','Музика ФУЧ / Музика  / Музика ФУЧ ',1,NULL,NULL,NULL,0 UNION ALL
select 78565,'Изобразително изкуство ІІ / Компютърно моделиране ІV / Информационни технологии ФУЧ Х','Изобразително изкуство ІІ / Компютърно моделиране ІV / Информационни технологии ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 78572,'Родинознание/История и цивилизации','Родинознание/История и цивилизации',1,NULL,NULL,NULL,0 UNION ALL
select 78591,'Музика ФУЧ VІІ и ІХ','Музика ФУЧ VІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 78972,'История и цивилизации/Гражданско образование','История и цивилизации/Гражданско образование',1,NULL,NULL,NULL,0 UNION ALL
select 78986,'Компютърно моделиране ІІІ и ІV / Информационни технологии V','Компютърно моделиране ІІІ и ІV / Информационни технологии V',1,NULL,NULL,NULL,0 UNION ALL
select 79043,'Изобразително изкуство ІІІ / Изобразително изкуство ФУЧ ІV, VІІІ и ІХ','Изобразително изкуство ІІІ / Изобразително изкуство ФУЧ ІV, VІІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 79302,'Музика ФУЧ/Музика/Музика ФУЧ','Музика ФУЧ/Музика/Музика ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 79407,'Изобразително изкуство ІІІ и V / История и цивилизации ІХ и Х','Изобразително изкуство ІІІ и V / История и цивилизации ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 79491,'Управление на предприемаческа дейност','Управление на предприемаческа дейност',1,NULL,NULL,NULL,0 UNION ALL
select 79517,'Родина','Родина',1,NULL,NULL,NULL,0 UNION ALL
select 79611,'Български език и литература ІІІ, ІV,VІ и VІІІ /Биология и здравно образование ІХ','Български език и литература ІІІ, ІV,VІ и VІІІ /Биология и здравно образование ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 79760,'УП - Диагностика на електрически превозни средства','УП - Диагностика на електрически превозни средства',1,NULL,NULL,NULL,0 UNION ALL
select 79768,'Изобразително изкуство/Изобразително изкуство ФУЧ','Изобразително изкуство/Изобразително изкуство ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 79831,'География на България','География на България',1,NULL,NULL,NULL,0 UNION ALL
select 79894,'Технологии и предприемачество ІІ и ІV / Изобразително изкуство ФУЧ Х','Технологии и предприемачество ІІ и ІV / Изобразително изкуство ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 79934,'Български език и и литература VІІ /Български език и литература ФУЧ в ІХ и Х','Български език и и литература VІІ /Български език и литература ФУЧ в ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 79936,'МП21 В реална работна среда','МП21 В реална работна среда',1,NULL,NULL,NULL,0 UNION ALL
select 79950,'Музика ІV / Музика ФУЧ VІІ и ІХ','Музика ІV / Музика ФУЧ VІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 79955,'Български език и литература / Български език и литература ИУЧ/Български език и литература ФУЧ','Български език и литература / Български език и литература ИУЧ/Български език и литература ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 79968,'Музика VІІ / Музика ФУЧ Х','Музика VІІ / Музика ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 80002,'Музика ІІ / Музика ФУЧ ІV и Х','Музика ІІ / Музика ФУЧ ІV и Х',1,NULL,NULL,NULL,0 UNION ALL
select 80391,'Математика ІІІ, ІV и V','Математика ІІІ, ІV и V',1,NULL,NULL,NULL,0 UNION ALL
select 80603,'Философия и политика','Философия и политика',1,NULL,NULL,NULL,0 UNION ALL
select 80612,'Музика VІІ / Музика ФУЧ ІХ и Х','Музика VІІ / Музика ФУЧ ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 80646,'Химия и опазване на околната среда/Технологии и предприемачество','Химия и опазване на околната среда/Технологии и предприемачество',1,NULL,NULL,NULL,0 UNION ALL
select 81006,'Клетката - елементарна биологична система','Клетката - елементарна биологична система',1,NULL,NULL,NULL,0 UNION ALL
select 81054,'Технологоии и предприемачество VІІ и ІХ / География и икономика Х','Технологоии и предприемачество VІІ и ІХ / География и икономика Х',1,NULL,NULL,NULL,0 UNION ALL
select 81166,'Л Ф К ІІІ и V','Л Ф К ІІІ и V',1,NULL,NULL,NULL,0 UNION ALL
select 81191,'Физическо възпитание ІІІ и V','Физическо възпитание ІІІ и V',1,NULL,NULL,NULL,0 UNION ALL
select 81365,'Човекът и природата ФУЧ ІІІ / Човекът и природата ІV и VІ / География и икономика ІХ','Човекът и природата ФУЧ ІІІ / Човекът и природата ІV и VІ / География и икономика ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 81400,'Изобразително изкуство ФУЧ ІІ и Х /Изобразително изкуство ІV','Изобразително изкуство ФУЧ ІІ и Х /Изобразително изкуство ІV',1,NULL,NULL,NULL,0 UNION ALL
select 81482,'Музика VІІ, ІХ и Х','Музика VІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 81641,'Музика ІІІ,ІV и VІ / Музика ФУЧ VІІІ и ІХ','Музика ІІІ,ІV и VІ / Музика ФУЧ VІІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 81668,'Л Ф К ІV, VІІ и ІХ','Л Ф К ІV, VІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 82008,'Български език и литература/Човекът и природата/ Български език и литература ','Български език и литература/Човекът и природата/ Български език и литература ',1,NULL,NULL,NULL,0 UNION ALL
select 82151,'Компютърно моделиране ІV / Информационни технологии VІІ и ІХ','Компютърно моделиране ІV / Информационни технологии VІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 82220,'Музика ІV, VІІ и ІХ','Музика ІV, VІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 82296,'Информационни технологии VІІ и ІХ / Информационни технологии ФУЧ Х','Информационни технологии VІІ и ІХ / Информационни технологии ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 82319,'Човекът и обществото ІІІ и ІV / История и цивилизации V и ІХ','Човекът и обществото ІІІ и ІV / История и цивилизации V и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 82344,'Човекът и природата/ Биология и здравно образование','Човекът и природата/ Биология и здравно образование ',1,NULL,NULL,NULL,0 UNION ALL
select 82432,'Човекът и обществото ІІІ и ІV / История и цивилизации VІ и ІХ','Човекът и обществото ІІІ и ІV / История и цивилизации VІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 82514,'Изобразително изкуство ФУЧ/Изобразително изкуство/Биология и здравно образование/Изобразително изкуство ФУЧ','Изобразително изкуство ФУЧ/Изобразително изкуство/Биология и здравно образование/Изобразително изкуство ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 82578,'Геометрия','Геометрия',1,NULL,NULL,NULL,0 UNION ALL
select 82631,'Български език ІІІ, ІV и VІ / Български език и литература ФУЧ VІІІ / Биология и здравно образование ІХ','Български език ІІІ, ІV и VІ / Български език и литература ФУЧ VІІІ / Биология и здравно образование ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 82726,'Музика ІІ и ІV / Музика ФУЧ Х','Музика ІІ и ІV / Музика ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 82766,'Информационни технологии VІІ, ІХ и Х','Информационни технологии VІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 82913,'Човекът и природата/География и икономика','Човекът и природата/География и икономика',1,NULL,NULL,NULL,0 UNION ALL
select 82989,'Музика ФУЧ/Компютърно моделиране','Музика ФУЧ/Компютърно моделиране',1,NULL,NULL,NULL,0 UNION ALL
select 83102,'Изобразително изкуство ІV и VІІ / Изобразително изкуство ФУЧ ІХ','Изобразително изкуство ІV и VІІ / Изобразително изкуство ФУЧ ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 83180,'Музика ІІІ, V и Х / Изобразително ІХ','Музика ІІІ, V и Х / Изобразително ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 83234,'Музика ІІІ, ІV и V','Музика ІІІ, ІV и V',1,NULL,NULL,NULL,0 UNION ALL
select 83317,'Математика VІІ, ІХ и Х','Математика VІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 83353,'Екология и опазване на околната среда','Екология и опазване на околната среда.',1,NULL,NULL,NULL,0 UNION ALL
select 83457,'Човекът и природата ІІІ, ІV и V / История и цивилизации ІХ','Човекът и природата ІІІ, ІV и V / История и цивилизации ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 83465,'Изобразително изкуство / География и икономика ','Изобразително изкуство / География и икономика ',1,NULL,NULL,NULL,0 UNION ALL
select 83551,' Компютърно моделиране /Компютърно моделиране и информационни технологии/Информационни технологии/Гражданско образование','Компютърно моделиране /Компютърно моделиране и информационни технологии/Информационни технологии/Гражданско образование',1,NULL,NULL,NULL,0 UNION ALL
select 83564,'Изобразително изкуство ФУЧ/ Човекът и природата ','Изобразително изкуство ФУЧ / Човекът и природата ',1,NULL,NULL,NULL,0 UNION ALL
select 83601,'Изобразително изкуство VІІ и ІХ / Изобразително изкуство ФУЧ Х','Изобразително изкуство VІІ и ІХ / Изобразително изкуство ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 83608,'Български език и литература /Български език и литература ИУЧ','Български език и литература /Български език и литература ИУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 83802,'Български език и литература ІІІ / Човекът и природата V / Биология и здравно образование ІХ и Х','Български език и литература ІІІ / Човекът и природата V / Биология и здравно образование ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 83922,'България и регионална политика','България и регионална политика',1,NULL,NULL,NULL,0 UNION ALL
select 83939,'Музика ІІІ, V и ІХ','Музика ІІІ, V и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 84001,'Български език и литература ІІІ, ІV и V','Български език и литература ІІІ, ІV и V',1,NULL,NULL,NULL,0 UNION ALL
select 84006,'Технологии и предприемачество ФУЧ І / Компютърно моделиране ІІІ и ІV','Технологии и предприемачество ФУЧ І / Компютърно моделиране ІІІ и ІV',1,NULL,NULL,NULL,0 UNION ALL
select 84015,'Български език и литература VІІ, ІХ и Х','Български език и литература VІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 84052,'Производствена практика ХІ','Производствена практика ХІ',1,NULL,NULL,NULL,0 UNION ALL
select 84053,'Технологии и предприемачество ФУЧ/ География и икономика ','Технологии и предприемачество ФУЧ/ География и икономика ',1,NULL,NULL,NULL,0 UNION ALL
select 84068,'Човекът и обществото / История и цивилизации','Човекът и обществото / История и цивилизации',1,NULL,NULL,NULL,0 UNION ALL
select 84237,'Технологии и предприемачество ІІІ / История и цивилизации V, ІХ и Х','Технологии и предприемачество ІІІ / История и цивилизации V, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 84379,'Информационни технологии ФУЧ','Информационни технологии ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 84446,'Околен свят ФУЧ ІІ / Човекът и обществото ІV / География и икономика Х','Околен свят ФУЧ ІІ / Човекът и обществото ІV / География и икономика Х',1,NULL,NULL,NULL,0 UNION ALL
select 84634,'Л Ф К ІІІ, V и ІХ','Л Ф К ІІІ, V и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 84666,'Музика / Музика ФУЧ','Музика / Музика ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 84687,'Технологии и предприемачество/География и икономика','Технологии и предприемачество/География и икономика',1,NULL,NULL,NULL,0 UNION ALL
select 84751,'Учебна практика Експлоатация на съоръжения и инсталации за производство на енергия от ВЕИ','Учебна практика Експлоатация на съоръжения и инсталации за производство на енергия от ВЕИ',1,NULL,NULL,NULL,0 UNION ALL
select 84934,'Географска и икономическа информация','Географска и икономическа информация',1,NULL,NULL,NULL,0 UNION ALL
select 84948,'Човекът и природата ІІІ, ІV и VІ / География и икономика ІХ','Човекът и природата ІІІ, ІV и VІ / География и икономика ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 84985,'Музика ІІІ, ІV и V / Музика ФУЧ ІХ','Музика ІІІ, ІV и V / Музика ФУЧ ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 85007,'Технологии и предприемачество VІІ','Технологии и предприемачество VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 85053,'История и цивилизации VІІ, ІХ и Х','История и цивилизации VІІ, ІХ и Х',1,NULL,NULL,NULL,0 UNION ALL
select 85117,'Технологии и предприемачество/Музика ФУЧ','Технологии и предприемачество/Музика ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 85171,'М20 Витла','М20 Витла',1,NULL,NULL,NULL,0 UNION ALL
select 85187,'Изобразително изкуство VІІ и Х / Изобразително изкуство ФУЧ ІХ','Изобразително изкуство VІІ и Х / Изобразително изкуство ФУЧ ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 85198,'Изобразително изкуство ФУЧ/ Изобразително изкуство/Изобразително изкуство ФУЧ ','Изобразително изкуство ФУЧ/ Изобразително изкуство/Изобразително изкуство ФУЧ ',1,NULL,NULL,NULL,0 UNION ALL
select 85269,'Родинознание/Човекът и обществото','Родинознание/Човекът и обществото',1,NULL,NULL,NULL,0 UNION ALL
select 85305,'Изобразително изкуство / Изобразително изкуство ФУЧ ','Изобразително изкуство / Изобразително изкуство ФУЧ ',1,NULL,NULL,NULL,0 UNION ALL
select 85344,'Татковина','Татковина',1,NULL,NULL,NULL,0 UNION ALL
select 85384,'Български език и литература ІV и VІІ / История и цивилизиции ІХ / Български език и литература ФУЧ ХІ','Български език и литература ІV и VІІ / История и цивилизиции ІХ / Български език и литература ФУЧ ХІ',1,NULL,NULL,NULL,0 UNION ALL
select 85411,'М4 Човешки фактор','М4 Човешки фактор',1,NULL,NULL,NULL,0 UNION ALL
select 85429,'География и икономика/География и икономика ФУЧ','География и икономика/География и икономика ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 85438,'Изобразително изкуство ІІІ, ІV и V / Биология и здравно образование ІХ','Изобразително изкуство ІІІ, ІV и V / Биология и здравно образование ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 85447,'Моят свят','Моят свят',1,NULL,NULL,NULL,0 UNION ALL
select 85470,'Изобразително изкуство/Биология и здравно образование/ Изобразително изкуство ФУЧ','Изобразително изкуство/Биология и здравно образование/ Изобразително изкуство ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 85524,'Математика VІІ и ІХ','Математика VІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 85542,'Български език и литература/Биология и здравно образование','Български език и литература/Биология и здравно образование',1,NULL,NULL,NULL,0 UNION ALL
select 85714,'Човекът и обществото ІІІ / Човекът и обществото ФУЧ ІV / История и цивилизации VІ и ІХ','Човекът и обществото ІІІ / Човекът и обществото ФУЧ ІV / История и цивилизации VІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 85905,'Родинознание ФУЧ','Родинознание ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 86054,'Изобразително изкуство ІІІ, ІV и V','Изобразително изкуство ІІІ, ІV и V',1,NULL,NULL,NULL,0 UNION ALL
select 86191,'Изобразително изкуство ІІ и ІV / Изобразително изкуство ФУЧ Х','Изобразително изкуство ІІ и ІV / Изобразително изкуство ФУЧ Х',1,NULL,NULL,NULL,0 UNION ALL
select 86237,'Биосфера-структура и процеси','Биосфера-структура и процеси',1,NULL,NULL,NULL,0 UNION ALL
select 86295,'Човекът и обществото/История и цивилизации/Гражданско образование ФУЧ ','Човекът и обществото/История и цивилизации/Гражданско образование ФУЧ ',1,NULL,NULL,NULL,0 UNION ALL
select 86303,'Технологии и предприемачество ФУЧ ІІ / Човекът и природата ІV / Биология и здравно образование Х','Технологии и предприемачество ФУЧ ІІ / Човекът и природата ІV / Биология и здравно образование Х',1,NULL,NULL,NULL,0 UNION ALL
select 86370,'Музика ФУЧ ІV / История и цивилизации VІІ и ІХ / Гражданско образование ХІ','Музика ФУЧ ІV / История и цивилизации VІІ и ІХ / Гражданско образование ХІ',1,NULL,NULL,NULL,0 UNION ALL
select 86520,'Български език и литература І,VІІ, VІІІ /Български език и литература ФУЧ в ІХ и ХІ','Български език и литература І,VІІ, VІІІ /Български език и литература ФУЧ в ІХ и ХІ',1,NULL,NULL,NULL,0 UNION ALL
select 86527,'Музика V и VІ / География и икономика VІІ','Музика V и VІ / География и икономика VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 86531,'Изобразително изкуство ФУЧ ІІ, ІV и Х','Изобразително изкуство ФУЧ ІІ, ІV и Х',1,NULL,NULL,NULL,0 UNION ALL
select 86542,'Технологоии и предприемачество VІІ и ІХ','Технологоии и предприемачество VІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 86544,'Математика І, VІІ, VІІІ, ІХ и ХІ','Математика І, VІІ, VІІІ, ІХ и ХІ',1,NULL,NULL,NULL,0 UNION ALL
select 86545,'Компютърно моделиране и информационни технологии V / Информационни технологии VІ и VІІ','Компютърно моделиране и информационни технологии V / Информационни технологии VІ и VІІ',1,NULL,NULL,NULL,0 UNION ALL
select 86556,'Музика І / Музика ФУЧ VІІ, VІІІ и ІХ','Музика І / Музика ФУЧ VІІ, VІІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 86564,'Изобразително изкуство ФУЧ/ Компютърно моделиране ','Изобразително изкуство ФУЧ/Компютърно моделиране',1,NULL,NULL,NULL,0 UNION ALL
select 86566,'Биология и здравно образование VІІ / Технологии и предприемачество ІХ','Биология и здравно образование VІІ / Технологии и предприемачество ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 86567,'Изобразително изкуство І / Български език и литература VІІ / Български език и литература ФУЧ VІІІ, ІХ, и ХІ','Изобразително изкуство І / Български език и литература VІІ / Български език и литература ФУЧ VІІІ, ІХ, и ХІ',1,NULL,NULL,NULL,0 UNION ALL
select 86580,'Биология и здравно образование VІІ, VІІІ и ІХ','Биология и здравно образование VІІ, VІІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 86611,'Физическо възпитание и спорт І, VІІ, VІІІ, ІХ и ХІ','Физическо възпитание и спорт І, VІІ, VІІІ, ІХ и ХІ',1,NULL,NULL,NULL,0 UNION ALL
select 86623,'Изобразително изкуство І и VІІ / Изобразително изкуство ФУЧ VІІІ и ІХ','Изобразително изкуство І и VІІ / Изобразително изкуство ФУЧ VІІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 86633,'Информационни технологии/ Информационни технологии ФУЧ ','Информационни технологии / Информационни технологии ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 86652,'Изобразително изкуство ФУЧ / Изобразително изкуство  / Изобразително изкуство ФУЧ','Изобразително изкуство ФУЧ/ Изобразително изкуство / Изобразително изкуство ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 86664,'Математика /География и икономика/Математика ФУЧ','Математика /География и икономика/Математика ФУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 86665,'Л Ф К І, VІІ, VІІІ, ІХ и ХІ','Л Ф К І, VІІ, VІІІ, ІХ и ХІ',1,NULL,NULL,NULL,0 UNION ALL
select 86669,'Технологии и предприемачество /Технологии и предприемачество ФУЧ/География и икономика','Технологии и предприемачество /Технологии и предприемачество ФУЧ/География и икономика',1,NULL,NULL,NULL,0 UNION ALL
select 86670,'Български език и литература І, VІІ, VІІІ, ІХ и ХІ','Български език и литература І, VІІ, VІІІ, ІХ и ХІ',1,NULL,NULL,NULL,0 UNION ALL
select 86682,'Изобразително изкуство І, VІІ, VІІІ и ІХ','Изобразително изкуство І, VІІ, VІІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 86713,'Български език и литература VІІ, VІІІ, ІХ и ХІ','Български език и литература VІІ, VІІІ, ІХ и ХІ',1,NULL,NULL,NULL,0 UNION ALL
select 86716,'Музика І, VІІ, VІІІ и ІХ','Музика І, VІІ, VІІІ и ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 86721,'Български език и литература ІІІ, ІV и VІ / Български език и литература ФУЧ VІІІ / Биология и здравно образование ІХ','Български език и литература ІІІ, ІV и VІ / Български език и литература ФУЧ VІІІ / Биология и здравно образование ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 86731,'Родинознание ФУЧ/Човекът и обществото/ География и икономика/ География и икономика ИУЧ','Родинознание ФУЧ/Човекът и обществото/ География и икономика/ География и икономика ИУЧ ',1,NULL,NULL,NULL,0 UNION ALL
select 86775,'История и цивилизации VІІ, VІІІ, ІХ','История и цивилизации VІІ, VІІІ, ІХ',1,NULL,NULL,NULL,0 UNION ALL
select 86785,'Технологии и предприемачество І, VІІ и ІХ / Технологии и предприемачество ФУЧ VІІІ','Технологии и предприемачество І, VІІ и ІХ / Технологии и предприемачество ФУЧ VІІІ',1,NULL,NULL,NULL,0 UNION ALL
select 98704,'Диагностика и ремонт на ЕПС','Диагностика и ремонт на ЕПС',NULL,NULL,NULL,NULL,0 UNION ALL
select 99147,'Държавен изпит за придобиване на ПК','Държавен изпит за придобиване на ПК',NULL,NULL,NULL,NULL,0 UNION ALL
select 99491,'експлоатационни материали','експлоатационни материали',NULL,NULL,NULL,NULL,0 UNION ALL
select 100771,'Електроника в автомобила и кара','Електроника в автомобила и кара',NULL,NULL,NULL,NULL,0 UNION ALL
select 102055,'ЗАДЪЛЖИТЕЛНОИЗБИРАЕМА ПОДГОТОВКА','Задължителноизбираема подготовка',NULL,NULL,NULL,NULL,0 UNION ALL
select 102311,'Защита на дипломен проект','Защита на дипломен проект',NULL,NULL,NULL,NULL,0 UNION ALL
select 120876,'Производство и ремонт на автомобила и кара','Производство и ремонт на автомобила и кара',NULL,NULL,NULL,NULL,0 UNION ALL
select 136078,'Учебна практика Измервателна и диагностична техника','Учебна практика Измервателна и диагностична техника',NULL,NULL,NULL,NULL,0 UNION ALL
select 142793,'История на античните цивилизации',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 142794,'Медии и комуникации',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 142818,'Учебна практика Специализирана',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 142833,'Учебна практика по заваряване',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 143989,'Учебна практика електрозадвижване и електрообзавеждане на електрически превозни средства',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 144690,'Многоклетъчна организация на биологичните системи',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 144797,'Автоматизация на производството','АП СПП',1,NULL,NULL,NULL,0 UNION ALL
select 144939,'Родинознание/Човекът и природата',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 145600,'Човекът и природата/Биология и здравно образование',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 145866,'Машинни елементи',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 145990,'- , . : ; ( ) /  "" ''','- , . : ; ( ) /  '' '' " "',1,NULL,NULL,NULL,0 UNION ALL
select 145993,'Електротехника и електроника','Електротехника и електроника',1,NULL,NULL,NULL,0 UNION ALL
select 146202,'Учебна практика - Експлоатация на топлотехнически съоръжения и инсталации',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146303,'Системи, съдържащи флуорирани парникови газове',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146309,'Учебна практика Експлоатация на автотранспортната техника',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146326,'Учебна практика Електрообзавеждане на автотранспортна техника',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146413,'Учебна практика Диагностика на електрически превозни средства',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146415,'Учебна практика Ремонт на топлотехнически съоръжения и инсталации',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146422,'Учебна практика Монтаж и ремонт на газови инсталации и разпределителни мрежи',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146423,'Електроенергетика и електрообзавеждане на ЕПС',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146425,'Електротранспорт',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146435,'Основи на електрообзавеждането',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146471,'Учебна практика Технология на изграждане на инсталации',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146474,'Учебна практика Технология на сглобяване и ремонт',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146476,'Устройство на електрическите превозни средства',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146477,'Учебна практика Обслужване и ремонт на електрически превозни средства',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 146482,'Учебна практика Електрически машини Лабораторна',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 147777,'Учебна практика МП  Металообработване и основи на технологията на демонтажно - монтажните операции',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 147778,'Учебна практика МП Основи на електротехниката',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 147816,'Компютърно моделиране и информационни технологии/Информационни технологии',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 147915,'Теория на автотранспортната техника',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 147989,'Човекът и обществото/Човекът и природата',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148196,'Еволюция на биологичните системи','Модул',1,NULL,NULL,NULL,0 UNION ALL
select 148292,'Музика/ФУЧ/Компютърно моделиране/Компютърно моделиране и информационни технологии',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148366,'Компютърно моделиране/Компютърно моделиране и информационни технологии/Информационни технологии ФУЧ/Гражданско образование',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148428,'Български език и литература/Човекът и природата/Биология и здравно образование/Български език и литература ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148429,'Човекът и обществото/География и икономика/Гражданско образование',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148463,'Компютърно моделиране/Компютърно моделиране и информационни технологии',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148487,'Рехабилитация на комуникативните нарушения',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148509,'Информационни технологии/Информационни технологии ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148510,'География и икономика/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148511,'География и икономика ФУЧ/География и икономика/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148512,'Изобразително изкуство ФУЧ/Изобразително изкуство/Изобразително изкуство ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148513,'Информационни технологии/Информационни технологии ФУЧ/Информационни технологии ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148520,'Психо-социална рехабилитация',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148536,'Технологии и предприемачество ФУЧ/Технологии и предприемачество',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148537,'Изобразително изкуство/География и икономика/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148538,'Музика/Биология и здравно образование/Биология и здравно образование ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148641,'Компютърно моделиране/Компютърно моделиране и информационни технологии/Изобразително изкуство',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148689,'Родинознание ФУЧ/Човекът и природата',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148826,'Музика/Музика ФУЧ/Музика/Музика ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148831,'Математика/География и икономика',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 148833,'Български език и литература/Човекът и природата/Биология и здравно образование/Български език и литература',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 149154,'ЗАДЪЛЖИТЕЛНО ИЗБИРАЕМА ПОДГОТОВКА -- ПРОФИЛИРАНА ПОДГОТОВКА',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 149155,'ЗАДЪЛЖИТЕЛНО ИЗБИРАЕМА ПОДГОТОВКА -- НЕПРОФИЛИРАНА ПОДГОТОВКА',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 149212,'Физическо възпитание',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 149431,'Древните и антични цивилизации',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 149432,'Процеси в клетката',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 149433,'Съвременните цивилизации',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 149611,'МОДУЛ 1 - Езикът и обществото',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149612,'МОДУЛ 2 - Езикови употреби',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149613,'МОДУЛ 3 - Диалогични прочити',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149614,'МОДУЛ 4 - Критическо четене',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149615,'МОДУЛ 1 - Устно общуване',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149616,'МОДУЛ 2 - Писмено общуване',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149617,'МОДУЛ 3 - Език и култура',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149618,'МОДУЛ 4 - Езикови практики',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149619,'МОДУЛ 1 - Устно общуване',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149620,'МОДУЛ 2 - Писмено общуване',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149621,'МОДУЛ 3 - Езикът чрез литературата',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149622,'МОДУЛ 4 - Култура и междукултурно общуване',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149623,'МОДУЛ 1 - Геометрия',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149624,'МОДУЛ 2 - Елементи на математическия анализ',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149625,'МОДУЛ 3 - Практическа математика',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149626,'МОДУЛ 4 - Вероятности и анализ на данни',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149627,'МОДУЛ 1 - Обектно ориентирано проектиране и програмиране',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149628,'МОДУЛ 2 - Структури от данни и алгоритми',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149629,'МОДУЛ 3 - Релационен модел на бази от данни',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149630,'МОДУЛ 4 - Програмиране на информационни системи',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149631,'МОДУЛ 1 - Обработка и анализ на данни',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149632,'МОДУЛ 2 - Мултимедия',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149633,'МОДУЛ 3 - Уеб дизайн',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149634,'МОДУЛ 4 - Решаване на проблеми с ИКТ',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149635,'МОДУЛ 1 - Власт и институции',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149636,'МОДУЛ 2 - Култура и духовност',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149637,'МОДУЛ 3 - Човек и общество',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149638,'МОДУЛ 1 - Природноресурсен потенциал. Устойчиво развитие',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149639,'МОДУЛ 2 - Геополитическа и обществена култура',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149640,'МОДУЛ 3 - Съвременно икономическо развитие',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149641,'МОДУЛ 4 - Европа, Азия и България',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149642,'МОДУЛ 5 - България и регионална политика',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149643,'МОДУЛ 6 - Географска и икономическа информация',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149644,'МОДУЛ 1 - История на идеите',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149645,'МОДУЛ 2 - Философия и ценности',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149646,'МОДУЛ 3 - Култура на мисленето',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149647,'МОДУЛ 4 - Социална психология',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149648,'МОДУЛ 5 - Философия и политика',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149649,'МОДУЛ 6 - Психология на личността',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149650,'МОДУЛ 1 - Клетката - елементарна биологична система',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149651,'МОДУЛ 2 - Многоклетъчна организация на биологичните системи',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149652,'МОДУЛ 3 - Биосфера - структура и процеси',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149653,'МОДУЛ 4 - Еволюция на биологичните системи',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149654,'МОДУЛ 1 - Движение и енергия',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149655,'МОДУЛ 2 - Поле и енергия',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149656,'МОДУЛ 3 - Експериментална физика',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149657,'МОДУЛ 4 - Атоми, вълни и кванти',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149658,'МОДУЛ 5 - Съвременна физика',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149659,'МОДУЛ 1 - Теоретични основи на химията',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149660,'МОДУЛ 2 - Химия на неорганичните вещества',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149661,'МОДУЛ 3 - Химия на органичните вещества',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149662,'МОДУЛ 4 - Методи за контрол и анализ на веществата',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149663,'МОДУЛ 1 - Музикална култура',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149664,'МОДУЛ 2 - Теория на музиката',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149665,'МОДУЛ 3 - Музикален инструмент/пеене',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149666,'МОДУЛ 4 - Пиано/електронни инструменти',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149667,'МОДУЛ 1 - Теория на изкуството',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149668,'МОДУЛ 2 - Изкуство и изразни средства',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149669,'МОДУЛ 3 - Визуална култура',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149670,'МОДУЛ 4 - Дигитални изкуства',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149671,'МОДУЛ 1 - Предприемачество и кариерно развитие',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149672,'МОДУЛ 2 - Пазарна икономика',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149673,'МОДУЛ 3 - Стартиране на собствен бизнес',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149674,'МОДУЛ 4 - Управление на предприемаческата дейност',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149675,'МОДУЛ 1 - Спортно усъвършенстване по вид спорт',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149676,'МОДУЛ 2 - Спортна анимация',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149677,'МОДУЛ 3 - Олимпизъм и олимпийски принципи',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149678,'МОДУЛ 4 - Наука и спорт',NULL,1,NULL,NULL,NULL,1 UNION ALL
select 149864,'Учебна практика Технология на изработване на инсталации',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 150590,'Учебна практика Начални професионални умения',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 150631,'Учебна практика Експлоатация на газови инсталации и разпределителни мрежи',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 151139,'Български език и литература/Български език и литература ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 151832,'Пеене и музика',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152209,'Технологии и предприемачество ФУЧ/Компютърно моделиране',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152228,'Технологии и предприемачество/Биология и здравно образование',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152229,'Изобразително изкуство ФУЧ/География и икономика',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152230,'Български език и литература/Технологии и предприемачество',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152231,'Български език и литература/Технологии и предприемачество/География и икономика ФУЧ/География и икономика',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152232,'Технологии и предприемачество ФУЧ/Информационни технологии',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152233,'Технологии и предприемачество ФУЧ/Информационни технологии/Информационни технологии ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152235,'Български език и литература/Биология и здравно образование/Български език и литература',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152236,'Родинознание/Човекът и природата/География и икономика',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152237,'Изобразително изкуство ФУЧ/История и цивилизации',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152238,'Родинознание ФУЧ/Човекът и природата/История и цивилизации',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152239,'Технологии и предприемачество ФУЧ/География и икономика/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152240,'Музика ФУЧ/Компютърно моделиране и информационни технологии/Информационни технологии/Музика ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152241,'Изобразително изкуство/Изобразително изкуство ФУЧ/Изобразително изкуство',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152242,'Изобразително изкуство ФУЧ/изобразително изкуство',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152245,'Изобразително изкуство ФУЧ/Човекът и обществото/История и цивилизации',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152246,'Музика/Музика ФУЧ/Музика',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152253,'Математика/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152254,'Технологии и предприемачество/Технологии и предприемачество ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152255,'История и цивилизации/История и цивилизации ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152256,'Човекът и природата/Български език и литература ИУЧ/Български език и литература',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152258,'Компютърно моделиране и информационни технологии/Технологии и предприемачество ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152259,'Човекът и природата/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152260,'География и икономика ФУЧ/География и икономика',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152261,'Материалознание РПП/Материалознание ОПП',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152262,'Процеси и машини в шевната промишленост СПП/Процеси и машини в шевната промишленост РПП',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152263,'Процеси и машини в шевната промишленост ФУЧ/Процеси и машини в шевната промишленост РПП',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152264,'Учебна практика по Процеси и машини в шевната промишленост СПП /Учебна практика по Процеси и машини в шевната промишленост   РПП',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152270,'Математика/История и цивилизации ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152271,'Изобразително изкуство/Гражданско образование',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152272,'Технологии и предприемачество/Информационни технологии ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152273,'Изобразително изкуство ФУЧ/Изобразително изкуство/География и икономика',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152274,'Музика ФУЧ/Музика/Изобразително изкуство/Музика ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152275,'Изобразително изкуство/География и икономика/Гражданско образование ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152276,'Компютърно моделиране и информационни технологии/Информационни технологии/Гражданско образование','Компютърно моделиране и информационни технологии/Информационни технологии/Гражданско образование',1,NULL,NULL,NULL,0 UNION ALL
select 152277,'Компютърно моделиране/Компютърно моделиране и информационни технологии/Гражданско образование ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152278,'Технологии и предприемачество ФУЧ/География и икономика',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152279,'Човекът и обществото/История и цивилизации/История и цивилизации ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152308,'Човекът и природата/Биология и здравно образование/Гражданско образование',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152309,'Математика/Изобразително изкуство ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152311,'Човекът и природата/Изобразително изкуство ФУЧ/Изобразително изкуство/Изобразително изкуство ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152313,'Български и литература ФУЧ/Биология и здравно образование/Български и литература ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152314,'Компютърно моделиране и информационни технологии/Информационни технологии/Информационни технологии ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152315,'Компютърно моделиране и информационни технологии/Информационни технологии ФУЧ/Информационни технологии ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152316,'Технологии и предприемачество/География и икономика/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152317,'География и икономика/История и цивилизации/История и цивилизации ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152318,'Технологии и предприемачество/Математика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152443,'Музика ФУЧ/Компютърно моделиране и информационни технологии/Информационни технологии/Информационни технологии ФУЧ/Музика ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152444,'Родинознание/Човекът и природата/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152445,'Изобразително изкуство ФУЧ/История  и цивилизации/Изобразително изкуство ФУЧ/История и цивилизации/Изобразително изкуство ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152446,'Изобразително изкуство ФУЧ/История и цивилизации/Изобразително изкуство/История и цивилизации/Изобразително изкуство ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152724,'Електроснабдяване и електрообзавеждане на  ЕПС',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152857,'Музика ФУЧ/Човекът и обществото',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152858,'Математика/Технологии и предприемачество/Математика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152859,'Изобразително изкуство/Български и литература/Български и литература ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 152860,'Български език  и литература/Български език  и литература ФУЧ/Български език  и литература ИУЧ','Български език  и литература/Български език  и литература ФУЧ/Български език  и литература ИУЧ',1,NULL,NULL,NULL,0 UNION ALL
select 153068,'ДИППК - Теория на професията',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 153069,'ДИППК - Практика на професията',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 153504,'Учебна практика Ремонт на съоръжения и инсталации за производство на енергия от ВЕИ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 154931,'Икономика/Икономика ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 154934,'Предприемачество/Здравословни и безопасни условия на труд ОПП',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 154935,'Предприемачество/Здравословни и безопасни условия на труд РПП',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 154936,'Процеси и машини в шевната промишленост РПП/Процеси и машини в шевната промишленост СПП',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 154937,'Материалознание ОПП/Материалознание РПП',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 154946,'Учебна практика по процеси и машини в шевната промишленост РПП/Учебна практика по процеси и машини в шевната промишленост СПП',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155006,'Човекът и природа/История и цивилизации ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155245,'Родинознание ФУЧ/Човекът и обществото/Биология и здравно образование ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155246,'Изобразително изкуство ФУЧ/Човекът  и природата/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155249,'Човекът и природата/Гражданско образование',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155252,'Музика/Технологии и предприемачество/Музика ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155253,'Технологии и предприемачество/Изобразително изкуство ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155254,'Човекът и природата/История и цивилизации ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155318,'Експлоатация на газови инсталации и разпределителни  мрежи',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155397,'Изобразително изкуство/Музика ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155398,'Музика/Музика ФУЧ/Информационни технологии ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155399,'Технологии и предприемачество/Информационни технологии ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155400,'География и иконолика/Технологии и предприемачество/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155401,'География и икономика/Технологии и предприемачество/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155402,'Човекът и природата/История и цивилизации/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155403,'Родинознание ФУЧ/Човекът и обществото',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155404,'Родинознание/Човекът и природата/Биология и здравно образование/География и икономика/География и икономика ИУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155405,'Музика/Компютърно моделиране и информационни технологии/Информационни технологии',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155406,'Изобразително изкуство ФУЧ/История и цивилизации/Изобразително изкуство ФУЧ',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155407,'Български език и литература/История и цивилизации/Български език и литература',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 155408,'Родинознание ФУЧ/Човекът и природата/Биология и здравно образование',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 156199,'Криминална психология',NULL,1,NULL,NULL,NULL,0 UNION ALL
select 156300,'Историческо мислене и аргументация',NULL,1,NULL,NULL,NULL,0;

set identity_insert [inst_nom].[Subject] off;

GO
