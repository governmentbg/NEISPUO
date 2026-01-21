GO
PRINT 'Insert Student'
GO

INSERT INTO [student].[Student](
    [PersonID],
    [GPPhone],
    [GPName]
)
SELECT
    DISTINCT
    [PersonID] = sc.[PersonID],
    [GPPhone] = CONCAT('тел. ', sc.[PersonID]),
    [GPName] = CONCAT('др Иванов ', sc.[PersonID])
FROM
    [#tempStudentClass] sc
GO
