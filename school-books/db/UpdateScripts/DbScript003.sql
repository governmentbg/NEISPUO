ALTER TABLE [school_books].[Absence] DROP CONSTRAINT [FK_Absence_Subject];
ALTER TABLE [school_books].[Absence] DROP CONSTRAINT [FK_Absence_SubjectType];
ALTER TABLE [school_books].[Absence] DROP COLUMN [SubjectId];
ALTER TABLE [school_books].[Absence] DROP COLUMN [SubjectTypeId];

ALTER TABLE [school_books].[Exam] DROP CONSTRAINT [FK_Exam_Subject];
ALTER TABLE [school_books].[Exam] DROP CONSTRAINT [FK_Exam_SubjectType];
ALTER TABLE [school_books].[Exam] DROP COLUMN [SubjectId];
ALTER TABLE [school_books].[Exam] DROP COLUMN [SubjectTypeId];

ALTER TABLE [school_books].[Grade] DROP CONSTRAINT [FK_Grade_Subject];
ALTER TABLE [school_books].[Grade] DROP CONSTRAINT [FK_Grade_SubjectType];
ALTER TABLE [school_books].[Grade] DROP COLUMN [SubjectId];
ALTER TABLE [school_books].[Grade] DROP COLUMN [SubjectTypeId];

ALTER TABLE [school_books].[MissingTopicsReportItem] DROP CONSTRAINT [FK_MissingTopicsReportItem_Subject];
ALTER TABLE [school_books].[MissingTopicsReportItem] DROP CONSTRAINT [FK_MissingTopicsReportItem_SubjectType];
ALTER TABLE [school_books].[MissingTopicsReportItem] DROP COLUMN [SubjectId];
ALTER TABLE [school_books].[MissingTopicsReportItem] DROP COLUMN [SubjectTypeId];

ALTER TABLE [school_books].[Remark] DROP CONSTRAINT [FK_Remark_Subject];
ALTER TABLE [school_books].[Remark] DROP CONSTRAINT [FK_Remark_SubjectType];
ALTER TABLE [school_books].[Remark] DROP COLUMN [SubjectId];
ALTER TABLE [school_books].[Remark] DROP COLUMN [SubjectTypeId];

ALTER TABLE [school_books].[GradeResultSubject] DROP CONSTRAINT [PK_GradeResultSubject];
ALTER TABLE [school_books].[GradeResultSubject] ADD CONSTRAINT [PK_GradeResultSubject] PRIMARY KEY ([SchoolYear], [GradeResultId], [CurriculumId]);
ALTER TABLE [school_books].[GradeResultSubject] DROP CONSTRAINT [FK_GradeResultSubject_Subject];
ALTER TABLE [school_books].[GradeResultSubject] DROP CONSTRAINT [FK_GradeResultSubject_SubjectType];
ALTER TABLE [school_books].[GradeResultSubject] DROP COLUMN [SubjectId];
ALTER TABLE [school_books].[GradeResultSubject] DROP COLUMN [SubjectTypeId];

ALTER TABLE [school_books].[ExamResultsBookRecord]    ALTER COLUMN [MiddleName] NVARCHAR(255) NULL;
ALTER TABLE [school_books].[LiableBookRecord]         ALTER COLUMN [MiddleName] NVARCHAR(255) NULL;
ALTER TABLE [school_books].[LiableBookRecordRelative] ALTER COLUMN [MiddleName] NVARCHAR(255) NULL;
ALTER TABLE [school_books].[MainBookRecord]           ALTER COLUMN [MiddleName] NVARCHAR(255) NULL;

ALTER TABLE [school_books].[GradeResultSubject] ADD CONSTRAINT [FK_GradeResultSubject_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]);
ALTER TABLE [school_books].[MissingTopicsReportItem] ADD CONSTRAINT [FK_MissingTopicsReportItem_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]);

ALTER TABLE [school_books].[PgResult] ADD CONSTRAINT [UK_PgResult_SchoolYear_ClassBookId_PersonId] UNIQUE ([SchoolYear], [ClassBookId], [PersonId]);

GO
