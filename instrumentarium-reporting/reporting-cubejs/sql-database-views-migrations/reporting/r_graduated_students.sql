Create View [reporting].[R_Graduated_Students]
AS
WITH cte AS
(
SELECT
	d.id AS DiplomaId,
	d.PersonId,
	YEAR(d.RegistrationDate) AS [Year],
	r.Name AS Region,
	isy.DetailedSchoolTypeID AS dst
FROM
	document.Diploma d
	INNER JOIN core.InstitutionSchoolYear isy ON isy.InstitutionId = d.InstitutionId
											AND isy.SchoolYear = d.SchoolYear
	INNER JOIN location.Town t ON t.TownID = isy.TownID
	INNER JOIN location.Municipality m ON m.MunicipalityID = t.MunicipalityID
	INNER JOIN location.Region r ON r.RegionID = m.RegionID
WHERE
	BasicDocumentId IN (25,50,51,35,36,60,61,253)
	AND (d.IsSigned = 1 AND d.SchoolYear > 2020)
	AND d.IsCancelled = 0
)

SELECT
	cte.[Year],
	cte.Region,
	COUNT(DiplomaId) AS 'AllDiplomasSUM',
	SUM(CASE WHEN dst = 124 THEN 1 ELSE 0 END) AS 'SecondarySchools',
	SUM(CASE WHEN dst = 114 THEN 1 ELSE 0 END) AS 'SportSchools',
	SUM(CASE WHEN dst = 111 THEN 1 ELSE 0 END) AS 'SeminarySchools',
	SUM(CASE WHEN dst = 125 THEN 1 ELSE 0 END) AS 'ProfiledSchools',
	SUM(CASE WHEN dst = 126 THEN 1 ELSE 0 END) AS 'ProfessionalSchools',
	SUM(CASE WHEN dst IN (131,132,133,134,141) THEN 1 ELSE 0 END) AS 'SpecialSchools',
	SUM(CASE WHEN dst IN (112,113) THEN 1 ELSE 0 END) AS 'ArtSchools'
FROM
	cte
GROUP BY
	[Year],
	Region