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
        [BasicClassId] = cg.BasicClassID,
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
        INNER JOIN [school_books].[ShiftHour] shh ON sl.SchoolYear = shh.SchoolYear AND sch.ShiftId = shh.ShiftId AND sl.Day = shh.Day AND sl.HourNumber = shh.HourNumber
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
        cg.BasicClassID,
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
        [BasicClassId] = cg.BasicClassID,
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
        INNER JOIN [school_books].[ShiftHour] shh ON sl.SchoolYear = shh.SchoolYear AND sch.ShiftId = shh.ShiftId AND sl.Day = shh.Day AND sl.HourNumber = shh.HourNumber
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
