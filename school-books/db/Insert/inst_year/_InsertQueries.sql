-- File:ClassGroup.sql Table:[inst_year].[ClassGroup] identity
SELECT
    [ClassID],
    [SchoolYear],
    [InstitutionID],
    [InstitutionDepartmentID] = NULL,
    [ClassGroupNum],
    [ClassName],
    [ParentClassID],
    [BasicClassID],
    [ClassTypeID],
    [AreaID] = NULL,
    [ClassEduFormID],
    [ClassEduDurationID] = NULL,
    [ClassShiftID] = NULL,
    [BudgetingClassTypeID] = NULL,
    [EntranceLevelID] = NULL,
    [ClassSpecialityID],
    [FLTypeID] = NULL,
    [FLID] = NULL,
    [IsProfModule],
    [StudentCountPlaces],
    [Notes],
    [IsCombined],
    [IsNoList],
    [IsSpecNeed],
    [ValidFrom],
    [ValidTo],
    [SysUserID] = NULL,
    [ParalellClassName],
    [IsWholeClass],
    [IsNotPresentForm]
FROM [inst_year].[ClassGroup]
WHERE InstitutionID IN ($(ScriptInstitutions)) AND [SchoolYear] IN ($(ScriptSchoolYears))
ORDER BY 1, 2, 3

-- File:Curriculum.sql Table:[inst_year].[Curriculum] identity
SELECT
    [CurriculumID],
    [SchoolYear],
    [InstitutionID],
    [CurriculumGroupNum],
    [SubjectID],
    [SubjectTypeID],
    [WeeksFirstTerm],
    [HoursWeeklyFirstTerm],
    [WeeksSecondTerm],
    [HoursWeeklySecondTerm],
    [IsFL],
    [FLSubjectID],
    [IsIndividualLesson],
    [NormaS],
    [InstitutionDepartmentID] = NULL,
    [ParentCurriculumID],
    [CurriculumPartID],
    [SortOrder],
    [ValidFrom],
    [ValidTo],
    [SysUserID] = NULL,
    [IsIndividualCurriculum],
    [TotalTermHours],
    [IsValid]
FROM [inst_year].[Curriculum]
WHERE InstitutionID IN ($(ScriptInstitutions)) AND [SchoolYear] IN ($(ScriptSchoolYears))
ORDER BY 1, 2, 3

-- File:CurriculumClass.sql Table:[inst_year].[CurriculumClass]
SELECT [CurriculumID], [ClassID], [IsValid] FROM [inst_year].[CurriculumClass] cc
WHERE EXISTS (SELECT * FROM [inst_year].[Curriculum] c WHERE c.CurriculumID = cc.CurriculumID AND c.InstitutionID IN ($(ScriptInstitutions)) AND c.[SchoolYear] IN ($(ScriptSchoolYears)))
ORDER BY 1, 2

-- File:CurriculumStudent.sql Table:[inst_year].[CurriculumStudent]
SELECT [CurriculumID], [StudentID], [PersonID], [IsValid] FROM [inst_year].[CurriculumStudent] cs
WHERE EXISTS (SELECT * FROM [inst_year].[Curriculum] c WHERE c.CurriculumID = cs.CurriculumID AND c.InstitutionID IN ($(ScriptInstitutions)) AND c.[SchoolYear] IN ($(ScriptSchoolYears)))
ORDER BY 1, 2

-- File:CurriculumTeacher.sql Table:[inst_year].[CurriculumTeacher] identity
SELECT
    [CurriculumTeacherID],
    [CurriculumID],
    [StaffPositionID],
    [ValidFrom],
    [SchoolYear],
    [IsValid],
    [StaffPositionStartDate],
    [StaffPositionTerminationDate],
    [NoReplacement]
FROM [inst_year].[CurriculumTeacher] ct
WHERE EXISTS (SELECT * FROM [inst_year].[Curriculum] c WHERE c.CurriculumID = ct.CurriculumID AND c.InstitutionID IN ($(ScriptInstitutions)) AND c.[SchoolYear] IN ($(ScriptSchoolYears)))
ORDER BY 1, 2
