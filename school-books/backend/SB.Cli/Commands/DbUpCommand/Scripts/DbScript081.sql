GO

-- change PgResult table schema
ALTER TABLE [school_books].[PgResult]
ADD
    [SubjectId] INT NULL,
    CONSTRAINT [FK_PgResult_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID])
GO

ALTER TABLE [school_books].[PgResult]
DROP CONSTRAINT [UK_PgResult_SchoolYear_ClassBookId_PersonId_CurriculumId]
GO

-- fill SubjectId and delete duplicated
UPDATE r
SET
    r.SubjectId = c.SubjectID,
    r.StartSchoolYearResult = prob.StartSchoolYearResult,
    r.EndSchoolYearResult = prob.EndSchoolYearResult
OUTPUT
    inserted.PgResultId,
    inserted.SubjectId,
    deleted.StartSchoolYearResult,
    deleted.EndSchoolYearResult,
    inserted.StartSchoolYearResult,
    inserted.EndSchoolYearResult
FROM school_books.PgResult r
JOIN inst_year.Curriculum c ON r.CurriculumId = c.CurriculumID
JOIN (
    SELECT
        r.ClassBookId,
        r.PersonId,
        c.SubjectID,
        StartSchoolYearResult = STRING_AGG(
            r.StartSchoolYearResult,
            CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10)
        ),
        EndSchoolYearResult = STRING_AGG(
            r.EndSchoolYearResult,
            CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10)
        )
    FROM school_books.PgResult r
    JOIN inst_year.Curriculum c ON r.CurriculumId = c.CurriculumID
    GROUP BY r.ClassBookId, r.PersonId, c.SubjectID
    HAVING COUNT(*) > 1
) AS prob
    ON r.ClassBookId = prob.ClassBookId
    AND r.PersonId = prob.PersonId
    AND c.SubjectId = prob.SubjectID

DELETE r
OUTPUT
    deleted.SchoolYear,
    deleted.PgResultId,
    deleted.ClassBookId,
    deleted.PersonId,
    deleted.StartSchoolYearResult,
    deleted.EndSchoolYearResult,
    deleted.CreateDate,
    deleted.CreatedBySysUserId,
    deleted.ModifyDate,
    deleted.ModifiedBySysUserId,
    deleted.Version,
    deleted.CurriculumId,
    deleted.SubjectId
FROM school_books.PgResult r
JOIN (
    SELECT
        r.PgResultId,
        ROW_NUMBER() OVER(
            PARTITION BY r.ClassBookId, r.PersonId, c.SubjectID
            ORDER BY r.CreateDate ASC
        ) AS row_num
    FROM school_books.PgResult r
    JOIN inst_year.Curriculum c ON r.CurriculumId = c.CurriculumID
) AS del
    ON r.PgResultId = del.PgResultId
WHERE del.row_num > 1

-- straight case
UPDATE pgr
SET SubjectId = c.SubjectID
FROM [school_books].[PgResult] pgr
JOIN [inst_year].[Curriculum] c
    ON pgr.CurriculumId = c.CurriculumID
WHERE pgr.SubjectId IS NULL

-- finish the PgResult table schema
ALTER TABLE [school_books].[PgResult]
ADD CONSTRAINT [UK_PgResult_SchoolYear_ClassBookId_PersonId_SubjectId]
UNIQUE ([SchoolYear], [ClassBookId], [PersonId], [SubjectId])

GO
