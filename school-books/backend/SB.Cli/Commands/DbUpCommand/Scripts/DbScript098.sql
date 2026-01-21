GO

ALTER TABLE [school_books].[Absence]
DROP CONSTRAINT [UK_Absence_ClassBookId_PersonId_ScheduleLessonId];
GO

ALTER TABLE [school_books].[Absence]
ADD CONSTRAINT [UK_Absence_PersonId_SchoolYear_ClassBookId_ScheduleLessonId]
        UNIQUE ([PersonId], [SchoolYear], [ClassBookId], [ScheduleLessonId]);
GO
