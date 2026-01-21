-- File:RelativeType.sql Table:[family].[RelativeType] identity
select * from [family].[RelativeType]

-- File:SysRole.sql Table:[core].[SysRole] identity
select * from [core].[SysRole]

-- File:Position.sql Table:[core].[Position] identity
select * from [core].[Position]

-- File:Institution.sql Table:[core].[Institution]
SELECT
    [InstitutionID],
    [Name],
    [Abbreviation],
    [LocalAreaID],
    [FinancialSchoolTypeID],
    [DetailedSchoolTypeID],
    [BudgetingSchoolTypeID],
    [TownId],
    [BaseSchoolTypeID]
FROM
  [core].[Institution]
WHERE
  [InstitutionID] IN ($(ScriptInstitutions))

-- File:InstitutionSchoolYear.sql Table:[core].[InstitutionSchoolYear]
SELECT
    [InstitutionId],
    [SchoolYear],
    [IsFinalized],
    [Name],
    [Abbreviation],
    [Bulstat],
    [CountryID],
    [LocalAreaID],
    [FinancialSchoolTypeID],
    [DetailedSchoolTypeID],
    [BudgetingSchoolTypeID],
    [TownID],
    [BaseSchoolTypeID],
    [IsCurrent],
    [SysUserID] = NULL,
    [ValidFrom],
    [ValidTo]
FROM
    [core].[InstitutionSchoolYear]
WHERE
  [InstitutionID] IN ($(ScriptInstitutions))

-- File:tempPerson.sql Table:[#tempPerson]
--create table [#tempPerson] (
--    [PersonID]             INT    NOT NULL
--)
SELECT
    [PersonID]
FROM (
    SELECT DISTINCT
      p.[PersonID]
    FROM
        [inst_year].[ClassGroup] cg
        INNER JOIN [student].[StudentClass] s ON cg.[ClassID] = s.[ClassId]
        INNER JOIN [core].[Person] p ON s.[PersonID] = p.[PersonID]
    WHERE cg.[InstitutionID] IN ($(ScriptInstitutions)) AND cg.[SchoolYear] IN ($(ScriptSchoolYears))
UNION
    SELECT DISTINCT
      p.[PersonID]
    FROM
        [inst_basic].[StaffPosition] sp
        INNER JOIN [inst_year].[CurriculumTeacher] ct ON sp.[StaffPositionID] = ct.[StaffPositionID]
        INNER JOIN [core].[Person] p ON sp.[PersonID] = p.[PersonID]
    WHERE
        sp.[InstitutionID] IN ($(ScriptInstitutions)) AND
        EXISTS (
            SELECT *
            FROM
                [inst_year].[Curriculum] c
            WHERE
                c.CurriculumID = ct.CurriculumID AND
                c.InstitutionID IN ($(ScriptInstitutions))
                AND c.[SchoolYear] IN ($(ScriptSchoolYears))
        )
UNION
    SELECT DISTINCT
      p.[PersonID]
    FROM
        [inst_year].[ClassGroup] cg
        INNER JOIN [student].[StudentClass] s ON cg.[ClassID] = s.[ClassId]
        INNER JOIN [family].[Relative] r ON s.[PersonID] = r.[PersonID]
        INNER JOIN [core].[Person] p ON r.[PersonID] = p.[PersonID]
    WHERE cg.[InstitutionID] IN ($(ScriptInstitutions)) AND cg.[SchoolYear] IN ($(ScriptSchoolYears))
) s_inner
ORDER BY 1

-- File:Relative.sql Table:[family].[Relative] identity
WITH StudentPersonIds AS (
    SELECT DISTINCT
        s.[PersonID]
    FROM
        [inst_year].[ClassGroup] cg
        INNER JOIN [student].[StudentClass] s ON cg.[ClassID] = s.[ClassId]
    WHERE
        cg.[InstitutionID] IN ($(ScriptInstitutions)) AND
        cg.[SchoolYear] IN ($(ScriptSchoolYears))
)
SELECT
    r.[RelativeID],
    r.[PersonID],
    [WorkStatusID] = NULL,
    [EducationTypeId] = NULL,
    r.[tmp_StudentID],
    r.[tmp_RelativeType],
    r.[RelativeTypeID]
FROM [family].[Relative] r
    INNER JOIN StudentPersonIds sids ON r.[PersonID] = sids.[PersonID]
ORDER BY 1, 2, 3

-- File:InstitutionDepartment.sql Table:[inst_basic].[InstitutionDepartment] identity
SELECT
    [InstitutionDepartmentID],
    [InstitutionID],
    [Name],
    [CountryID],
    [TownID],
    [LocalAreaID],
    [Address],
    [PostCode],
    [IsMain],
    [ValidFrom],
    [ValidTo],
    [IsValid],
    [SysUserID] =
        CASE
            WHEN [InstitutionID] = 300125 THEN 1001
            WHEN [InstitutionID] = 2206409 THEN 1002
            ELSE 1
        END
FROM [inst_basic].[InstitutionDepartment]
WHERE [InstitutionID] IN ($(ScriptInstitutions))

-- File:InstitutionConfData.sql Table:[core].[InstitutionConfData] identity
SELECT
    [InstitutionConfDataID],
    [InstitutionID],
    [SchoolYear],
    [SOVersion],
    [CBVersion],
    [SysUserID],
    [SOExtProviderID] = NULL,
    [CBExtProviderID] = NULL
FROM [core].[InstitutionConfData]
WHERE [InstitutionID] IN ($(ScriptInstitutions))
