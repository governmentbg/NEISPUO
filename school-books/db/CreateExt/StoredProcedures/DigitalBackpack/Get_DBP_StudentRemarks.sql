PRINT 'Create Get_DBP_StudentRemarks stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_DBP_StudentRemarks]
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
        [RemarkId] = r.RemarkId,
        [Date] = r.Date,
        [CurriculumId] = c.CurriculumId,
        [CurriculumGroupName] = c.CurriculumGroupNum,
        [SubjectId] = s.SubjectID,
        [SubjectName] = s.SubjectName,
        [SubjectNameShort] = s.SubjectNameShort,
        [SubjectTypeId] = st.SubjectTypeID,
        [SubjectTypeName] = st.Name,
        [Type] = r.[Type],
        [Description] = r.[Description]
    FROM
        #StudentClassBooks cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Remark] r ON cb.SchoolYear = r.SchoolYear AND cb.ClassBookId = r.ClassBookId AND cb.PersonId = r.PersonId
        INNER JOIN [inst_year].[Curriculum] c ON r.CurriculumId = c.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
    ORDER BY
        r.Date,
        r.CreateDate;

    DROP TABLE #StudentClassBooks;
GO
