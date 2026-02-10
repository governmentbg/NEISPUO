PRINT 'Create Grade table'
GO

EXEC [school_books].[spCreateIdSequence] N'Grade'
GO

CREATE TABLE [school_books].[Grade] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [GradeId]               INT              NOT NULL,

    -- discriminator
    [Category]              INT              NOT NULL,

    -- base
    [ClassBookId]           INT              NOT NULL,
    [PersonId]              INT              NOT NULL,
    [CurriculumId]          INT              NOT NULL,
    [Date]                  DATE             NOT NULL,
    [Type]                  INT              NOT NULL,
    [Term]                  INT              NOT NULL,
    [ScheduleLessonId]      INT              NULL,
    [TeacherAbsenceId]      INT              NULL,
    [Comment]               NVARCHAR(1000)   NULL,
    [IsReadFromParent]      BIT              NOT NULL,

    -- decimal
    [DecimalGrade]          DECIMAL(3,2)     NULL,

    -- special needs
    [SpecialGrade]          INT              NULL,

    -- qualitative
    [QualitativeGrade]      INT              NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Grade] PRIMARY KEY NONCLUSTERED ([SchoolYear], [GradeId]),
    CONSTRAINT [UK_Grade] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [GradeId]),

    CONSTRAINT [FK_Grade_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [FK_Grade_ScheduleLesson] FOREIGN KEY ([SchoolYear], [ScheduleLessonId])
        REFERENCES [school_books].[ScheduleLesson] ([SchoolYear], [ScheduleLessonId]),
    CONSTRAINT [FK_Grade_TeacherAbsenceHour] FOREIGN KEY ([SchoolYear], [TeacherAbsenceId], [ScheduleLessonId])
        REFERENCES [school_books].[TeacherAbsenceHour] ([SchoolYear], [TeacherAbsenceId], [ScheduleLessonId]),

    CONSTRAINT [CHK_Grade_Category] CHECK ([Category] IN (1, 2, 3)),
    CONSTRAINT [CHK_Grade_Type] CHECK ([Type] IN (1, 2, 3, 4, 5, 6, 11, 12, 21, 22, 98, 99, 100, 101)),
    CONSTRAINT [CHK_Grade_Term] CHECK ([Term] IN (1, 2)),
    CONSTRAINT [CHK_Grade_SpecialGrade] CHECK ([SpecialGrade] IN (1, 2, 3)),
    CONSTRAINT [CHK_Grade_QualitativeGrade] CHECK ([QualitativeGrade] IN (2, 3, 4, 5, 6)),

    -- DecimalGrade can be 2 or between 3 and 6
    CONSTRAINT [CHK_Grade_DecimalGrade] CHECK ([DecimalGrade] = 2 OR [DecimalGrade] BETWEEN 3 AND 6),

    -- ScheduleLessonId is mandatory unless the grade is Term/Final/OtherClass/OtherSchool when it must be null
    CONSTRAINT [CHK_Grade_ScheduleLessonId] CHECK (
        ([Type] NOT IN (21, 22, 98, 99, 100, 101) AND [ScheduleLessonId] IS NOT NULL) OR
        ([Type] IN     (21, 22, 98, 99, 100, 101) AND [ScheduleLessonId] IS NULL)
    ),

    INDEX [IX_Grade_ScheduleLesson] ([SchoolYear], [ScheduleLessonId], [TeacherAbsenceId]),
    INDEX [IX_Grade_PersonId] ([PersonId], [Type]), -- Kontrax index

    -- external references
    CONSTRAINT [FK_Grade_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_Grade_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]),
    CONSTRAINT [FK_Grade_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Grade_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    -- Curriculum deletetion helper index
    INDEX [IX_Grade_CurriculumId] ([CurriculumId] ASC),
);
GO

exec school_books.spDescTable  N'Grade', N'Оценка.'

exec school_books.spDescColumn N'Grade', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Grade', N'GradeId'                   , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Grade', N'Category'                  , N'Категория на оценката. 1 - Количествена, 2 - СОП, 3 - Качествена'

exec school_books.spDescColumn N'Grade', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'Grade', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'Grade', N'CurriculumId'              , N'Идентификатор на предмет от учебния план.'
exec school_books.spDescColumn N'Grade', N'Date'                      , N'Дата на оценката.'
exec school_books.spDescColumn N'Grade', N'Type'                      , N'Вид на оценката.'
exec school_books.spDescColumn N'Grade', N'Term'                      , N'Срок. 1 - Първи срок, 2 - Втори срок.'
exec school_books.spDescColumn N'Grade', N'ScheduleLessonId'          , N'Идентификатор на часа от учебното разписание.'
exec school_books.spDescColumn N'Grade', N'TeacherAbsenceId'          , N'Идентификатор на заместване.'
exec school_books.spDescColumn N'Grade', N'Comment'                   , N'Коментар за оценката.'
exec school_books.spDescColumn N'Grade', N'IsReadFromParent'          , N'Прочетено от родител – да/не.'

exec school_books.spDescColumn N'Grade', N'DecimalGrade'              , N'Количествена оценка с точно два знака след десетичната запетая. Задължително за категория на оценката "Количествена".'
exec school_books.spDescColumn N'Grade', N'SpecialGrade'              , N'СОП оценка. Задължително за категория на оценката "СОП".'
exec school_books.spDescColumn N'Grade', N'QualitativeGrade'          , N'Качествена оценка. Задължително за категория на оценката "Качествена".'

exec school_books.spDescColumn N'Grade', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Grade', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Grade', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Grade', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Grade', N'Version'                   , N'Версия.'
