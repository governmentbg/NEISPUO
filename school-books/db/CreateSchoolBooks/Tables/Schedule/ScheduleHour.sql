PRINT 'Create ScheduleHour table'
GO

CREATE TABLE [school_books].[ScheduleHour] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ScheduleId]            INT              NOT NULL,
    [Day]                   INT              NOT NULL,
    [HourNumber]            INT              NOT NULL,
    [CurriculumId]          INT              NOT NULL,
    [Location]              NVARCHAR(550)    NULL,

    CONSTRAINT [PK_ScheduleHour] PRIMARY KEY ([SchoolYear], [ScheduleId], [Day], [HourNumber], [CurriculumId]),

    CONSTRAINT [FK_ScheduleHour_Schedule] FOREIGN KEY ([SchoolYear], [ScheduleId]) REFERENCES [school_books].[Schedule] ([SchoolYear], [ScheduleId]),

    CONSTRAINT [CHK_ScheduleHour_Day] CHECK ([Day] BETWEEN 1 AND 7),
    CONSTRAINT [CHK_ScheduleHour_HourNumber] CHECK ([HourNumber] >= 0),

    -- external references
    CONSTRAINT [FK_ScheduleHour_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_ScheduleHour_CurriculumId] ([CurriculumId] ASC),
);
GO

exec school_books.spDescTable  N'ScheduleHour', N'Учебно разписание - час.'

exec school_books.spDescColumn N'ScheduleHour', N'SchoolYear'           , N'Учебна година.'
exec school_books.spDescColumn N'ScheduleHour', N'ScheduleId'           , N'Идентификатор на учебно разписание.'
exec school_books.spDescColumn N'ScheduleHour', N'Day'                  , N'Ден от седмицата. Число от 1 (Понеделник) до 7 (Неделя).'
exec school_books.spDescColumn N'ScheduleHour', N'HourNumber'           , N'Номер на часа отговарящ на смяната на разписанието.'
exec school_books.spDescColumn N'ScheduleHour', N'CurriculumId'         , N'Идентификатор на предмет от учебния план.'
exec school_books.spDescColumn N'ScheduleHour', N'Location'             , N'Място на провеждане.'
