GO

DELETE od FROM [school_books].[ClassBookOffDayDate] od
WHERE NOT EXISTS (
    SELECT 1
    FROM [school_books].[ClassBook] cb
    WHERE od.[SchoolYear] = cb.[SchoolYear]
        AND od.[ClassBookId] = cb.[ClassBookId]
)
GO

ALTER TABLE [school_books].[ClassBookOffDayDate]
ADD CONSTRAINT [FK_ClassBookOffDayDate_ClassBook]
    FOREIGN KEY ([SchoolYear], [ClassBookId])
    REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]);
GO

DELETE di FROM [school_books].[ClassBookSchoolYearDateInfo] di
WHERE NOT EXISTS (
    SELECT 1
    FROM [school_books].[ClassBook] cb
    WHERE di.[SchoolYear] = cb.[SchoolYear]
        AND di.[ClassBookId] = cb.[ClassBookId]
)
GO

ALTER TABLE [school_books].[ClassBookSchoolYearDateInfo]
ADD CONSTRAINT [FK_ClassBookSchoolYearDateInfo_ClassBook]
    FOREIGN KEY ([SchoolYear], [ClassBookId])
    REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]);
GO
