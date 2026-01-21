CREATE VIEW [reporting].[R_StudentsDetails]
AS
   SELECT
		r.RegionID AS RegionID --да не се показва в справката
	   ,r.Name AS RegionName
	   ,m.MunicipalityID AS MunicipalityID--да не се показва в справката
	   ,m.Name AS MunicipalityName
	   ,t.Name AS TownName -- 'Населено място'
	   ,sc.InstitutionID AS InstitutionID --'Код по НЕИСПУО'
	   ,isy.Name AS InstitutionName --'Институция'
	   ,dst.Name AS InstitutionKind -- 'Вид институция'
	   ,p.FirstName AS FirstName --'Име'
	   ,ISNULL(p.MiddleName,'') AS MiddleName --'Презиме'
	   ,p.LastName AS LastName  --'Фамилия'
	   ,PIdType.Name AS IdType --'Вид идентификатор'
	   ,p.PersonalID AS PersonalID --'Идентификатор'
	   ,p.BirthDate --'Дата на раждане'
	   ,bc.RomeName AS RomeName --'Випуск'
	   ,cg.ClassName AS ClassName --'Група/Паралелка'
	   ,ef.Name AS EduFormName --'Форма на обучение'
	   ,ct.Name AS ClassType --Профил'
	   ,ISNULL(prof.Name,'не е посочено') AS ProfName --'Професия'
	   ,ISNULL(spec.Name,'не е посочено') AS SpecName --'Специалност'
	   ,CASE WHEN sc.IsIndividualCurriculum = 1
			 THEN 'да'
			 ELSE 'не'
		END AS IsIndividualCurriculum --'Инд. уч. план'
	   ,commt.Name AS IsTravel -- 'Пътуващ от друго населено място'
	   ,rep.Name AS IsRepeatClass --'Второгодник'
	   ,CASE WHEN sny.Id IS NOT NULL
			 THEN 'да'
			 ELSE 'не'
		END AS IsSOP --'СОП'
	   ,CASE WHEN rsr.Id IS NOT NULL 
			 THEN 'да'
			 ELSE 'не'
		END IsRP --'РП'
	FROM
		student.StudentClass sc
		INNER JOIN core.InstitutionSchoolYear isy ON sc.InstitutionId = isy.InstitutionId
													 AND sc.SchoolYear = isy.SchoolYear
		INNER JOIN location.Town t ON t.TownID = isy.TownID
		INNER JOIN location.Municipality m ON m.MunicipalityID = t.MunicipalityID
		INNER JOIN location.Region r ON r.RegionID = m.RegionID
		INNER JOIN inst_nom.ClassType ct ON ct.ClassTypeID = sc.ClassTypeId
		INNER JOIN core.Person p ON p.PersonID = sc.PersonId
		INNER JOIN noms.PersonalIDType PIdType ON PIdType.PersonalIDTypeID = p.PersonalIDType
		INNER JOIN inst_basic.CurrentYear cy ON cy.CurrentYearID = sc.SchoolYear
		INNER JOIN noms.DetailedSchoolType dst ON isy.DetailedSchoolTypeID = dst.DetailedSchoolTypeID
		LEFT OUTER JOIN student.SpecialNeedsYear sny ON sny.PersonId = sc.PersonId
														AND sny.SchoolYear = sc.SchoolYear
		LEFT OUTER JOIN student.ResourceSupportReport rsr ON rsr.PersonId = sc.PersonId
															 AND rsr.SchoolYear = sc.SchoolYear
		INNER JOIN inst_nom.BasicClass bc ON bc.BasicClassID = sc.BasicClassId
		INNER JOIN inst_year.ClassGroup cg ON cg.ClassID = sc.ClassId
		INNER JOIN inst_nom.EduForm ef ON ef.ClassEduFormID = sc.StudentEduFormId
		LEFT OUTER JOIN inst_nom.SPPOOSpeciality spec ON spec.SPPOOSpecialityID = sc.StudentSpecialityId
		LEFT OUTER JOIN inst_nom.SPPOOProfession prof ON prof.SPPOOProfessionID = spec.ProfessionID
		LEFT OUTER JOIN student.CommuterType commt ON commt.Id = sc.CommuterTypeId
		LEFT OUTER JOIN student.RepeaterReason rep ON rep.Id = sc.RepeaterId
	WHERE
		cy.IsValid = 1
		AND sc.IsCurrent = 1
		--записани в училище или детска градина, без обучаващите се в ЦСОП
		--и без пришълци (учещите немски в съседното училище)
		AND (dst.InstType IN (1,2) AND ct.ClassKind = 1 AND sc.PositionId NOT IN (7,10)
				OR
			 --обучаващи се в ЦСОП
			 dst.InstType =4 AND ct.ClassKind = 1 AND sc.PositionId = 7
			)
		AND isy.InstitutionId < 2999900 --махаме тестовите институции

