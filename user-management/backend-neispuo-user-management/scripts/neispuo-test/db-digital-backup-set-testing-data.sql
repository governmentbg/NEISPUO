-- This script must be copied and executed in neispuo-test after every database restore in order to map test MON/Digital Backup users to anonymized schools and people.
-- DO NOT RUN ON PROD !!!

BEGIN TRANSACTION

UPDATE core.Person
SET FirstName=N'Маргарита', MiddleName=N'тестов', LastName=N'служител',PublicEduNumber=N'margarita.sluzhitel', PersonalID=N'1111180151', AzureID=N'9683ee06-db0f-4eec-a055-74f5967fa3f9'
WHERE PersonID=84417399;

UPDATE core.SysUser
SET Username=N'margarita.sluzhitel@edu.mon.bg'
WHERE PersonID=84417399;

UPDATE core.Person
SET FirstName=N'Eкатерина', MiddleName=N'тестов', LastName=N'ученик',  PublicEduNumber=N'et73946789', PersonalID=N'1111265566',AzureID=N'a1b660b1-1a3f-4097-b26d-f567336eafcd'
WHERE PersonID=83724086;

DELETE from core.EducationalState WHERE PersonID = 83724086 AND PositionID = 8;
UPDATE core.Person
SET FirstName=N'Елица', MiddleName=N'тестов', LastName=N'ученик', PublicEduNumber=N'et78453768', PersonalID=N'1111290001', AzureID=N'447502a5-29e2-43fd-aec7-5397fbd02d13'
WHERE PersonID=83737069;
UPDATE core.Person
SET FirstName=N'Димана', MiddleName=N'тестов', LastName=N'ученик',  PublicEduNumber=N'dt81762252', PersonalID=N'1111300047', AzureID=N'c15be5d3-4bb8-4291-8c2b-def3ceca777e'
WHERE PersonID=83827866;

UPDATE core.SysUser
SET Username=N'et73946789@edu.mon.bg'
WHERE PersonID=83724086;
UPDATE core.SysUser
SET Username=N'et78453768@edu.mon.bg'
WHERE PersonID=83737069;
UPDATE core.SysUser
SET Username=N'dt81762252@edu.mon.bg'
WHERE PersonID=83827866;

UPDATE core.Person
SET FirstName=N'Стоян', MiddleName=N'тестов', LastName=N'ученик', PublicEduNumber=N'st10694967',PersonalID=N'1111083204',AzureID=N'84fe52c6-f77f-4158-b9bb-03c32a480c27'
WHERE PersonID=83297142;
UPDATE core.Person
SET FirstName=N'Кристияна', MiddleName=N'тестов', LastName=N'ученик',PublicEduNumber=N'kt44455133', PersonalID=N'1111034046',AzureID=N'93ec2baf-14fe-4fb3-a6be-bab20189c608'
WHERE PersonID=83857493;


UPDATE core.SysUser
SET Username=N'st10694967@edu.mon.bg', PersonID=83297142
WHERE PersonID=83297142;
UPDATE core.SysUser
SET Username=N'kt44455133@edu.mon.bg', PersonID=83857493
WHERE PersonID=83857493;

UPDATE core.Person
SET FirstName=N'Надя', MiddleName=N'тестов', LastName=N'учител', PublicEduNumber=N'nadia.sluzhitel', PersonalID=N'1111080045', AzureID=N'1c69f444-2e8a-45f7-b591-0672df8e034e'
WHERE PersonID=84418866;
UPDATE core.Person
SET FirstName=N'Диана', MiddleName=N'тестов', LastName=N'учител',PublicEduNumber=N'diana.sluzhitel', PersonalID=N'1111020099', AzureID=N'7bf6f2f1-d983-4ec5-b151-0df3e5d7bf2e'
WHERE PersonID=85783927;

UPDATE core.Person
SET FirstName=N'Христина', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'ул. "Васил Левски" №207', PermanentTownID=73198, CurrentAddress=N'ул. "Васил Левски" №207', CurrentTownID=73198, PublicEduNumber=N'hristina.sluzhitel', PersonalIDType=0, NationalityID=NULL, BirthDate='1948-04-14', BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=1, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'2d9d7ca9-8aa5-4792-b5f1-cc2d2bbd7631', SysUserType=NULL
WHERE PersonID=83031814;

UPDATE core.Person
SET FirstName=N'Здравко', MiddleName=N'тестов', LastName=N'служаител', PermanentAddress=N'ул."Дим. Гимиджийски"23а', PermanentTownID=73198, CurrentAddress=N'ул."Дим. Гимиджийски"23а', CurrentTownID=73198, PublicEduNumber=N'zdravko.sluzhitel', PersonalIDType=0, NationalityID=NULL, BirthDate='1958-02-27', BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=1, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'76c5022a-c0d6-4ec7-bc76-4fd1ccf481ef', SysUserType=NULL
WHERE PersonID=84349811;
UPDATE core.Person
SET FirstName=N'Диана', MiddleName=N'тестов', LastName=N'ученик', PermanentAddress=N'жк "Ален мак", бл. 30, вх.Б ет.2 ап. 4', PermanentTownID=4279, CurrentAddress=N'жк "Ален мак", бл. 30, вх.Б ет.2 ап. 4', CurrentTownID=4279, PublicEduNumber=N'dt76734102', PersonalIDType=0, NationalityID=34, BirthDate='2014-08-06', BirthPlaceTownID=68134, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'Gwey6Ug4', BirthPlace=NULL, AzureID=N'1c749b34-b55e-48b1-b033-8ff4834a5d79', SysUserType=NULL
WHERE PersonID=84077373;
UPDATE core.Person
SET FirstName=N'Мануела', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'ул. Захари Петров № 2, вх. Б, ет. 4, ап. 12', PermanentTownID=73403, CurrentAddress=N'ул. Захари Петров № 2, вх. Б, ет. 4, ап. 12', CurrentTownID=73403, PublicEduNumber=N'manuela.l.atanasova', PersonalIDType=0, NationalityID=NULL, BirthDate='1949-02-19', BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=2, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'3e3912f1-d985-448a-bb67-8fa5e2ddbf30', SysUserType=NULL
WHERE PersonID=85116474;



UPDATE core.Person
SET FirstName=N'Наджие', MiddleName=N'тестов', LastName=N'ученик', PermanentAddress=N'', PermanentTownID=32024, CurrentAddress=N'', CurrentTownID=32024, PublicEduNumber=N'nt34590816', PersonalIDType=0, NationalityID=34, BirthDate='2009-01-25', BirthPlaceTownID=77181, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'RTWhG7Ta', BirthPlace=NULL, AzureID=N'f6675845-c8e9-4148-8c32-2f3f6f34843a', SysUserType=NULL
WHERE PersonID=83706310;
UPDATE core.Person
SET FirstName=N'Белис', MiddleName=N'тестов', LastName=N'ученик', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'bt37995697', PersonalIDType=0, NationalityID=34, BirthDate='2009-02-10', BirthPlaceTownID=39970, BirthPlaceCountry=34, Gender=1, SchoolBooksCodesID=N'FFafNlqv', BirthPlace=NULL, AzureID=N'4cdcb37d-071c-40aa-a904-6beb46320c76', SysUserType=NULL
WHERE PersonID=83709334;
UPDATE core.Person
SET FirstName=N'Анна', MiddleName=N'тестово', LastName=N'дете', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'at81264295', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'085bc4ae-fff6-4708-9ef7-29bd707ef1ed', SysUserType=NULL
WHERE PersonID=83753594;

UPDATE core.Person
SET FirstName=N'Жорето', MiddleName=N'тестов', LastName=N'Служител', PermanentAddress=N'ул." П.Яворов " N:10', PermanentTownID=32024, CurrentAddress=N'ул." П.Яворов " N:10', CurrentTownID=32024, PublicEduNumber=N'joreto.slizhitel', PersonalIDType=0, NationalityID=NULL, BirthDate='1962-02-15', BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=1, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'ea642e0e-8589-4075-a8a8-b583e4052102', SysUserType=NULL
WHERE PersonID=84376784;
UPDATE core.Person
SET FirstName=N'Йованито', MiddleName=N'тестов', LastName=N'Служител', PermanentAddress=N'Гео Милев 18', PermanentTownID=32024, CurrentAddress=N'Гео Милев 18', CurrentTownID=32024, PublicEduNumber=N'jovanito.slizhitel', PersonalIDType=0, NationalityID=NULL, BirthDate='1975-03-30', BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=1, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'ed2cdba4-1e42-49d5-8f86-f078dfbd1aa5', SysUserType=NULL
WHERE PersonID=84460164;

