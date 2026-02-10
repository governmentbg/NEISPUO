GO
PRINT 'Insert [inst_basic].[InstitutionDetail]'
GO

insert [inst_basic].[InstitutionDetail] ([InstitutionID],[Email],[Website],[EstablishedYear],[ConstitActFirst],[ConstitActLast],[IsODZ],[IsProfSchool],[IsNational],[IsProvideEduServ],[IsDelegateBudget],[IsNonIndDormitory],[IsInternContract],[BankIBAN],[BankBIC],[BankName],[BankAccountHolder],[SysUserID],[IsAppInnovSystem])
select 100006,'info-100006@edu.mon.bg','https://zvezdichkabukovo.wixsite.com','2019','АКТ № 2700 за поправка на акт за публична общинска собственост №х 639 / 26.01.2009 г.',NULL,0,0,0,0,1,0,NULL,'BG05FINV91503117344714','FINVBGSF','Първа инвестиционна банка АД','Детска градина "Звездичка"',NULL,0 UNION ALL
select 200277,'info-200277@edu.mon.bg','http//www.andersenbs.com/','1977','100','100',0,0,0,0,1,0,NULL,'BG20SOMB91303124011000','SOMBBGSF','Общинска банка АД','Детска градина "Ханс Кристиан Андерсен"',NULL,0 UNION ALL
select 300110,'info-300110@edu.mon.bg','https//oubalvan.weebly.com','1879','Акт №4232/26.06.2007г за публична общинска собственост',NULL,0,0,0,0,1,0,NULL,'BG55SOMB91303142885600','SOMBBGSF','Общинска банка АД','ОУ"Свети Иван Рилски"',NULL,0 UNION ALL
select 300125,'info-300125@edu.mon.bg','velavt.net','1985','Заповед на министъра на народната просвета от месец април 1985 година','Заповед РД-14-12 / 04 април 1995 г. на министъра на образованието, науката и технологиите',0,0,0,0,1,0,NULL,'BG86SOMB91303124757100','SOMBBGSF','Общинска банка АД','СУ "Вела Благоева", гр. Велико Търново',NULL,0 UNION ALL
select 607055,'info-607055@edu.mon.bg','http//csop-beron.idwebbg.com/index.php','1965',NULL,'Заповед № РД 14-257 от 20.07.2017 г. на МОН',0,1,0,0,1,1,NULL,'BG55UBBS80023106177306','UBBSBGSF','Обединена българска банка АД','ЦСОП "Д-р Петър Берон"-Враца',NULL,0 UNION ALL
select 1690180,'info-1690180@edu.mon.bg','www.rcplovdiv.com','2006','Заповед РД-14-180 от 13.09.2006 на МОН','Заповед РД-14-164 от 139.09.2016 на МОН',0,0,0,0,1,0,NULL,'BG20UNCR75273164921800','UNCRBGSF','УниКредит Булбанк АД','РЦПППО Пловдив',NULL,0 UNION ALL
select 2000218,'info-2000218@edu.mon.bg',NULL,'1969',NULL,'Заповед РД 13-237/01.08.2016 на кмета на Община Котел за промяна наименование',0,0,0,0,0,0,NULL,'BG75SOMB91303115509900','SOMBBGSF','Общинска банка','Дирекция СХД',NULL,0 UNION ALL
select 2206409,'info-2206409@edu.mon.bg','www.pgtehford.com','1960','Заповед на МОН с № РД-09-332/07.04.2003 г','Заповед № І-3422/01.08.1960 г. Министерство на просветата и културата (мин. Начо Папазов)',0,1,0,0,1,1,NULL,'BG51BPBI79403187841701','BPBIBGSF','Юробанк България АД','инж. Николай Пламенов Панайотов',NULL,0;

GO
