PRINT 'Create vwStudentClassBooks'
GO

DROP INDEX IF EXISTS [UQ_vwNonCombinedStudentClassBooks] ON [school_books].[vwNonCombinedStudentClassBooks]
GO

CREATE OR ALTER VIEW [school_books].[vwNonCombinedStudentClassBooks]
WITH SCHEMABINDING
AS
    SELECT
        sc.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId,
        COUNT_BIG(*) AS COUNT
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.SchoolYear = cg.SchoolYear AND cb.ClassId = cg.ParentClassId
        INNER JOIN [student].[StudentClass] sc ON cg.SchoolYear = sc.SchoolYear AND cg.ClassId = sc.ClassId
    WHERE
        cb.ClassIsLvl2 = 0 AND
        cg.IsValid = 1 AND
        sc.IsNotPresentForm = 0
    GROUP BY
        sc.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_vwNonCombinedStudentClassBooks]
    ON [school_books].[vwNonCombinedStudentClassBooks] ([PersonId], [SchoolYear], [InstId], [ClassBookId], [ClassId])
GO

DROP INDEX IF EXISTS [UQ_vwCombinedStudentClassBooks] ON [school_books].[vwCombinedStudentClassBooks]
GO

CREATE OR ALTER VIEW [school_books].[vwCombinedStudentClassBooks]
WITH SCHEMABINDING
AS
    SELECT
        sc.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId,
        COUNT_BIG(*) AS COUNT
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [student].[StudentClass] sc ON cb.SchoolYear = sc.SchoolYear AND cb.ClassId = sc.ClassId
    WHERE
        cb.ClassIsLvl2 = 1 AND
        sc.IsNotPresentForm = 0
    GROUP BY
        sc.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_vwCombinedStudentClassBooks]
    ON [school_books].[vwCombinedStudentClassBooks] ([PersonId], [SchoolYear], [InstId], [ClassBookId], [ClassId])
GO

CREATE OR ALTER VIEW [school_books].[vwStudentClassBooks]
AS
    SELECT * FROM vwNonCombinedStudentClassBooks
    UNION
    SELECT * FROM vwCombinedStudentClassBooks
GO

PRINT 'Create vwTeacherClassBooks'
GO

DROP INDEX IF EXISTS [UQ_vwTeacherScheduleLessonClassBooks] ON [school_books].[vwTeacherScheduleLessonClassBooks]
GO

CREATE OR ALTER VIEW [school_books].[vwTeacherScheduleLessonClassBooks]
WITH SCHEMABINDING
AS
    SELECT
        sp.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId,
        COUNT_BIG(*) AS COUNT
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [school_books].[Schedule] s ON cb.SchoolYear = s.SchoolYear AND cb.ClassBookId = s.ClassBookId
        INNER JOIN [school_books].[ScheduleLesson] sl ON s.SchoolYear = sl.SchoolYear AND s.ScheduleId = sl.ScheduleId
        INNER JOIN [inst_year].[CurriculumTeacher] ct ON sl.CurriculumId = ct.CurriculumId
        INNER JOIN [inst_basic].[StaffPosition] sp ON ct.StaffPositionId = sp.StaffPositionId
    WHERE
        ct.IsValid = 1
    GROUP BY
        sp.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_vwTeacherScheduleLessonClassBooks]
    ON [school_books].[vwTeacherScheduleLessonClassBooks] ([PersonId], [SchoolYear], [InstId], [ClassBookId], [ClassId])
GO

DROP INDEX IF EXISTS [UQ_vwTeacherAbsenceHourClassBooks] ON [school_books].[vwTeacherAbsenceHourClassBooks]
GO

