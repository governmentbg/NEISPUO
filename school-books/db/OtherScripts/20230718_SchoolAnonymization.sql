DECLARE @institutionID INT = 300125;
DECLARE @schoolYearsTable TABLE(year INT);

INSERT INTO @schoolYearsTable VALUES(2021), (2022), (2023);

UPDATE
    i
SET
    i.[Name] = '10 СУ "Антон Попстоилов"',
    i.[Abbreviation] = '10 СУ "Антон Попстоилов"'
FROM
    [core].[Institution] i
WHERE i.InstitutionID = @institutionID;

UPDATE
    iy
SET
    iy.[Name] = '10 СУ "Антон Попстоилов"',
    iy.[Abbreviation] = '10 СУ "Антон Попстоилов"'
FROM
    [core].[InstitutionSchoolYear] iy
WHERE iy.InstitutionID = @institutionID AND iy.SchoolYear IN (select year from @schoolYearsTable);

WITH AllPersonIds AS (
    SELECT
        [PersonID]
    FROM (
        SELECT DISTINCT
          p.[PersonID]
        FROM
            [inst_year].[ClassGroup] cg
            INNER JOIN [student].[StudentClass] s ON cg.[ClassID] = s.[ClassId]
            INNER JOIN [core].[Person] p ON s.[PersonID] = p.[PersonID]
        WHERE cg.[InstitutionID] = @institutionID AND cg.[SchoolYear] IN (select year from @schoolYearsTable)
    UNION
        SELECT DISTINCT
          p.[PersonID]
        FROM
            [inst_basic].[StaffPosition] sp
            INNER JOIN [inst_year].[CurriculumTeacher] ct ON sp.[StaffPositionID] = ct.[StaffPositionID]
            INNER JOIN [core].[Person] p ON sp.[PersonID] = p.[PersonID]
        WHERE
            sp.[InstitutionID] = @institutionID AND
            EXISTS (
                SELECT *
                FROM
                    [inst_year].[Curriculum] c
                WHERE
                    c.CurriculumID = ct.CurriculumID AND
                    c.InstitutionID = @institutionID
                    AND c.[SchoolYear] IN (select year from @schoolYearsTable)
            )
    UNION
        SELECT DISTINCT
          p.[PersonID]
        FROM
            [inst_year].[ClassGroup] cg
            INNER JOIN [student].[StudentClass] s ON cg.[ClassID] = s.[ClassId]
            INNER JOIN [family].[Relative] r ON s.[PersonID] = r.[PersonID]
            INNER JOIN [core].[Person] p ON r.[PersonID] = p.[PersonID]
        WHERE cg.[InstitutionID] = @institutionID AND cg.[SchoolYear] IN (select year from @schoolYearsTable)
    ) s_inner
)
UPDATE
    p
SET
    p.FirstName = pn.FirstName,
    p.MiddleName = pn.MiddleName,
    p.LastName = pn.LastName,
    p.PublicEduNumber = pn.PublicEduNumber,
    p.Gender = pn.Gender
FROM
    [core].[Person] p
    INNER JOIN
        (
            SELECT *, ROW_NUMBER() OVER (ORDER BY [PersonID]) as row_num
            FROM AllPersonIds
        ) tp ON p.PersonID = tp.PersonID
        LEFT JOIN (
            SELECT *, ROW_NUMBER() OVER (ORDER BY [Order]) as row_num
            FROM ##tempPersonNames
        ) pn ON tp.row_num = pn.row_num;

WITH StudentPersonIds AS (
    SELECT DISTINCT
        s.[PersonID]
    FROM
        [inst_year].[ClassGroup] cg
        INNER JOIN [student].[StudentClass] s ON cg.[ClassID] = s.[ClassId]
    WHERE
        cg.[InstitutionID] = @institutionID AND
        cg.[SchoolYear] IN (select year from @schoolYearsTable)
)
UPDATE r
SET
    r.FirstName = pn.FirstName,
    r.MiddleName = pn.MiddleName,
    r.LastName = pn.LastName
FROM (
        SELECT
            *,
            ROW_NUMBER() OVER (ORDER BY [PersonID]) AS row_num,
            ROW_NUMBER() OVER (PARTITION BY PersonID ORDER BY [PersonID]) AS RelativeNum
        FROM [family].[Relative]
        WHERE
            [PersonID] IN (SELECT [PersonID] FROM StudentPersonIds)
    ) r
    LEFT JOIN (
        SELECT *, ROW_NUMBER() OVER (ORDER BY [Order] Desc) as row_num
        FROM ##tempPersonNames
        WHERE Gender = 1
    ) pn ON r.row_num = pn.row_num
WHERE
    RelativeNum = 1;

WITH StudentPersonIds AS (
    SELECT DISTINCT
        s.[PersonID]
    FROM
        [inst_year].[ClassGroup] cg
        INNER JOIN [student].[StudentClass] s ON cg.[ClassID] = s.[ClassId]
    WHERE
        cg.[InstitutionID] = @institutionID AND
        cg.[SchoolYear] IN (select year from @schoolYearsTable)
)
UPDATE r
SET
    r.FirstName = pn.FirstName,
    r.MiddleName = pn.MiddleName,
    r.LastName = pn.LastName
FROM (
        SELECT
            *,
            ROW_NUMBER() OVER (ORDER BY [PersonID]) AS row_num,
            ROW_NUMBER() OVER (PARTITION BY PersonID ORDER BY [PersonID]) AS RelativeNum
        FROM [family].[Relative]
        WHERE
            [PersonID] IN (SELECT [PersonID] FROM StudentPersonIds)
    ) r
    LEFT JOIN (
        SELECT *, ROW_NUMBER() OVER (ORDER BY [Order] Desc) as row_num
        FROM ##tempPersonNames
        WHERE Gender = 2
    ) pn ON r.row_num = pn.row_num
WHERE
    RelativeNum = 2;

WITH students AS (
    SELECT
        sc.*,
        row_number() OVER (PARTITION BY ClassId ORDER BY p.FirstName, p.MiddleName, p.LastName) as NewClassNumber
    FROM
        [student].[StudentClass] sc
        INNER JOIN [core].[Person] p ON sc.[PersonID] = p.[PersonID]
    WHERE
        sc.[InstitutionID] = @institutionID AND
        sc.[SchoolYear] IN (select year from @schoolYearsTable)
)
UPDATE students
SET
    ClassNumber = students.NewClassNumber;

DROP TABLE ##tempPersonNames;
