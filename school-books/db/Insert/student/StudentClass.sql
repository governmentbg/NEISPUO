GO
PRINT 'Insert StudentClass'
GO

INSERT INTO [student].[StudentClass](
    [InstitutionId],
    [SchoolYear],
    [ClassId],
    [PersonId],
    [StudentSpecialityId],
    [StudentEduFormId],
    [ClassNumber],
    [Status],
    [IsIndividualCurriculum],
    [IsNotPresentForm],
    [RepeaterId],
    [CommuterTypeId]
)
SELECT
    [InstitutionId] = s.[InstitutionId],
    [SchoolYear] = s.[SchoolYear],
    [ClassId] = s.[ClassId],
    [PersonId] = s.[PersonId],
    [StudentSpecialityId] = s.[StudentSpecialityId],
    [StudentEduFormId] = s.[StudentEduFormId],
    [ClassNumber] = s.[ClassNumber],
    [Status] = s.[Status],
    [IsIndividualCurriculum] = s.[IsIndividualCurriculum],
    [IsNotPresentForm] = s.[IsNotPresentForm],
    [RepeaterId] = 1,
    [CommuterTypeId] = 1
FROM
    [#tempStudentClass] s
    INNER JOIN [core].[Person] p ON s.[PersonId] = p.[PersonId]
WHERE
    s.[ClassId] IS NOT NULL
GO