UPDATE core.Person
SET FirstName=N'Преслава', MiddleName=N'тестово', LastName=N'дете', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'pu20934921', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'2728ca77-4316-46fe-a753-d96f03b70dd0', SysUserType=NULL
WHERE PersonID=83853604;
UPDATE core.Person
SET FirstName=N'Надя', MiddleName=N'тестово', LastName=N'дете', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'nt59735113', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'6670cdce-f82e-40ea-bebf-9f88cad5ca0f', SysUserType=NULL
WHERE PersonID=83785474;
UPDATE core.Person
SET FirstName=N'Алишан', MiddleName=N'тестово', LastName=N'дете', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'at13935938', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'76112d7d-5037-4307-b5e8-8236a61b3789', SysUserType=NULL
WHERE PersonID=83675440;
UPDATE core.Person
SET FirstName=N'Борислав', MiddleName=N'тестово', LastName=N'дете', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'bt10998733', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'a280cd4b-ab35-49d8-87f1-2d206008a035', SysUserType=NULL
WHERE PersonID=83564184;
UPDATE core.Person
SET FirstName=N'Георги', MiddleName=N'тестово', LastName=N'дете', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'gv10934921', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'1a429d98-5cc8-45a0-bc27-03eff661f4e0', SysUserType=NULL
WHERE PersonID=83492213;
UPDATE core.Person
SET FirstName=N'Илияна', MiddleName=N'тестово', LastName=N'дете', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'it14843481', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'f159528f-010c-4b53-9ea8-1f8adc3cc30a', SysUserType=NULL
WHERE PersonID=83425185;
UPDATE core.Person
SET FirstName=N'Азис', MiddleName=N'тестово', LastName=N'дете', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'au20934921', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'02f99e21-c3d0-4b88-b939-fd06098b38fe', SysUserType=NULL
WHERE PersonID=84042629;
UPDATE core.Person
SET FirstName=N'Софи', MiddleName=N'тестово', LastName=N'дете', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'su20934921', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'9c2619e1-d2a6-44c8-9757-2db2d55a6e1e', SysUserType=NULL
WHERE PersonID=84105540;
UPDATE core.Person
SET FirstName=N'Константин', MiddleName=N'тестово', LastName=N'дете', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'ku20934921', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'34344218-6c0f-4ad1-ada6-8fbf973011fc', SysUserType=NULL
WHERE PersonID=83979773;
UPDATE core.Person
SET FirstName=N'Преслава', MiddleName=N'тестово', LastName=N'дете', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'pu30934921', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'9d5a25b1-8022-4329-801b-31a99a89771e', SysUserType=NULL
WHERE PersonID=83917117;

UPDATE core.Person
SET FirstName=N'Бобито', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'bobito.slizhitel', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'3229de57-9147-4794-94fd-e62079575e57', SysUserType=NULL
WHERE PersonID=84356885;
UPDATE core.Person
SET FirstName=N'Радито', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'radito.slizhitel', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'4db6e1f7-a336-4865-b3d7-3e3cb3e20a31', SysUserType=NULL
WHERE PersonID=84432771;
UPDATE core.Person
SET FirstName=N'Данито', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'danito.slizhitel', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'4fbe3f88-7d77-43f9-a441-e6a2b036a93a', SysUserType=NULL
WHERE PersonID=84394064;
UPDATE core.Person
SET FirstName=N'Катето', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'kateto.slizhitel', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'f674109f-bfe4-4c25-bb94-3aa3d4d17bf6', SysUserType=NULL
WHERE PersonID=84409870;
UPDATE core.Person
SET FirstName=N'Валето', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'valeto.slizhitel', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'5bff6e3b-2b25-4835-af30-0a8953958bff', SysUserType=NULL
WHERE PersonID=84435094;
UPDATE core.Person
SET FirstName=N'Азизето', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'azizeto.slizhitel', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'1c68cd62-8816-44c1-b932-11c83002496d', SysUserType=NULL
WHERE PersonID=84441895;
UPDATE core.Person
SET FirstName=N'Теменужка', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932, PublicEduNumber=N'temenujka.slizhitel', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, AzureID=N'2c872966-15d5-4fe5-b3dd-88fa8d015b7c', SysUserType=NULL
WHERE PersonID=84437656;
UPDATE core.Person
SET FirstName=N'Илиян', MiddleName=N'тестов', LastName=N'ученик', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932,
PublicEduNumber=N'iu20934921', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, 
SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, 
AzureID=N'3b42b443-0d41-4a92-bdd0-47dc2d157efb',
SysUserType=NULL
WHERE PersonID=84217223;

UPDATE core.Person
SET FirstName=N'Дамян', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'', PermanentTownID=66932, CurrentAddress=N'', CurrentTownID=66932,
PublicEduNumber=N'damian.slizhitel', PersonalIDType=0, NationalityID=34, BirthDate='2009-09-11', BirthPlaceTownID=40909, BirthPlaceCountry=34, Gender=2, 
SchoolBooksCodesID=N'udYeaZPf', BirthPlace=NULL, 
AzureID=N'43d5e556-4326-4f3e-9e4a-38da52e7945e',
SysUserType=NULL
WHERE PersonID=84375705;

UPDATE core.Person
SET FirstName=N'Меди', MiddleName=N'тестов', LastName=N'ученик', PermanentAddress=N'-', PermanentTownID=16359, CurrentAddress=N'-', CurrentTownID=16359, PublicEduNumber=N'mu20934921', PersonalIDType=0, NationalityID=34, PersonalID=N'1052151426', BirthDate='2010-12-15', BirthPlaceTownID=16359, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'SkafZTrv', BirthPlace=NULL, AzureID=N'65ff16dd-7aca-4fdc-9d14-b79826356667', SysUserType=NULL
WHERE PersonID=83848188;

UPDATE core.Person
SET FirstName=N'Галена', MiddleName=N'тестов', LastName=N'ученик', PermanentAddress=N'', PermanentTownID=73420, CurrentAddress=N'', CurrentTownID=73420, PublicEduNumber=N'gu30934921', PersonalIDType=0, NationalityID=34, PersonalID=N'0844217590', BirthDate='2008-04-21', BirthPlaceTownID=35167, BirthPlaceCountry=34, Gender=1, SchoolBooksCodesID=N'ueHfxjUf', BirthPlace=NULL, AzureID=N'f9b7ed53-f22e-428e-915c-b1398a021164', SysUserType=NULL
WHERE PersonID=83650105;

UPDATE core.Person
SET FirstName=N'Гери', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'ул."Георги Измирлиев" №6, вход Б, ап.51', PermanentTownID=10447, CurrentAddress=N'ул."Георги Измирлиев" №6, вход Б, ап.51', CurrentTownID=10447, PublicEduNumber=N'geri.sluzhitel', PersonalIDType=0, NationalityID=NULL, PersonalID=N'9006301414', BirthDate='1990-06-30', BirthPlaceTownID=NULL, BirthPlaceCountry=34, Gender=1, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'681d76b7-2650-4f48-9681-b39b297c202f', SysUserType=NULL
WHERE PersonID=84884032;

UPDATE core.Person
SET FirstName=N'Марти', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'ул. Синчец № 12', PermanentTownID=49494, CurrentAddress=N'ул. Синчец № 12', CurrentTownID=49494, PublicEduNumber=N'marti.sluzhitel', PersonalIDType=0, NationalityID=NULL, PersonalID=N'6806227774', BirthDate='1968-06-22', BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=1, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'bfc56be2-b65e-48a4-a1bb-2bc622c28889', SysUserType=NULL
WHERE PersonID=84414170;



