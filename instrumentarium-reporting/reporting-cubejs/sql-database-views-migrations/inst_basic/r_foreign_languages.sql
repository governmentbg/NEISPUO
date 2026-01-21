CREATE VIEW [inst_basic].[R_foreign_languages] AS

WITH StudentCounts
AS
(
SELECT COUNT(CurriculumStudent.StudentID) AS StdsCount
		, CurriculumID 
		, CurriculumStudent.SchoolYear
FROM inst_year.CurriculumStudent
INNER JOIN student.StudentClass
	ON CurriculumStudent.StudentID=StudentClass.ID
WHERE IsCurrent=1
GROUP BY CurriculumID, CurriculumStudent.SchoolYear
)

SELECT Curriculum.SchoolYear AS SchoolYear --Учебна година
		, Curriculum.InstitutionID  AS InstitutionID --Код по НЕИСПУО
		, i.Name as 'InstitutionName' --Институция
		--, CurriculumClass.ClassID AS 'Клас - код'
		, CG1.BasicClassID AS 'ClassYear' -- Випуск
		, ClassGroup.ClassName AS ClassName--Клас - име
		, ClassType.Name as 'ClassType' --Профил на паралелката
		, ISNULL(Person.FirstName,'') + ' ' + ISNULL(Person.LastName,'') as 'Teacher' --Преподавател
		, Curriculum.CurriculumPartID -- Раздел от учебния план - код
		, CurriculumPart.Name AS 'CurriculumPartName' --Раздел от учебния план - име
		, Curriculum.SubjectID  --Учебен предмет - код
		, SubjectName --Учебен предмет - име
		, inst_year.Curriculum.SubjectTypeID AS 'CurriculumSubjectTypeID' --Начин на изучаване - код
		, SubjectType.Name AS 'SubjectTypeName' --Начин на изучаване - име
		, CASE WHEN CG1.FLTypeID=-1 THEN '' ELSE CG1.FLTypeID END AS 'FLTypeID' --Начин на изучаване на ЧЕ - код
		, CASE WHEN CG1.FLTypeID=-1 THEN '' ELSE FLStudyType.Name END AS 'FLStudyTypeName' --Начин на изучаване на ЧЕ - име
		, CASE WHEN CG1.FLID=-1 THEN '' ELSE CG1.FLID END AS 'FLID' --ЧЕ - код
		, CASE WHEN CG1.FLID=-1 THEN '' ELSE FL.Name END AS 'FLName' --ЧЕ - име
		, StudentCounts.StdsCount AS 'StudentsCount' --Брой ученици
		, r.RegionID
		, r.Name as 'RegionName' --Област
		, m.MunicipalityID
		, m.Name as 'MunicipalityName' --Община
		, t.TownID
		, t.Name as 'TownName' --Населено място
		, LocalArea.Name as 'LocalAreaName' --Район
		, bst.Name as 'BaseSchoolTypeName' --по чл. 37
		, dst.Name as 'DetailedSchoolTypeName' --по чл. 38
		, fst.Name as 'FinancialSchoolTypeName' --Финансиране
FROM  inst_year.Curriculum
	INNER JOIN inst_year.CurriculumClass 
		ON Curriculum.CurriculumID = CurriculumClass.CurriculumID
	INNER JOIN inst_year.CurriculumTeacher 
		ON CurriculumTeacher.CurriculumID = Curriculum.CurriculumID
	INNER JOIN core.SysUser 
		ON SysUser.SysUserID = CurriculumTeacher.SysUserID
	INNER JOIN core.Person
		ON Person.PersonID = SysUser.PersonID
	INNER JOIN inst_year.ClassGroup
		ON CurriculumClass.ClassID=ClassGroup.ClassID
	INNER JOIN inst_year.ClassGroup CG1
		ON ClassGroup.ClassID=CG1.ParentClassID
	INNER JOIN inst_nom.ClassType
		ON ClassGroup.ClassTypeID=inst_nom.ClassType.ClassTypeID
	LEFT OUTER JOIN StudentCounts
		ON Curriculum.CurriculumID = StudentCounts.CurriculumID
			AND Curriculum.SchoolYear = StudentCounts.SchoolYear
	INNER JOIN core.InstitutionSchoolYear ISY
		ON Curriculum.InstitutionID=ISY.InstitutionId
			AND Curriculum.SchoolYear=ISY.SchoolYear
	join core.Institution i 
		on inst_year.ClassGroup.InstitutionID = i.InstitutionID
	join location.Town t 
		on i.TownID = t.TownID
	join location.Municipality m 
		on t.MunicipalityID = m.MunicipalityID
	join location.Region r 
		on m.RegionID = r.RegionID
	join location.LocalArea
		on LocalArea.TownCode = t.TownID
	join noms.BaseSchoolType bst 
		on bst.BaseSchoolTypeID = i.BaseSchoolTypeID
	join noms.DetailedSchoolType dst 
		on dst.DetailedSchoolTypeID = i.DetailedSchoolTypeID
	join noms.FinancialSchoolType fst 
		on fst.FinancialSchoolTypeID = i.FinancialSchoolTypeID
	LEFT OUTER JOIN inst_nom.Subject
		ON Curriculum.SubjectID=Subject.SubjectID
	LEFT OUTER JOIN inst_nom.SubjectType
		ON Curriculum.SubjectTypeID=SubjectType.SubjectTypeID
	LEFT OUTER JOIN inst_nom.FL
		ON CG1.FLID=FL.FLID
	LEFT OUTER JOIN inst_nom.FLStudyType
		ON CG1.FLTypeID=FLStudyType.FLStudyTypeID
	LEFT OUTER JOIN inst_nom.CurriculumPart
		ON Curriculum.CurriculumPartID=CurriculumPart.CurriculumPartID

WHERE  inst_year.Curriculum.SchoolYear = 2021 -- ограничено до текущата
	AND i.BaseSchoolTypeID IN (11,12,13,14) -- само училища
	AND ClassKind=1 -- само учебни паралелки
	AND 
		--(
			Curriculum.SubjectID BETWEEN 100 AND 143 -- ограничение за чуждите езици от стандартния списък
			--OR Subject.SubjectName LIKE '%англ%') -- търси по стринг в имената на предметите
	AND Curriculum.SubjectTypeID NOT IN (152,153,154,155) -- изключват се базовите профилиращи предмети
--ORDER BY inst_year.Curriculum.InstitutionID, inst_year.CurriculumClass.ClassID, SubjectID, SubjectTypeID
GO

