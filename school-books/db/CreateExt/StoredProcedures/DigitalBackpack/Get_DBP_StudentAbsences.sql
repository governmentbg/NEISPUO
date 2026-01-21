PRINT 'Create Get_DBP_StudentAbsences stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_DBP_StudentAbsences]
    @schoolYear INT,
    @azureID VARCHAR(100)
AS
    DECLARE @personId INT;
    SET @personId = (SELECT PersonId FROM [core].[Person] WHERE AzureID = @azureID);

    CREATE TABLE #StudentClassBooks (
        [PersonId] INT NOT NULL,
        [SchoolYear] INT NOT NULL,
        [InstId] INT NOT NULL,
        [ClassBookId] INT NOT NULL,
        [ClassId] INT NOT NULL
    );

    INSERT #StudentClassBooks
    SELECT
        [PersonId],
        [SchoolYear],
        [InstId],
        [ClassBookId],
        [ClassId]
    FROM
        [school_books].[vwStudentClassBooks]
    WHERE
        PersonId = @personId AND
        SchoolYear = @schoolYear;

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
        #StudentClassBooks cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Absence] a ON cb.SchoolYear = a.SchoolYear AND cb.ClassBookId = a.ClassBookId AND cb.PersonId = a.PersonId
        INNER JOIN [school_books].[ScheduleLesson] sl ON a.ScheduleLessonId = sl.ScheduleLessonId
        INNER JOIN [inst_year].[Curriculum] c ON sl.CurriculumId = c.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
    ORDER BY
        a.Date,
        a.CreateDate;

    DROP TABLE #StudentClassBooks;
GO
