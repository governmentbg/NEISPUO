GO

DROP INDEX [IX_ClassBookStudentActivity_ClassId] ON [school_books].[ClassBookStudentActivity];
ALTER TABLE [school_books].[ClassBookStudentActivity]
DROP
    CONSTRAINT [PK_ClassBookStudentActivity],
    CONSTRAINT [FK_ClassBookStudentActivity_ClassGroup],
    COLUMN [ClassId];
ALTER TABLE [school_books].[ClassBookStudentActivity]
ADD CONSTRAINT [PK_ClassBookStudentActivity] PRIMARY KEY ([SchoolYear], [ClassBookId], [PersonId]);
GO

DROP INDEX [IX_ClassBookStudentGradeless_ClassId] ON [school_books].[ClassBookStudentGradeless];
ALTER TABLE [school_books].[ClassBookStudentGradeless]
DROP
    CONSTRAINT [PK_ClassBookStudentGradeless],
    CONSTRAINT [FK_ClassBookStudentGradeless_ClassGroup],
    COLUMN [ClassId];
ALTER TABLE [school_books].[ClassBookStudentGradeless]
ADD CONSTRAINT [PK_ClassBookStudentGradeless] PRIMARY KEY ([SchoolYear], [ClassBookId], [PersonId], [CurriculumId]);
GO

WITH ClassBookStudentSpecialNeedsByPersonId AS (
    SELECT
        ROW_NUMBER() OVER(
            PARTITION BY
                [SchoolYear],
                [ClassBookId],
                [PersonId],
                [CurriculumId]
            ORDER BY [ClassId]
        ) AS RowNumber,
        ClassBookStudentSpecialNeeds.*
    FROM [school_books].[ClassBookStudentSpecialNeeds]
)
DELETE FROM ClassBookStudentSpecialNeedsByPersonId
WHERE RowNumber > 1
GO

DROP INDEX [IX_ClassBookStudentSpecialNeeds_ClassId] ON [school_books].[ClassBookStudentSpecialNeeds];
ALTER TABLE [school_books].[ClassBookStudentSpecialNeeds]
DROP
    CONSTRAINT [PK_ClassBookStudentSpecialNeeds],
    CONSTRAINT [FK_ClassBookStudentSpecialNeeds_ClassGroup],
    COLUMN [ClassId];
ALTER TABLE [school_books].[ClassBookStudentSpecialNeeds]
ADD CONSTRAINT [PK_ClassBookStudentSpecialNeeds] PRIMARY KEY ([SchoolYear], [ClassBookId], [PersonId], [CurriculumId]);
GO
