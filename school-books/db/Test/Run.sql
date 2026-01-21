USE [$(dbName)]

:r "./Test/Tests/spStudentAdmitted.sql"

EXEC [tSQLt].[RunAll]

EXIT(SELECT COUNT(*) FROM [tSQLt].[TestResult] WHERE [Result] != 'Success')