CREATE OR ALTER VIEW [school_books].[vwTeacherAbsenceHourClassBooks]
WITH SCHEMABINDING
AS
    SELECT
        tah.ReplTeacherPersonId AS PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId,
        COUNT_BIG(*) AS COUNT
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [school_books].[Schedule] s ON cb.SchoolYear = s.SchoolYear AND cb.ClassBookId = s.ClassBookId
        INNER JOIN [school_books].[ScheduleLesson] sl ON s.SchoolYear = sl.SchoolYear AND s.ScheduleId = sl.ScheduleId
        INNER JOIN [school_books].[TeacherAbsenceHour] tah ON sl.SchoolYear = tah.SchoolYear AND sl.ScheduleLessonId = tah.ScheduleLessonId
    WHERE
        tah.ReplTeacherPersonId IS NOT NULL
    GROUP BY
        tah.ReplTeacherPersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_vwTeacherAbsenceHourClassBooks]
    ON [school_books].[vwTeacherAbsenceHourClassBooks] ([PersonId], [SchoolYear], [InstId], [ClassBookId], [ClassId])
GO

CREATE OR ALTER VIEW [school_books].[vwTeacherScheduleClassBooks]
AS
    SELECT * FROM vwTeacherScheduleLessonClassBooks
    UNION
    SELECT * FROM vwTeacherAbsenceHourClassBooks
GO

PRINT 'Create Get_DBP_StudentAbsences stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_DBP_StudentAbsences]
    @schoolYear INT,
    @azureID VARCHAR(100)
AS
    DECLARE @personId INT;
    SET @personId = (SELECT PersonId FROM [core].[Person] WHERE AzureID = @azureID);

    DECLARE @studentClassBooks TABLE (
        [PersonId] INT NOT NULL,
        [SchoolYear] INT NOT NULL,
        [InstId] INT NOT NULL,
        [ClassBookId] INT NOT NULL,
        [ClassId] INT NOT NULL
    );

    INSERT @studentClassBooks
    SELECT
        [PersonId],
        [SchoolYear],
        [InstId],
        [ClassBookId],
        [ClassId]
    FROM
        [school_books].[vwStudentClassBooks]
    WHERE
        PersonId = @personId AND
        SchoolYear = @schoolYear;

    SELECT
        [SchoolYear] = cb.SchoolYear,
        [InstitutionId] = cb.InstId,
        [ClassId] = cb.ClassId,
        [ClassName] = cg.ClassName,
        [AbsenceId] = a.AbsenceId,
        [Date] = a.Date,
        [CurriculumId] = c.CurriculumId,
        [CurriculumGroupName] = c.CurriculumGroupNum,
        [SubjectId] = s.SubjectID,
        [SubjectName] = s.SubjectName,
        [SubjectNameShort] = s.SubjectNameShort,
        [SubjectTypeId] = st.SubjectTypeID,
        [SubjectTypeName] = st.Name,
        [Type] = a.[Type],
        [Term] = a.[Term],
        [ExcusedReasonId] = a.[ExcusedReasonId],
        [ExcusedReasonComment] = a.[ExcusedReasonComment]
    FROM
        @studentClassBooks cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Absence] a ON cb.SchoolYear = a.SchoolYear AND cb.ClassBookId = a.ClassBookId AND cb.PersonId = a.PersonId
        INNER JOIN [inst_year].[Curriculum] c ON a.CurriculumId = c.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
    ORDER BY
        a.Date,
        a.CreateDate
GO

PRINT 'Create Get_DBP_StudentGrades stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_DBP_StudentGrades]
    @schoolYear INT,
    @azureID VARCHAR(100)
