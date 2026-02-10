PRINT 'Empty schema school_books'
GO

USE [$(dbName)]
GO

:r "./CreateSchoolBooks/StoredProcedures/spDropSchemaObjects.sql"

EXEC [school_books].[spDropSchemaObjects] N'school_books'
GO

:r "./CreateSchoolBooks/Create.sql"

:r "./Insert/school_books/SupportActivityType.sql"
:r "./Insert/school_books/SupportDifficultyType.sql"
:r "./Insert/school_books/ProtocolExamType.sql"
:r "./Insert/school_books/ProtocolExamSubType.sql"
:r "./Insert/school_books/QualificationDegree.sql"
:r "./Insert/school_books/QualificationExamType.sql"
:r "./Insert/school_books/AbsenceReason.sql"

:r "./Insert/school_books/ClassBook.sql"
