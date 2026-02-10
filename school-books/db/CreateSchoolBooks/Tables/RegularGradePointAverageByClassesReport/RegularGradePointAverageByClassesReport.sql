PRINT 'Create RegularGradePointAverageByClassesReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'RegularGradePointAverageByClassesReport'
GO

CREATE TABLE [school_books].[RegularGradePointAverageByClassesReport] (
    [SchoolYear]                                        SMALLINT         NOT NULL,
    [RegularGradePointAverageByClassesReportId]         INT              NOT NULL,

    [InstId]                                            INT              NOT NULL,
    [Period]                                            INT              NOT NULL,
    [ClassBookNames]                                    NVARCHAR(MAX)    NOT NULL,

    [CreateDate]                                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                                INT              NOT NULL,
    [ModifyDate]                                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                               INT              NOT NULL,
    [Version]                                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_RegularGradePointAverageByClassesReport] PRIMARY KEY ([SchoolYear], [RegularGradePointAverageByClassesReportId]),

    CONSTRAINT [CHK_RegularGradePointAverageByClassesReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_RegularGradePointAverageByClassesReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_RegularGradePointAverageByClassesReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_RegularGradePointAverageByClassesReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_RegularGradePointAverageByClassesReport] UNIQUE ([SchoolYear], [InstId], [RegularGradePointAverageByClassesReportId]),
);
GO

exec school_books.spDescTable  N'RegularGradePointAverageByClassesReport', N'Справка среден успех от текущи оценки по класове.'

exec school_books.spDescColumn N'RegularGradePointAverageByClassesReport', N'SchoolYear'                                   , N'Учебна година.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReport', N'RegularGradePointAverageByClassesReportId'    , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'RegularGradePointAverageByClassesReport', N'InstId'                                       , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReport', N'Period'                                       , N'Филтър за период. 1 - първи срок, 2 - втори срок 3 - цялата година.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReport', N'ClassBookNames'                               , N'Филтър за дневници.'

exec school_books.spDescColumn N'RegularGradePointAverageByClassesReport', N'CreateDate'                                   , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReport', N'CreatedBySysUserId'                           , N'Създадено от.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReport', N'ModifyDate'                                   , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReport', N'ModifiedBySysUserId'                          , N'Последна модификация от.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReport', N'Version'                                      , N'Версия.'
