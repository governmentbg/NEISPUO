PRINT 'Create LectureSchedulesReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'LectureSchedulesReport'
GO

CREATE TABLE [school_books].[LectureSchedulesReport] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [LectureSchedulesReportId]  INT              NOT NULL,

    [InstId]                    INT              NOT NULL,
    [Period]                    INT              NOT NULL,
    [Year]                      INT              NULL,
    [Month]                     INT              NULL,
    [TeacherPersonId]           INT              NULL, -- no FK, we dont need a hard reference
    [TeacherPersonName]         NVARCHAR(1000)   NULL,

    [CreateDate]                DATETIME2        NOT NULL,
    [CreatedBySysUserId]        INT              NOT NULL,
    [ModifyDate]                DATETIME2        NOT NULL,
    [ModifiedBySysUserId]       INT              NOT NULL,
    [Version]                   ROWVERSION       NOT NULL,

    CONSTRAINT [PK_LectureSchedulesReport] PRIMARY KEY ([SchoolYear], [LectureSchedulesReportId]),
    CONSTRAINT [CHK_LectureSchedulesReport_Period] CHECK ([Period] IN (1, 2, 3, 4)),

    -- external references
    CONSTRAINT [FK_LectureSchedulesReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_LectureSchedulesReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_LectureSchedulesReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_LectureSchedulesReport] UNIQUE ([SchoolYear], [InstId], [LectureSchedulesReportId]),
);
GO

exec school_books.spDescTable  N'LectureSchedulesReport', N'Справка лекторски часове.'

exec school_books.spDescColumn N'LectureSchedulesReport', N'SchoolYear'                   , N'Учебна година.'
exec school_books.spDescColumn N'LectureSchedulesReport', N'LectureSchedulesReportId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'LectureSchedulesReport', N'InstId'                       , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'LectureSchedulesReport', N'Period'                       , N'Период. 1 - за месец, 2 - За първи срок, 3 - за втори срок, 4 - за Цялата година.'
exec school_books.spDescColumn N'LectureSchedulesReport', N'Year'                         , N'Филтър за година.'
exec school_books.spDescColumn N'LectureSchedulesReport', N'Month'                        , N'Филтър за месец.'
exec school_books.spDescColumn N'LectureSchedulesReport', N'TeacherPersonId'              , N'Филтър за учител (идентификатор).'
exec school_books.spDescColumn N'LectureSchedulesReport', N'TeacherPersonName'            , N'Филтър за учител (име).'

exec school_books.spDescColumn N'LectureSchedulesReport', N'CreateDate'                   , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'LectureSchedulesReport', N'CreatedBySysUserId'           , N'Създадено от.'
exec school_books.spDescColumn N'LectureSchedulesReport', N'ModifyDate'                   , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'LectureSchedulesReport', N'ModifiedBySysUserId'          , N'Последна модификация от.'
exec school_books.spDescColumn N'LectureSchedulesReport', N'Version'                      , N'Версия.'
