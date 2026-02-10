-- refugee.[Записани в институция търсещи или получили закрила] source

CREATE VIEW refugee.RefugeesSearchingOrReceivedAdmission
AS
SELECT
	ac.SchoolYear as SchoolYear,
	-- 'Учебна година',
	MAX(ac.Town) as TownName,
	-- 'Населено място',
	MAX(ac.Municipality) as MunicipalityName,
	-- 'Община',
	MAX(ac.Region) as RegionName,
	-- 'Област',
	sc.InstitutionId as InstitutionID,
	--'Код на институция',
	MAX(i.Abbreviation) as InstitutionAbbreviation,
	--'Наименование',
	MAX(fst.Name) FinancialSchoolTypeName,
	-- 'Вид',
	COUNT(ac.PersonId) StudentCount,
	-- 'Брой деца/ученици',
	MAX(ac.TownID) TownID,
	MAX(ac.MunicipalityID) MunicipalityID,
	MAX(ac.RegionID) RegionID
FROM
	refugee.v_ApplicationChildren ac
INNER JOIN student.StudentClass sc ON
	ac.PersonId = sc.PersonId
INNER JOIN inst_nom.ClassType ct ON
	sc.ClassTypeId = ct.ClassTypeID
	AND ct.ClassKind = 1
INNER JOIN core.InstitutionSchoolYear i ON
	sc.InstitutionId = i.InstitutionId
	AND sc.SchoolYear = i.SchoolYear
INNER JOIN noms.FinancialSchoolType fst ON
	fst.FinancialSchoolTypeID = i.FinancialSchoolTypeID
GROUP BY
	ac.SchoolYear,
	sc.InstitutionId
