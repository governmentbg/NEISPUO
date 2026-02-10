GO

ALTER TABLE [school_books].[SchoolYearSettings]
ADD
    [HasFutureEntryLock] BIT NOT NULL
        CONSTRAINT [DEFAULT_SchoolYearSettings_HasFutureEntryLock] DEFAULT 0,
    [PastMonthLockDay] INT NULL
        CONSTRAINT [CHK_SchoolYearSettings_PastMonthLockDay] CHECK ([PastMonthLockDay] BETWEEN 1 AND 31)
GO

ALTER TABLE [school_books].[SchoolYearSettings]
DROP CONSTRAINT [DEFAULT_SchoolYearSettings_HasFutureEntryLock]
GO

ALTER TABLE [school_books].[SchoolYearSettings]
ALTER COLUMN [SchoolYearStartDate] DATE NULL
GO

ALTER TABLE [school_books].[SchoolYearSettings]
ALTER COLUMN [FirstTermEndDate] DATE NULL
GO

ALTER TABLE [school_books].[SchoolYearSettings]
ALTER COLUMN [SecondTermStartDate] DATE NULL
GO

ALTER TABLE [school_books].[SchoolYearSettings]
ALTER COLUMN [SchoolYearEndDate] DATE NULL
GO

ALTER TABLE [school_books].[ClassBookSchoolYearSettings]
ADD
    [HasFutureEntryLock] BIT NOT NULL
        CONSTRAINT [DEFAULT_ClassBookSchoolYearSettings_HasFutureEntryLock] DEFAULT 0,
    [PastMonthLockDay] INT NULL
GO

ALTER TABLE [school_books].[ClassBookSchoolYearSettings]
DROP CONSTRAINT [DEFAULT_ClassBookSchoolYearSettings_HasFutureEntryLock]
GO

ALTER TABLE [school_books].[ClassBook]
ADD
    [IsFinalized] BIT NOT NULL
        CONSTRAINT [DEFAULT_ClassBook_IsFinalized] DEFAULT 0
GO

ALTER TABLE [school_books].[ClassBook]
DROP CONSTRAINT [DEFAULT_ClassBook_IsFinalized]
GO