AS
    DECLARE @personId INT;
    SET @personId = (SELECT PersonId FROM [core].[Person] WHERE AzureID = @azureID);

    DECLARE @studentClassBooks TABLE (
        [PersonId] INT NOT NULL,
        [SchoolYear] INT NOT NULL,
        [InstId] INT NOT NULL,
        [ClassBookId] INT NOT NULL,
        [ClassId] INT NOT NULL
    );

    INSERT @studentClassBooks
    SELECT
        [PersonId],
        [SchoolYear],
        [InstId],
        [ClassBookId],
        [ClassId]
    FROM
        [school_books].[vwStudentClassBooks]
    WHERE
        PersonId = @personId AND
        SchoolYear = @schoolYear;

    SELECT
        [SchoolYear] = cb.SchoolYear,
        [InstitutionId] = cb.InstId,
        [ClassId] = cb.ClassId,
        [ClassName] = cg.ClassName,
        [GradeId] = g.GradeId,
        [Date] = g.Date,
        [CurriculumId] = c.CurriculumId,
        [CurriculumGroupName] = c.CurriculumGroupNum,
        [SubjectId] = s.SubjectID,
        [SubjectName] = s.SubjectName,
        [SubjectNameShort] = s.SubjectNameShort,
        [SubjectTypeId] = st.SubjectTypeID,
        [SubjectTypeName] = st.Name,
        [Type] = g.[Type],
        [Term] = g.[Term],
        [Category] = g.[Category],
        [DecimalGrade] = g.[DecimalGrade],
        [SpecialGrade] = g.[SpecialGrade],
        [QualitativeGrade] = g.[QualitativeGrade],
        [Comment] = g.[Comment]
    FROM
        @studentClassBooks cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Grade] g ON cb.SchoolYear = g.SchoolYear AND cb.ClassBookId = g.ClassBookId AND cb.PersonId = g.PersonId
        INNER JOIN [inst_year].[Curriculum] c ON g.CurriculumId = c.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
    ORDER BY
        g.Date,
        g.CreateDate
GO

PRINT 'Create Get_DBP_StudentRemarks stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_DBP_StudentRemarks]
    @schoolYear INT,
    @azureID VARCHAR(100)
AS
    DECLARE @personId INT;
    SET @personId = (SELECT PersonId FROM [core].[Person] WHERE AzureID = @azureID);

    DECLARE @studentClassBooks TABLE (
        [PersonId] INT NOT NULL,
        [SchoolYear] INT NOT NULL,
        [InstId] INT NOT NULL,
        [ClassBookId] INT NOT NULL,
        [ClassId] INT NOT NULL
    );

    INSERT @studentClassBooks
    SELECT
        [PersonId],
        [SchoolYear],
        [InstId],
        [ClassBookId],
        [ClassId]
    FROM
        [school_books].[vwStudentClassBooks]
    WHERE
        PersonId = @personId AND
        SchoolYear = @schoolYear;

    SELECT
        [SchoolYear] = cb.SchoolYear,
        [InstitutionId] = cb.InstId,
        [ClassId] = cb.ClassId,
        [ClassName] = cg.ClassName,
        [RemarkId] = r.RemarkId,
        [Date] = r.Date,
        [CurriculumId] = c.CurriculumId,
        [CurriculumGroupName] = c.CurriculumGroupNum,
        [SubjectId] = s.SubjectID,
        [SubjectName] = s.SubjectName,
        [SubjectNameShort] = s.SubjectNameShort,
        [SubjectTypeId] = st.SubjectTypeID,
        [SubjectTypeName] = st.Name,
        [Type] = r.[Type],
        [Description] = r.[Description]
    FROM
        @studentClassBooks cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Remark] r ON cb.SchoolYear = r.SchoolYear AND cb.ClassBookId = r.ClassBookId AND cb.PersonId = r.PersonId
        INNER JOIN [inst_year].[Curriculum] c ON r.CurriculumId = c.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
    ORDER BY
        r.Date,
        r.CreateDate
GO

PRINT 'Create Get_DBP_StudentSchedule stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_DBP_StudentSchedule]
    @schoolYear INT,
    @azureID VARCHAR(100),
    @fromDate DATE,
    @toDate DATE
