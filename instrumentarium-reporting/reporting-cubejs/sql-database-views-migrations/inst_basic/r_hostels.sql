CREATE VIEW [inst_basic].[R_hostels]
AS


SELECT 
	sc.ClassId as 'ClassId',
	i.InstitutionID as 'InstitutionID', -- Код по НЕИСПУО
	i.Name as 'InstitutionName', -- Институция
	cg.ClassName as 'ClassName', --Паралелка/група
	COUNT(sc.ID) as 'StudentsCount'
from student.StudentClass sc
join inst_year.ClassGroup cg on cg.ClassID = sc.ClassID
join inst_nom.ClassType ct on sc.ClassTypeId = ct.ClassTypeID
join core.Institution i on cg.InstitutionID = i.InstitutionID

where ct.ClassTypeID in (39,49) AND ct.IsValid = 1

group by 
	sc.ClassId,
	i.InstitutionID ,
	i.Name,
	cg.ClassName
GO