UPDATE core.SysUser
SET Username=N'nt34590816@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83706310, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83706310;
UPDATE core.SysUser
SET Username=N'bt37995697@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83709334, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83709334;
UPDATE core.SysUser
SET Username=N'at81264295@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83753594, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83753594;
UPDATE core.SysUser
SET Username=N'joreto.slizhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84376784, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84376784;
UPDATE core.SysUser
SET Username=N'jovanito.slizhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84460164, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84460164;

UPDATE core.SysUser
SET Username=N'2999982@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=86226981, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=86226981;

UPDATE core.SysUser
SET Username=N'pu20934921@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83853604, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83853604;
UPDATE core.SysUser
SET Username=N'nt59735113@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83785474, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83785474;
UPDATE core.SysUser
SET Username=N'at13935938@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83675440, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83675440;
UPDATE core.SysUser
SET Username=N'bt10998733@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83564184, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83564184;
UPDATE core.SysUser
SET Username=N'gv10934921@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83492213, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83492213;
UPDATE core.SysUser
SET Username=N'it14843481@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83425185, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83425185;
UPDATE core.SysUser
SET Username=N'au20934921@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84042629, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84042629;
UPDATE core.SysUser
SET Username=N'su20934921@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84105540, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84105540;
UPDATE core.SysUser
SET Username=N'ku20934921@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83979773, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83979773;
UPDATE core.SysUser
SET Username=N'pu30934921@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83917117, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83917117;

UPDATE core.SysUser
SET Username=N'bobito.slizhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84356885, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84356885;
UPDATE core.SysUser
SET Username=N'radito.slizhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84432771, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84432771;
UPDATE core.SysUser
SET Username=N'danito.slizhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84394064, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84394064;
UPDATE core.SysUser
SET Username=N'kateto.slizhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84409870, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84409870;
UPDATE core.SysUser
SET Username=N'valeto.slizhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84435094, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84435094;
UPDATE core.SysUser
SET Username=N'azizeto.slizhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84441895, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84441895;
UPDATE core.SysUser
SET Username=N'temenujka.slizhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84437656, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84437656; 

UPDATE core.SysUser
SET Username=N'hristina.sluzhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83031814, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83031814;

UPDATE core.SysUser
SET Username=N'zdravko.sluzhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84349811, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84349811;
UPDATE core.SysUser
SET Username=N'dt76734102@edu.mon.bg', Password=N'n!nQ4%91Y91', IsAzureUser=1, PersonID=84077373, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84077373;
UPDATE core.SysUser
SET Username ='manuela.l.atanasova@edu.mon.b', DeletedOn=GETUTCDATE() 
WHERE PersonID=83035262;
UPDATE core.SysUser
SET Username=N'manuela.l.atanasova@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=85116474, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=85116474 AND DeletedOn IS NULL;

UPDATE core.SysUser
SET Username=N'marti.sluzhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84414170, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84414170;

UPDATE core.SysUser
SET Username=N'geri.sluzhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84884032, isAzureSynced=1, InitialPassword=N'wa4qmk1@xO', DeletedOn=NULL
WHERE PersonID=84884032 AND DeletedOn IS NULL;

UPDATE core.SysUser
SET Username=N'gu30934921@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83650105, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83650105;

UPDATE core.SysUser
SET Username=N'mu20934921@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83848188, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83848188;



UPDATE core.SysUser
SET Username=N'nadia.sluzhitel@edu.mon.bg'
WHERE SysUserID=1238177;
UPDATE core.SysUser
SET Username=N'diana.sluzhitel@edu.mon.bg'
WHERE SysUserID=55700;

UPDATE core.SysUser
SET Username=N'damian.sluzhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84375705, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84375705;

UPDATE core.SysUser
SET Username=N'iu20934921@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84217223, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84217223;

UPDATE core.Person
SET FirstName=N'Веселка', MiddleName=N'тестов', LastName=N'учител', PublicEduNumber=N'veselka.te.sluzhitel',PersonalID=N'1111250097', AzureID=N'e767d81f-3028-4072-8be8-9b7b48bfe9fa'
WHERE PersonID=84490987;

UPDATE core.Person
SET FirstName=N'Девето основно училище " Пейо Крачолов Яворов"', MiddleName=N'', LastName=N'', AzureID=N'98d7e63b-a815-4b3c-a550-4e2aeb15643e'
WHERE PersonID=86222957;

	DECLARE @OutputTbl1 TABLE (ID INT)
	DECLARE @OutputTbl2 TABLE (ID INT)
	DECLARE @OutputTbl3 TABLE (ID INT)
	DECLARE @OutputTbl4 TABLE (ID INT)
	DECLARE @OutputTbl5 TABLE (ID INT)
	DECLARE @OutputTbl6 TABLE (ID INT)
	DECLARE @OutputTbl7 TABLE (ID INT)
	DECLARE @OutputTbl8 TABLE (ID INT)
	DECLARE @OutputTbl9 TABLE (ID INT)
	DECLARE @OutputTbl10 TABLE (ID INT)
	DECLARE @OutputTbl11 TABLE (ID INT)
	DECLARE @OutputTbl12 TABLE (ID INT)
	DECLARE @OutputTbl13 TABLE (ID INT)
	DECLARE @OutputTbl14 TABLE (ID INT)

INSERT INTO core.Person
( FirstName, MiddleName, LastName, PermanentAddress, PermanentTownID, CurrentAddress, CurrentTownID, PublicEduNumber, PersonalIDType, NationalityID, PersonalID, BirthDate, BirthPlaceTownID, BirthPlaceCountry, Gender, SchoolBooksCodesID, BirthPlace, AzureID, SysUserType)
OUTPUT INSERTED.PersonID INTO @OutputTbl1(ID)
VALUES( N'Родител', N'', N'БезДеца', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'38dfc2d8-46d2-46ae-b819-b4dce55ec829', NULL);
INSERT INTO core.Person
( FirstName, MiddleName, LastName, PermanentAddress, PermanentTownID, CurrentAddress, CurrentTownID, PublicEduNumber, PersonalIDType, NationalityID, PersonalID, BirthDate, BirthPlaceTownID, BirthPlaceCountry, Gender, SchoolBooksCodesID, BirthPlace, AzureID, SysUserType)
OUTPUT INSERTED.PersonID INTO @OutputTbl2(ID)
VALUES( N'Родител', N'', N'ЕдноДетеЕднаИнст', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'4c89ceec-948b-40dc-9f02-e9a4a0b15d6a', NULL);
INSERT INTO core.Person
( FirstName, MiddleName, LastName, PermanentAddress, PermanentTownID, CurrentAddress, CurrentTownID, PublicEduNumber, PersonalIDType, NationalityID, PersonalID, BirthDate, BirthPlaceTownID, BirthPlaceCountry, Gender, SchoolBooksCodesID, BirthPlace, AzureID, SysUserType)
OUTPUT INSERTED.PersonID INTO @OutputTbl3(ID)
VALUES( N'Родител', N'', N'ЕдноДете', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'98ad1001-d76c-4d1d-ad3c-8575d8496ac9', NULL);
INSERT INTO core.Person
( FirstName, MiddleName, LastName, PermanentAddress, PermanentTownID, CurrentAddress, CurrentTownID, PublicEduNumber, PersonalIDType, NationalityID, PersonalID, BirthDate, BirthPlaceTownID, BirthPlaceCountry, Gender, SchoolBooksCodesID, BirthPlace, AzureID, SysUserType)
OUTPUT INSERTED.PersonID INTO @OutputTbl4(ID)
VALUES( N'Родител', N'', N'ДвеДецаДвеИнст', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'c94c2945-350a-4949-8a00-630d77200932', NULL);

INSERT INTO core.Person
( FirstName, MiddleName, LastName, PermanentAddress, PermanentTownID, CurrentAddress, CurrentTownID, PublicEduNumber, PersonalIDType, NationalityID, PersonalID, BirthDate, BirthPlaceTownID, BirthPlaceCountry, Gender, SchoolBooksCodesID, BirthPlace, AzureID, SysUserType)
OUTPUT INSERTED.PersonID INTO @OutputTbl5(ID)
VALUES( N'Родител', N'', N'Дведеца', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'fe6b750a-f1ea-4fc9-9ff0-d5b7c8c22db3', NULL);

