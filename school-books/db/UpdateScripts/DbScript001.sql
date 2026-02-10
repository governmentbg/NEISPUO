PRINT 'Create AbsenceReason table'
GO

CREATE TABLE [school_books].[AbsenceReason] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_AbsenceReason] PRIMARY KEY ([Id])
);
GO

PRINT 'Insert AbsenceReason'
GO

SET IDENTITY_INSERT [school_books].[AbsenceReason] ON;

INSERT INTO [school_books].[AbsenceReason] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'Здравословни', 'Здравословни', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'Семейни', 'Семейни', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'Други', 'Други', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[AbsenceReason] OFF;

GO

PRINT 'Create SanctionType table'
GO

CREATE TABLE [school_books].[SanctionType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_SanctionType] PRIMARY KEY ([Id])
);
GO

PRINT 'Insert SanctionType'
GO

SET IDENTITY_INSERT [school_books].[SanctionType] ON;

INSERT INTO [school_books].[SanctionType] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'чл.199 ал.1 т.1 Забележка', 'чл.199 ал.1 т.1 Забележка', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'чл.199 ал.1 т.2 Преместване в друга паралелка в същото училище', 'чл.199 ал.1 т.2 Преместване в друга паралелка в същото училище', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'чл.199 ал.1 т.3 Предупреждение за преместване в друго училище', 'чл.199 ал.1 т.3 Предупреждение за преместване в друго училище', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'чл.199 ал.1 т.4 Преместване в друго училище', 'чл.199 ал.1 т.4 Преместване в друго училище', 1, '2021-01-01 00:00:00.000', NULL),
(5, 5, 'чл.199 ал.1 т.5 Преместване от дневна в самостоятелна форма на обучение', 'чл.199 ал.1 т.5 Преместване от дневна в самостоятелна форма на обучение', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[SanctionType] OFF;

GO

PRINT 'Create SupportActivityType table'
GO

CREATE TABLE [school_books].[SupportActivityType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_SupportActivityType] PRIMARY KEY ([Id])
);
GO

PRINT 'Insert SupportActivityType'
GO

SET IDENTITY_INSERT [school_books].[SupportActivityType] ON;

INSERT INTO [school_books].[SupportActivityType] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'Допълнително обучение по учебен предмет', 'Допълнително обучение по учебен предмет', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'Допълнително обучение по БЕЛ', 'Допълнително обучение по БЕЛ', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'Консултации', 'Консултации', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'Екипна работа между учители', 'Екипна работа между учители', 1, '2021-01-01 00:00:00.000', NULL),
(5, 5, 'Кариерно ориентиране', 'Кариерно ориентиране', 1, '2021-01-01 00:00:00.000', NULL),
(6, 6, 'Занимания по интереси', 'Занимания по интереси', 1,'2021-01-01 00:00:00.000', NULL),
(7, 7, 'Библиотечно-информационно обслужване', 'Библиотечно-информационно обслужване', 1, '2021-01-01 00:00:00.000', NULL),
(8, 8, 'Грижа за здравето', 'Грижа за здравето', 1, '2021-01-01 00:00:00.000', NULL),
(9, 9, 'Осигуряване на общежитие', 'Осигуряване на общежитие', 1, '2021-01-01 00:00:00.000', NULL),
(10, 10, 'Поощряване с морални и материални награди', 'Поощряване с морални и материални награди', 1, '2021-01-01 00:00:00.000', NULL),
(11, 11, 'Превенция на проблемно поведение', 'Превенция на проблемно поведение', 1,'2021-01-01 00:00:00.000', NULL),
(12, 12, 'Превенция на обучителни затруднения', 'Превенция на обучителни затруднения', 1, '2021-01-01 00:00:00.000', NULL),
(13, 13, 'Логопедична работа', 'Логопедична работа', 1, '2021-01-01 00:00:00.000', NULL),
(14, 14, 'Работа по конкретен случай', 'Работа по конкретен случай', 1, '2021-01-01 00:00:00.000', NULL),
(15, 15, 'Психо-социална рехабилитация', 'Психо-социална рехабилитация', 1, '2021-01-01 00:00:00.000', NULL),
(16, 16, 'Осигуряване на достъпна среда', 'Осигуряване на достъпна среда', 1, '2021-01-01 00:00:00.000', NULL),
(17, 17, 'Обучение за ученици със сензорни увреждания', 'Обучение за ученици със сензорни увреждания', 1, '2021-01-01 00:00:00.000', NULL),
(18, 18, 'Ресурсно подпомагане (по предмет)', 'Ресурсно подпомагане (по предмет)', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[SupportActivityType] OFF;

GO

PRINT 'Create SupportDifficultyType table'
GO

CREATE TABLE [school_books].[SupportDifficultyType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_SupportDifficultyType] PRIMARY KEY ([Id])
);
GO

