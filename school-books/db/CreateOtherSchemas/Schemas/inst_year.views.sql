SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE   VIEW [inst_year].[ClassTeacher199]
AS
SELECT
	*
FROM
(
	--в училище класен ръководител е онзи, на когото е възложен предмет "Час на класа"
	SELECT
		 FirstName + ' ' + LastName AS ClassTeacherName
		,p.PersonID
		,c.InstitutionID
		,c.SchoolYear
		,cc.ClassID  
	FROM
		inst_basic.StaffPosition sp
		INNER JOIN core.Person p ON sp.PersonID = p.PersonID
		INNER JOIN inst_year.CurriculumTeacher ct ON ct.StaffPositionID = sp.StaffPositionID
		INNER JOIN inst_year.Curriculum c ON c.CurriculumID = ct.CurriculumID
		INNER JOIN inst_year.CurriculumClass cc ON c.CurriculumID = cc.CurriculumID
		INNER JOIN inst_year.ClassGroup cg ON cc.ClassID = cg.ParentClassID
	WHERE
		(
			(c.SubjectID = 199 AND cg.BasicClassID > 0) --час на класа за ученици от първи клас нагоре
		 OR
			(c.SubjectID = 80 AND cg.BasicClassID < 0) --обр. напр. БЕЛ за ДГ
		)
		AND cg.ClassGroupNum = 1
		AND sp.CurrentlyValid = 1
		AND cg.IsValid = 1
		AND ct.IsValid = 1
 
UNION

	SELECT
		 FirstName + ' ' + LastName AS ClassTeacherName
		,p.PersonID
		,cg.InstitutionID
		,cg.SchoolYear
		,cg.ClassID  
	FROM
		student.LeadTeacher lt
		INNER JOIN inst_basic.StaffPosition sp ON lt.StaffPositionId = sp.StaffPositionID
		INNER JOIN core.Person p ON sp.PersonID = p.PersonID
		INNER JOIN inst_year.ClassGroup cg ON lt.ClassID = cg.ClassID
	WHERE 
		 cg.IsValid=1
		 AND sp.CurrentlyValid = 1
) t
 
GROUP BY
	ClassTeacherName
   ,PersonID
   ,InstitutionID
   ,SchoolYear
   ,ClassID
GO