INSERT INTO core.Person
( FirstName, MiddleName, LastName, PermanentAddress, PermanentTownID, CurrentAddress, CurrentTownID, PublicEduNumber, PersonalIDType, NationalityID, PersonalID, BirthDate, BirthPlaceTownID, BirthPlaceCountry, Gender, SchoolBooksCodesID, BirthPlace, AzureID, SysUserType)
OUTPUT INSERTED.PersonID INTO @OutputTbl6(ID)
VALUES( N'Родител', N'', N'ДвеДецаЕднаДвеИнст', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'465cb063-99db-4ed6-95af-6a87a4eeff1f', NULL);

DECLARE @InsertedID1 int = (SELECT TOP 1 ID FROM @OutputTbl1);
DECLARE @InsertedID2 int = (SELECT TOP 1 ID FROM @OutputTbl2);
DECLARE @InsertedID3 int = (SELECT TOP 1 ID FROM @OutputTbl3);
DECLARE @InsertedID4 int = (SELECT TOP 1 ID FROM @OutputTbl4);
DECLARE @InsertedID5 int = (SELECT TOP 1 ID FROM @OutputTbl5);
DECLARE @InsertedID6 int = (SELECT TOP 1 ID FROM @OutputTbl6);

INSERT INTO core.SysUser
( Username, Password, IsAzureUser, PersonID, isAzureSynced, InitialPassword, DeletedOn)
OUTPUT INSERTED.SysUserID INTO @OutputTbl7(ID)
VALUES( N'parent0@abv.bg', NULL, 1, @InsertedID1, 1, N'iNnEJ3HmxdULV/WS5Cv6bFXVZJ3RpEtCOhXtJLnzuQtarDMS9wwrXd6gcgNRTChDj/w9OHdSVCTy5LsUSHSvizarf4Zf4kPRvP1gy4o9UvnSHnTrmKlM4Ao790WLCoBq5HgrPXKCgefA+gkdauA8UJVjbxHMEuqaYmASqBgFNWA=', NULL);
INSERT INTO core.SysUser
( Username, Password, IsAzureUser, PersonID, isAzureSynced, InitialPassword, DeletedOn)
OUTPUT INSERTED.SysUserID INTO @OutputTbl8(ID)
VALUES( N'parent1@abv.bg', NULL, 1, @InsertedID2, 1, N'JIpucD3i5Cf8B1BMByxvRicoLVXUPmvpXGdLaYGa10ZvNKmp7rb8E9RmASbnrpDyLveWAY/vFqpIvQTh1iDDZmZgEDjXw/DPNjIZmgF+CBcXlMxbBZtfbAx02SycVdI6xtVOBULI1J/kL2btIBCAM23cEwyEPYT40POdmQQNAFs=', NULL);
INSERT INTO core.SysUser
( Username, Password, IsAzureUser, PersonID, isAzureSynced, InitialPassword, DeletedOn)
OUTPUT INSERTED.SysUserID INTO @OutputTbl9(ID)
VALUES( N'parent11@abv.bg', NULL, 1, @InsertedID3, 1, N'l37q3GE9IFuDEHbSxZMlAfgquk5fLjOn4hLWXIxuIa1KS+MIuD8QlmNXCBU1WAHM13vKUC3nyVMVhYXQwauVngCuPs3awpgj5BStrAMIiXwg581VeuH8PeRXniUD8S0Adgr4pp/ICikM03SsijXEzLqA9CT+jAWLZ2BWBxsdDxE=', NULL);
INSERT INTO core.SysUser
( Username, Password, IsAzureUser, PersonID, isAzureSynced, InitialPassword, DeletedOn)
OUTPUT INSERTED.SysUserID INTO @OutputTbl10(ID)
VALUES( N'parent122@abv.bg', NULL, 1, @InsertedID4, 1, N'Cdhp9W6NTq1imRVXIjvkVmzpgeJrr8XnfIg+V2EnHlWLlLUE6dQTrtx8/v0NxboH4de6BzUGafpp0MioS3imX1E2vidkFGCkgFj/C8rMecvl5YoXqPKphP8dBSCIqHDTGrf/e8PuZbK/xUgYRFRXwxKv6Ehc9tY37xL6O1ZTH80=', NULL);
INSERT INTO core.SysUser
( Username, Password, IsAzureUser, PersonID, isAzureSynced, InitialPassword, DeletedOn)
OUTPUT INSERTED.SysUserID INTO @OutputTbl11(ID)
VALUES( N'parent2@abv.bg', NULL, 1, @InsertedID5, 1, N'J8wWy9Z/mFv+zpXh1P2/GvbILBEuqs636wcXg0QUsibDXPdhGnDnITja63Ej2Mx1k710xCdy/u0fGM50zWUizVOcFYJGsMBEZ/cVJmupb4N+NBTd1kMuIiWYZ1QRR3IYdI2eUjYqdqGd8UjA4kSg9vs/ofuJWM81UOY7zgVcCUI=', NULL);
INSERT INTO core.SysUser
( Username, Password, IsAzureUser, PersonID, isAzureSynced, InitialPassword, DeletedOn)
OUTPUT INSERTED.SysUserID INTO @OutputTbl12(ID)
VALUES( N'parent22@abv.bg', NULL, 1, @InsertedID6, 1, N'Cb5evtSVvW2lbNxESofm8akdOyefPjczqKMipUcAuJGCdMllPYvUXpnmIbTVTrworwUMG2vAwXfSxpiE+GAG4MzAbXMcGfvdRC1CCsDACwNn2xRLoKZRXwcSikxFlmKR/T2HSIyVx7PUulCNYoUo9W3/uzXInOI5au/j5Rb8AUs=', NULL);


DECLARE @InsertedID7 int = (SELECT TOP 1 ID FROM @OutputTbl7);
DECLARE @InsertedID8 int = (SELECT TOP 1 ID FROM @OutputTbl8);
DECLARE @InsertedID9 int = (SELECT TOP 1 ID FROM @OutputTbl9);
DECLARE @InsertedID10 int = (SELECT TOP 1 ID FROM @OutputTbl10);
DECLARE @InsertedID11 int = (SELECT TOP 1 ID FROM @OutputTbl11);
DECLARE @InsertedID12 int = (SELECT TOP 1 ID FROM @OutputTbl12);

INSERT INTO core.SysUserSysRole
(SysUserID, SysRoleID, InstitutionID, BudgetingInstitutionID, MunicipalityID, RegionID)
VALUES(@InsertedID7, 7, NULL, NULL, NULL, NULL);
INSERT INTO core.SysUserSysRole
(SysUserID, SysRoleID, InstitutionID, BudgetingInstitutionID, MunicipalityID, RegionID)
VALUES(@InsertedID8, 7, NULL, NULL, NULL, NULL);
INSERT INTO core.SysUserSysRole
(SysUserID, SysRoleID, InstitutionID, BudgetingInstitutionID, MunicipalityID, RegionID)
VALUES(@InsertedID9, 7, NULL, NULL, NULL, NULL);
INSERT INTO core.SysUserSysRole
(SysUserID, SysRoleID, InstitutionID, BudgetingInstitutionID, MunicipalityID, RegionID)
VALUES(@InsertedID10, 7, NULL, NULL, NULL, NULL);
INSERT INTO core.SysUserSysRole
(SysUserID, SysRoleID, InstitutionID, BudgetingInstitutionID, MunicipalityID, RegionID)
VALUES(@InsertedID11, 7, NULL, NULL, NULL, NULL);
INSERT INTO core.SysUserSysRole
(SysUserID, SysRoleID, InstitutionID, BudgetingInstitutionID, MunicipalityID, RegionID)
VALUES(@InsertedID12, 7, NULL, NULL, NULL, NULL);


INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 83737069, @InsertedID2, 1); --elica

INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 83297142, @InsertedID3, 1); --stoqn

INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 83297142, @InsertedID4, 1); --stoqn
INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 83857493, @InsertedID4, 1); --kristiana

INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 83737069, @InsertedID5, 1); --elitsa
INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 83724086, @InsertedID5, 1); --ekaterina

INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 83737069, @InsertedID6, 1); --elitsa
INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 83297142, @InsertedID6, 1); --stoqn

INSERT INTO core.Person
( FirstName, MiddleName, LastName, PermanentAddress, PermanentTownID, CurrentAddress, CurrentTownID, PublicEduNumber, PersonalIDType, NationalityID, PersonalID, BirthDate, BirthPlaceTownID, BirthPlaceCountry, Gender, SchoolBooksCodesID, BirthPlace, AzureID, SysUserType)
OUTPUT INSERTED.PersonID INTO @OutputTbl13(ID)
VALUES( N'Средно училище Христо Ботев', N'', N'', NULL, NULL, NULL, NULL, N'85863898', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'6298f12e-643d-46e9-9d1e-49274ed73286', NULL);

DECLARE @InsertedID13 int = (SELECT TOP 1 ID FROM @OutputTbl13);
INSERT INTO core.SysUser
(Username, Password, IsAzureUser, PersonID, isAzureSynced, InitialPassword, DeletedOn)
OUTPUT INSERTED.SysUserID INTO @OutputTbl14(ID)
VALUES(N'2999984@edu.mon.bg', NULL, 1, @InsertedID13, 1, NULL, NULL);

DECLARE @InsertedID14 int = (SELECT TOP 1 ID FROM @OutputTbl14);

INSERT INTO core.SysUserSysRole
(SysUserID, SysRoleID, InstitutionID, BudgetingInstitutionID, MunicipalityID, RegionID)
VALUES(@InsertedID14, 0, 101700, NULL, NULL, NULL);

UPDATE core.SysUser
SET Username=N'vu-vera.shcherbatiuk@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=86246789, isAzureSynced=1, InitialPassword=N'm9n%j63*R0', DeletedOn=NULL
WHERE SysUserID=1161809;
UPDATE core.Person
SET FirstName=N'Вера', MiddleName=N'', LastName=N'Ганева', PermanentAddress=NULL, PermanentTownID=NULL, CurrentAddress=NULL, CurrentTownID=NULL, PublicEduNumber=N'vu-vera.shcherbatiuk', PersonalIDType=NULL, NationalityID=NULL, PersonalID=N'9999999135', BirthDate=NULL, BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=NULL, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'4e99de38-d627-4e70-bc0d-90b3498e0bdd', SysUserType=NULL
WHERE PersonID=86246789;
UPDATE core.SysUser
SET Username=N'vu-tihon.rizhikov@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=86246798, isAzureSynced=1, InitialPassword=N'7rQ_VD6xqI', DeletedOn=NULL
WHERE SysUserID=1161819;
UPDATE  core.Person
SET FirstName=N'Тихон', MiddleName=N'', LastName=N'Димитрова', PermanentAddress=NULL, PermanentTownID=NULL, CurrentAddress=NULL, CurrentTownID=NULL, PublicEduNumber=N'vu-tihon.rizhikov', PersonalIDType=NULL, NationalityID=NULL, PersonalID=N'9999999144', BirthDate=NULL, BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=NULL, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'db3bf919-fb36-4ca7-ac2f-db53f5b1fae1', SysUserType=NULL
WHERE PersonID=86246798;

DECLARE @OutputTbl15 TABLE (ID INT)
DECLARE @OutputTbl16 TABLE (ID INT)

	INSERT INTO core.Person
( FirstName, MiddleName, LastName, PermanentAddress, PermanentTownID, CurrentAddress, CurrentTownID, PublicEduNumber, PersonalIDType, NationalityID, PersonalID, BirthDate, BirthPlaceTownID, BirthPlaceCountry, Gender, SchoolBooksCodesID, BirthPlace, AzureID, SysUserType)
OUTPUT INSERTED.PersonID INTO @OutputTbl15(ID)
VALUES( N'Алекс', N'', N'Родител', NULL, NULL, NULL, NULL, 'alekszarev@aol.com', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1425ee53-f435-42c2-973e-ab63efe80ded', NULL);

DECLARE @InsertedID15 int = (SELECT TOP 1 ID FROM @OutputTbl15);

INSERT INTO core.SysUser
( Username, Password, IsAzureUser, PersonID, isAzureSynced, InitialPassword, DeletedOn)
OUTPUT INSERTED.SysUserID INTO @OutputTbl16(ID)
VALUES( N'alekszarev@aol.com', NULL, 1, @InsertedID15, 1, N'iNnEJ3HmxdULV/WS5Cv6bFXVZJ3RpEtCOhXtJLnzuQtarDMS9wwrXd6gcgNRTChDj/w9OHdSVCTy5LsUSHSvizarf4Zf4kPRvP1gy4o9UvnSHnTrmKlM4Ao790WLCoBq5HgrPXKCgefA+gkdauA8UJVjbxHMEuqaYmASqBgFNWA=', NULL);

DECLARE @InsertedID16 int = (SELECT TOP 1 ID FROM @OutputTbl16);
INSERT INTO core.SysUserSysRole
(SysUserID, SysRoleID, InstitutionID, BudgetingInstitutionID, MunicipalityID, RegionID)
VALUES(@InsertedID16, 7, NULL, NULL, NULL, NULL);

INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 84217223, @InsertedID15, 1); --ilian
INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 83737069, @InsertedID15, 1);--elica
INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 83848188, @InsertedID15, 1);--medi
INSERT INTO core.ParentChildSchoolBookAccess
( ChildID, ParentID, HasAccess)
VALUES( 83650105, @InsertedID15, 1); --galena

    
               
UPDATE core.Person SET AzureID=N'6ab4c7db-29ec-4eed-834d-2ce459137583'
WHERE PersonID=85795526; 

UPDATE core.Person SET AzureID=N'8ca10f66-f60d-45f1-9702-979983764955'
WHERE PersonID=84437165; 


UPDATE core.Person SET AzureID=N'cdeb63c2-e865-45b6-bfc8-f10add0cd9f8'
WHERE PersonID=85359010;

UPDATE core.Person SET AzureID=N'a94a52dc-dd03-4980-8835-5a00df65029d'
WHERE PersonID=83068983; 


UPDATE core.Person SET AzureID=N'f5918729-36e9-405b-b1fc-4e87b092098b'
WHERE PersonID=84403926; 


UPDATE core.Person SET AzureID=N'1cc18d85-cec2-4487-a3e1-44f26f901b66'
WHERE PersonID=84805496; 

UPDATE core.Person SET AzureID=N'2f851ae6-c562-4265-8e92-96e5e45f205d'
WHERE PersonID=85780519; 

UPDATE core.Person SET AzureID=N'e7f2085e-cc13-4146-8af8-34f6ea566c04'
WHERE PersonID=85088321; 

UPDATE core.Person SET AzureID=N'cba36b0b-48fc-4738-88f9-66b7bc40f0de'
WHERE PersonID=83070508; 

UPDATE core.Person SET AzureID=N'da96381e-aa8a-471b-9796-c7f01f7472eb'
WHERE PersonID=84468916; 

UPDATE core.Person SET AzureID=N'3c8bd4f3-8d02-4d38-9cde-f0faf078e20b'
WHERE PersonID=84605893; 

UPDATE core.Person SET AzureID=N'32f59a87-ded1-4b30-a81c-054f9f558b01'
WHERE PersonID=84419498; 

UPDATE core.Person SET AzureID=N'c9b21dc1-8074-4cfb-96e2-f878992aae4a'
WHERE PersonID=84460514; 

UPDATE core.Person SET AzureID=N'81326e8f-7ea2-4d32-9206-1c26083f1e83'
WHERE PersonID=84457980; 

UPDATE core.Person SET AzureID=N'c67b3817-6a83-495f-85a1-bbe98f70142f'
WHERE PersonID=84543192; 

UPDATE core.Person SET AzureID=N'9767f672-4901-457b-a360-7be3219ce8d5'
WHERE PersonID=85794852;

UPDATE core.Person SET AzureID=N'bd49f3dc-19f8-400e-90f6-d78ae9d44832'
WHERE PersonID=84377175; 

UPDATE core.Person SET AzureID=N'61ba899e-a2ea-41fb-b71b-744d86782841'
WHERE PersonID=84436511; 

