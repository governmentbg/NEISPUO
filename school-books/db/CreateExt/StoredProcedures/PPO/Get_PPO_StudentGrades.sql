PRINT 'Create Get_PPO_StudentGrades stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_PPO_StudentGrades]
    @schoolYear INT,
    @institutionID INT,
    @personalID VARCHAR(100)
AS
    DECLARE @personId INT;
    SET @personId = (SELECT PersonId FROM [core].[Person] WHERE PersonalID = @personalID);

    DECLARE @studentClassBooks TABLE (
        [PersonId] INT NOT NULL,
        [SchoolYear] INT NOT NULL,
        [InstId] INT NOT NULL,
        [ClassBookId] INT NOT NULL,
        [ClassId] INT NOT NULL
    );

    INSERT @studentClassBooks
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
        SchoolYear = @schoolYear AND
        InstId = @institutionID;

    SELECT
        [SchoolYear] = cb.SchoolYear,
        [InstitutionId] = cb.InstId,
        [ClassId] = cb.ClassId,
        [ClassName] = cg.ClassName,
        [GradeId] = g.GradeId,
        [Date] = g.Date,
        [CurriculumId] = c.CurriculumId,
        [SubjectId] = s.SubjectID,
        [SubjectName] = s.SubjectName,
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
        @studentClassBooks cb
        INNER JOIN [school_books].[ClassBook] cb2 ON cb.SchoolYear = cb2.SchoolYear AND cb.ClassBookId = cb2.ClassBookId
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Grade] g ON cb.SchoolYear = g.SchoolYear AND cb.ClassBookId = g.ClassBookId AND cb.PersonId = g.PersonId
        INNER JOIN [inst_year].[Curriculum] c ON g.CurriculumId = c.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
    WHERE
        cb2.BookType IN (2, 3, 4) AND -- 3-14 Дневник I - III клас, 3-16 Дневник IV клас, 3-87 Дневник V - XII клас
        cg.BasicClassID IN (1, 2, 3, 4, 5, 6, 7) AND
        g.Type IN (11, 22) AND -- Входно ниво, Годишна
        c.CurriculumPartID = 1 -- Раздел А
    ORDER BY
        g.Date,
        g.CreateDate
GO
