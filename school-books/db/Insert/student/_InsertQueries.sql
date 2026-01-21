-- File:CommuterType.sql Table:[student].[CommuterType] identity
select * from [student].[CommuterType]

-- File:RepeaterReason.sql Table:[student].[RepeaterReason] identity
select * from [student].[RepeaterReason]

-- File:Ministry.sql Table:[student].[Ministry] identity
select * from [student].[Ministry]

-- File:SanctionType.sql Table:[student].[SanctionType] identity
select * from [student].[SanctionType]

-- File:tempStudentClass.sql Table:[#tempStudentClass]
-- create table [#tempStudentClass] (
--     [SchoolYear]               SMALLINT NOT NULL,
--     [InstitutionID]            INT NOT NULL,
--     [PersonID]                 INT NOT NULL,
--     [ClassID]                  INT NULL,
--     [StudentSpecialityID]      INT NULL,
--     [StudentEduFormID]         INT NULL,
--     [Status]                   INT NOT NULL,
--     [ClassNumber]              INT NULL,
--     [IsIndividualCurriculum]   BIT NULL,
--     [IsNotPresentForm]         BIT NULL,
-- )
SELECT
    s.[SchoolYear],
    s.[InstitutionID],
    s.[PersonID],
    s.[ClassID],
    s.[StudentSpecialityID],
    s.[StudentEduFormID],
    s.[Status],
    s.[ClassNumber],
    s.[IsIndividualCurriculum],
    s.[IsNotPresentForm]
FROM
    [student].[StudentClass] s
    INNER JOIN [inst_year].ClassGroup cg ON s.[SchoolYear] = cg.[SchoolYear] AND s.ClassId = cg.ClassID
WHERE
  cg.InstitutionID IN ($(ScriptInstitutions)) AND cg.[SchoolYear] IN ($(ScriptSchoolYears))
ORDER BY
  1, 2, 3, 4
