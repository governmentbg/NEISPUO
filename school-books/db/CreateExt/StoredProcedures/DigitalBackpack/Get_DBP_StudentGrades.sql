PRINT 'Create Get_DBP_StudentGrades stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_DBP_StudentGrades]
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
        [GradeId] = g.GradeId,
        [Date] = g.Date,
        [CurriculumId] = c.CurriculumId,
        [CurriculumGroupName] = c.CurriculumGroupNum,
        [SubjectId] = s.SubjectID,
        [SubjectName] = s.SubjectName,
        [SubjectNameShort] = s.SubjectNameShort,
        [SubjectTypeId] = st.SubjectTypeID,
        [SubjectTypeName] = st.Name,
        [Type] = g.[Type],
        [Term] = g.[Term],
        [Category] = g.[Category],
        [DecimalGrade] = g.[DecimalGrade],
        [SpecialGrade] = g.[SpecialGrade],
        [QualitativeGrade] = g.[QualitativeGrade],
        [Comment] = g.[Comment]
    FROM
        #StudentClassBooks cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Grade] g ON cb.SchoolYear = g.SchoolYear AND cb.ClassBookId = g.ClassBookId AND cb.PersonId = g.PersonId
        INNER JOIN [inst_year].[Curriculum] c ON g.CurriculumId = c.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
    ORDER BY
        g.Date,
        g.CreateDate;

    DROP TABLE #StudentClassBooks;
GO
