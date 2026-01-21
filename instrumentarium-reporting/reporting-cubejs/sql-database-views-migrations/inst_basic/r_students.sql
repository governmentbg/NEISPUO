--- Name: Брой ученици по групи / паралелки
CREATE VIEW [inst_basic].[R_students] AS
select
	cg.ParentClassID as ParentClassID,
	-- код на класа
	sc.ClassId as ClassId,
	-- код на група
	sc.InstitutionID as 'InstitutionID',
	-- Код по НЕИСПУО
	isy.Name as 'InstitutionName',
	-- Институция
	isy.Abbreviation as 'InstitutionAbbreviation',
	-- кратко наименование
	bc.RomeName as 'RomeClassName',
	-- Клас 
	cg.ClassName as 'ClassName',
	-- Паралелка/Група
	COUNT(sc.ID) as 'StudentsCount',
	-- Брой ученици
	COUNT(sny.PersonId) as 'StudentSOPCount',
	--- Брой СОП
	spec.Name as 'SpecialtyName',
	CASE
		WHEN cg.IsSpecNeed = 1 THEN 'да'
		ELSE 'не'
	END as 'IsSOP',
	-- Група за деца със СОП
	CASE
		WHEN ct.ClassKind = 2 THEN 'да'
		ELSE 'не'
	END as 'IsCDO',
	-- ЦДО
	sc.SchoolYear as 'SchoolYear',
	-- година
	ef.Name as 'EduFormName',
	-- Форма на обучение
	r.Name as 'RegionName',
	-- Област
	m.Name as 'MunicipalityName',
	-- Община
	t.Name as 'TownName',
	-- Населено място
	bst.Name as 'BasicSchoolTypeName',
	-- по чл. 37
	dst.Name as 'DetailedSchoolTypeName',
	-- по чл. 38
	fst.Name as 'FinancialSchoolTypeName',
	-- Финансиране
	r.RegionID,
	m.MunicipalityID,
	t.TownID,
	isy.BudgetingSchoolTypeID as BudgetingSchoolTypeID,
	ck.Name as 'ClassKind',
	--Вид
	-- филтрираме тук за предефиниераните справки
	ct.Name as 'ClassType',
	--Вид на паралелка/група
	SUM (
		CASE
			WHEN repeaterReason.Id > 1 THEN 1
			ELSE 0
		END
	) as 'RepeaterCount',
	--- Брой второгодници
	SUM (
		CASE
			WHEN commuterType.ID > 1 THEN 1
			ELSE 0
		END
	) as 'CommuterCount',
	--- Брой пъруващи
	CASE
		WHEN sc.IsHourlyOrganization = 1 THEN 'да'
		ELSE 'не'
	END as 'IsHourlyOrganization',
	--Почасова организация
	sc.PositionId,
	--- where position = 10 
	dst.InstType --- 
from
	student.StudentClass sc
	join student.RepeaterReason repeaterReason ON sc.RepeaterId = repeaterReason.Id -- check id
	join student.CommuterType commuterType ON sc.CommuterTypeID = commuterType.ID -- check id
	join inst_nom.BasicClass bc on bc.BasicClassID = sc.BasicClassId
	join inst_year.ClassGroup cg on cg.ClassID = sc.ClassID
	and cg.IsValid = 1 -- връща 3 записа за 1 клас , ако той е разбит на 3 групи
	join inst_nom.EduForm ef on ef.ClassEduFormID = sc.StudentEduFormId
	join inst_nom.ClassType ct on sc.ClassTypeId = ct.ClassTypeID
	join inst_nom.ClassKind ck on ck.ClassKindID = ct.ClassKind
	join core.Person p on p.PersonID = sc.PersonID
	join core.InstitutionSchoolYear isy on sc.InstitutionId = isy.InstitutionId
	and sc.SchoolYear = isy.SchoolYear -- текуща година за институция
	join location.Town t on isy.TownID = t.TownID
	join location.Municipality m on t.MunicipalityID = m.MunicipalityID
	join location.Region r on m.RegionID = r.RegionID
	join noms.BaseSchoolType bst on bst.BaseSchoolTypeID = isy.BaseSchoolTypeID
	join noms.DetailedSchoolType dst on dst.DetailedSchoolTypeID = isy.DetailedSchoolTypeID
	join noms.FinancialSchoolType fst on fst.FinancialSchoolTypeID = isy.FinancialSchoolTypeID
	join inst_nom.SPPOOSpeciality spec on spec.SPPOOSpecialityID = sc.StudentSpecialityId
	left join student.SpecialNeedsYear sny on sny.PersonId = p.PersonID
	AND sny.SchoolYear = isy.SchoolYear
	join inst_basic.CurrentYear cy on sc.SchoolYear = cy.CurrentYearID -- указва текуща учебна година за НЕИСПУО
	--- лятото се преминава в нова уч.година -> на първия ден модул справки няма да показва данни;
	--- да се сложат данни 
where
	cy.IsValid = 1
	and sc.IsCurrent = 1
	and bc.IsValid = 1 -- and (
	-- 	ct.ClassKind = 1 -- 1 - всичко видове учебни паралелки, учебни паралелки; 
	-- 	or ct.ClassKind = 2 -- 2 - групи в целодневни организации ЦДО; 
	-- 	-- 3 - всички останали; 
	-- )
group by
	sc.ClassId,
	sc.SchoolYear,
	bc.Name,
	cg.ClassName,
	sc.InstitutionId,
	isy.Name,
	isy.Abbreviation,
	ef.Name,
	spec.Name,
	r.Name,
	m.Name,
	t.Name,
	bst.Name,
	dst.Name,
	fst.Name,
	r.RegionID,
	m.MunicipalityID,
	t.TownID,
	ct.ClassTypeID,
	sc.PositionId,
	ct.ClassKind,
	bc.BasicClassID,
	isy.BudgetingSchoolTypeID,
	cg.IsSpecNeed,
	cg.ParentClassID,
	bc.RomeName,
	ck.Name,
	ct.Name,
	sc.IsHourlyOrganization,
	sc.PositionId,
	dst.InstType