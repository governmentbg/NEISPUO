PRINT 'Create FinalGradePointAverageByClassesReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'FinalGradePointAverageByClassesReport'
GO

CREATE TABLE [school_books].[FinalGradePointAverageByClassesReport] (
    [SchoolYear]                                        SMALLINT         NOT NULL,
    [FinalGradePointAverageByClassesReportId]           INT              NOT NULL,

    [InstId]                                            INT              NOT NULL,
    [Period]                                            INT              NOT NULL,
    [ClassBookNames]                                    NVARCHAR(MAX)    NOT NULL,

    [CreateDate]                                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                                INT              NOT NULL,
    [ModifyDate]                                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                               INT              NOT NULL,
    [Version]                                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_FinalGradePointAverageByClassesReport] PRIMARY KEY ([SchoolYear], [FinalGradePointAverageByClassesReportId]),

    CONSTRAINT [CHK_FinalGradePointAverageByClassesReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_FinalGradePointAverageByClassesReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_FinalGradePointAverageByClassesReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_FinalGradePointAverageByClassesReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_FinalGradePointAverageByClassesReport] UNIQUE ([SchoolYear], [InstId], [FinalGradePointAverageByClassesReportId]),
);
GO

exec school_books.spDescTable  N'FinalGradePointAverageByClassesReport', N'Справка среден успех от срочни/годишни оценки по класове.'

exec school_books.spDescColumn N'FinalGradePointAverageByClassesReport', N'SchoolYear'                                   , N'Учебна година.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReport', N'FinalGradePointAverageByClassesReportId'      , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'FinalGradePointAverageByClassesReport', N'InstId'                                       , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReport', N'Period'                                       , N'Филтър за период. 1 - първи срок, 2 - втори срок 3 - цялата година.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReport', N'ClassBookNames'                               , N'Филтър за дневници.'

exec school_books.spDescColumn N'FinalGradePointAverageByClassesReport', N'CreateDate'                                   , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReport', N'CreatedBySysUserId'                           , N'Създадено от.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReport', N'ModifyDate'                                   , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReport', N'ModifiedBySysUserId'                          , N'Последна модификация от.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReport', N'Version'                                      , N'Версия.'
