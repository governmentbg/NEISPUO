CREATE VIEW [inst_basic].[RImmigrantsDOBHours]

AS

SELECT
	Curriculum.SchoolYear AS SchoolYear,
	--AS 'Учебна година'
	Curriculum.InstitutionID AS [InstitutionID],
	A.Name AS InstitutionName,
	-- 'Пълно наименование'
	A.Abbreviation AS InstitutionAbbreviation,
	---'Кратко наименование'
	A.TownID as [TownID],
	location.Town.Name AS TownName,
	--'Населено място - наименование'
	location.Municipality.MunicipalityID as [MunicipalityID],
	location.Municipality.Name AS MunicipalityName,
	--'Община - наименование'
	location.Region.RegionID as RegionID,
	location.Region.Name AS RegionName,
	-- 'Област- наименование'
	A.BaseSchoolTypeID AS BaseSchoolTypeID,
	-- 'Вид по чл.37 - код'
	noms.BaseSchoolType.Name AS BaseSchoolTypeName,
	-- 'Вид по чл.37 - наименование'
	A.DetailedSchoolTypeID AS DetailedSchoolTypeID,
	--'Вид по чл.38 (детайлен) - код'
	noms.DetailedSchoolType.Name AS DetailedSchoolTypeName,
	--'Вид по чл.38 (детайлен) - наименование'
	A.BudgetingSchoolTypeID AS BudgetingSchoolTypeID,
	noms.BudgetingInstitution.Name AS BudgetingInstitutionName,
	-- 'Източник на финансиране - наименование'
	A.FinancialSchoolTypeID AS FinancialSchoolTypeID,
	-- 'Вид по чл.35-36 (според собствеността) - код'
	noms.FinancialSchoolType.Name AS FinancialSchoolTypeName,
	-- 'Вид по чл.35-36 (според собствеността) - наименование'
	Person.FirstName + ' ' + Person.MiddleName + ' ' + Person.LastName AS Lecturer,
	--'Преподавател'
	CASE
		WHEN IsIndividualLesson = 1 
					THEN ((ISNULL(WeeksFirstTerm,
		0)* ISNULL(HoursWeeklyFirstTerm,
		0))+(ISNULL(WeeksSecondTerm,
		0)* ISNULL(HoursWeeklySecondTerm,
		0)))* StudentsCount
		ELSE (ISNULL(WeeksFirstTerm,
		0)* ISNULL(HoursWeeklyFirstTerm,
		0))+(ISNULL(WeeksSecondTerm,
		0)* ISNULL(HoursWeeklySecondTerm,
		0))
	END AS HoursWeekly
	--'Брой часове'
FROM
	inst_year.Curriculum
INNER JOIN inst_year.CurriculumTeacher 
		ON
	Curriculum.CurriculumID = CurriculumTeacher.CurriculumID
	AND Curriculum.SchoolYear = CurriculumTeacher.SchoolYear
INNER JOIN inst_basic.StaffPosition 
		ON
	CurriculumTeacher.StaffPositionID = StaffPosition.StaffPositionID
INNER JOIN core.Person 
		ON
	StaffPosition.PersonID = Person.PersonID
INNER JOIN core.Institution AS A 
		ON
	Curriculum.InstitutionID = A.InstitutionID
INNER JOIN location.Town
		ON
	A.TownID = location.Town.TownID
INNER JOIN location.Municipality
		ON
	location.Town.MunicipalityID = location.Municipality.MunicipalityID
INNER JOIN location.Region
		ON
	location.Municipality.RegionID = location.Region.RegionID
INNER JOIN noms.BaseSchoolType
		ON
	noms.BaseSchoolType.BaseSchoolTypeID = A.BaseSchoolTypeID
INNER JOIN noms.DetailedSchoolType
		ON
	noms.DetailedSchoolType.DetailedSchoolTypeID = A.DetailedSchoolTypeID
INNER JOIN noms.BudgetingInstitution
		ON
	noms.BudgetingInstitution.BudgetingInstitutionID = A.BudgetingSchoolTypeID
INNER JOIN noms.FinancialSchoolType
		ON
	noms.FinancialSchoolType.FinancialSchoolTypeID = A.FinancialSchoolTypeID
WHERE
	Curriculum.SubjectID = 99
	AND Curriculum.SubjectTypeID = 147
	AND Curriculum.IsValid = 1
	AND CurriculumTeacher.IsValid = 1
	AND StaffPosition.IsValid = 1