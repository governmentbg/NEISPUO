CREATE OR ALTER PROCEDURE [ext].[Get_DBP_StudentAbsences]
    @schoolYear INT,
    @azureID VARCHAR(100)
AS
    SELECT
        [SchoolYear] = cb.SchoolYear,
        [InstitutionId] = cb.InstId,
        [ClassId] = cb.ClassId,
        [ClassName] = cg.ClassName,
        [AbsenceId] = a.AbsenceId,
        [Date] = a.Date,
        [CurriculumId] = c.CurriculumId,
        [CurriculumGroupName] = c.CurriculumGroupNum,
        [SubjectId] = s.SubjectID,
        [SubjectName] = s.SubjectName,
        [SubjectNameShort] = s.SubjectNameShort,
        [SubjectTypeId] = st.SubjectTypeID,
        [SubjectTypeName] = st.Name,
        [Type] = a.[Type],
        [Term] = a.[Term],
        [ExcusedReasonId] = a.[ExcusedReasonId],
        [ExcusedReasonComment] = a.[ExcusedReasonComment]
    FROM
        [core].[Person] student
        INNER JOIN [school_books].[vwStudentClassBooks] cb ON student.PersonID = cb.PersonId
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Absence] a ON cb.SchoolYear = a.SchoolYear AND cb.ClassBookId = a.ClassBookId AND cb.PersonId = a.PersonId
        INNER JOIN [school_books].[ScheduleLesson] sl ON a.ScheduleLessonId = sl.ScheduleLessonId
        INNER JOIN [inst_year].[Curriculum] c ON sl.CurriculumId = c.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
    WHERE
        student.AzureID = @azureID AND
        cb.SchoolYear = @schoolYear
    ORDER BY
        a.Date,
        a.CreateDate
GO

DROP INDEX [IX_Absence_CurriculumId] ON [school_books].[Absence]
GO

ALTER TABLE [school_books].[Absence] DROP CONSTRAINT [FK_Absence_Curriculum]
GO

ALTER TABLE [school_books].[Absence] DROP COLUMN [CurriculumId]
GO
