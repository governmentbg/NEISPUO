ALTER TABLE [school_books].[MissingTopicsReportItem] DROP CONSTRAINT [PK_MissingTopicsReportItem];
GO

ALTER TABLE [school_books].[MissingTopicsReportItem]
ADD
    CONSTRAINT [PK_MissingTopicsReportItem] PRIMARY KEY ([SchoolYear], [MissingTopicsReportId], [MissingTopicsReportItemId]),
    [ClassBookName] NVARCHAR(560) NULL,
    [CurriculumName] NVARCHAR(550) NULL;
GO

EXEC [school_books].[spCreateIdSequence] N'MissingTopicsReportItemTeacher'
GO

CREATE TABLE [school_books].[MissingTopicsReportItemTeacher] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [MissingTopicsReportId]             INT              NOT NULL,
    [MissingTopicsReportItemId]         INT              NOT NULL,
    [MissingTopicsReportItemTeacherId]  INT              NOT NULL,
    [PersonName]                        NVARCHAR(500)    NOT NULL,

    CONSTRAINT [PK_MissingTopicsReportItemTeacher] PRIMARY KEY ([SchoolYear], [MissingTopicsReportId], [MissingTopicsReportItemId], [MissingTopicsReportItemTeacherId]),
    CONSTRAINT [FK_MissingTopicsReportItemTeacher_MissingTopicsReportItem]
        FOREIGN KEY ([SchoolYear], [MissingTopicsReportId], [MissingTopicsReportItemId])
        REFERENCES [school_books].[MissingTopicsReportItem] ([SchoolYear], [MissingTopicsReportId], [MissingTopicsReportItemId]),
);
GO

UPDATE i
    SET [ClassBookName] = cb.[FullBookName]
    FROM [school_books].[MissingTopicsReportItem] i
    INNER JOIN [school_books].[ClassBook] cb ON cb.[SchoolYear] = i.[SchoolYear] AND cb.[ClassId] = i.[ClassId]
GO

UPDATE i
    SET [CurriculumName] = CONCAT_WS(' / ', s.SubjectName, st.Name)
    FROM [school_books].[MissingTopicsReportItem] i
    INNER JOIN [inst_year].[Curriculum] c ON i.CurriculumId = c.CurriculumID
    INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
    INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
GO

ALTER TABLE [school_books].[MissingTopicsReportItem]
ALTER COLUMN [ClassBookName] NVARCHAR(560) NOT NULL;
ALTER TABLE [school_books].[MissingTopicsReportItem]
ALTER COLUMN [CurriculumName] NVARCHAR(550) NOT NULL;
GO

INSERT INTO [school_books].[MissingTopicsReportItemTeacher]
        ([SchoolYear], [MissingTopicsReportId], [MissingTopicsReportItemId], [MissingTopicsReportItemTeacherId], [PersonName])
    SELECT
        i.[SchoolYear],
        i.[MissingTopicsReportId],
        i.[MissingTopicsReportItemId],
        [MissingTopicsReportItemTeacherId] = NEXT VALUE FOR [school_books].[MissingTopicsReportItemTeacherIdSequence],
        [school_books].[fn_join_names2](p.[FirstName], p.[LastName]) AS PersonName
    FROM [school_books].[MissingTopicsReportItem] i
    INNER JOIN [core].[Person] p ON i.TeacherPersonId = p.PersonID
GO

DROP INDEX [IX_MissingTopicsReportItem_CurriculumId] ON [school_books].[MissingTopicsReportItem];
DROP INDEX [IX_MissingTopicsReportItem_ClassId] ON [school_books].[MissingTopicsReportItem];
GO

ALTER TABLE [school_books].[MissingTopicsReportItem]
DROP
    CONSTRAINT [FK_MissingTopicsReportItem_TeacherPersonId],
    COLUMN [TeacherPersonId],
    CONSTRAINT [FK_MissingTopicsReportItem_ClassGroup],
    COLUMN [ClassId],
    CONSTRAINT [FK_MissingTopicsReportItem_Curriculum],
    COLUMN [CurriculumId];
GO
