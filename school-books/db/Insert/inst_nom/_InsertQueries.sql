-- File:BasicClass.sql Table:[inst_nom].[BasicClass]
SELECT * FROM [inst_nom].[BasicClass]

-- File:BasicSubjectType.sql Table:[inst_nom].[BasicSubjectType]
SELECT * FROM [inst_nom].[BasicSubjectType]

-- File:ClassType.sql Table:[inst_nom].[ClassType]
SELECT * FROM [inst_nom].[ClassType]

-- File:CurriculumPart.sql Table:[inst_nom].[CurriculumPart]
SELECT * FROM [inst_nom].[CurriculumPart]

-- File:CustomVarValue.sql Table:[inst_nom].[CustomVarValue] identity
SELECT * FROM [inst_nom].[CustomVarValue]
WHERE InstitutionID IN ($(ScriptInstitutions))

-- File:EduForm.sql Table:[inst_nom].[EduForm]
SELECT * FROM [inst_nom].[EduForm]

-- File:FLLevel.sql Table:[inst_nom].[FLLevel]
SELECT * FROM [inst_nom].[FLLevel]

-- File:NKPDPosition.sql Table:[inst_nom].[NKPDPosition]
SELECT
    [NKPDPositionID],
    [NKPDSubGroupID] = NULL,
    [Code],
    [Name],
    [StaffTypeID],
    [Description],
    [IsValid],
    [ValidFrom],
    [ValidTo]
FROM
    [inst_nom].[NKPDPosition]

-- File:SPPOOEducArea.sql Table:[inst_nom].[SPPOOEducArea]
SELECT * FROM [inst_nom].[SPPOOEducArea]

-- File:SPPOOProfArea.sql Table:[inst_nom].[SPPOOProfArea]
SELECT * FROM [inst_nom].[SPPOOProfArea]

-- File:SPPOOProfession.sql Table:[inst_nom].[SPPOOProfession]
SELECT * FROM [inst_nom].[SPPOOProfession]

-- File:SPPOOSpeciality.sql Table:[inst_nom].[SPPOOSpeciality]
SELECT *
FROM [inst_nom].[SPPOOSpeciality]
ORDER BY [SPPOOSpecialityID], [ProfessionID]

-- File:SubjectType.sql Table:[inst_nom].[SubjectType]
SELECT * FROM [inst_nom].[SubjectType]
ORDER BY 1, 2

-- File:Subject.sql Table:[inst_nom].[Subject] identity
SELECT * FROM [inst_nom].[Subject]
WHERE SubjectID IN (SELECT SubjectID FROM [inst_year].[Curriculum] c WHERE c.InstitutionID IN ($(ScriptInstitutions)) AND c.[SchoolYear] IN ($(ScriptSchoolYears))) OR
      SubjectID IN (SELECT CustomVarValue FROM [inst_nom].[CustomVarValue] cvv WHERE cvv.InstitutionID IN ($(ScriptInstitutions)))
ORDER BY 1, 2

-- File:StaffType.sql Table:[inst_nom].[StaffType]
SELECT * FROM [inst_nom].[StaffType]