AS
    DECLARE @personId INT;
    SET @personId = (SELECT PersonId FROM [core].[Person] WHERE AzureID = @azureID);

    DECLARE @studentClassBooks TABLE (
        [SchoolYear] INT NOT NULL,
        [InstId] INT NOT NULL,
        [ClassBookId] INT NOT NULL,
        [ClassId] INT NOT NULL
    );

    INSERT @studentClassBooks
    SELECT
        [SchoolYear],
        [InstId],
        [ClassBookId],
        [ClassId]
    FROM
        [school_books].[vwStudentClassBooks]
    WHERE
        PersonId = @personId AND
        SchoolYear = @schoolYear;

    SELECT
        [SchoolYear] = cb.SchoolYear,
        [InstitutionId] = cb.InstId,
        [ClassId] = cb.ClassId,
        [ClassName] = cg.ClassName,
        [ScheduleLessonId] = sl.ScheduleLessonId,
        [ShiftId] = sch.ShiftId,
        [ShiftName] = sh.Name,
        [Date] = sl.Date,
        [Day] = sl.Day,
        [HourNumber] = sl.HourNumber,
        [StartTime] = shh.StartTime,
        [EndTime] = shh.EndTime,
        [CurriculumId] = c.CurriculumId,
        [CurriculumGroupName] = c.CurriculumGroupNum,
        [SubjectId] = s.SubjectID,
        [SubjectName] = s.SubjectName,
        [SubjectNameShort] = s.SubjectNameShort,
        [SubjectTypeId] = st.SubjectTypeID,
        [SubjectTypeName] = st.Name,
        [CurriculumTeachers] = STRING_AGG(CONCAT_WS(' ', p.FirstName, p.LastName), ', '),
        [ReplTeacher] = NULLIF(CONCAT_WS(' ', repl.FirstName, repl.LastName), ''),
        [IsEmptyHour] = IIF(tah.TeacherAbsenceId IS NOT NULL AND tah.ReplTeacherPersonId IS NULL, CAST(1 AS BIT), CAST(0 AS BIT))
    FROM
        @studentClassBooks cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Schedule] sch ON cb.SchoolYear = sch.SchoolYear AND cb.ClassBookId = sch.ClassBookId
        INNER JOIN [school_books].[Shift] sh ON sch.SchoolYear = sh.SchoolYear AND sch.ShiftId = sh.ShiftId
        INNER JOIN [school_books].[ScheduleLesson] sl ON sch.SchoolYear = sl.SchoolYear AND sch.ScheduleId = sl.ScheduleId
        INNER JOIN [school_books].[ShiftHour] shh ON sl.SchoolYear = shh.SchoolYear AND sch.ShiftId = shh.ShiftId AND sl.HourNumber = shh.HourNumber
        INNER JOIN [inst_year].[Curriculum] c ON sl.CurriculumId = c.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
        INNER JOIN [inst_year].[CurriculumTeacher] t ON c.CurriculumID = t.CurriculumID
        INNER JOIN [inst_basic].[StaffPosition] sp ON t.StaffPositionID = sp.StaffPositionID
        INNER JOIN [core].[Person] p ON sp.PersonID = p.PersonID
        LEFT JOIN [school_books].[TeacherAbsenceHour] tah ON sl.SchoolYear = tah.SchoolYear AND sl.ScheduleLessonId = tah.ScheduleLessonId
        LEFT JOIN [core].[Person] repl ON tah.ReplTeacherPersonId = repl.PersonID
    WHERE
        sl.[Date] >= @fromDate AND
        sl.[Date] <= @toDate
    GROUP BY
        cb.SchoolYear,
        cb.InstId,
        cb.ClassId,
        cg.ClassName,
        sl.ScheduleLessonId,
        sch.ShiftId,
        sh.Name,
        sl.Date,
        sl.Day,
        sl.HourNumber,
        shh.StartTime,
        shh.EndTime,
        c.CurriculumId,
        c.CurriculumGroupNum,
        s.SubjectID,
        s.SubjectName,
        s.SubjectNameShort,
        st.SubjectTypeID,
        st.Name,
        tah.TeacherAbsenceId,
        tah.ReplTeacherPersonId,
        repl.FirstName,
        repl.LastName
    ORDER BY
        sl.Date,
        shh.StartTime
