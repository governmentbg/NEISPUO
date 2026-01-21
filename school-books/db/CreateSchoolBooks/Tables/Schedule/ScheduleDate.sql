PRINT 'Create ScheduleDate table'
GO

CREATE TABLE [school_books].[ScheduleDate] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ScheduleId]            INT              NOT NULL,
    [Date]                  DATE             NOT NULL,

    [Year]                  INT             NOT NULL,
    [WeekNumber]            INT             NOT NULL,
    [Day]                   INT             NOT NULL,

    CONSTRAINT [PK_ScheduleDate] PRIMARY KEY ([SchoolYear], [ScheduleId], [Date]),

    CONSTRAINT [FK_ScheduleDate_Schedule] FOREIGN KEY ([SchoolYear], [ScheduleId])
        REFERENCES [school_books].[Schedule] ([SchoolYear], [ScheduleId]),

    CONSTRAINT [CHK_ScheduleDate_Day] CHECK ([Day] BETWEEN 1 AND 7),
);
GO

-- ensure there are no two schedules for the same day
CREATE VIEW [school_books].[VW_UQ_ScheduleDate_Date]
WITH SCHEMABINDING
AS
    SELECT
        s.[SchoolYear],
        s.[ClassBookId],
        s.[IsIndividualSchedule],
        s.[PersonId],
        sd.[Date]
    FROM
        [school_books].[Schedule] s
        INNER JOIN [school_books].[ScheduleDate] sd ON
            s.[SchoolYear] = sd.[SchoolYear] AND
            s.[ScheduleId] = sd.[ScheduleId]
    WHERE
        s.[IsSplitting] = 0
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_ScheduleDate_Date]
    ON [school_books].[VW_UQ_ScheduleDate_Date] ([SchoolYear], [ClassBookId], [IsIndividualSchedule], [PersonId], [Date])
GO

exec school_books.spDescTable  N'ScheduleDate', N'Учебно разписание - дата.'

exec school_books.spDescColumn N'ScheduleDate', N'SchoolYear'           , N'Учебна година.'
exec school_books.spDescColumn N'ScheduleDate', N'ScheduleId'           , N'Идентификатор на учебно разписание.'
exec school_books.spDescColumn N'ScheduleDate', N'Date'                 , N'Дата.'

exec school_books.spDescColumn N'ScheduleDate', N'Year'                 , N'Година на седмицата по ISO 8601.'
exec school_books.spDescColumn N'ScheduleDate', N'WeekNumber'           , N'Номер на седмицата по ISO 8601. Число от 1 до 53.'
exec school_books.spDescColumn N'ScheduleDate', N'Day'                  , N'Ден от седмицата. Число от 1 (Понеделник) до 7 (Неделя).'
