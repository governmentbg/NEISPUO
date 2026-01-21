PRINT 'Create RegularGradePointAverageByStudentsReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'RegularGradePointAverageByStudentsReport'
GO

CREATE TABLE [school_books].[RegularGradePointAverageByStudentsReport] (
    [SchoolYear]                                        SMALLINT         NOT NULL,
    [RegularGradePointAverageByStudentsReportId]        INT              NOT NULL,

    [InstId]                                            INT              NOT NULL,
    [Period]                                            INT              NOT NULL,
    [ClassBookNames]                                    NVARCHAR(MAX)    NOT NULL,

    [CreateDate]                                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                                INT              NOT NULL,
    [ModifyDate]                                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                               INT              NOT NULL,
    [Version]                                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_RegularGradePointAverageByStudentsReport] PRIMARY KEY ([SchoolYear], [RegularGradePointAverageByStudentsReportId]),

    CONSTRAINT [CHK_RegularGradePointAverageByStudentsReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_RegularGradePointAverageByStudentsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_RegularGradePointAverageByStudentsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_RegularGradePointAverageByStudentsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_RegularGradePointAverageByStudentsReport] UNIQUE ([SchoolYear], [InstId], [RegularGradePointAverageByStudentsReportId]),
);
GO

exec school_books.spDescTable  N'RegularGradePointAverageByStudentsReport', N'Справка среден успех от текущи оценки по ученици.'

exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReport', N'SchoolYear'                                   , N'Учебна година.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReport', N'RegularGradePointAverageByStudentsReportId'   , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReport', N'InstId'                                       , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReport', N'Period'                                       , N'Филтър за период. 1 - първи срок, 2 - втори срок 3 - цялата година.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReport', N'ClassBookNames'                               , N'Филтър за дневници.'

exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReport', N'CreateDate'                                   , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReport', N'CreatedBySysUserId'                           , N'Създадено от.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReport', N'ModifyDate'                                   , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReport', N'ModifiedBySysUserId'                          , N'Последна модификация от.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReport', N'Version'                                      , N'Версия.'
