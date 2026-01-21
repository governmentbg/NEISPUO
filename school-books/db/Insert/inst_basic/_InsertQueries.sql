-- File:StaffPosition.sql Table:[inst_basic].[StaffPosition] identity
SELECT
    [StaffPositionID],
    [PersonID],
    [InstitutionID],
    [ContractTypeID] = NULL,
    [StaffPositionNo],
    [NKPDPositionID],
    [StaffTypeID]
FROM [inst_basic].[StaffPosition] sp
WHERE EXISTS (
    SELECT *
    FROM
        [inst_year].[CurriculumTeacher] ct
        INNER JOIN [inst_year].[Curriculum] c ON ct.CurriculumID = c.CurriculumID
    WHERE
        ct.StaffPositionID = sp.StaffPositionID AND
        c.InstitutionID IN ($(ScriptInstitutions)) AND c.[SchoolYear] IN ($(ScriptSchoolYears))
)
ORDER BY 1, 2

-- File:CurrentYear.sql Table:[inst_basic].[CurrentYear]
SELECT * FROM [inst_basic].[CurrentYear]

-- File:InstitutionDetail.sql Table:[inst_basic].[InstitutionDetail]
SELECT
    [InstitutionID],
    [Email],
    [Website],
    [EstablishedYear],
    [ConstitActFirst],
    [ConstitActLast],
    [IsODZ],
    [IsProfSchool],
    [IsNational],
    [IsProvideEduServ],
    [IsDelegateBudget],
    [IsNonIndDormitory],
    [IsInternContract],
    [BankIBAN],
    [BankBIC],
    [BankName],
    [BankAccountHolder],
    [SysUserID] = NULL,
    [IsAppInnovSystem]
FROM
    [inst_basic].[InstitutionDetail]
WHERE
    InstitutionID IN ($(ScriptInstitutions))
