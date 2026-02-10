PRINT 'Create LectureSchedulesReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'LectureSchedulesReportItem'
GO

CREATE TABLE [school_books].[LectureSchedulesReportItem] (
    [SchoolYear]                      SMALLINT         NOT NULL,
    [LectureSchedulesReportId]        INT              NOT NULL,
    [LectureSchedulesReportItemId]    INT              NOT NULL,

    [TeacherPersonId]                 INT              NOT NULL, -- no FK, we dont need a hard reference
    [TeacherPersonName]               NVARCHAR(1000)   NOT NULL,
    [Date]                            DATE             NOT NULL,
    [ClassBookId]                     INT              NOT NULL, -- no FK, we dont need a hard reference
    [ClassBookName]                   NVARCHAR(1000)   NOT NULL,
    [CurriculumId]                    INT              NOT NULL, -- no FK, we dont need a hard reference
    [CurriculumName]                  NVARCHAR(1000)   NOT NULL,
    [LectureScheduleId]               INT              NOT NULL, -- no FK, we dont need a hard reference
    [OrderNumber]                     NVARCHAR(1000)   NOT NULL,
    [OrderDate]                       DATE             NOT NULL,
    [HoursTaken]                      INT              NOT NULL,

    CONSTRAINT [PK_LectureSchedulesReportItem] PRIMARY KEY ([SchoolYear], [LectureSchedulesReportId], [LectureSchedulesReportItemId]),
    CONSTRAINT [FK_LectureSchedulesReportItem_LectureSchedulesReport] FOREIGN KEY ([SchoolYear], [LectureSchedulesReportId]) REFERENCES [school_books].[LectureSchedulesReport] ([SchoolYear], [LectureSchedulesReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'LectureSchedulesReportItem', N'Справка лекторски часове - елемент.'

exec school_books.spDescColumn N'LectureSchedulesReportItem', N'SchoolYear'                       , N'Учебна година.'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'LectureSchedulesReportId'         , N'Идентификатор на справка невписани теми.'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'LectureSchedulesReportItemId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'LectureSchedulesReportItem', N'TeacherPersonId'                  , N'Идентификатор на учителя.'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'TeacherPersonName'                , N'Име на учителя.'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'Date'                             , N'Дата.'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'ClassBookId'                      , N'Идентификатор на дневника.'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'ClassBookName'                    , N'Име на дневника.'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'CurriculumId'                     , N'Идентификатор на предмета.'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'CurriculumName'                   , N'Име на предмета.'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'LectureScheduleId'                , N'Идентификатор на лекторски график'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'OrderNumber'                      , N'Номер на заповед.'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'OrderDate'                        , N'Дата на заповед.'
exec school_books.spDescColumn N'LectureSchedulesReportItem', N'HoursTaken'                       , N'Брой на взетите часове.'
