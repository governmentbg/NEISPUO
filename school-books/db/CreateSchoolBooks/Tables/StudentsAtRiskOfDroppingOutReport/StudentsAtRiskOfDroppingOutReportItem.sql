PRINT 'Create StudentsAtRiskOfDroppingOutReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'StudentsAtRiskOfDroppingOutReportItem'
GO

CREATE TABLE [school_books].[StudentsAtRiskOfDroppingOutReportItem] (
    [SchoolYear]                                SMALLINT         NOT NULL,
    [StudentsAtRiskOfDroppingOutReportId]       INT              NOT NULL,
    [StudentsAtRiskOfDroppingOutReportItemId]   INT              NOT NULL,

    [PersonId]                                  INT              NOT NULL, -- no FK, we dont need a hard reference
    [PersonalId]                                NVARCHAR(100)    NOT NULL,
    [FirstName]                                 NVARCHAR(100)    NOT NULL,
    [MiddleName]                                NVARCHAR(100)    NOT NULL,
    [LastName]                                  NVARCHAR(100)    NOT NULL,
    [ClassBookName]                             NVARCHAR(100)    NOT NULL,
    [UnexcusedAbsenceHoursCount]                DECIMAL(4,1)     NULL,
    [UnexcusedAbsenceDaysCount]                 INT              NULL,

    CONSTRAINT [PK_StudentsAtRiskOfDroppingOutReportItem] PRIMARY KEY ([SchoolYear], [StudentsAtRiskOfDroppingOutReportId], [StudentsAtRiskOfDroppingOutReportItemId]),
    CONSTRAINT [FK_StudentsAtRiskOfDroppingOutReportItem_StudentsAtRiskOfDroppingOutReport] FOREIGN KEY ([SchoolYear], [StudentsAtRiskOfDroppingOutReportId]) REFERENCES [school_books].[StudentsAtRiskOfDroppingOutReport] ([SchoolYear], [StudentsAtRiskOfDroppingOutReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'StudentsAtRiskOfDroppingOutReportItem', N'Справка ученици с риск от отпадане - елемент.'

exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReportItem', N'SchoolYear'                                , N'Учебна година.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReportItem', N'StudentsAtRiskOfDroppingOutReportId'       , N'Идентификатор на справка ученици с риск от отпадане.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReportItem', N'StudentsAtRiskOfDroppingOutReportItemId'   , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReportItem', N'PersonId'                                  , N'Идентификатор на ученика.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReportItem', N'PersonalId'                                , N'Уникален персонален идентификатор на ученика.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReportItem', N'FirstName'                                 , N'Първо име на ученика.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReportItem', N'MiddleName'                                , N'Презиме на ученика.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReportItem', N'LastName'                                  , N'Фамилия на ученика.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReportItem', N'ClassBookName'                             , N'Име на класа на ученика.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReportItem', N'UnexcusedAbsenceHoursCount'                , N'Неизвинени отсъствия в часове.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReportItem', N'UnexcusedAbsenceDaysCount'                 , N'Неизвинени отсъствия в дни'