PRINT 'Insert SupportDifficultyType'
GO

SET IDENTITY_INSERT [school_books].[SupportDifficultyType] ON;

INSERT INTO [school_books].[SupportDifficultyType] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'Затруднения в обучението', 'Затруднения в обучението', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'Рискови фактори в средата', 'Рискови фактори в средата', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'Проблемно поведение', 'Проблемно поведение', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'Хронично заболяване', 'Хронично заболяване', 1, '2021-01-01 00:00:00.000', NULL),
(5, 5, 'Напредва по-бързо от останалите ученици', 'Напредва по-бързо от останалите ученици', 1, '2021-01-01 00:00:00.000', NULL),
(6, 6, 'Друго', 'Друго', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[SupportDifficultyType] OFF;

GO

PRINT 'Create ProtocolExamType table'
GO

CREATE TABLE [school_books].[ProtocolExamType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_ProtocolExamType] PRIMARY KEY ([Id])
);
GO

PRINT 'Insert ProtocolExamType'
GO

SET IDENTITY_INSERT [school_books].[ProtocolExamType] ON;

INSERT INTO [school_books].[ProtocolExamType] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'писмен изпит', 'писмен изпит', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'устен изпит', 'устен изпит', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'практически изпит', 'практически изпит', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'изпит за определяне на срочна оценка', 'изпит за определяне на срочна оценка', 1, '2021-01-01 00:00:00.000', NULL),
(5, 5, 'изпит за определяне на крайна оценка', 'изпит за определяне на крайна оценка', 1, '2021-01-01 00:00:00.000', NULL),
(6, 6, 'изпит за определяне на годишна оценка', 'изпит за определяне на годишна оценка', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[ProtocolExamType] OFF;

GO

PRINT 'Create ProtocolExamSubType table'
GO

CREATE TABLE [school_books].[ProtocolExamSubType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_ProtocolExamSubType] PRIMARY KEY ([Id])
);
GO

PRINT 'Insert ProtocolExamSubType'
GO

SET IDENTITY_INSERT [school_books].[ProtocolExamSubType] ON;

INSERT INTO [school_books].[ProtocolExamSubType] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'поправителен изпит', 'поправителен изпит', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'приравнителен изпит', 'приравнителен изпит', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'изпит за промяна на годишна оценка', 'изпит за промяна на годишна оценка', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'държавен изпит за придобиване на степен на професионална квалификация', 'държавен изпит за придобиване на степен на професионална квалификация', 1, '2021-01-01 00:00:00.000', NULL),
(5, 5, 'определяне на срочна оценка по учебен предмет', 'определяне на срочна оценка по учебен предмет', 1, '2021-01-01 00:00:00.000', NULL),
(6, 6, 'изпит за промяна на окончателна оценка', 'изпит за промяна на окончателна оценка', 1,'2021-01-01 00:00:00.000', NULL),
(7, 7, 'определяне на годишна оценка по учебен предмет', 'определяне на годишна оценка по учебен предмет', 1, '2021-01-01 00:00:00.000', NULL),
(8, 8, 'изпит за установяване степента на постигане на компетентности по учебен предмет за определен клас', 'изпит за установяване степента на постигане на компетентности по учебен предмет за определен клас', 1, '2021-01-01 00:00:00.000', NULL),
(9, 9, 'изпит за установяване степента на постигане на компетентности за определен етап от степента на образование', 'изпит за установяване степента на постигане на компетентности за определен етап от степента на образование', 1, '2021-01-01 00:00:00.000', NULL),
(10, 10, 'изпит за придобиване на професионална квалификация по част от професия', 'изпит за придобиване на професионална квалификация по част от професия', 1, '2021-01-01 00:00:00.000', NULL),
(11, 11, 'държавен изпит за придобиване на степен на ПК - част по теория на професията', 'държавен изпит за придобиване на степен на ПК - част по теория на професията', 1,'2021-01-01 00:00:00.000', NULL),
(12, 12, 'държавен изпит за придобиване на степен на ПК - част по практика на професията', 'държавен изпит за придобиване на степен на ПК - част по практика на професията', 1, '2021-01-01 00:00:00.000', NULL),
(13, 13, 'изпит за придобиване на квалификация по част от професията - част по теория', 'изпит за придобиване на квалификация по част от професията - част по теория', 1, '2021-01-01 00:00:00.000', NULL),
(14, 14, 'изпит за придобиване на квалификация по част от професията - част по практика', 'изпит за придобиване на квалификация по част от професията - част по практика', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[ProtocolExamSubType] OFF;

GO

PRINT 'Create QualificationDegree table'
GO

CREATE TABLE [school_books].[QualificationDegree] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_QualificationDegree] PRIMARY KEY ([Id])
);
GO

