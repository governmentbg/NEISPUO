GO
PRINT 'Insert [inst_nom].[EduForm]'
GO

insert [inst_nom].[EduForm] ([ClassEduFormID],[Name],[Description],[ValidForClass],[ValidforStudent],[IsValid],[SortOrd],[ValidFrom],[ValidTo],[IsNotPresentForm],[ValidforDiploma],[CanChoose],[NameEN],[NameDE],[NameFR],[NameShort])
select -1,'не е приложимо',NULL,0,1,1,NULL,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'не е приложимо' UNION ALL
select 0,'подлежи на поправителен изпит','подлежи на поправителен изпит',0,1,1,54,'2022-06-28 14:03:52.570',NULL,1,0,1,NULL,NULL,NULL,'подлежи на поправителен изпит' UNION ALL
select 1,'дневна','Дн',1,1,1,1,'2021-01-28 00:00:00.000',NULL,0,1,1,'full-time','Direktuntericht','quotidien','дневна' UNION ALL
select 2,'вечерна','Веч',1,1,1,2,'2021-01-28 00:00:00.000',NULL,0,1,1,'evening','Abendunterricht','soirée','вечерна' UNION ALL
select 3,'задочна','Зад',1,1,1,3,'2021-01-28 00:00:00.000',NULL,0,1,1,'part-time','Fernunterricht','à temps partiel','задочна' UNION ALL
select 4,'кореспондентска','Кор',1,0,0,10,'2021-01-28 00:00:00.000',NULL,1,0,1,NULL,NULL,NULL,'кореспондентска' UNION ALL
select 5,'индивидуална - по здр. причини','Инд/Здр',0,1,1,10,'2021-01-28 00:00:00.000',NULL,0,0,1,'individual - for health reasons','individueller Unterricht, aus Gesundheitsgründen','individuel - pour des raisons de santé','индивидуална - по здр. причини' UNION ALL
select 6,'самостоятелна - по здр. причини','С/Здр',0,1,1,16,'2021-01-28 00:00:00.000',NULL,1,0,1,'independent - for health reasons','selbständiger Unterricht, aus Gesundheitsgründen','indépendant - pour des raisons de santé','самостоятелна - по здр. причини' UNION ALL
select 7,'дистанционна -  по здр.причини','Дист/Здр',0,1,1,23,'2021-01-28 00:00:00.000',NULL,1,0,1,NULL,NULL,NULL,'дистанционна -  по здр.причини' UNION ALL
select 8,'комбинирана - за ученици със СОП','Комб/СОП',0,1,1,19,'2021-01-28 00:00:00.000',NULL,0,0,1,'combined - for students with Special Educational Needs','kombinierter Unterricht - für Schüler mit besonderen pädagogischen Bedürfnissen','combiné - pour les étudiants ayant des besoins éducatifs spéciaux','комбинирана - за ученици със СОП' UNION ALL
select 9,'обучение чрез работа - дуална система на обучение','Обуч/Раб',1,1,1,4,'2021-01-28 00:00:00.000',NULL,0,1,1,'work-based training (dual system of training)','Ausbilung durch Arbeit (duales Ausbildungssystem)','formation en milieu professionnel (système de formation en alternance)','обучение чрез работа' UNION ALL
select 10,'постоянна','пост',1,0,1,31,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'постоянна' UNION ALL
select 11,'временна през учебната година','вруг',1,0,1,32,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'временна през учебната година' UNION ALL
select 12,'временна през ваканцията','врв',1,0,1,33,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'временна през ваканцията' UNION ALL
select 13,'за обучение на деца в предучилищна възраст','ПГ',1,1,1,27,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'за обучение на деца в предучилищна възраст' UNION ALL
select 21,'ОРЕС - по здр. причини, в същото у-ще (чл.115а, ал.4, т.1 и т.2 от ЗПУО)','ОРЕС/ЗП',0,1,0,5,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'ОРЕС - по здр. причини, в същото у-ще (чл.115а, ал.4, т.1 и т.2 от ЗПУО)' UNION ALL
select 22,'ОРЕС - по здр. причини, в друго у-ще (чл.115а, ал.4, т.1 и т.2 във връзка с ал.5 от ЗПУО)','ОРЕС/ЗПДУ',0,1,0,6,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'ОРЕС - по здр. причини, в друго у-ще (чл.115а, ал.4, т.1 и т.2 във връзка с ал.5 от ЗПУО)' UNION ALL
select 23,'ОРЕС - по избор на ученик или родител, в същото у-ще (чл.115а, ал.4, т.3 от ЗПУО)','ОРЕС/ИР',0,1,0,7,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'ОРЕС - по избор на ученик или родител, в същото у-ще (чл.115а, ал.4, т.3 от ЗПУО)' UNION ALL
select 24,'ОРЕС - по избор на ученик или родител, в друго у-ще (чл.115а, ал.4, т.3 във връзка с ал.5 от ЗПУО)','ОРЕС/ИРДУ',0,1,0,8,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'ОРЕС - по избор на ученик или родител, в друго у-ще (чл.115а, ал.4, т.3 във връзка с ал.5 от ЗПУО)' UNION ALL
select 51,'индивидуална - ученици с изявени дарби','Инд/Дар',0,1,1,11,'2021-01-28 00:00:00.000',NULL,0,0,1,'individual - students with outstanding gifts','individueller Unterricht, begabte Schüler','individuel - étudiants ayant des dons exceptionnels','индивидуална - ученици с изявени дарби' UNION ALL
select 52,'индивидуална - в други срокове','Инд/Ср',0,1,1,12,'2021-01-28 00:00:00.000',NULL,0,0,1,'individual - in other terms','individueller Unterricht, andere Frsiten','individuel - en d''autres termes','индивидуална - в други срокове' UNION ALL
select 53,'индивидуална - за ученици със СОП','Инд/СОП',0,1,1,13,'2021-01-28 00:00:00.000',NULL,0,0,1,'individual - for students with Special Educational Needs','individueller Unterricht, für Schüler mit besonderen pädagogischen Bedürfnissen','individuel - pour les étudiants ayant des besoins éducatifs spéciaux','индивидуална - за ученици със СОП' UNION ALL
select 54,'индивидуална - за ученици след випуска си','Инд/Възр',0,1,1,14,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'индивидуална - за ученици след випуска си' UNION ALL
select 61,'самостоятелна - ученици с изявени дарби','С/Дар',0,1,1,17,'2021-01-28 00:00:00.000',NULL,1,0,1,'independent - students with outstanding gifts','selbständiger  Unterricht, begabte Schüler','indépendants - étudiants ayant des dons exceptionnels','самостоятелна - ученици с изявени дарби' UNION ALL
select 62,'самостоятелна - над 16 г.','С/16+',0,1,1,15,'2021-01-28 00:00:00.000',NULL,1,0,1,'independent - over 16','selbständiger Unterricht - ab 16 J.','indépendant - plus de 16 ans','самостоятелна - над 16 г.' UNION ALL
select 63,'самостоятелна - по желание, под 16 г.','С/16-',0,1,1,18,'2021-01-28 00:00:00.000',NULL,1,0,1,'independent - on request, under 16','selbständiger Unterricht - auf Wunsch, ab 16 J.','indépendant - sur demande, moins de 16 ans','самостоятелна - по желание, под 16 г.' UNION ALL
select 71,'дистанционна - за ученици със СОП','Дист/СОП',0,1,1,24,'2021-01-28 00:00:00.000',NULL,1,0,1,NULL,NULL,NULL,'дистанционна - за ученици със СОП' UNION ALL
select 72,'дистанционна - за ученици с изявени дарби','Дист/Дар',0,1,1,25,'2021-01-28 00:00:00.000',NULL,1,0,1,NULL,NULL,NULL,'дистанционна - за ученици с изявени дарби' UNION ALL
select 73,'дистанционна - по семейни причини','Дист/Сем',0,1,1,26,'2021-01-28 00:00:00.000',NULL,1,0,1,NULL,NULL,NULL,'дистанционна - по семейни причини' UNION ALL
select 81,'комбинирана - за ученици с изявени дарби','Комб/Дар',0,1,1,20,'2021-01-28 00:00:00.000',NULL,0,0,1,'Combined - for students with outstanding gifts','kombinierter Unterricht - für begabte Schüler','combiné - pour les étudiants ayant des dons exceptionnels','комбинирана - за ученици с изявени дарби' UNION ALL
select 82,'комбинирана - за ученици от друго училище по чужд език','Комб/ЧЕ',0,1,1,21,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'комбинирана - за ученици от друго училище по чужд език' UNION ALL
select 83,'комбинирана - за ученици от друго училище по учебен предмет','Комб/УП',0,1,1,22,'2021-01-28 00:00:00.000',NULL,0,0,1,NULL,NULL,NULL,'комбинирана - за ученици от друго училище по учебен предмет' UNION ALL
select 501,'индивидуална','индивидуална',0,1,0,50,'2022-01-01 00:00:00.000',NULL,0,1,1,NULL,NULL,NULL,'индивидуална' UNION ALL
select 601,'самостоятелна','самостоятелна',0,1,0,51,'2022-01-01 00:00:00.000',NULL,1,1,1,NULL,NULL,NULL,'самостоятелна' UNION ALL
select 701,'дистанционна','дистанционна',0,1,0,52,'2022-01-01 00:00:00.000',NULL,1,1,1,NULL,NULL,NULL,'дистанционна' UNION ALL
select 801,'комбинирана','комбинирана',0,1,0,53,'2022-01-01 00:00:00.000',NULL,0,1,1,NULL,NULL,NULL,'комбинирана';

GO