UPDATE core.Person SET AzureID=N'abff94ac-e0cc-4bad-b96a-d38690221c94'
WHERE PersonID=85566173; 

UPDATE core.Person SET AzureID=N'557e5da7-c6b7-42e5-8935-7ac9b5247331'
WHERE PersonID=84473637; 

UPDATE core.Person SET AzureID=N'607cba0c-65f6-48fa-a9cd-303bed3abaee'
WHERE PersonID=84624653; 

UPDATE core.Person SET AzureID=N'5d3ac288-c63a-4fef-9db4-53b63eca9457'
WHERE PersonID=84502787; 



UPDATE core.SysUser
SET Username=N'tu-varna@edu.mon.bg'
WHERE PersonID=85795526;
     
UPDATE core.SysUser
SET Username=N'tu-varna@edu.mon.bg'
WHERE PersonID=85795526; 

UPDATE core.SysUser
SET Username=N'obo.izoblok@edu.mon.bg'
WHERE PersonID=84437165; 


UPDATE core.SysUser
SET Username=N'obo.institute-hr@edu.mon.bg'
WHERE PersonID=85359010;

UPDATE core.SysUser
SET Username=N'obo.informaconsult@edu.mon.bg'
WHERE PersonID=83068983; 


UPDATE core.SysUser
SET Username=N'obo.matev-stroy@edu.mon.bg'
WHERE PersonID=84403926; 

UPDATE core.SysUser
SET Username=N'obo.new-education-center@edu.mon.bg'
WHERE PersonID=84805496; 

UPDATE core.SysUser
SET Username=N'obo.orak1@edu.mon.bg'
WHERE PersonID=85780519; 

UPDATE core.SysUser
SET Username=N'obo.prosveta-sofia@edu.mon.bg'
WHERE PersonID=85088321; 

UPDATE core.SysUser
SET Username=N'obo.raabe5@edu.mon.bg'
WHERE PersonID=83070508; 

UPDATE core.SysUser
SET Username=N'obo.center-lang-mng@edu.mon.bg'
WHERE PersonID=84468916; 

UPDATE core.SysUser
SET Username=N'obo.interactivebg1@edu.mon.bg'
WHERE PersonID=84605893; 

UPDATE core.SysUser
SET Username=N'obo.nadejda-crd@edu.mon.bg'
WHERE PersonID=84419498; 

UPDATE core.SysUser
SET Username=N'obo.obrazovanie-bezgranici@edu.mon.bg'
WHERE PersonID=84460514; 

UPDATE core.SysUser
SET Username=N'obo.cei-bg@edu.mon.bg'
WHERE PersonID=84457980; 

UPDATE core.SysUser
SET Username=N'uni-zlatarov-burgas@edu.mon.bg'
WHERE PersonID=84543192; 

UPDATE core.SysUser
SET Username=N'nsa-sofia@edu.mon.bg'
WHERE PersonID=85794852;

UPDATE core.SysUser
SET Username=N'shumenski-uni@edu.mon.bg'
WHERE PersonID=84377175; 

UPDATE core.SysUser
SET Username=N'ruse-uni@edu.mon.bg'
WHERE PersonID=84436511; 

UPDATE core.SysUser
SET Username=N'ltu-sofia@edu.mon.bg'
WHERE PersonID=85566173; 

UPDATE core.SysUser
SET Username=N'vuzf-sofia@edu.mon.bg'
WHERE PersonID=84473637; 

UPDATE core.SysUser
SET Username=N'bfu-burgas@edu.mon.bg'
WHERE PersonID=84624653; 

UPDATE core.SysUser
SET Username=N'sofia-uni@edu.mon.bg'
WHERE PersonID=84502787; 

UPDATE core.Person
SET FirstName=N'Димана', MiddleName=N'тестов', LastName=N'ученик', PermanentAddress=N'Багоевград, жк Ален мак бл.33 вх.А ет.3 ап.8', PermanentTownID=4279, CurrentAddress=N'Багоевград, жк Ален мак бл.33 вх.А ет.3 ап.8', CurrentTownID=4279, PublicEduNumber=N'dt81762252', PersonalIDType=0, NationalityID=34, PersonalID=N'1111100047', BirthDate='2010-08-30', BirthPlaceTownID=4279, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'ZnrnKnYG', BirthPlace=NULL, AzureID=N'c15be5d3-4bb8-4291-8c2b-def3ceca777e', SysUserType=NULL
WHERE PersonID=83765677;

UPDATE core.Person
SET FirstName=N'Йоана', MiddleName=N'Силванова', LastName=N'Димитрова', PermanentAddress=N'Предел №6', PermanentTownID=4279, CurrentAddress=N'Предел №6', CurrentTownID=4279, PublicEduNumber=N'yd61067886', PersonalIDType=0, NationalityID=34, PersonalID=N'0951070017', BirthDate='2009-11-07', BirthPlaceTownID=4279, BirthPlaceCountry=34, Gender=1, SchoolBooksCodesID=N'dE5f7yKY', BirthPlace=NULL, AzureID=N'e240f070-3cc8-425f-8828-34b578d368a6', SysUserType=NULL
WHERE PersonID=83827866;

UPDATE core.SysUser
SET Username=N'dt817622521@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83765677, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83765677;

UPDATE core.SysUser
SET Username=N'yd61067886@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83827866, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83827866;

UPDATE core.SysUser
SET Username=N'dt81762252@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83765677, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83765677;

UPDATE core.Person
SET FirstName=N'Eкатерина', MiddleName=N'тестов', LastName=N'ученик', PermanentAddress=N'', PermanentTownID=4279, CurrentAddress=N'', CurrentTownID=4279, PublicEduNumber=N'et73946789', PersonalIDType=0, NationalityID=34, PersonalID=N'1111365566', BirthDate='2009-04-26', BirthPlaceTownID=4279, BirthPlaceCountry=34, Gender=2, SchoolBooksCodesID=N'8wzhCg0W', BirthPlace=NULL, AzureID=N'a1b660b1-1a3f-4097-b26d-f567336eafcd', SysUserType=NULL
WHERE PersonID=83787844;
 
UPDATE core.Person
SET FirstName=N'Емилия', MiddleName=N'Владимирова', LastName=N'Радкова', PermanentAddress=N'Предел №19', PermanentTownID=4279, CurrentAddress=N'Предел №19', CurrentTownID=4279, PublicEduNumber=N'er20012426', PersonalIDType=0, NationalityID=34, PersonalID=N'1042010014', BirthDate='2010-02-01', BirthPlaceTownID=4279, BirthPlaceCountry=34, Gender=1, SchoolBooksCodesID=N'KjRS5bGg', BirthPlace=NULL, AzureID=N'79effb92-39cd-4d6b-8b3e-332a5e5ff586', SysUserType=NULL
WHERE PersonID=83724086;

UPDATE core.SysUser
SET Username=N'et739467891@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83787844, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83787844;

UPDATE core.SysUser
SET Username=N'er20012426@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83724086, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83724086;

UPDATE core.SysUser
SET Username=N'et73946789@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83787844, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83787844;

UPDATE core.Person
SET FirstName=N'Дули', MiddleName=N'тестов', LastName=N'учител', PermanentAddress=N'няма информация', PermanentTownID=68134, CurrentAddress=N'няма информация', CurrentTownID=68134, PublicEduNumber=N'duli.sluzhitel', PersonalIDType=0, NationalityID=NULL, PersonalID=N'6201057230', BirthDate='1962-01-05', BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=1, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'3499ba0e-6255-40f3-9c1e-a36fbcb9816d', SysUserType=NULL
WHERE PersonID=84375991;

UPDATE core.SysUser
SET Username=N'duli.sluzhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84375991, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84375991;

UPDATE core.Person
SET FirstName=N'Лъчи', MiddleName=N'тестов', LastName=N'учител', PermanentAddress=N'ул. Димитър Константинов № 20А, ет. 2, ап. 6', PermanentTownID=56722, CurrentAddress=N'ул. Димитър Константинов № 20А, ет. 2, ап. 6', CurrentTownID=56722, PublicEduNumber=N'lachi.sluzhitel', PersonalIDType=0, NationalityID=34, PersonalID=N'6110144078', BirthDate='1961-10-14', BirthPlaceTownID=63327, BirthPlaceCountry=34, Gender=1, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'605eb991-f427-4184-b844-888fc038f0f2', SysUserType=NULL
WHERE PersonID=84374595;