GO

PRINT 'Create Get_DBP_TeacherSchedule stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_DBP_TeacherSchedule]
    @schoolYear INT,
    @azureID VARCHAR(100),
    @fromDate DATE,
    @toDate DATE
AS
    DECLARE @personId INT;
    SET @personId = (SELECT PersonId FROM [core].[Person] WHERE AzureID = @azureID);

    DECLARE @teacherScheduleClassBooks TABLE (
        [SchoolYear] INT NOT NULL,
        [InstId] INT NOT NULL,
        [ClassBookId] INT NOT NULL,
        [ClassId] INT NOT NULL
    );

    INSERT @teacherScheduleClassBooks
    SELECT
        [SchoolYear],
        [InstId],
        [ClassBookId],
        [ClassId]
    FROM
        [school_books].[vwTeacherScheduleClassBooks]
    WHERE
        PersonId = @personId AND
        SchoolYear = @schoolYear;

    SELECT
        [SchoolYear] = cb.SchoolYear,
        [InstitutionId] = cb.InstId,
        [ClassId] = cb.ClassId,
        [ClassName] = cg.ClassName,
        [ScheduleLessonId] = sl.ScheduleLessonId,
        [ShiftId] = sch.ShiftId,
        [ShiftName] = sh.Name,
        [Date] = sl.Date,
        [Day] = sl.Day,
        [HourNumber] = sl.HourNumber,
        [StartTime] = shh.StartTime,
        [EndTime] = shh.EndTime,
        [CurriculumId] = c.CurriculumId,
        [CurriculumGroupName] = c.CurriculumGroupNum,
        [SubjectId] = s.SubjectID,
        [SubjectName] = s.SubjectName,
        [SubjectNameShort] = s.SubjectNameShort,
        [SubjectTypeId] = st.SubjectTypeID,
        [SubjectTypeName] = st.Name
    FROM
        @teacherScheduleClassBooks cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassID
        INNER JOIN [school_books].[Schedule] sch ON cb.SchoolYear = sch.SchoolYear AND cb.ClassBookId = sch.ClassBookId
        INNER JOIN [school_books].[Shift] sh ON sch.SchoolYear = sh.SchoolYear AND sch.ShiftId = sh.ShiftId
        INNER JOIN [school_books].[ScheduleLesson] sl ON sch.SchoolYear = sl.SchoolYear AND sch.ScheduleId = sl.ScheduleId
        INNER JOIN [school_books].[ShiftHour] shh ON sl.SchoolYear = shh.SchoolYear AND sch.ShiftId = shh.ShiftId AND sl.HourNumber = shh.HourNumber
        INNER JOIN [inst_year].[Curriculum] c ON sl.CurriculumId = c.CurriculumID
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
        LEFT JOIN [school_books].[TeacherAbsenceHour] tah ON sl.SchoolYear = tah.SchoolYear AND sl.ScheduleLessonId = tah.ScheduleLessonId
    WHERE
        sl.[Date] >= @fromDate AND
        sl.[Date] <= @toDate AND (
            (tah.TeacherAbsenceId IS NULL AND EXISTS (
                SELECT 1
                FROM [inst_year].[CurriculumTeacher] t
                    INNER JOIN [inst_basic].[StaffPosition] sp ON t.StaffPositionID = sp.StaffPositionID
                WHERE
                    t.CurriculumID = c.CurriculumID AND sp.PersonID = @personId
            )) OR
            (tah.TeacherAbsenceId IS NOT NULL AND tah.ReplTeacherPersonId = @personId)
        )
    ORDER BY
        sl.Date,
        shh.StartTime
GO