PRINT 'Insert QualificationDegree'
GO

SET IDENTITY_INSERT [school_books].[QualificationDegree] ON;

INSERT INTO [school_books].[QualificationDegree] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'първа', 'първа', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'втора', 'втора', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'трета', 'трета', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'четвърта', 'четвърта', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[QualificationDegree] OFF;

GO

PRINT 'Create QualificationExamType table'
GO

CREATE TABLE [school_books].[QualificationExamType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_QualificationExamType] PRIMARY KEY ([Id])
);
GO

PRINT 'Insert QualificationExamType'
GO

SET IDENTITY_INSERT [school_books].[QualificationExamType] ON;

INSERT INTO [school_books].[QualificationExamType] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'държавен изпит за придобиване на степен на ПК - част по теория на професията', 'държавен изпит за придобиване на степен на ПК - част по теория на професията', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'държавен изпит за придобиване на степен на ПК - част по практика на професията', 'държавен изпит за придобиване на степен на ПК - част по практика на професията', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'изпит за придобиване на квалификация по част от професията - част по теория', 'изпит за придобиване на квалификация по част от професията - част по теория', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'изпит за придобиване на квалификация по част от професията - част по практика', 'изпит за придобиване на квалификация по част от професията - част по практика', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[QualificationExamType] OFF;

GO

ALTER TABLE [school_books].[Absence]
DROP
    CONSTRAINT [CHK_Absence_ExcusedReason],
    CONSTRAINT [CHK_Absences_ExcusedReason_ExcusedReasonComment];
GO

EXEC sp_rename 'school_books.Absence.ExcusedReason', 'ExcusedReasonId', 'COLUMN';
GO

ALTER TABLE [school_books].[Absence] ADD CONSTRAINT [CHK_Absences_ExcusedReason_ExcusedReasonComment] CHECK ([Type] = 3 OR ([ExcusedReasonId] IS NULL AND [ExcusedReasonComment] IS NULL))
GO

ALTER TABLE [school_books].[Absence] ADD CONSTRAINT [FK_Absence_ExcusedReason]
    FOREIGN KEY ([ExcusedReasonId])
    REFERENCES [school_books].[AbsenceReason] ([Id])
GO

ALTER TABLE [school_books].[Attendance]
DROP
    CONSTRAINT [CHK_Attendance_ExcusedReason],
    CONSTRAINT [CHK_Attendance_ExcusedReason_ExcusedReasonComment];
GO

EXEC sp_rename 'school_books.Attendance.ExcusedReason', 'ExcusedReasonId', 'COLUMN';
GO

ALTER TABLE [school_books].[Attendance] ADD CONSTRAINT [CHK_Attendance_ExcusedReason_ExcusedReasonComment] CHECK ([Type] = 3 OR ([ExcusedReasonId] IS NULL AND [ExcusedReasonComment] IS NULL))
GO

ALTER TABLE [school_books].[Attendance] ADD CONSTRAINT [FK_Attendance_ExcusedReason]
    FOREIGN KEY ([ExcusedReasonId]) REFERENCES [school_books].[AbsenceReason] ([Id])
GO

ALTER TABLE [school_books].[ExamDutyProtocol]
DROP
    CONSTRAINT [CHK_ExamDutyProtocol_ExamType],
    CONSTRAINT [CHK_ExamDutyProtocol_ExamSubType];
GO

EXEC sp_rename 'school_books.ExamDutyProtocol.ExamType', 'ProtocolExamTypeId', 'COLUMN';
EXEC sp_rename 'school_books.ExamDutyProtocol.ExamSubType', 'ProtocolExamSubTypeId', 'COLUMN';
GO

ALTER TABLE [school_books].[ExamDutyProtocol] ADD CONSTRAINT [FK_ExamDutyProtocol_ProtocolExamType]
    FOREIGN KEY ([ProtocolExamTypeId]) REFERENCES [school_books].[ProtocolExamType] ([Id])