UPDATE core.SysUser
SET Username=N'lachi.sluzhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84374595, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84374595;

UPDATE core.Person
SET FirstName=N'Галин', MiddleName=N'тестов', LastName=N'ученик', PermanentAddress=N'гр. Плевен, ул. Хаджи Димитър 57 вх. В ет. 1 ап. 4', PermanentTownID=56722, CurrentAddress=N'гр. Плевен, ул. Хаджи Димитър 57 вх. В ет. 1 ап. 4', CurrentTownID=56722, PublicEduNumber=N'gu20934921', PersonalIDType=0, NationalityID=34, PersonalID=N'0641203976', BirthDate='2006-01-20', BirthPlaceTownID=56722, BirthPlaceCountry=34, Gender=1, SchoolBooksCodesID=N'cdxnFwTc', BirthPlace=NULL, AzureID=N'd601a084-2453-4fbc-8f17-522a4022b3f2', SysUserType=NULL
WHERE PersonID=83490979;

UPDATE core.SysUser
SET Username=N'gu20934921@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83490979, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE  PersonID=83490979;

UPDATE core.Person
SET FirstName=N'Галена', MiddleName=N'тестов', LastName=N'ученик', PermanentAddress=N'гр. Плевен, ул. Хаджи Димитър 57 вх. В ет. 1 ап. 4', PermanentTownID=56722, CurrentAddress=N'гр. Плевен, ул. Хаджи Димитър 57 вх. В ет. 1 ап. 4', CurrentTownID=56722, PublicEduNumber=N'gu30934921', 
PersonalIDType=0, NationalityID=34, PersonalID=N'06412039762', BirthDate='2006-01-20', BirthPlaceTownID=56722, BirthPlaceCountry=34, Gender=1, 
SchoolBooksCodesID=N'cdxnFwTc', BirthPlace=NULL, AzureID=N'f9b7ed53-f22e-428e-915c-b1398a021164', SysUserType=NULL
WHERE PersonID=83737069;

UPDATE core.Person
SET FirstName=N'Галената', MiddleName=N'тестов', LastName=N'ученик', PermanentAddress=N'', PermanentTownID=73420, CurrentAddress=N'', CurrentTownID=73420, PublicEduNumber=N'gu30934921', PersonalIDType=0, NationalityID=34, PersonalID=N'0844217590', BirthDate='2008-04-21', BirthPlaceTownID=35167, BirthPlaceCountry=34, Gender=1, SchoolBooksCodesID=N'ueHfxjUf', BirthPlace=NULL, AzureID=N'f9b7ed53-f22e-428e-915c-b1398a02116454', SysUserType=NULL
WHERE PersonID=83650105;

UPDATE core.SysUser
SET Username=N'gu309349211@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83650105, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=83650105;

UPDATE core.SysUser
SET Username=N'gu30934921@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=83737069, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE  PersonID=83737069;

INSERT INTO [neispuo-test].student.StudentClass

(SchoolYear, ClassId, PersonId, StudentSpecialityId, StudentEduFormId, ClassNumber, Status, IsIndividualCurriculum, IsHourlyOrganization, IsForSubmissionToNRA, IsCurrent, RepeaterId, CommuterTypeId, HasSuportiveEnvironment, SupportiveEnvironment, EnrollmentDate, AdmissionDocumentId, PositionId, BasicClassId, ClassTypeId, newClassId, OldClassId, FromStudentClassId, DischargeReasonId, DischargeDocumentId, RelocationDocumentId, ORESTypeId, IsFTACOutsourced, InstitutionId, IsNotPresentForm, ExternalID, ToDeleteByAdmin, EntryDate, DischargeDate)

VALUES(2005, 4478437, 86612007, -1, -1, NULL, 1, NULL, NULL, 1, 0, 1, 1, NULL, NULL, '2005-09-15 00:00:00.000', NULL, 8, 23, 35, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 101700, 0, NULL, 0, NULL, NULL);
 
 
INSERT INTO [neispuo-test].student.StudentClass

(SchoolYear, ClassId, PersonId, StudentSpecialityId, StudentEduFormId, ClassNumber, Status, IsIndividualCurriculum, IsHourlyOrganization, IsForSubmissionToNRA, IsCurrent, RepeaterId, CommuterTypeId, HasSuportiveEnvironment, SupportiveEnvironment, EnrollmentDate, AdmissionDocumentId, PositionId, BasicClassId, ClassTypeId, newClassId, OldClassId, FromStudentClassId, DischargeReasonId, DischargeDocumentId, RelocationDocumentId, ORESTypeId, IsFTACOutsourced, InstitutionId, IsNotPresentForm, ExternalID, ToDeleteByAdmin, EntryDate, DischargeDate)

VALUES(2005, 4478437, 83765677, -1, -1, NULL, 1, NULL, NULL, 1, 0, 1, 1, NULL, NULL, '2005-09-15 00:00:00.000', NULL, 8, 23, 35, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 101700, 0, NULL, 0, NULL, NULL);

INSERT INTO [neispuo-test].student.StudentClass
(SchoolYear, ClassId, PersonId, StudentSpecialityId, StudentEduFormId, ClassNumber, Status, IsIndividualCurriculum, IsHourlyOrganization, IsForSubmissionToNRA, IsCurrent, RepeaterId, CommuterTypeId, HasSuportiveEnvironment, SupportiveEnvironment, EnrollmentDate, AdmissionDocumentId, PositionId, BasicClassId, ClassTypeId, newClassId, OldClassId, FromStudentClassId, DischargeReasonId, DischargeDocumentId, RelocationDocumentId, ORESTypeId, IsFTACOutsourced, InstitutionId, IsNotPresentForm, ExternalID, ToDeleteByAdmin, EntryDate, DischargeDate)
VALUES(2024, 5019676, 86612007, -1, 1, 13, 1, 0, 0, 1, 1, 1, 1, 0, N'', '2024-08-08 00:00:00.000', NULL, 3, 6, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 101700, 0, NULL, 0, '2024-08-08', NULL);

INSERT INTO [neispuo-test].student.StudentClass
(SchoolYear, ClassId, PersonId, StudentSpecialityId, StudentEduFormId, ClassNumber, Status, IsIndividualCurriculum, IsHourlyOrganization, IsForSubmissionToNRA, IsCurrent, RepeaterId, CommuterTypeId, HasSuportiveEnvironment, SupportiveEnvironment, EnrollmentDate, AdmissionDocumentId, PositionId, BasicClassId, ClassTypeId, newClassId, OldClassId, FromStudentClassId, DischargeReasonId, DischargeDocumentId, RelocationDocumentId, ORESTypeId, IsFTACOutsourced, InstitutionId, IsNotPresentForm, ExternalID, ToDeleteByAdmin, EntryDate, DischargeDate)
VALUES(2024, 5019676, 83765677, -1, 1, 13, 1, 0, 0, 1, 1, 1, 1, 0, N'', '2024-08-08 00:00:00.000', NULL, 3, 6, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 101700, 0, NULL, 0, '2024-08-08', NULL);
 
 
--et73946789@edu.mon.bg
 
UPDATE [neispuo-test].core.EducationalState
SET PersonID=83787844, InstitutionID=101700, PositionID=3
WHERE PersonID=83787844;
 
 
--margarita.sluzhitel@edu.mon.bg
 
SELECT x.* FROM [neispuo-test].inst_basic.StaffPosition x
WHERE x.PersonID = 84417399
 
--et73946789@edu.mon.bg
 
