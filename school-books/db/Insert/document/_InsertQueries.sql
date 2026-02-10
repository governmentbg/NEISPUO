-- File:BasicDocument.sql Table:[document].[BasicDocument] identity
SELECT * FROM [document].[BasicDocument]

-- File:EducationType.sql Table:[document].[EducationType] identity
SELECT * FROM [document].[EducationType]

-- File:ITLevel.sql Table:[document].[ITLevel] identity
SELECT * FROM [document].[ITLevel]

-- File:Diploma.sql Table:[document].[Diploma] identity
SELECT *
FROM [document].[Diploma] d
WHERE EXISTS (
    SELECT 1
    FROM
        [inst_year].[ClassGroup] cg
        INNER JOIN [student].[StudentClass] s ON cg.[ClassID] = s.[ClassId]
        INNER JOIN [core].[Person] p ON s.[PersonID] = p.[PersonID]
    WHERE cg.[InstitutionID] IN ($(ScriptInstitutions))
        AND cg.[SchoolYear] IN ($(ScriptSchoolYears))
        AND d.[InstitutionID] = cg.[InstitutionID]
        AND d.[SchoolYear] = cg.[SchoolYear]
        AND d.[PersonID] = p.[PersonID]
) AND ([CreatedBySysUserID] IS NULL OR [CreatedBySysUserID] BETWEEN 1001 AND 1012)
    AND ([ModifiedBySysUserId] IS NULL OR [ModifiedBySysUserId] BETWEEN 1001 AND 1012)
    AND ([CancelledBySysUserId] IS NULL OR [CancelledBySysUserId] BETWEEN 1001 AND 1012)
    AND ([SignedBySysUserID] IS NULL OR [SignedBySysUserID] BETWEEN 1001 AND 1012)
    AND ([EditableSetBySysUserId] IS NULL OR [EditableSetBySysUserId] BETWEEN 1001 AND 1012)