GO

ALTER TABLE [school_books].[ExamDutyProtocol] ADD CONSTRAINT [FK_ExamDutyProtocol_ProtocolExamSubType]
    FOREIGN KEY ([ProtocolExamSubTypeId]) REFERENCES [school_books].[ProtocolExamSubType] ([Id])
GO

ALTER TABLE [school_books].[ExamResultProtocol]
DROP
    CONSTRAINT [CHK_ExamResultProtocol_ExamType],
    CONSTRAINT [CHK_ExamResultProtocol_ProtocolType];
GO

EXEC sp_rename 'school_books.ExamResultProtocol.ExamType', 'ProtocolExamTypeId', 'COLUMN';
EXEC sp_rename 'school_books.ExamResultProtocol.ProtocolType', 'ProtocolExamSubTypeId', 'COLUMN';
GO

ALTER TABLE [school_books].[ExamResultProtocol] ADD CONSTRAINT [FK_ExamResultProtocol_ProtocolExamType]
    FOREIGN KEY ([ProtocolExamTypeId]) REFERENCES [school_books].[ProtocolExamType] ([Id])
GO

ALTER TABLE [school_books].[ExamResultProtocol] ADD CONSTRAINT [FK_ExamResultProtocol_ProtocolExamSubType]
    FOREIGN KEY ([ProtocolExamSubTypeId]) REFERENCES [school_books].[ProtocolExamSubType] ([Id])
GO

ALTER TABLE [school_books].[QualificationExamResultProtocol]
DROP
    CONSTRAINT [CHK_QualificationExamResultProtocol_ExamType],
    CONSTRAINT [CHK_QualificationExamResultProtocol_QualificationDegree];
GO

EXEC sp_rename 'school_books.QualificationExamResultProtocol.ExamType', 'QualificationExamTypeId', 'COLUMN';
EXEC sp_rename 'school_books.QualificationExamResultProtocol.QualificationDegree', 'QualificationDegreeId', 'COLUMN';
GO

ALTER TABLE [school_books].[QualificationExamResultProtocol] ADD CONSTRAINT [FK_QualificationExamResultProtocol_QualificationDegree]
    FOREIGN KEY ([QualificationDegreeId]) REFERENCES [school_books].[QualificationDegree] ([Id])
GO

ALTER TABLE [school_books].[QualificationExamResultProtocol] ADD CONSTRAINT [FK_QualificationExamResultProtocol_QualificationExamType]
    FOREIGN KEY ([QualificationExamTypeId]) REFERENCES [school_books].[QualificationExamType] ([Id])
GO

ALTER TABLE [school_books].[Sanction] DROP CONSTRAINT [CHK_Sanction_Type];
GO

EXEC sp_rename 'school_books.Sanction.Type', 'SanctionTypeId', 'COLUMN';
GO

ALTER TABLE [school_books].[Sanction] ADD CONSTRAINT [FK_Sanction_SanctionType]
    FOREIGN KEY ([SanctionTypeId]) REFERENCES [school_books].[SanctionType] ([Id])
GO

ALTER TABLE [school_books].[SupportActivity] DROP CONSTRAINT [CHK_SupportDifficulty_ActivityType];
GO

EXEC sp_rename 'school_books.SupportActivity.ActivityType', 'SupportActivityTypeId', 'COLUMN';
GO

ALTER TABLE [school_books].[SupportActivity] ADD CONSTRAINT [FK_SupportActivity_SupportActivityType]
    FOREIGN KEY ([SupportActivityTypeId]) REFERENCES [school_books].[SupportActivityType] ([Id])
GO

ALTER TABLE [school_books].[SupportDifficulty]
DROP
    CONSTRAINT [PK_SupportDifficulty],
    CONSTRAINT [CHK_SupportDifficulty_DifficultyType];
GO

EXEC sp_rename 'school_books.SupportDifficulty.DifficultyType', 'SupportDifficultyTypeId', 'COLUMN';
GO

ALTER TABLE [school_books].[SupportDifficulty] ADD CONSTRAINT [PK_SupportDifficulty]
    PRIMARY KEY ([SchoolYear], [SupportId], [SupportDifficultyTypeId])
GO

ALTER TABLE [school_books].[SupportDifficulty] ADD CONSTRAINT [FK_SupportDifficulty_SupportDifficultyType]
    FOREIGN KEY ([SupportDifficultyTypeId]) REFERENCES [school_books].[SupportDifficultyType] ([Id])
GO