INSERT INTO [neispuo-test].inst_year.CurriculumStudent
(CurriculumID, StudentID, PersonID, ValidFrom, ValidTo, SysUserID, SchoolYear, ExternalID, IsValid, isAzureEnrolled, WeeksFirstTerm, HoursWeeklyFirstTerm, WeeksSecondTerm, HoursWeeklySecondTerm)
VALUES(19210973, 83787844, 83976800, '2024-09-12 10:06:56.968', '9999-12-31 23:59:59.999', 865090, 2024, NULL, 1, 1, 18, 5.0, 16, 5.0);
 
 
INSERT INTO [neispuo-test].inst_year.CurriculumStudent
(CurriculumID, StudentID, PersonID, ValidFrom, ValidTo, SysUserID, SchoolYear, ExternalID, IsValid, isAzureEnrolled, WeeksFirstTerm, HoursWeeklyFirstTerm, WeeksSecondTerm, HoursWeeklySecondTerm)
VALUES(19210973, 83765677, 83976800, '2024-09-12 10:06:56.968', '9999-12-31 23:59:59.999', 865090, 2024, NULL, 1, 1, 18, 5.0, 16, 5.0);
 
INSERT INTO [neispuo-test].core.Person
( FirstName, MiddleName, LastName, PermanentAddress, PermanentTownID, CurrentAddress, CurrentTownID, PublicEduNumber, PersonalIDType, NationalityID, PersonalID, BirthDate, BirthPlaceTownID, BirthPlaceCountry, Gender, SchoolBooksCodesID, BirthPlace, AzureID, SysUserType)
VALUES( N'Елица', N'тестов', N'ученик', N'', 4279, N'', 4279, N'et78453768', 0, 34, N'1111290001', '2009-06-29', 4279, 34, 2, N'pze6LWEP', NULL, N'447502a5-29e2-43fd-aec7-5397fbd02d13', NULL);
 
INSERT INTO [neispuo-test].inst_year.CurriculumStudent
(CurriculumID, StudentID, PersonID, ValidFrom, ValidTo, SysUserID, SchoolYear, ExternalID, IsValid, isAzureEnrolled, WeeksFirstTerm, HoursWeeklyFirstTerm, WeeksSecondTerm, HoursWeeklySecondTerm)
VALUES(19210973, 86612007, 83976800, '2024-09-12 10:06:56.968', '9999-12-31 23:59:59.999', 865090, 2024, NULL, 1, 1, 18, 5.0, 16, 5.0);
 
 
INSERT INTO [neispuo-test].core.SysUser
(Username, Password, IsAzureUser, PersonID, isAzureSynced, InitialPassword, DeletedOn)
VALUES(N'et78453768@edu.mon.bg', NULL, 1, 86612007, 1, NULL, NULL);

 
--  dt81762252@edu.mon.bg
INSERT INTO [neispuo-test].student.StudentClass
(ID, SchoolYear, ClassId, PersonId, StudentSpecialityId, StudentEduFormId, ClassNumber, Status, IsIndividualCurriculum, IsHourlyOrganization, IsForSubmissionToNRA, IsCurrent, RepeaterId, CommuterTypeId, HasSuportiveEnvironment, SupportiveEnvironment, EnrollmentDate, AdmissionDocumentId, PositionId, BasicClassId, ClassTypeId, newClassId, OldClassId, FromStudentClassId, DischargeReasonId, DischargeDocumentId, RelocationDocumentId, ORESTypeId, IsFTACOutsourced, InstitutionId, IsNotPresentForm, ExternalID, ToDeleteByAdmin, EntryDate, DischargeDate)
VALUES(39707233, 2024, 5019676, 86612007, -1, 1, 13, 1, 0, 0, 1, 1, 1, 1, 0, N'', '2024-08-08 00:00:00.000', NULL, 3, 6, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 101700, 0, NULL, 0, '2024-08-08', NULL);

-- et73946789@edu.mon.bg
INSERT INTO [neispuo-test].student.StudentClass
(ID, SchoolYear, ClassId, PersonId, StudentSpecialityId, StudentEduFormId, ClassNumber, Status, IsIndividualCurriculum, IsHourlyOrganization, IsForSubmissionToNRA, IsCurrent, RepeaterId, CommuterTypeId, HasSuportiveEnvironment, SupportiveEnvironment, EnrollmentDate, AdmissionDocumentId, PositionId, BasicClassId, ClassTypeId, newClassId, OldClassId, FromStudentClassId, DischargeReasonId, DischargeDocumentId, RelocationDocumentId, ORESTypeId, IsFTACOutsourced, InstitutionId, IsNotPresentForm, ExternalID, ToDeleteByAdmin, EntryDate, DischargeDate)
VALUES(39707234, 2024, 5019676, 83765677, -1, 1, 13, 1, 0, 0, 1, 1, 1, 1, 0, N'', '2024-08-08 00:00:00.000', NULL, 3, 6, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 101700, 0, NULL, 0, '2024-08-08', NULL);


UPDATE core.Person
SET FirstName=N'Ивелинито', MiddleName=N'тестов', LastName=N'Служител', PermanentAddress=N'СЪЕДИНЕНИЕ 19', PermanentTownID=32024, CurrentAddress=N'СЪЕДИНЕНИЕ 19', CurrentTownID=32024, PublicEduNumber=N'ivelinito.slizhitel', PersonalIDType=0, NationalityID=NULL, BirthDate='1960-01-09', BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=2, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'8a87276e-783d-48f6-8024-96f2f6deb034', SysUserType=NULL
WHERE PersonID=84386738;
 
UPDATE core.SysUser
SET Username=N'ivelinito.slizhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84386738, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84386738;
 
UPDATE core.Person
SET FirstName=N'Валентина', MiddleName=N'тестов', LastName=N'учител', PublicEduNumber=N'lora.marinova', PersonalID=N'1111140196',AzureID=N'08d5ab11-d83a-4b54-b0b1-0a8a37954146'
WHERE PersonID=83056269;
 
INSERT INTO [neispuo-test].core.SysUser
(SysUserID, Username, Password, IsAzureUser, PersonID, isAzureSynced, InitialPassword, DeletedOn)
VALUES(4921, N'lora.marinova@edu.mon.bg', NULL, 1, 83056269, 1, NULL, NULL);

UPDATE core.SysUser
SET Username=N'galia.sluzhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84519611, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=84519611;
 
UPDATE core.Person
SET FirstName=N'Галя', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'ул. Инженер Бъркли 2', PermanentTownID=63427, CurrentAddress=N'ул. Инженер Бъркл8и 2', CurrentTownID=63427, PublicEduNumber=N'galia.sluzhitel', PersonalIDType=0, NationalityID=NULL, BirthDate='1947-11-16', BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=1, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'515e2715-8585-4edd-b1f7-e9d7bc8457c5', SysUserType=NULL
WHERE PersonID=84519611;
 
UPDATE core.SysUser
SET Username=N'iliana.sluzhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=84347831, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=86588231;
 
UPDATE core.Person
SET FirstName=N'Илиана', MiddleName=N'тестов', LastName=N'служител', PermanentAddress=N'ул. Драган Цанков № 20, бл. Еделвайс, вх. В', PermanentTownID=63427, CurrentAddress=N'ул. Драган Цанков № 20, бл. Еделвайс, вх. В', CurrentTownID=63427, PublicEduNumber=N'iliana.sluzhitel', PersonalIDType=0, NationalityID=NULL, BirthDate='1957-11-14', BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=1, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'246f93c0-33a7-4a23-8da8-5c56657a37b0', SysUserType=NULL
WHERE PersonID=86588231;

UPDATE core.SysUser
SET Username=N'iveto.slizhitel@edu.mon.bg', Password=NULL, IsAzureUser=1, PersonID=85794679, isAzureSynced=1, InitialPassword=NULL, DeletedOn=NULL
WHERE PersonID=85794679;
 
UPDATE core.Person
SET FirstName=N'Ивето', MiddleName=N'тестов', LastName=N'Служител', PermanentAddress=N'Витоша 5', PermanentTownID=32024, CurrentAddress=N'Витоша 5', CurrentTownID=32024, PublicEduNumber=N'iveto.slizhitel', PersonalIDType=0, NationalityID=NULL, BirthDate='1961-01-24', BirthPlaceTownID=NULL, BirthPlaceCountry=NULL, Gender=1, SchoolBooksCodesID=NULL, BirthPlace=NULL, AzureID=N'858433d2-fca3-4c18-89fd-9372d0642239', SysUserType=NULL
WHERE PersonID=85794679;

COMMIT