UNION ALL

	SELECT DISTINCT
		r.RegionID --да не се показва в справката
	   ,r.Name AS RegionName
	   ,m.MunicipalityID --да не се показва в справката
	   ,m.Name AS MunicipalityName
	   ,t.Name AS TownName
	   ,sc.InstitutionID AS InstitutionID
	   ,isy.Name AS InstitutionName
	   ,dst.Name AS InstitutionKind
	   ,p.FirstName AS FirstName
	   ,ISNULL(p.MiddleName,'') As MiddleName --'Презиме'
	   ,p.LastName AS LastName --'Фамилия'
	   ,PIdType.Name AS IdType --'Вид идентификатор'
	   ,p.PersonalID AS PersonalID --'Идентификатор'
	   ,p.BirthDate--'Дата на раждане'
	   ,'-' RomeName --'Випуск'
	   ,'-' ClassName --'Група/Паралелка'
	   ,'-' EduFormName --'Форма на обучение'
	   ,'-' ClassType --'Профил'
	   ,'-' ProfName --'Професия'
	   ,'-' SpecName --'Специалност'
	   ,'-' IsIndividualCurriculum --I'Инд. уч. план'
	   ,'-' IsTravel --'Пътуващ'
	   ,'-' IsRepeatClass --'Второгодник'
	   ,CASE WHEN sny.Id IS NOT NULL
			 THEN 'да'
			 ELSE 'не'
		END IsSOP --'СОП'
	   ,CASE WHEN rsr.Id IS NOT NULL 
			 THEN 'да'
			 ELSE 'не'
		END IsRP --'РП'
	FROM
		student.StudentClass sc
		INNER JOIN core.InstitutionSchoolYear isy ON sc.InstitutionId = isy.InstitutionId
													 AND sc.SchoolYear = isy.SchoolYear
		INNER JOIN location.Town t ON t.TownID = isy.TownID
		INNER JOIN location.Municipality m ON m.MunicipalityID = t.MunicipalityID
		INNER JOIN location.Region r ON r.RegionID = m.RegionID
		INNER JOIN inst_nom.ClassType ct ON ct.ClassTypeID = sc.ClassTypeId
		INNER JOIN core.Person p ON p.PersonID = sc.PersonId
		INNER JOIN noms.PersonalIDType PIdType ON PIdType.PersonalIDTypeID = p.PersonalIDType
		INNER JOIN inst_basic.CurrentYear cy ON cy.CurrentYearID = sc.SchoolYear
		INNER JOIN noms.DetailedSchoolType dst ON isy.DetailedSchoolTypeID = dst.DetailedSchoolTypeID
		LEFT OUTER JOIN student.SpecialNeedsYear sny ON sny.PersonId = sc.PersonId
														AND sny.SchoolYear = sc.SchoolYear
		LEFT OUTER JOIN student.ResourceSupportReport rsr ON rsr.PersonId = sc.PersonId
															 AND rsr.SchoolYear = sc.SchoolYear
		--INNER JOIN inst_nom.BasicClass bc ON bc.BasicClassID = sc.BasicClassId
		--INNER JOIN inst_year.ClassGroup cg ON cg.ClassID = sc.ClassId
		--INNER JOIN inst_nom.EduForm ef ON ef.ClassEduFormID = sc.StudentEduFormId
		--LEFT OUTER JOIN inst_nom.SPPOOSpeciality spec ON spec.SPPOOSpecialityID = sc.StudentSpecialityId
		--LEFT OUTER JOIN inst_nom.SPPOOProfession prof ON prof.SPPOOProfessionID = spec.ProfessionID
		--LEFT OUTER JOIN student.CommuterType commt ON commt.Id = sc.CommuterTypeId
		--LEFT OUTER JOIN student.RepeaterReason rep ON rep.Id = sc.RepeaterId
	WHERE
		cy.IsValid = 1
		AND sc.IsCurrent = 1
		--записани в училище или детска градина, без обучаващите се в ЦСОП
		--и без пришълци (учещите немски в съседното училище)
		AND (dst.InstType NOT IN (1,2,4) AND ct.ClassKind <> 1 AND sc.PositionId = 8)
		AND isy.InstitutionId < 2999900 --махаме тестовите институции