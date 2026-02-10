PRINT 'Create DateAbsencesReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'DateAbsencesReportItem'
GO

CREATE TABLE [school_books].[DateAbsencesReportItem] (
    [SchoolYear]                             SMALLINT         NOT NULL,
    [DateAbsencesReportId]                   INT              NOT NULL,
    [DateAbsencesReportItemId]               INT              NOT NULL,

    [ClassBookId]                            INT              NOT NULL, -- no FK, we dont need a hard reference
    [ClassBookName]                          NVARCHAR(560)    NOT NULL,
    [ShiftId]                                INT              NULL,     -- no FK, we dont need a hard reference
    [ShiftName]                              NVARCHAR(100)    NULL,
    [HourNumber]                             INT              NOT NULL,
    [AbsenceStudentNumbers]                  NVARCHAR(100)    NULL,
    [AbsenceStudentCount]                    INT              NOT NULL,
    [IsOffDay]                               BIT              NOT NULL,
    [HasScheduleDate]                        BIT              NOT NULL,

    CONSTRAINT [PK_DateAbsencesReportItem] PRIMARY KEY ([SchoolYear], [DateAbsencesReportId], [DateAbsencesReportItemId]),
    CONSTRAINT [FK_DateAbsencesReportItem_DateAbsencesReport] FOREIGN KEY ([SchoolYear], [DateAbsencesReportId]) REFERENCES [school_books].[DateAbsencesReport] ([SchoolYear], [DateAbsencesReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'DateAbsencesReportItem', N'Справка отсъстващи за деня - елемент.'

exec school_books.spDescColumn N'DateAbsencesReportItem', N'SchoolYear'                        , N'Учебна година.'
exec school_books.spDescColumn N'DateAbsencesReportItem', N'DateAbsencesReportId'              , N'Идентификатор на справка отсъствия по ученици.'
exec school_books.spDescColumn N'DateAbsencesReportItem', N'DateAbsencesReportItemId'          , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'DateAbsencesReportItem', N'ClassBookId'                       , N'Идентификатор на дневника.'
exec school_books.spDescColumn N'DateAbsencesReportItem', N'ClassBookName'                     , N'Име на дневника.'
exec school_books.spDescColumn N'DateAbsencesReportItem', N'ShiftId'                           , N'Идентификатор на учебна смяна.'
exec school_books.spDescColumn N'DateAbsencesReportItem', N'ShiftName'                         , N'Наименование на учебна смяна.'
exec school_books.spDescColumn N'DateAbsencesReportItem', N'HourNumber'                        , N'Номер на часа.'
exec school_books.spDescColumn N'DateAbsencesReportItem', N'AbsenceStudentNumbers'             , N'Списък от номерата на отсъстващите ученици.'
exec school_books.spDescColumn N'DateAbsencesReportItem', N'AbsenceStudentCount'               , N'Общ брой на отсъстващите ученици.'
exec school_books.spDescColumn N'DateAbsencesReportItem', N'IsOffDay'                          , N'Този ден е неучебен за класа и избраната дата – Да/Не.'
exec school_books.spDescColumn N'DateAbsencesReportItem', N'HasScheduleDate'                   , N'Този клас има въведено разписание за избраната дата – Да/Не.'
