CREATE UNIQUE INDEX [UQ_SchoolYearDateInfo_SchoolYear_InstId_IsForAllClasses]
    ON [school_books].[SchoolYearDateInfo]([SchoolYear], [InstId])
    INCLUDE ([SchoolYearDateInfoId])
    WHERE [IsForAllClasses] = 1
GO
