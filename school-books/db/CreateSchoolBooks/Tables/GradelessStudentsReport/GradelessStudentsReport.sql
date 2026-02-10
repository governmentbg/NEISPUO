PRINT 'Create GradelessStudentsReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'GradelessStudentsReport'
GO

CREATE TABLE [school_books].[GradelessStudentsReport] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [GradelessStudentsReportId]         INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [OnlyFinalGrades]                   BIT              NOT NULL,
    [Period]                            INT              NOT NULL,
    [ClassBookNames]                    NVARCHAR(MAX)    NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_GradelessStudentsReport] PRIMARY KEY ([SchoolYear], [GradelessStudentsReportId]),

    CONSTRAINT [CHK_GradelessStudentsReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_GradelessStudentsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_GradelessStudentsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_GradelessStudentsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_GradelessStudentsReport] UNIQUE ([SchoolYear], [InstId], [GradelessStudentsReportId]),
);
GO

exec school_books.spDescTable  N'GradelessStudentsReport', N'Справка ученици без оценки.'

exec school_books.spDescColumn N'GradelessStudentsReport', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'GradelessStudentsReport', N'GradelessStudentsReportId' , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'GradelessStudentsReport', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'GradelessStudentsReport', N'OnlyFinalGrades'           , N'Справката е само за срочни/годишни оценки – да/не.'
exec school_books.spDescColumn N'GradelessStudentsReport', N'Period'                    , N'Филтър за период. 1 - първи срок, 2 - втори срок 3 - цялата година.'
exec school_books.spDescColumn N'GradelessStudentsReport', N'ClassBookNames'            , N'Филтър за дневници.'

exec school_books.spDescColumn N'GradelessStudentsReport', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'GradelessStudentsReport', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'GradelessStudentsReport', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'GradelessStudentsReport', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'GradelessStudentsReport', N'Version'                   , N'Версия.'
