PRINT 'Create ShiftHour table'
GO

CREATE TABLE [school_books].[ShiftHour] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ShiftId]               INT              NOT NULL,
    [Day]                   INT              NOT NULL,
    [HourNumber]            INT              NOT NULL,

    [StartTime]             TIME             NOT NULL,
    [EndTime]               TIME             NOT NULL,

    CONSTRAINT [PK_ShiftHour] PRIMARY KEY ([SchoolYear], [ShiftId], [Day], [HourNumber]),

    CONSTRAINT [FK_ShiftHour_Shift] FOREIGN KEY ([SchoolYear], [ShiftId]) REFERENCES [school_books].[Shift] ([SchoolYear], [ShiftId]),

    CONSTRAINT [CHK_ShiftHour_Day] CHECK ([Day] BETWEEN 1 AND 7),
    CONSTRAINT [CHK_ShiftHour_HourNumber] CHECK ([HourNumber] >= 0),
);
GO

exec school_books.spDescTable  N'ShiftHour', N'Учебна смяна - час.'

exec school_books.spDescColumn N'ShiftHour', N'SchoolYear'           , N'Учебна година.'
exec school_books.spDescColumn N'ShiftHour', N'ShiftId'              , N'Идентификатор на учебна смяна.'
exec school_books.spDescColumn N'ShiftHour', N'Day'                  , N'Ден за който е часа. Число от 1 (Понеделник) до 7 (Неделя).'
exec school_books.spDescColumn N'ShiftHour', N'HourNumber'           , N'Номер на часа. Число >= 0.'

exec school_books.spDescColumn N'ShiftHour', N'StartTime'            , N'Начален час.'
exec school_books.spDescColumn N'ShiftHour', N'EndTime'              , N'Краен час.'
