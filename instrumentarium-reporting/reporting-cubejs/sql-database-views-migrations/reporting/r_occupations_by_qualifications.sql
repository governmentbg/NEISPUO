CREATE VIEW [reporting].[OccupationsByQualifications]
AS
WITH cte AS
(
SELECT
	d.id,
	d.PersonId,
	YEAR(d.RegistrationDate) AS [Year],
	r.Name AS Region,
	prof.ProfGroupMONID AS ProfGroupMON,
	d.VetLevel AS VetLevel
FROM
	document.Diploma d
	INNER JOIN [core].InstitutionSchoolYear isy ON isy.InstitutionId = d.InstitutionId
											AND isy.SchoolYear = d.SchoolYear
	INNER JOIN location.Town t ON t.TownID = isy.TownID
	INNER JOIN location.Municipality m ON m.MunicipalityID = t.MunicipalityID
	INNER JOIN location.Region r ON r.RegionID = m.RegionID
	LEFT OUTER JOIN inst_nom.SPPOOSpeciality spec ON spec.SPPOOSpecialityID = d.SPPOOSpecialityId
	LEFT OUTER JOIN inst_nom.SPPOOProfession prof ON prof.SPPOOProfessionID = spec.ProfessionID
WHERE
	BasicDocumentId IN (17, 64, 194, 196)
	AND (d.IsSigned = 1 AND d.SchoolYear > 2020)
	AND d.IsCancelled = 0
)

SELECT
	cte.[Year],
	cte.Region,
	SUM(CASE WHEN cte.ProfGroupMON = 2 AND cte.VetLevel = 1 THEN 1 ELSE 0 END) AS col2,
	SUM(CASE WHEN cte.ProfGroupMON = 2 AND cte.VetLevel = 2 THEN 1 ELSE 0 END) AS col3,
	SUM(CASE WHEN cte.ProfGroupMON = 2 AND cte.VetLevel = 3 THEN 1 ELSE 0 END) AS col4,
	SUM(CASE WHEN cte.ProfGroupMON = 2 AND cte.VetLevel = 4 THEN 1 ELSE 0 END) AS col5,
	SUM(CASE WHEN cte.ProfGroupMON = 3 AND cte.VetLevel = 1 THEN 1 ELSE 0 END) AS col6,
	SUM(CASE WHEN cte.ProfGroupMON = 3 AND cte.VetLevel = 2 THEN 1 ELSE 0 END) AS col7,
	SUM(CASE WHEN cte.ProfGroupMON = 3 AND cte.VetLevel = 3 THEN 1 ELSE 0 END) AS col8,
	SUM(CASE WHEN cte.ProfGroupMON = 3 AND cte.VetLevel = 4 THEN 1 ELSE 0 END) AS col9,
	SUM(CASE WHEN cte.ProfGroupMON = 4 AND cte.VetLevel = 1 THEN 1 ELSE 0 END) AS col10,
	SUM(CASE WHEN cte.ProfGroupMON = 4 AND cte.VetLevel = 2 THEN 1 ELSE 0 END) AS col11,
	SUM(CASE WHEN cte.ProfGroupMON = 4 AND cte.VetLevel = 3 THEN 1 ELSE 0 END) AS col12,
	SUM(CASE WHEN cte.ProfGroupMON = 4 AND cte.VetLevel = 4 THEN 1 ELSE 0 END) AS col13,
	SUM(CASE WHEN cte.ProfGroupMON = 5 AND cte.VetLevel = 1 THEN 1 ELSE 0 END) AS col14,
	SUM(CASE WHEN cte.ProfGroupMON = 5 AND cte.VetLevel = 2 THEN 1 ELSE 0 END) AS col15,
	SUM(CASE WHEN cte.ProfGroupMON = 5 AND cte.VetLevel = 3 THEN 1 ELSE 0 END) AS col16,
	SUM(CASE WHEN cte.ProfGroupMON = 5 AND cte.VetLevel = 4 THEN 1 ELSE 0 END) AS col17,
	SUM(CASE WHEN cte.ProfGroupMON = 6 AND cte.VetLevel = 1 THEN 1 ELSE 0 END) AS col18,
	SUM(CASE WHEN cte.ProfGroupMON = 6 AND cte.VetLevel = 2 THEN 1 ELSE 0 END) AS col19,
	SUM(CASE WHEN cte.ProfGroupMON = 6 AND cte.VetLevel = 3 THEN 1 ELSE 0 END) AS col20,
	SUM(CASE WHEN cte.ProfGroupMON = 6 AND cte.VetLevel = 4 THEN 1 ELSE 0 END) AS col21,
	SUM(CASE WHEN cte.ProfGroupMON = 7 AND cte.VetLevel = 1 THEN 1 ELSE 0 END) AS col22,
	SUM(CASE WHEN cte.ProfGroupMON = 7 AND cte.VetLevel = 2 THEN 1 ELSE 0 END) AS col23,
	SUM(CASE WHEN cte.ProfGroupMON = 7 AND cte.VetLevel = 3 THEN 1 ELSE 0 END) AS col24,
	SUM(CASE WHEN cte.ProfGroupMON = 7 AND cte.VetLevel = 4 THEN 1 ELSE 0 END) AS col25,
	SUM(CASE WHEN cte.ProfGroupMON = 8 AND cte.VetLevel = 1 THEN 1 ELSE 0 END) AS col26,
	SUM(CASE WHEN cte.ProfGroupMON = 8 AND cte.VetLevel = 2 THEN 1 ELSE 0 END) AS col27,
	SUM(CASE WHEN cte.ProfGroupMON = 8 AND cte.VetLevel = 3 THEN 1 ELSE 0 END) AS col28,
	SUM(CASE WHEN cte.ProfGroupMON = 8 AND cte.VetLevel = 4 THEN 1 ELSE 0 END) AS col29,
	SUM(CASE WHEN (cte.ProfGroupMON = 0
				   OR cte.ProfGroupMON IS NULL
				   OR cte.VetLevel = 0
				   OR cte.VetLevel IS NULL
				  ) THEN 1 ELSE 0 END
	   ) AS col30
FROM
	cte
GROUP BY
	[Year],
	Region