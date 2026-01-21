PRINT 'Create FinalGradePointAverageByStudentsReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'FinalGradePointAverageByStudentsReport'
GO

CREATE TABLE [school_books].[FinalGradePointAverageByStudentsReport] (
    [SchoolYear]                                        SMALLINT         NOT NULL,
    [FinalGradePointAverageByStudentsReportId]          INT              NOT NULL,

    [InstId]                                            INT              NOT NULL,
    [Period]                                            INT              NOT NULL,
    [ClassBookNames]                                    NVARCHAR(MAX)    NOT NULL,
    [MinimumGradePointAverage]                          DECIMAL(3,2)     NULL,

    [CreateDate]                                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                                INT              NOT NULL,
    [ModifyDate]                                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                               INT              NOT NULL,
    [Version]                                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_FinalGradePointAverageByStudentsReport] PRIMARY KEY ([SchoolYear], [FinalGradePointAverageByStudentsReportId]),

    CONSTRAINT [CHK_FinalGradePointAverageByStudentsReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_FinalGradePointAverageByStudentsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_FinalGradePointAverageByStudentsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_FinalGradePointAverageByStudentsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_FinalGradePointAverageByStudentsReport] UNIQUE ([SchoolYear], [InstId], [FinalGradePointAverageByStudentsReportId]),
);
GO

exec school_books.spDescTable  N'FinalGradePointAverageByStudentsReport', N'Справка среден успех от срочни/годишни оценки по ученици.'

exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReport', N'SchoolYear'                                   , N'Учебна година.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReport', N'FinalGradePointAverageByStudentsReportId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReport', N'InstId'                                       , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReport', N'Period'                                       , N'Филтър за период. 1 - първи срок, 2 - втори срок 3 - цялата година.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReport', N'ClassBookNames'                               , N'Филтър за дневници.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReport', N'MinimumGradePointAverage'                     , N'Филтър за минимална средна оценка.'

exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReport', N'CreateDate'                                   , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReport', N'CreatedBySysUserId'                           , N'Създадено от.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReport', N'ModifyDate'                                   , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReport', N'ModifiedBySysUserId'                          , N'Последна модификация от.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReport', N'Version'                                      , N'Версия.'
